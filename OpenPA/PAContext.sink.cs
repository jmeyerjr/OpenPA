using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenPA
{
    public unsafe partial class PAContext
    {

        #region Sink
        /// <summary>
        /// State flag for the get_sink_info callback
        /// 0 = pending, 1 = pa_sink_info valid, -1 = end of list (no data)
        /// </summary>
        static CallbackState gotSink;
        /// <summary>
        /// Structure to hold the sink_info from the callback
        /// </summary>
        static pa_sink_info* sink_info;

        /// <summary>
        /// Gets a SinkInfo object describing a sink
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <returns>SinkInfo object</returns>
        public Task<SinkInfo?> GetSinkInfoAsync(string sink)
        {
            #region Local Functions
            // Callback for the get_sink_info call
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static unsafe void Callback(pa_context* ctx, pa_sink_info* info, int eol, void* userdata)
            {
                pa_threaded_mainloop* m = (pa_threaded_mainloop*)userdata;

                // Test for the end of list
                if (eol == 0 && info != null)
                {
                    // Not the end of the list

                    // Copy the sink_info data to the static structure
                    sink_info = info;

                    // Flag that there is valid data
                    gotSink = CallbackState.HasData;

                    // Signal the mainloop to continue
                    pa_threaded_mainloop.pa_threaded_mainloop_signal(m, 1);

                }
                else if (eol < 0)
                {
                    // Flag there was an error
                    gotSink = CallbackState.Failed;

                    // Signal the mainloop to continue
                    pa_threaded_mainloop.pa_threaded_mainloop_signal(m, 0);
                }
                else
                {
                    // End of the list

                    // Flag that we have reached the end of the list
                    gotSink = CallbackState.Success;

                    // Signal the mainloop to continue
                    pa_threaded_mainloop.pa_threaded_mainloop_signal(m, 0);

                }

            }
            #endregion

            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task.Run(() =>
            {
                SinkInfo? info = default;

                // Lock the context so that we can remain thread-safe
                Monitor.Enter(this);

                // Copy the name of the sink to an unmanaged buffer
                IntPtr name = Marshal.StringToHGlobalAnsi(sink);

                // Set flag to 'pending'
                gotSink = CallbackState.Pending;

                // Lock the mainloop
                _mainLoop.Lock();

                // Call the native get_sink_info native function passing in the name and callback
                pa_operation* op = pa_context.pa_context_get_sink_info_by_name(pa_Context, name, &Callback, _mainLoop.ptr);

                // Wait for the callback to signal
                while (gotSink == CallbackState.Pending)
                    _mainLoop.Wait();

                // If the callback returned data
                if (gotSink == CallbackState.HasData)
                {
                    // Copy the unmanaged sink_info structure into a SinkInfo object
                    info = SinkInfo.Convert(*sink_info);

                    // Allow PulseAudio to free the sink_info
                    _mainLoop.Accept();

                    // Wait for the mainloop to complete
                    _mainLoop.Wait();

                }

                // Dereference the pa_operation
                pa_operation.pa_operation_unref(op);

                // Unlock the mainloop
                _mainLoop.Unlock();

                // Free the unmanaged memory that holds the sink name
                Marshal.FreeHGlobal(name);

                // Unlock the context
                Monitor.Exit(this);

                // Return the SinkInfo object
                return info;
            });
        }

        /// <summary>
        /// Gets a SinkInfo object describing a sink
        /// </summary>
        /// <param name="sink">Name of the sink</param>
        /// <returns>SinkInfo object</returns>
        public Task<SinkInfo?> GetSinkInfoAsync(uint index)
        {
            #region Local Functions
            // Callback for the get_sink_info call
            [UnmanagedCallersOnly]
            static unsafe void Callback(pa_context* ctx, pa_sink_info* info, int eol, void* userdata)
            {
                // Test for the end of list
                if (eol <= 0)
                {
                    // Not the end of the list

                    // Copy the sink_info data to the static structure
                    //sink_info = Marshal.PtrToStructure<pa_sink_info>((IntPtr)info);

                    // Flag that there is valid data
                    gotSink = CallbackState.HasData;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(1);
                }
                else
                {
                    // End of the list

                    // Flag that we have reached the end of the list
                    gotSink = CallbackState.Success;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(0);
                }

            }
            #endregion

            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task.Run(() =>
            {
                SinkInfo? info = default;

                // Lock the context so that we can remain thread-safe
                Monitor.Enter(this);

                // Set flag to 'pending'
                gotSink = CallbackState.Pending;

                // Lock the mainloop
                _mainLoop.Lock();

                // Call the native get_sink_info native function passing in the name and callback
                pa_operation* op = pa_context.pa_context_get_sink_info_by_index(pa_Context, index, &Callback, null);

                // Wait for the callback to signal
                while (gotSink == CallbackState.Pending)
                    _mainLoop.Wait();

                // Dereference the pa_operation
                pa_operation.pa_operation_unref(op);

                // If the callback returned data
                if (gotSink == CallbackState.HasData)
                {
                    // Copy the unmanaged sink_info structure into a SinkInfo object
                    info = SinkInfo.Convert(*sink_info);
                }

                // Allow PulseAudio to free the sink_info
                _mainLoop.Accept();

                // Set the flag to 'pending'
                gotSink = CallbackState.Pending;

                // Call the native get_sink_info passing in the name and callback.
                // It is required to call this again even though we are only requesting
                // info on a single sink. This finishes the request and sets the state to 'success'.
                op = pa_context.pa_context_get_sink_info_by_index(pa_Context, index, &Callback, null);

                // Wait until the callback signals
                while (gotSink == CallbackState.Pending)
                    _mainLoop.Wait();

                // Dereference the pa_operation
                pa_operation.pa_operation_unref(op);

                // Unlock the mainloop
                _mainLoop.Unlock();


                // Unlock the context
                Monitor.Exit(this);

                // Return the SinkInfo object
                return info;
            });
        }

        /// <summary>
        /// Gets the list of sinks on currently on the server
        /// </summary>        
        /// <returns>List of SinkInfo objects</returns>
        public Task<IReadOnlyList<SinkInfo?>> GetSinkInfoListAsync()
        {
            #region Local Functions
            // Callback for the get_sink_info call
            [UnmanagedCallersOnly]
            static unsafe void Callback(pa_context* ctx, pa_sink_info* info, int eol, void* userdata)
            {
                // Test for the end of list
                if (eol == 0)
                {
                    // Not the end of the list

                    // Copy the sink_info data to the static structure                    
                    sink_info = info;

                    // Flag that there is valid data
                    gotSink = CallbackState.HasData;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(1);
                }
                else
                {
                    // End of the list

                    // Flag that we have reached the end of the list
                    gotSink = CallbackState.Success;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(0);
                }

            }
            #endregion

            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task.Run(() =>
            {
                List<SinkInfo?> sinks = new();

                // Lock the context so that we can remain thread-safe
                Monitor.Enter(this);

                // Lock the mainloop
                _mainLoop.Lock();

                do
                {
                    // Set flag to 'pending'
                    gotSink = CallbackState.Pending;

                    // Call the native get_sink_info native function passing in the name and callback
                    pa_operation* op = pa_context.pa_context_get_sink_info_list(pa_Context, &Callback, null);

                    // Wait for the callback to signal
                    while (gotSink == CallbackState.Pending)
                        _mainLoop.Wait();

                    // If the callback returned data
                    if (gotSink == CallbackState.HasData && sink_info != null)
                    {
                        
                        // Copy the unmanaged sink_info structure into a SinkInfo object
                        sinks.Add(SinkInfo.Convert(*sink_info));

                        
                        // Allow PulseAudio to free the sink_info
                        _mainLoop.Accept();

                        
                        _mainLoop.Wait();

                        
                        // Dereference the pa_operation
                        pa_operation.pa_operation_unref(op);

                        continue;
                    }
                    else
                    {
                        pa_operation.pa_operation_unref(op);
                        break;
                    }

                } while (gotSink != CallbackState.Success);

                
                // Unlock the mainloop
                _mainLoop.Unlock();


                // Unlock the context
                Monitor.Exit(this);

                // Return the SinkInfo object
                return (IReadOnlyList<SinkInfo?>)sinks;
            });
        }
        #endregion

    }
}
