using OpenPA.Enums;
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
    /// <summary>
    /// PulseAudio Context object
    /// </summary>
    public unsafe class PAContext : IDisposable
    {
        #region Fields
        /// <summary>
        /// Pointer to the unmanaged pa_context object
        /// </summary>
        private pa_context* pa_Context;

        /// <summary>
        /// Disposed flag
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Connected flag
        /// </summary>
        private bool connected;

        /// <summary>
        /// Reference to the threaded mainloop object
        /// </summary>
        private static MainLoop? _mainLoop;
        #endregion

        #region Properties
        /// <summary>
        /// True when connected to a PulseAudio server
        /// </summary>
        public bool Connected => connected;
        #endregion

        #region Enums
        private enum CallbackState
        {
            Pending = 0,
            Success = 1,
            HasData = 2,
            Failed = -1,
        }
        #endregion

        #region Constructor
        public PAContext(MainLoop mainLoop, string name)
        {
            // Store a reference to the mainloop object
            _mainLoop = mainLoop;

            // Copy the name of this client to unmanaged memory
            IntPtr ptr = Marshal.StringToHGlobalAnsi(name);

            // Create the unmanaged pa_context object
            pa_Context = pa_context.pa_context_new(mainLoop.API, ptr);

            // Free the client name from unmanaged memory
            Marshal.FreeHGlobal(ptr);

            // Initialize the connected flag
            connected = false;
        }
        #endregion

        #region Connection
        /// <summary>
        /// Returns the current state of the context
        /// </summary>
        /// <returns>Context state</returns>
        public ContextState GetState()
        {
            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            // Lock the mainloop
            _mainLoop.Lock();

            // Call the native function to get the state of the context
            ContextState state = (ContextState)pa_context.pa_context_get_state(pa_Context);

            // Unlock the mainloop
            _mainLoop.Unlock();

            // Return the context state
            return state;
        }

        /// <summary>
        /// Holds state data while connecting
        /// </summary>
        static IntPtr ptrState;

        /// <summary>
        /// Connects to a PulseAudio server
        /// </summary>
        /// <param name="server">Server address (i.e. "tcp:localhost")</param>
        /// <returns>True if successful</returns>
        public Task<bool> ConnectAsync(string server) => Task<bool>.Run(() =>
        {

            #region Local Functions
            // Callback for context state
            [UnmanagedCallersOnly]
            unsafe static void StateCallback(pa_context* ctx, void* userdata)
            {
                // Get the state of the context
                var state = pa_context.pa_context_get_state(ctx);

                if (userdata != null)
                {
                    // Cast the state pointer to an int
                    int* ready = (int*)userdata;

                    // Copy the context state into the buffer
                    *ready = (int)state;
                }

                // Signal the main loop to continue,
                // passing 0 because we are not using any transient data
                _mainLoop?.Signal(0);
            }

            // Helper for connect call
            bool Connect(string server)
            {
                // Copy the server address to unmanaged memory
                IntPtr ptr = Marshal.StringToHGlobalAnsi(server);

                // Call the native function to connect to the server,
                // passing in the server address
                var result = pa_context.pa_context_connect(pa_Context, ptr, ContextFlags.NOFLAGS, null);

                // Free the unmanaged memory buffer
                Marshal.FreeHGlobal(ptr);

                // Allocate unmanaged memory for the current connection state
                // and initialize it to zero
                ptrState = Marshal.AllocHGlobal(sizeof(int));
                Marshal.WriteInt32(ptrState, 0);

                // Set the context state callback
                pa_context.pa_context_set_state_callback(pa_Context, &StateCallback, (void*)ptrState);

                // Return the result (true if no error)
                return true;
            }
            #endregion

            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            // Lock the context object so as not to cause a race condition
            // This allows the context to be thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            _mainLoop.Lock();

            // Begin the connection process
            bool succeeded = Connect(server);

            // If there was an error return false
            if (!succeeded)
            {
                return false;
            }

            // Read the state object to get the initial state
            ContextState state = (ContextState)Marshal.ReadInt32(ptrState);
            Console.WriteLine(state);

            // Loop until the connection completes
            while (state != ContextState.READY)
            {
                // If there is an error connecting, return false
                if (state == ContextState.FAILED || state == ContextState.TERMINATED)
                {
                    succeeded = false;
                    break;
                }

                // Wait on the mainloop until something is signaled
                _mainLoop.Wait();

                // Get the current state
                state = (ContextState)Marshal.ReadInt32(ptrState);
                Console.WriteLine(state);
            }

            // Clear the state callback
            pa_context.pa_context_set_state_callback(pa_Context, null, null);

            // Unlock the mainloop
            _mainLoop.Unlock();



            // Unlock the context object
            Monitor.Exit(this);

            connected = true;

            // Return success
            return succeeded;

        });

        /// <summary>
        /// Disconnects from the connected PulseAudio server
        /// </summary>
        public void Disconnect()
        {
            if (connected)
            {
                // Call the native function to disconnect from the server
                pa_context.pa_context_disconnect(pa_Context);

                // Set the connected flag to 'disconnected' (false)
                connected = false;
            }
        }
        #endregion

        #region Stats
        /// <summary>
        /// Gets the name of the connected server.
        /// </summary>
        /// <returns>Server name (This is usually the address)</returns>
        public Task<string?> GetServerNameAsync()
        {
            if (!connected)
                throw new InvalidOperationException("Not connected");

            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task<string>.Run(() =>
            {
                // Lock the context so that we can remain thread-safe
                Monitor.Enter(this);

                // Lock the mainloop
                _mainLoop.Lock();

                // Call the native function to get the connected server name
                IntPtr ptr = pa_context.pa_context_get_server(pa_Context);

                // Copy the name of the server from unmanaged memory to a string
                string? server = Marshal.PtrToStringAnsi(ptr);

                // Unlock the mainloop
                _mainLoop.Unlock();

                // Unlock the context
                Monitor.Exit(this);

                // Return the server name
                return server;
            });
        }

        /// <summary>
        /// Gets the current protocol version
        /// </summary>
        /// <returns>Protocol version</returns>
        public Task<uint> GetServerProtocolVersionAsync()
        {
            if (!connected)
                throw new InvalidOperationException("Not Connected");
            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task<uint>.Run(() =>
            {
                // Lock the context so we can remain thread-safe
                Monitor.Enter(this);

                // Lock the mainloop
                _mainLoop.Lock();

                // Call the native function to get the protocol version
                uint ver = pa_context.pa_context_get_server_protocol_version(pa_Context);

                // Unlock the mainloop
                _mainLoop.Unlock();

                // Unlock the context
                Monitor.Exit(this);

                // Return the protocol version
                return ver;
            });
        }
        #endregion

        #region Server
        /// <summary>
        /// State flag for the get_server_info callback
        /// 0 = pending, 1 = complete
        /// </summary>
        static CallbackState gotServer;
        /// <summary>
        /// Structure to hold the server_info from the callback
        /// </summary>
        static pa_server_info server_info;

        public Task<ServerInfo?> GetServerInfoAsync()
        {
            #region Local Functions
            // Callback used for the get_server_info call
            [UnmanagedCallersOnly]
            static unsafe void Callback(pa_context* ctx, pa_server_info* info, void* userdata)
            {
                if (info != null)
                {
                    // Copy the returned pa_server_info pointer into the static buffer
                    server_info = Marshal.PtrToStructure<pa_server_info>((IntPtr)info);
                }

                // Flag 'complete'
                gotServer = CallbackState.HasData;

                // Signal the mainloop to continue
                _mainLoop?.Signal(1);
            }
            #endregion

            if (_mainLoop == null)
                throw new InvalidOperationException("MainLoop is null");

            return Task.Run<ServerInfo?>(() =>
            {
                ServerInfo? info = default;

                // Lock the context to remain thread-safe
                Monitor.Enter(this);

                // Set the flag to pending
                gotServer = CallbackState.Pending;

                // Lock the mainloop
                _mainLoop.Lock();

                // Call the native get_server_info function, passing in the callback
                pa_operation* op = pa_context.pa_context_get_server_info(pa_Context, &Callback, null);

                // Wait for the operation to complete,
                // the callback will signal the mainloop when it is called
                while (gotServer == CallbackState.Pending)
                    _mainLoop.Wait();

                // Free the pa_operation handle
                pa_operation.pa_operation_unref(op);

                // Copy the pa_server_info structure to a ServerInfo object
                info = ServerInfo.Convert(server_info);

                // Allow PulseAudio to free the pa_server_info data structure
                _mainLoop.Accept();

                // Unlock the mainloop
                _mainLoop.Unlock();

                // Unlock the context
                Monitor.Exit(this);

                // Return the created ServerInfo object
                return info;
            });


        }
        #endregion        

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

                    // Dereference the pa_operation
                    pa_operation.pa_operation_unref(op);

                    // If the callback returned data
                    if (gotSink == CallbackState.HasData)
                    {
                        // Copy the unmanaged sink_info structure into a SinkInfo object
                        sinks.Add(SinkInfo.Convert(*sink_info));

                        // Allow PulseAudio to free the sink_info
                        _mainLoop.Accept();
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


        #region Source
        /// <summary>
        /// State flag for the get_source_info callback
        /// 0 = pending, 1 = pa_source_info valid, -1 = end of list (no data)
        /// </summary>
        static CallbackState gotSource;
        /// <summary>
        /// Structure to hold the source_info from the callback
        /// </summary>
        static pa_source_info* source_info;

        /// <summary>
        /// Gets a SourceInfo object describing a source
        /// </summary>
        /// <param name="source">Name of the sink</param>
        /// <returns>SourceInfo object</returns>
        public Task<SourceInfo?> GetSourceInfoAsync(string source)
        {
            #region Local Functions
            // Callback for the get_source_info call
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static unsafe void Callback(pa_context* ctx, pa_source_info* info, int eol, void* userdata)
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
            #endregion

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
                pa_operation* op = pa_context.pa_context_get_source_info_by_name(pa_Context, name, &Callback, _mainLoop.ptr);

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
            #region Local Functions
            // Callback for the get_source_info call
            [UnmanagedCallersOnly]
            static unsafe void Callback(pa_context* ctx, pa_source_info* info, int eol, void* userdata)
            {
                // Test for the end of list
                if (eol <= 0)
                {
                    // Not the end of the list

                    // Copy the source_info data to the static structure
                    //source_info = Marshal.PtrToStructure<pa_source_info>((IntPtr)info);

                    // Flag that there is valid data
                    gotSource = CallbackState.HasData;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(1);
                }
                else
                {
                    // End of the list

                    // Flag that we have reached the end of the list
                    gotSource = CallbackState.Success;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(0);
                }

            }
            #endregion

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
                pa_operation* op = pa_context.pa_context_get_source_info_by_index(pa_Context, index, &Callback, null);

                // Wait for the callback to signal
                while (gotSource == CallbackState.Pending)
                    _mainLoop.Wait();

                // Dereference the pa_operation
                pa_operation.pa_operation_unref(op);

                // If the callback returned data
                if (gotSource == CallbackState.HasData)
                {
                    // Copy the unmanaged source_info structure into a SourceInfo object
                    info = SourceInfo.Convert(*source_info);
                }

                // Allow PulseAudio to free the source_info
                _mainLoop.Accept();

                // Set the flag to 'pending'
                gotSource = CallbackState.Pending;

                // Call the native get_source_info passing in the name and callback.
                // It is required to call this again even though we are only requesting
                // info on a single sink. This finishes the request and sets the state to 'success'.
                op = pa_context.pa_context_get_source_info_by_index(pa_Context, index, &Callback, _mainLoop.ptr);

                // Wait until the callback signals
                while (gotSource == CallbackState.Pending)
                    _mainLoop.Wait();

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
            #region Local Functions
            // Callback for the get_source_info call
            [UnmanagedCallersOnly]
            static unsafe void Callback(pa_context* ctx, pa_source_info* info, int eol, void* userdata)
            {
                // Test for the end of list
                if (eol <= 0)
                {
                    // Not the end of the list

                    // Copy the source_info data to the static structure
                    //source_info = Marshal.PtrToStructure<pa_source_info>((IntPtr)info);

                    // Flag that there is valid data
                    gotSource = CallbackState.HasData;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(1);
                }
                else
                {
                    // End of the list

                    // Flag that we have reached the end of the list
                    gotSource = CallbackState.Success;

                    // Signal the mainloop to continue
                    _mainLoop?.Signal(0);
                }

            }
            #endregion

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
                    pa_operation* op = pa_context.pa_context_get_source_info_list(pa_Context, &Callback, _mainLoop.ptr);

                    // Wait for the callback to signal
                    while (gotSource == CallbackState.Pending)
                        _mainLoop.Wait();

                    // Dereference the pa_operation
                    pa_operation.pa_operation_unref(op);

                    // If the callback returned data
                    if (gotSource == CallbackState.HasData)
                    {
                        // Copy the unmanaged source_info structure into a SinkInfo object
                        sources.Add(SourceInfo.Convert(*source_info));

                        // Allow PulseAudio to free the source_info
                        _mainLoop.Accept();
                    }

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


        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer

                // If we are connected to a PulseAudio server, disconnect
                Disconnect();

                // Dereference the pa_context
                pa_context.pa_context_unref(pa_Context);

                // Set our pointer to null
                pa_Context = null;

                // Free the state used in the connect callback            
                Marshal.FreeHGlobal(ptrState);
                ptrState = IntPtr.Zero;

                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~PAContext()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
