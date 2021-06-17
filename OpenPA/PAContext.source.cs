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


        #region Source

        #region Callbacks
        /// <summary>
        /// Callback for the get_source_info family of calls
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="info">source_info data</param>
        /// <param name="eol">End of list marker</param>
        /// <param name="userdata">Mainloop pointer should be passed in here</param>
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static unsafe void SourceCallback(pa_context* ctx, pa_source_info* info, int eol, void* userdata)
        {

            // Test for the end of list
            if (eol == 0)
            {
                // Not the end of the list

                // Copy the pa_source_info address
                *((pa_source_info**)userdata) = info;

                // Signal the mainloop to continue
                MainLoop.Instance.Signal(1);
            }
            else
            {
                // Return null
                *((pa_source_info**)userdata) = null;

                // Signal the mainloop
                MainLoop.Instance.Signal(0);
            }

        }

        // Success callback for operations on sources
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static unsafe void SourceSuccessCallback(pa_context* ctx, int success, void* userdata)
        {
            *((bool*)userdata) = success == 1;

            MainLoop.Instance.Signal(0);
        }
        #endregion

        /// <summary>
        /// Gets a SourceInfo object describing a source
        /// </summary>
        /// <param name="source">Name of the sink</param>
        /// <returns>SourceInfo object</returns>
        public Task<SourceInfo?> GetSourceInfoAsync(string source) => Task.Run(() => GetSourceInfo(source));

        /// <summary>
        /// Gets a SourceInfo object describing a source
        /// </summary>
        /// <param name="source">Name of the sink</param>
        /// <returns>SourceInfo object</returns>
        public SourceInfo? GetSourceInfo(string source)
        {
            // Returned object
            SourceInfo? info = default;

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Copy the name of the sink to an unmanaged buffer
            IntPtr name = Marshal.StringToHGlobalAnsi(source);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to returned structure
            pa_source_info* source_info = null;

            // Call the native get_source_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_source_info_by_name(pa_Context, name, &SourceCallback, &source_info);

            // Wait for the callback to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && source_info == null)
                MainLoop.Instance.Wait();


            // If the callback returned data
            if (source_info != null)
            {
                // Copy the unmanaged source_info structure into a SourceInfo object
                info = SourceInfo.Convert(*source_info);

                // Allow PulseAudio to free the pa_source_info structure
                MainLoop.Instance.Accept();

                // Wait for the operation to complete
                MainLoop.Instance.Wait();

            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory that holds the source name
            Marshal.FreeHGlobal(name);

            // Unlock the context
            Monitor.Exit(this);

            // Return the SourceInfo object
            return info;

        }

        /// <summary>
        /// Gets a SourceInfo object describing a source
        /// </summary>
        /// <param name="source">Index of the source</param>
        /// <returns>SourceInfo object</returns>
        public Task<SourceInfo?> GetSourceInfoAsync(uint index) => Task.Run(() => GetSourceInfo(index));

        /// <summary>
        /// Gets a SourceInfo object describing a source
        /// </summary>
        /// <param name="source">Index of the source</param>
        /// <returns>SourceInfo object</returns>
        public SourceInfo? GetSourceInfo(uint index)
        {
            // Returned object
            SourceInfo? info = default;

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to the returned structure
            pa_source_info* source_info = null;

            // Call the native get_source_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_source_info_by_index(pa_Context, index, &SourceCallback, &source_info);

            // Wait for the callback to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && source_info == null)
                MainLoop.Instance.Wait();

            // If the callback returned data
            if (source_info != null)
            {
                // Copy the unmanaged source_info structure into a SourceInfo object
                info = SourceInfo.Convert(*source_info);

                // Allow PulseAudio to free the source_info
                MainLoop.Instance.Accept();

                // Wait for the operation to complete
                MainLoop.Instance.Wait();

            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock the context
            Monitor.Exit(this);

            // Return the SourceInfo object
            return info;

        }

        /// <summary>
        /// Gets the list of sources on currently on the server
        /// </summary>        
        /// <returns>List of SourceInfo objects</returns>
        public Task<IReadOnlyList<SourceInfo?>> GetSourceInfoListAsync() => Task.Run(GetSourceInfoList);

        /// <summary>
        /// Gets the list of sources on currently on the server
        /// </summary>        
        /// <returns>List of SourceInfo objects</returns>
        public IReadOnlyList<SourceInfo?> GetSourceInfoList()
        {

            List<SourceInfo?> sources = new();

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            pa_source_info* source_info = null;

            // Call the native get_source_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_source_info_list(pa_Context, &SourceCallback, &source_info);

            do
            {

                // Wait for the callback to signal
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && source_info == null)
                    MainLoop.Instance.Wait();


                // If the callback returned data
                if (source_info != null)
                {
                    // Copy the unmanaged source_info structure into a SourceInfo object
                    // and add to the list
                    sources.Add(SourceInfo.Convert(*source_info));

                    // Allow PulseAudio to free the source_info
                    MainLoop.Instance.Accept();

                    source_info = null;

                    continue;
                }
                else
                {
                    break;
                }


            } while (true);

            // Dereference the pa_operation
            pa_operation_unref(op);


            // Unlock the mainloop
            MainLoop.Instance.Unlock();


            // Unlock the context
            Monitor.Exit(this);

            // Return the SinkInfo object
            return sources;

        }

        /// <summary>
        /// Sets the volume of a source
        /// </summary>
        /// <param name="index">Index of the source</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public Task<bool> SetSourceVolumeAsync(uint index, Volume volume) => Task.Run(() => SetSourceVolume(index, volume));

        /// <summary>
        /// Sets the volume of a source
        /// </summary>
        /// <param name="index">Index of the source</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public bool SetSourceVolume(uint index, Volume volume)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to store the result
            bool result = false;

            // Copy the volume object to a native structure
            pa_cvolume cvolume = Volume.Convert(volume);

            // Invoke the operation
            pa_operation* op = pa_context_set_source_volume_by_index(pa_Context, index, &cvolume, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Sets the volume of a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public Task<bool> SetSourceVolumeAsync(string source, Volume volume) => Task.Run(() => SetSourceVolume(source, volume));

        /// <summary>
        /// Sets the volume of a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="volume">New volume</param>
        /// <returns>Success</returns>
        public bool SetSourceVolume(string source, Volume volume)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the name of the source to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(source);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup somewhere to catch the result of the operation
            bool result = false;

            // Copy the volume object to a native structure
            pa_cvolume cvolume = Volume.Convert(volume);

            // Invoke the operation
            pa_operation* op = pa_context_set_source_volume_by_name(pa_Context, name, &cvolume, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation object
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
        /// Sets the mute switch of a source
        /// </summary>
        /// <param name="index">Index of source</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public Task<bool> SetSourceMuteAsync(uint index, bool mute) => Task.Run(() => SetSourceMute(index, mute));

        /// <summary>
        /// Sets the mute switch of a source
        /// </summary>
        /// <param name="index">Index of source</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public bool SetSourceMute(uint index, bool mute)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to store the result
            bool result = false;            

            // Invoke the operation
            pa_operation* op = pa_context_set_source_mute_by_index(pa_Context, index, mute ? 1 : 0, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Sets the mute switch of a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public Task<bool> SetSourceMuteAsync(string source, bool mute) => Task.Run(() => SetSourceMute(source, mute));

        /// <summary>
        /// Sets the mute switch of a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="mute">Mute value</param>
        /// <returns>Success</returns>
        public bool SetSourceMute(string source, bool mute)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the name of the source to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(source);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup somewhere to catch the result of the operation
            bool result = false;
            
            // Invoke the operation
            pa_operation* op = pa_context_set_source_mute_by_name(pa_Context, name, mute ? 1 : 0, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation object
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
       /// Suspend/Resume a source
       /// </summary>
       /// <param name="index">Index of source</param>
       /// <param name="suspend">True = suspend, False = resume</param>
       /// <returns>Success</returns>
        public Task<bool> SuspendSourceAsync(uint index, bool suspend) => Task.Run(() => SuspendSource(index, suspend));

        /// <summary>
        /// Suspend/Resume a source
        /// </summary>
        /// <param name="index">Index of source</param>
        /// <param name="suspend">True = suspend, False = resume</param>
        /// <returns>Success</returns>
        public bool SuspendSource(uint index, bool suspend)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to store the result
            bool result = false;

            // Invoke the operation
            pa_operation* op = pa_context_suspend_source_by_index(pa_Context, index, suspend ? 1 : 0, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock this object
            Monitor.Exit(this);

            // Return the result
            return result;
        }

        /// <summary>
        /// Suspend/Resume a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="suspend">True = suspend, False = resume</param>
        /// <returns>Success</returns>
        public Task<bool> SuspendSourceAsync(string source, bool suspend) => Task.Run(() => SuspendSource(source, suspend));

        /// <summary>
        /// Suspend/Resume a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="suspend">True = suspend, False = resume</param>
        /// <returns>Success</returns>
        public bool SuspendSource(string source, bool suspend)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the name of the source to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(source);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup somewhere to catch the result of the operation
            bool result = false;

            // Invoke the operation
            pa_operation* op = pa_context_suspend_source_by_name(pa_Context, name, suspend ? 1 : 0, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation object
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
        /// Sets the profile of a source
        /// </summary>
        /// <param name="index">Index of the source</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public Task<bool> SetSourcePortAsync(uint index, string profile) => Task.Run(() => SetSourcePort(index, profile));

        /// <summary>
        /// Sets the profile of a source
        /// </summary>
        /// <param name="index">Index of the source</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public bool SetSourcePort(uint index, string profile)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the profile name to unmanaged memory
            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a place to store the result
            bool result = false;

            // Invoke the operation
            pa_operation* op = pa_context_set_source_port_by_index(pa_Context, index, profileName, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
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
        /// Sets the profile of a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="profile">Name of profile</param>
        /// <returns></returns>
        public Task<bool> SetSourcePortAsync(string source, string profile) => Task.Run(() => SetSourcePort(source, profile));

        /// <summary>
        /// Sets the profile of a source
        /// </summary>
        /// <param name="source">Name of source</param>
        /// <param name="profile">Name of profile</param>
        /// <returns></returns>
        public bool SetSourcePort(string source, string profile)
        {
            // Lock this object to remain thread-safe
            Monitor.Enter(this);

            // Copy the name of the source to unmanaged memory
            IntPtr name = Marshal.StringToHGlobalAnsi(source);

            // Copy the profile name to unmanaged memory
            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup somewhere to catch the result of the operation
            bool result = false;

            // Invoke the operation
            pa_operation* op = pa_context_set_source_port_by_name(pa_Context, name, profileName, &SourceSuccessCallback, &result);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation object
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
