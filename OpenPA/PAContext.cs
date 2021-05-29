using OpenPA.Enums;
using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenPA
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void ContextStateCallback(pa_context* ctx, void* userdata);

    public unsafe class PAContext : IDisposable
    {
        private pa_context* pa_Context;
        private bool disposedValue;
        private bool connected;
        public bool Connected => connected;
        private IntPtr ptrState;
        private static MainLoop _mainLoop;

        public PAContext(MainLoop mainLoop, string name)
        {
            _mainLoop = mainLoop;
            IntPtr ptr = Marshal.StringToHGlobalAnsi(name);
            pa_Context = pa_context.pa_context_new(mainLoop.API, ptr);
            Marshal.FreeHGlobal(ptr);

            connected = false;
        }

        public ContextState GetState()
        {
            _mainLoop.Lock();
            ContextState state = (ContextState)pa_context.pa_context_get_state(pa_Context);
            _mainLoop.Unlock();

            return state;
        }

        public Task<bool> ConnectAsync(string server) => Task<bool>.Run(() =>
        {
            Monitor.Enter(this);

            _mainLoop.Lock();

            bool succeeded = Connect(server);

            if (!succeeded)
            {
                return false;
            }

            ContextState state = (ContextState)Marshal.ReadInt32(ptrState);
            Console.WriteLine(state);

            while (state != ContextState.READY)
            {

                if (state == ContextState.FAILED || state == ContextState.TERMINATED)
                {
                    succeeded = false;
                    break;
                }

                _mainLoop.Wait();
                state = (ContextState)Marshal.ReadInt32(ptrState);
                Console.WriteLine(state);
            }

            _mainLoop.Unlock();


            Monitor.Exit(this);

            return succeeded;

        });


        public bool Connect(string server)
        {


            IntPtr ptr = Marshal.StringToHGlobalAnsi(server);


            var result = pa_context.pa_context_connect(pa_Context, ptr, ContextFlags.NOFLAGS, null);


            Marshal.FreeHGlobal(ptr);


            ptrState = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(ptrState, 0);

            pa_context.pa_context_set_state_callback(pa_Context, &StateCallback, (void*)ptrState);


            connected = result >= 0;



            return connected;
        }

        [UnmanagedCallersOnly]
        unsafe static void StateCallback(pa_context* ctx, void* userdata)
        {

            var state = pa_context.pa_context_get_state(ctx);


            int* ready = (int*)userdata;


            *ready = (int)state;

            _mainLoop.Signal(0);
        }


        public void Disconnect()
        {
            if (connected)
            {
                pa_context.pa_context_disconnect(pa_Context);
                connected = false;
            }
        }

        public Task<string?> GetServerNameAsync()
        {
            if (!connected)
                throw new InvalidOperationException("Not connected");

            return Task<string>.Run(() =>
            {
                Monitor.Enter(this);

                _mainLoop.Lock();
                IntPtr ptr = pa_context.pa_context_get_server(pa_Context);

                string? server = Marshal.PtrToStringAnsi(ptr);
                _mainLoop.Unlock();

                Monitor.Exit(this);

                return server;
            });
        }

        public Task<uint> GetServerProtocolVersionAsync()
        {
            if (!connected)
                throw new InvalidOperationException("Not Connected");

            return Task<uint>.Run(() =>
            {
                Monitor.Enter(this);

                _mainLoop.Lock();
                uint ver = pa_context.pa_context_get_server_protocol_version(pa_Context);
                _mainLoop.Unlock();

                Monitor.Exit(this);
                return ver;
            });
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe delegate void CB(pa_context* ctx, pa_server_info* info, void* userdata);
        static string? host_name;
        static pa_server_info server_info;

        public Task<ServerInfo?> GetServerInfoAsync()
        {
            [UnmanagedCallersOnly]
            static unsafe void Callback(pa_context* ctx, pa_server_info* info, void* userdata)
            {

                //byte** i = (byte**)userdata;
                server_info = Marshal.PtrToStructure<pa_server_info>((IntPtr)info);

                host_name = Marshal.PtrToStringUTF8(info->host_name);

                
                _mainLoop.Signal(1);
            }

            return Task.Run(() =>
           {
               ServerInfo? info = default;

               Monitor.Enter(this);

               _mainLoop.Lock();
               pa_operation* op = pa_context.pa_context_get_server_info(pa_Context, &Callback, null);

                //while (pa_operation.pa_operation_get_state(op) == (int)OperationState.RUNNING)
                while (String.IsNullOrEmpty(host_name))
                   _mainLoop.Wait();

               pa_operation.pa_operation_unref(op);

               if (server_info.default_sink_name != IntPtr.Zero)
               {
                   info = ServerInfo.Convert(server_info);                   
               }
               else
               {
                   Console.WriteLine("No default sink");
               }
               _mainLoop.Accept();
               _mainLoop.Unlock();
               Monitor.Exit(this);

               return info;
           });
            

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                Disconnect();
                pa_context.pa_context_unref(pa_Context);
                pa_Context = null;

                if (ptrState != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrState);
                    ptrState = IntPtr.Zero;
                }

                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PAContext()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
