using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OpenPA.Native.pa_operation;
using static OpenPA.Native.pa_context;

namespace OpenPA
{
    public unsafe partial class PAContext
    {

        #region Sink

        #region Callbacks
        // Callback for the get_sink_info call
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static unsafe void SinkCallback(pa_context* ctx, pa_sink_info* info, int eol, void* userdata)
        {
            // Test for the end of list
            if (eol == 0)
            {
                // Not the end of the list

                // Copy the address of the pa_sink_info data
                *((pa_sink_info**)userdata) = info;

                // Signal the mainloop to continue
                MainLoop.Instance.Signal(1);
            }
            else
            {
                // End of the list

                // Signal end of the list by setting sink_info to null
                *((pa_sink_info**)userdata) = null;

                // Signal the mainloop to continue
                MainLoop.Instance.Signal(0);
            }

        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static unsafe void SinkSuccessCallback(pa_context* ctx, int success, void* userdata)
        {            
            *((bool*)userdata) = success == 1;

            MainLoop.Instance.Signal(0);
        }
        #endregion

        /// <summary>
        /// Gets a SinkInfo object describing a sink
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <returns>SinkInfo object</returns>
        public Task<SinkInfo?> GetSinkInfoAsync(string sink) => Task.Run(() => GetSinkInfo(sink));

        /// <summary>
        /// Gets a SinkInfo object describing a sink
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <returns>SinkInfo object</returns>
        public SinkInfo? GetSinkInfo(string sink)
        {
            // Returned object
            SinkInfo? info = default;

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Copy the name of the sink to an unmanaged buffer
            IntPtr name = Marshal.StringToHGlobalAnsi(sink);

            // Pointer to returned pa_sink_info structure
            pa_sink_info* sink_info = null;

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Call the native get_sink_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_sink_info_by_name(pa_Context, name, &SinkCallback, &sink_info);

            // Wait for the callback to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && sink_info == null)
                MainLoop.Instance.Wait();

            // If the callback returned data
            if (sink_info != null)
            {
                // Copy the unmanaged sink_info structure into a SinkInfo object
                info = SinkInfo.Convert(*sink_info);

                // Allow PulseAudio to free the sink_info
                MainLoop.Instance.Accept();

                // Wait for the mainloop to complete
                MainLoop.Instance.Wait();

            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory that holds the sink name
            Marshal.FreeHGlobal(name);

            // Unlock the context
            Monitor.Exit(this);

            // Return the SinkInfo object
            return info;

        }

        /// <summary>
        /// Gets a SinkInfo object describing a sink
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <returns>SinkInfo object</returns>
        public Task<SinkInfo?> GetSinkInfoAsync(uint index) => Task.Run(() => GetSinkInfo(index));

        /// <summary>
        /// Gets a SinkInfo object describing a sink
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <returns>SinkInfo object</returns>
        public SinkInfo? GetSinkInfo(uint index)
        {
            // Returned object
            SinkInfo? info = default;

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to the returned pa_sink_info structure
            pa_sink_info* sink_info;

            // Call the native get_sink_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_sink_info_by_index(pa_Context, index, &SinkCallback, &sink_info);

            // Wait for the callback to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && sink_info == null)
                MainLoop.Instance.Wait();

            // If the callback returned data
            if (sink_info != null)
            {
                // Copy the unmanaged sink_info structure into a SinkInfo object
                info = SinkInfo.Convert(*sink_info);

                // Allow PulseAudio to free the sink_info
                MainLoop.Instance.Accept();

                // Signal the loop
                MainLoop.Instance.Wait();
            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock the context
            Monitor.Exit(this);

            // Return the SinkInfo object
            return info;

        }

        /// <summary>
        /// Gets the list of sinks on currently on the server
        /// </summary>        
        /// <returns>List of SinkInfo objects</returns>
        public Task<IReadOnlyList<SinkInfo?>> GetSinkInfoListAsync() => Task.Run(GetSinkInfoList);

        /// <summary>
        /// Gets the list of sinks on currently on the server
        /// </summary>        
        /// <returns>List of SinkInfo objects</returns>
        public IReadOnlyList<SinkInfo?> GetSinkInfoList()
        {
            // Returned list
            List<SinkInfo?> sinks = new();

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to the returned pa_sink_info structure
            pa_sink_info* sink_info = null;

            // Call the native get_sink_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_sink_info_list(pa_Context, &SinkCallback, &sink_info);

            // Loop through list of sinks
            while (true)
            {
                // Wait for the callback to signal
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && sink_info == null)
                    MainLoop.Instance.Wait();

                // If the callback returned data
                if (sink_info != null)
                {
                    // Copy the unmanaged sink_info structure into a SinkInfo object
                    // and add it to the list
                    sinks.Add(SinkInfo.Convert(*sink_info));

                    // Allow PulseAudio to free the pa_sink_info structure
                    MainLoop.Instance.Accept();

                    // Reset sink_info pointer
                    sink_info = null;
                }
                else
                {
                    // Break out of the loop when the list is done
                    break;
                }
            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock the context
            Monitor.Exit(this);

            // Return the SinkInfo object List
            return sinks;

        }

        /// <summary>
        /// Set the volume of a sink device
        /// </summary>
        /// <param name="index">Index of the sink</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public Task<bool> SetSinkVolumeAsync(uint index, Volume volume) => Task.Run(() => SetSinkVolume(index, volume));

        /// <summary>
        /// Set the volume of a sink device
        /// </summary>
        /// <param name="index">Index of the sink</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public bool SetSinkVolume(uint index, Volume volume)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;

            // Get a native structure of the Volume object
            pa_cvolume cvolume = Volume.Convert(volume);            

            // Set the volume of the sink
            pa_operation* op = pa_context_set_sink_volume_by_index(pa_Context, index, &cvolume, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Set the volume of a sink device
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public Task<bool> SetSinkVolumeAsync(string sink, Volume volume) => Task.Run(() => SetSinkVolume(sink, volume));

        /// <summary>
        /// Set the volume of a sink device
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public bool SetSinkVolume(string sink, Volume volume)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the sink name string to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(sink);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;

            // Get a native structure of the Volume object
            pa_cvolume cvolume = Volume.Convert(volume);

            // Set the volume of the sink
            pa_operation* op = pa_context_set_sink_volume_by_name(pa_Context, name, &cvolume, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory
            Marshal.FreeHGlobal(name);

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Set the mute switch of a sink device
        /// </summary>
        /// <param name="index">Index of the sink</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public Task<bool> SetSinkMuteAsync(uint index, bool mute) => Task.Run(() => SetSinkMute(index, mute));

        /// <summary>
        /// Set the mute switch of a sink device
        /// </summary>
        /// <param name="index">Index of the sink</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public bool SetSinkMute(uint index, bool mute)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;
            
            // Set the volume of the sink
            pa_operation* op = pa_context_set_sink_mute_by_index(pa_Context, index, mute ? 1 : 0, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Sets the mute switch of a sink device
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public Task<bool> SetSinkMuteAsync(string sink, bool mute) => Task.Run(() => SetSinkMute(sink, mute));

        /// <summary>
        /// Sets the mute switch of a sink device
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public bool SetSinkMute(string sink, bool mute)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the sink name string to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(sink);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;            

            // Set the volume of the sink
            pa_operation* op = pa_context_set_sink_mute_by_name(pa_Context, name, mute ? 1 : 0, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory
            Marshal.FreeHGlobal(name);

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Suspend/Resume a sink
        /// </summary>
        /// <param name="index">Index of sink (use PA_INVALID_INDEX to suspend all sinks)</param>
        /// <param name="suspend">True to suspend, False to resume</param>
        /// <returns>Success</returns>
        public Task<bool> SuspendSinkAsync(uint index, bool suspend) => Task.Run(() => SuspendSink(index, suspend));

        /// <summary>
        /// Suspend/Resume a sink
        /// </summary>
        /// <param name="index">Index of sink (use PA_INVALID_INDEX to suspend all sinks)</param>
        /// <param name="suspend">True to suspend, False to resume</param>
        /// <returns>Success</returns>
        public bool SuspendSink(uint index, bool suspend)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;

            // Set the volume of the sink
            pa_operation* op = pa_context_suspend_sink_by_index(pa_Context, index, suspend ? 1 : 0, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Suspend/Resume a sink
        /// </summary>
        /// <param name="sink">Name of sink</param>
        /// <param name="suspend">True to suspend, False to resume</param>
        /// <returns>Success</returns>
        public Task<bool> SuspendSinkAsync(string sink, bool suspend) => Task.Run(() => SuspendSink(sink, suspend));

        /// <summary>
        /// Suspend/Resume a sink
        /// </summary>
        /// <param name="sink">Name of sink</param>
        /// <param name="suspend">True to suspend, False to resume</param>
        /// <returns>Success</returns>
        public bool SuspendSink(string sink, bool suspend)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the sink name string to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(sink);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;

            // Set the volume of the sink
            pa_operation* op = pa_context_suspend_sink_by_name(pa_Context, name, suspend ? 1 : 0, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory
            Marshal.FreeHGlobal(name);

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Sets the profile of the sink
        /// </summary>
        /// <param name="index">Index of the sink</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public Task<bool> SetSinkPortAsync(uint index, string profile) => Task.Run(() => SetSinkPort(index, profile));

        /// <summary>
        /// Sets the profile of the sink
        /// </summary>
        /// <param name="index">Index of the sink</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public bool SetSinkPort(uint index, string profile)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the name of the profile to unmanaged memory
            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;            

            // Set the volume of the sink
            pa_operation* op = pa_context_set_sink_port_by_index(pa_Context, index, profileName, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory
            Marshal.FreeHGlobal(profileName);

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Sets the profile of a sink
        /// </summary>
        /// <param name="sink">Name of sink</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public Task<bool> SetSinkPortAsync(string sink, string profile) => Task.Run(() => SetSinkPort(sink, profile));

        /// <summary>
        /// Sets the profile of a sink
        /// </summary>
        /// <param name="sink">Name of sink</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public bool SetSinkPort(string sink, string profile)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the sink name string to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(sink);

            // Copy the profile name to unmanaged memory
            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to receive the result
            bool result = false;

            // Set the volume of the sink
            pa_operation* op = pa_context_set_sink_port_by_name(pa_Context, name, profileName, &SinkSuccessCallback, &result);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory
            Marshal.FreeHGlobal(name);
            Marshal.FreeHGlobal(profileName);

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }
        #endregion

    }
}
