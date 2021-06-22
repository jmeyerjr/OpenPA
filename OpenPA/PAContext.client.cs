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
        
        #region Callbacks
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void ClientInfoCallback(pa_context* ctx, pa_client_info* info, int eol, void* userdata)
        {
            if (eol == 0)
            {
                *((pa_client_info**)userdata) = info;

                MainLoop.Instance.Signal(1);
            }
            else
            {
                *((pa_client_info**)userdata) = null;

                MainLoop.Instance.Signal(0);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void KillClientCallback(pa_context* ctx, int result, void* userdata)
        {
            *((bool*)userdata) = result == 1;

            MainLoop.Instance.Signal(0);

        }
        #endregion

        public Task<ClientInfo?> GetClientInfoAsync(uint index) => Task.Run(() => GetClientInfo(index));

        public ClientInfo? GetClientInfo(uint index)
        {
            // Create return object
            ClientInfo? clientInfo = default;

            // Hold lock to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Create a pointer for the return value
            pa_client_info* client_info;

            // Invoke the operation
            pa_operation* op = pa_context_get_client_info(pa_Context, index, &ClientInfoCallback, &client_info);

            // Wait for the operation to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // If the operation returned something
            if (client_info != null)
            {
                // Convert the native structure to a ClientInfo object
                clientInfo = ClientInfo.Convert(*client_info);

                // Allow pulseaudio to free the pa_client_info structure
                MainLoop.Instance.Accept();

                // Signal the mainloop
                MainLoop.Instance.Wait();
            }

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Release the lock
            Monitor.Exit(this);

            // Return the ClientInfo object
            return clientInfo;
        }

        public Task<IReadOnlyList<ClientInfo?>> GetClientInfoListAsync() => Task.Run(GetClientInfoList);

        public IReadOnlyList<ClientInfo?> GetClientInfoList()
        {
            // Returned list
            List<ClientInfo?> clients = new();

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to the returned pa_client_info structure
            pa_client_info* client_info = null;

            // Call the native get_sink_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_client_info_list(pa_Context, &ClientInfoCallback, &client_info);

            // Loop through list of clients
            while (true)
            {
                // Wait for the callback to signal
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && client_info == null)
                    MainLoop.Instance.Wait();

                // If the callback returned data
                if (client_info != null)
                {
                    // Copy the unmanaged sink_info structure into a ClientInfo object
                    // and add it to the list
                    clients.Add(ClientInfo.Convert(*client_info));

                    // Allow PulseAudio to free the pa_client_info structure
                    MainLoop.Instance.Accept();

                    // Reset client_info pointer
                    client_info = null;
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

            // Return the ClientInfo object List
            return clients;

        }


        public bool KillClient(uint index)
        {
            // Hold lock to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Set up somewhere to catch the result
            bool success = false;

            // Invoke the operation
            pa_operation* op = pa_context_kill_client(pa_Context, index, &KillClientCallback, &success);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Release the lock
            Monitor.Exit(this);

            // Return the result
            return success;
        }
    }
}
