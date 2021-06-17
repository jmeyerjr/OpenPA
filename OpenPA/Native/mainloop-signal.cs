using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// Turn off warning for never-set fields and naming violations.
#pragma warning disable CS0649, IDE1006

namespace OpenPA.Native
{
    // An opaque UNIX signal event source object
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_signal_event
    {                
        // Initialie the UNIX signal subsystem and bing it to the specified main loop
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_mainloop_api*, int> pa_signal_init;
        // Cleanup the signal subsystem        
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<void> pa_signal_done;
        // Create a new UNIX signal event source object
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            int, 
            delegate* <pa_mainloop_api*, pa_signal_event*, int, void*>,
            void*,
            pa_signal_event*> pa_signal_new;
        // Free a UNIX signal event source object        
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_signal_event*, void> pa_signal_free;
        // Set a function that is called when the signal event source is destroyed. Use this to free the userdata argument if required        
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_signal_event*, void> pa_signal_set_destroy;
    }

    // Callback for signal events
    internal unsafe delegate void pa_signal_cb_t(pa_mainloop_api* api, pa_signal_event* e, int sig, void* userdata);
        //delegate* unmanaged[Cdecl]<pa_mainloop_api*, pa_signal_event*, int, void*, void> pa_signal_cb_t;
    // Destroy callback for signal events
    internal unsafe delegate void pa_signal_destroy_cb_t(pa_mainloop_api* api, pa_signal_event* e, void* userdata);

    
}
#pragma warning restore CS0649, IDE1006