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

namespace OpenPA
{
    public unsafe partial class PAContext
    {


        #region Source

        #region Callback
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
        #endregion


    }
}
