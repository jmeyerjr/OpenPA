using OpenPA.Interop;
using System.Runtime.InteropServices;
using System;

// Turn off warning for never-set fields and naming violations.
#pragma warning disable CS0169,CS0649, IDE1006, IDE0051


namespace OpenPA.Native
{
    /// <summary>
    /// An opaque IO event source object
    /// </summary>
    internal struct pa_io_event
    { }

    /// <summary>
    /// An opaque time event source object
    /// </summary>
    internal struct pa_time_event
    { }



    internal struct pa_defer_event
    { }


    // An IO event callback
    internal unsafe delegate void pa_io_event_cb_t(pa_mainloop_api* mainLoopApi,  pa_io_event* e, int fd, IOEventFlags events,  void* userdata);
    // An IO event destroy callback
    internal unsafe delegate void pa_io_event_destroy_cb_t(pa_mainloop_api* mainLoopApi, pa_io_event* e, void* userdata);
    // A Time event callback
    internal unsafe delegate void pa_time_event_cb_t(pa_mainloop_api* mainLoopApi, pa_time_event* e, timeval* tv, void* userdata);
    // A time event destroy callback
    internal unsafe delegate void pa_time_event_destroy_cb_t(pa_mainloop_api* mainLoopApi, pa_time_event* e, void* userdata);
    // A defer event callback
    internal unsafe delegate void pa_defer_event_cb_t(pa_mainloop_api* mainLoopApi, pa_defer_event* e, void* userdata);
    // A defer event destroy callback
    internal unsafe delegate void pa_defer_event_destroy_cb_t(pa_mainloop_api* mainLoopApi, pa_defer_event* e, void* userdata);

    /** Run the specified callback function once from the main loop using an
    * anonymous defer event. If the mainloop runs in a different thread, you need
    * to follow the mainloop implementation's rules regarding how to safely create
    * defer events. In particular, if you're using \ref pa_threaded_mainloop, you
    * must lock the mainloop before calling this function. */
    internal unsafe delegate void pa_mainloop_api_once_cb(pa_mainloop_api* m, void* userdata);
    internal unsafe delegate void pa_mainloop_api_once(pa_mainloop_api* m, pa_mainloop_api_once_cb callback, void* userdata);

    // An abstract mainloop API vtable
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_mainloop_api
    {
        // A pointer to some private, arbitrary data of the main loop implementation
        void* userdata;

        // Create a new IO event source object
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_mainloop_api*,
            int,
            IOEventFlags,
            delegate* <pa_mainloop_api*, pa_io_event*, int, IOEventFlags, void*, void>,
            void*,
            pa_io_event*> io_new;

        // Enable or disable IO events on this object        
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_io_event*, IOEventFlags, void> io_enable;

        // Free an IO event source object
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_io_event*, void> io_free;

        // Set a function that is called when the IO event source is destroyed. Use thid to free the userdata argument if required
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_io_event*,
            delegate* <pa_mainloop_api*, pa_io_event*, void*, void>,
            void> io_set_destroy;

        // Create a new timer event source object for the specified Unix time
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_mainloop_api*,
            timeval*,
            delegate*<pa_mainloop_api*, pa_time_event*, timeval*, void*, void>,
            void*,
            pa_time_event*> time_new;

        // Restart a running or expired timer event source with a new Unix time
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_time_event*, timeval*, void> time_restart;

        // Free a deferred timer event source object
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_time_event*, void> time_free;

        // Set a function that is called when the time event source is destroyed. Use this to free the userdata argument if required
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_time_event*,
            delegate*<pa_mainloop_api*, pa_time_event*, void*, void>,
            void> time_set_destroy;

        // Create a new deferred event source object
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_mainloop_api*,
            delegate*<pa_mainloop_api*, pa_defer_event*, void*, void>,
            void*,
            pa_defer_event*> defer_new;

        // Enable of disable a deferred event source temporarily
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_defer_event*, int, void> defer_enable;

        // Free a deferred event source object
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_defer_event*, void> defer_free;

        // Set a function that is called when the deferre event source is destroyed. Use this to free the userdata argument if required.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_defer_event*,
            delegate*<pa_mainloop_api*, pa_defer_event*, void*, void>,
            void> defer_set_destroy;

        // Exit the main loop and return the specified retval
        [NativeMethod]
        public static delegate*<pa_mainloop_api*, int, void> quit;
    }

    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_threaded_mainloop
    {
        // Allocate a new threaded main loop object.
        // You have to call pa_threaded_mainloop_start() before the event loop thread starts
        // running. Free with pa_threaded_mainloop_free.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*> pa_threaded_mainloop_new;
        // Free a threaded main loop object. If the event loop thread is
        // still running, terminate it with pa_threaded_mainloop_stop() first.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, void> pa_threaded_mainloop_free;
        // Start the event loop thread. Returns zero on success, negative on error.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, int> pa_threaded_mainloop_start;
        // Terminate the event loop thread cleanly. Make sure to unlock the
        // mainloop object before calling this function.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, void> pa_threaded_mainloop_stop;
        // Lock the event loop object, effectively blocking the event loop
        // thread from processing events. You can use this to enforce
        // exclusive access to all object attached to the event loop. This
        // lock is recursive. This function may not be called inside the event
        // loop thread. Events that are dispatched from the event loop thread
        // are executed with this lock held.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, void> pa_threaded_mainloop_lock;
        // Unlock the event loop object, inverse of pa_threaded_mainloop_lock().
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, void> pa_threaded_mainloop_unlock;
        // Wait for an event to be signalled by the event loop thread. You
        // can use this to pass data from the event loop thread to the main
        // thread in a synchronized fashion. This function may not be called
        // inside the event loop thread. Prior to this call the event loop
        // object needs to be locked using pa_threaded_mainloop_lock(). While
        // waiting the lock will be released. Immediately before returning it
        // will be aquired again. This function may spuriously wake up even
        // without pa_threaded_mainloop_signal() being called. You need to
        // make sure to handle that!
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, void> pa_threaded_mainloop_wait;
        // Signal all the threads waiting for a signalling event in
        // pa_threaded_mainloop_wait(). If wait_for_accept is non-zero, do
        // not return before the signal was accepted be a
        // pa_threaded_mainloop_accept() call. White waiting for that condition
        // the event loop object is unlocked.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, int, void> pa_threaded_mainloop_signal;
        // Accept a signal from the event thread issued with
        // pa_threaded_mainloop_signal(). This call should only be used in
        // conjunction with pa_threaded_mainloop_signal() with a non-zero
        // wait_for_accept value.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, void> pa_threaded_mainloop_accept;
        // Return the return value as specified with the main loop's
        // pa_mainloop_quit() routine.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, int> pa_threaded_mainloop_get_retval;
        // Return the main loop abstraction layer vtable for this main loop.
        // There is no need to free this object as it is owned by the loop
        // and is destroyed when the loop is freed.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, pa_mainloop_api*> pa_threaded_mainloop_get_api;
        // Returns non-zero when called from within the event loop thread.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, int> pa_threaded_mainloop_in_thread;
        // Sets the name of the thread.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_threaded_mainloop*, IntPtr, void> pa_threaded_mainloop_set_name;
        // Runs the given callback in the mainloop thread without the lock held. The
        // caller is responsible for ensuring that PulseAudio data structures are only
        // accessed in a thread-safe way (that is, APIs that take pa_context and
        // pa_stream are not thread-safe, and should not accessed without some
        // synchronisation). This is the only situation in which
        // pa_threaded_mainloop_lock() and pa_threaded_mainloop_unlock() may be used
        // in the mainloop thread context.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_threaded_mainloop*,
            delegate*<pa_threaded_mainloop*, void*, void>,
            void*, void> pa_threaded_mainloop_once_unlocked;
        
    }
}

#pragma warning restore CS0169,CS0649, IDE1006
