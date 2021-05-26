using OpenPA.Enums;
using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pa_usec_t = System.UInt64;
using size_t = System.UInt32;

namespace OpenPA.Native
{
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_context
    {
        /** Instantiate a new connection context with an abstract mainloop API
        * and an application name. It is recommended to use pa_context_new_with_proplist()
        * instead and specify some initial properties.*/
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_mainloop_api*, IntPtr, pa_context*> pa_context_new;

        /** Instantiate a new connection context with an abstract mainloop API
        * and an application name, and specify the initial client property
        * list. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_mainloop_api*, IntPtr, pa_proplist*, pa_context*> pa_context_new_with_proplist;

        // Decrease the reference counter of the context by one
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*,void> pa_context_unref;

        // Increase the reference counter of the context by one
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, void> pa_context_ref;

        // Set a callback function that is called whenever the context status changes
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            //delegate* <pa_context*, void*, void>,
            IntPtr,
            void*,
            void> pa_context_set_state_callback;

        // Set a callback function that is called whenever a meta/policy
        // control event is received.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            delegate*<pa_context*, IntPtr, pa_proplist*, void*, void>,
            void*,
            void> pa_context_set_event_callback;

        // Return the error number of the last failed operation
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, int> pa_context_errno;

        // Return non-zero if some data is pending to be written to the connection
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, int> pa_context_is_pending;

        // Return the current context status
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, ContextState> pa_context_get_state;

        /** Connect the context to the specified server. If server is NULL,
        * connect to the default server. This routine may but will not always
        * return synchronously on error. Use pa_context_set_state_callback() to
        * be notified when the connection is established. If flags doesn't have
        * PA_CONTEXT_NOAUTOSPAWN set and no specific server is specified or
        * accessible a new daemon is spawned. If api is non-NULL, the functions
        * specified in the structure are used when forking a new child
        * process. Returns negative on certain errors such as invalid state
        * or parameters. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, IntPtr, ContextFlags, pa_spawn_api*, int> pa_context_connect;

        // Terminate the context connection immediately
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, void> pa_context_disconnect;

        // Drain the context. If there is nothing to drain, the function returns NULL
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            delegate*<pa_context*, void*, void>,
            void*,
            pa_operation*> pa_context_drain;

        /** Tell the daemon to exit. The returned operation is unlikely to
        * complete successfully, since the daemon probably died before
        * returning a success notification */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            delegate*<pa_context*, int, void*, void>,
            void*,
            pa_operation*> ps_context_exit_daemon;

        // Set the name of the default sink.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            delegate*<pa_context*, int, void*, void>,
            void*, 
            pa_operation*> pa_context_set_default_sink;

        // Set the name of the default source.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            delegate*<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_default_source;

        // Returns 1 when the connection is to a local daemon. Return negative when no connection has been made yet.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, int> pa_context_is_local;

        // Set a different application name for context on the server.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            delegate*<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_name;

        // Return the server name this context is connected to.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, IntPtr> pa_context_get_server;

        // Return the protocol version of the library.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, uint> pa_contet_get_protocol_version;

        // Return the protocol version of the connected server.
        // Returns PA_INVALID_INDEX on error.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, uint> pa_context_get_server_protocol_version;

        /** Update the property list of the client, adding new entries. Please
        * note that it is highly recommended to set as many properties
        * initially via pa_context_new_with_proplist() as possible instead a
        * posteriori with this function, since that information may then be
        * used to route streams of the client to the right device. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            UpdateMode,
            pa_proplist*,
            delegate*<pa_context, int, void*, void>, void*,
            pa_operation*> pa_context_proplist_update;

        /** Update the property list of the client, adding new entries. Please
        * note that it is highly recommended to set as many properties
        * initially via pa_context_new_with_proplist() as possible instead a
        * posteriori with this function, since that information may then be
        * used to route streams of the client to the right device. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr[],
            delegate*<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_proplist_remove;

        /** Return the client index this context is
        * identified in the server with. This is useful for usage with the
        * introspection functions, such as pa_context_get_client_info().
        * Returns PA_INVALID_INDEX on error. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, uint> pa_context_get_index;

        // Create a new timer event source for the specified time (wrapper
        // for mainloop->time_new).
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            pa_usec_t,
            delegate*<pa_mainloop_api*, pa_time_event*, timeval*, void*, void>,
            void*,
            pa_time_event*> pa_context_rttime_new;

        // Restart a running or expired timer event source (wrapper for
        // mainloop->time_restart).
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, pa_time_event*, pa_usec_t, void> pa_context_rttime_restart;

        /** Return the optimal block size for passing around audio buffers. It
        * is recommended to allocate buffers of the size returned here when
        * writing audio data to playback streams, if the latency constraints
        * permit this. It is not recommended writing larger blocks than this
        * because usually they will then be split up internally into chunks
        * of this size. It is not recommended writing smaller blocks than
        * this (unless required due to latency demands) because this
        * increases CPU usage. If ss is NULL you will be returned the
        * byte-exact tile size. if ss is invalid, (size_t) -1 will be
        * returned. If you pass a valid ss, then the tile size
        * will be rounded down to multiple of the frame size. This is
        * supposed to be used in a construct such as
        * pa_context_get_tile_size(pa_stream_get_context(s),
        * pa_stream_get_sample_spec(ss)); \since 0.9.20 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, pa_sample_spec*, size_t> pa_context_get_tile_size;

        /** Load the authentication cookie from a file. This function is primarily
        * meant for PulseAudio's own tunnel modules, which need to load the cookie
        * from a custom location. Applications don't usually need to care about the
        * cookie at all, but if it happens that you know what the authentication
        * cookie is and your application needs to load it from a non-standard
        * location, feel free to use this function. \since 5.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, IntPtr, int> pa_context_load_cookie_from_file;

    }
}
