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


        #region Source
        /// <summary>
        /// State flag for the get_source_info callback
        /// </summary>
        static CallbackState gotSource;
        /// <summary>
        /// Structure to hold the source_info from the callback
        /// </summary>
        static pa_source_info* source_info;

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
            pa_threaded_mainloop* m = (pa_threaded_mainloop*)userdata;

            // Test for the end of list
            if (eol == 0 && info != null)
            {
                // Not the end of the list

                // Copy the source_info address into the static pointer
                source_info = info;

                // Flag that there is valid data
                gotSource = CallbackState.HasData;

                // Signal the mainloop to continue
                pa_threaded_mainloop.pa_threaded_mainloop_signal(m, 1);
            }
            else if (eol < 0)
            {
                gotSource = CallbackState.Failed;
                pa_threaded_mainloop.pa_threaded_mainloop_signal(m, 0);
            }
            else
            {
                gotSource = CallbackState.Success;
                pa_threaded_mainloop.pa_threaded_mainloop_signal(m, 0);
            }

        }

        /// <summary>
        /// Gets a SourceInfo object describing a source
        /// </summary>
        /// <param name="source">Name of the sink</param>
        /// <returns>SourceInfo object</returns>
        public Task<SourceInfo?> GetSourceInfoAsync(string source)
        {
            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task.Run(() =>
            {
                SourceInfo? info = default;

                // Lock the context so that we can remain thread-safe
                Monitor.Enter(this);

                // Copy the name of the sink to an unmanaged buffer
                IntPtr name = Marshal.StringToHGlobalAnsi(source);

                // Set flag to 'pending'
                gotSource = CallbackState.Pending;

                // Lock the mainloop
                _mainLoop.Lock();


                // Call the native get_source_info native function passing in the name and callback
                pa_operation* op = pa_context.pa_context_get_source_info_by_name(pa_Context, name, &SourceCallback, _mainLoop.ptr);

                // Wait for the callback to signal
                while (gotSource == CallbackState.Pending)
                    _mainLoop.Wait();


                // If the callback returned data
                if (gotSource == CallbackState.HasData)
                {
                    // Copy the unmanaged source_info structure into a SourceInfo object
                    info = SourceInfo.Convert(*source_info);

                    // Allow PulseAudio to free the source_info
                    _mainLoop.Accept();

                    // Wait for the operation to complete
                    _mainLoop.Wait();

                }

                // Dereference the pa_operation
                pa_operation.pa_operation_unref(op);

                // Unlock the mainloop
                _mainLoop.Unlock();

                // Free the unmanaged memory that holds the source name
                Marshal.FreeHGlobal(name);

                // Unlock the context
                Monitor.Exit(this);

                // Return the SourceInfo object
                return info;
            });
        }

        /// <summary>
        /// Gets a SourceInfo object describing a source
        /// </summary>
        /// <param name="source">Index of the source</param>
        /// <returns>SourceInfo object</returns>
        public Task<SourceInfo?> GetSourceInfoAsync(uint index)
        {
            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task.Run(() =>
            {
                SourceInfo? info = default;

                // Lock the context so that we can remain thread-safe
                Monitor.Enter(this);

                // Set flag to 'pending'
                gotSource = CallbackState.Pending;

                // Lock the mainloop
                _mainLoop.Lock();

                // Call the native get_source_info native function passing in the name and callback
                pa_operation* op = pa_context.pa_context_get_source_info_by_index(pa_Context, index, &SourceCallback, null);

                // Wait for the callback to signal
                while (gotSource == CallbackState.Pending)
                    _mainLoop.Wait();

                // If the callback returned data
                if (gotSource == CallbackState.HasData)
                {
                    // Copy the unmanaged source_info structure into a SourceInfo object
                    info = SourceInfo.Convert(*source_info);

                    // Allow PulseAudio to free the source_info
                    _mainLoop.Accept();

                    // Wait for the operation to complete
                    _mainLoop.Wait();

                }

                // Dereference the pa_operation
                pa_operation.pa_operation_unref(op);

                // Unlock the mainloop
                _mainLoop.Unlock();

                // Unlock the context
                Monitor.Exit(this);

                // Return the SourceInfo object
                return info;
            });
        }

        /// <summary>
        /// Gets the list of sources on currently on the server
        /// </summary>        
        /// <returns>List of SourceInfo objects</returns>
        public Task<IReadOnlyList<SourceInfo?>> GetSourceInfoListAsync()
        {

            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task.Run(() =>
            {
                List<SourceInfo?> sources = new();

                // Lock the context so that we can remain thread-safe
                Monitor.Enter(this);

                // Lock the mainloop
                _mainLoop.Lock();

                do
                {
                    // Set flag to 'pending'
                    gotSource = CallbackState.Pending;

                    // Call the native get_source_info native function passing in the name and callback
                    pa_operation* op = pa_context.pa_context_get_source_info_list(pa_Context, &SourceCallback, _mainLoop.ptr);

                    // Wait for the callback to signal
                    while (gotSource == CallbackState.Pending)
                        _mainLoop.Wait();


                    // If the callback returned data
                    if (gotSource == CallbackState.HasData)
                    {
                        // Copy the unmanaged source_info structure into a SourceInfo object
                        // and add to the list
                        sources.Add(SourceInfo.Convert(*source_info));

                        // Allow PulseAudio to free the source_info
                        _mainLoop.Accept();
                        _mainLoop.Wait();

                        // Dereference the pa_operation
                        pa_operation.pa_operation_unref(op);

                        continue;
                    }

                    // Dereference the pa_operation
                    pa_operation.pa_operation_unref(op);


                } while (gotSource != CallbackState.Success);



                // Unlock the mainloop
                _mainLoop.Unlock();


                // Unlock the context
                Monitor.Exit(this);

                // Return the SinkInfo object
                return (IReadOnlyList<SourceInfo?>)sources;
            });
        }
        #endregion


    }
}
