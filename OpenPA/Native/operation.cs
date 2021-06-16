using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pa_operation_state_t = OpenPA.Enums.OperationState;

namespace OpenPA.Native
{
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_operation
    {
        // Increase the reference cound by one
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_operation*, pa_operation*> pa_operation_ref;

        // Decrease the reference cound by one
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_operation*, void> pa_operation_unref;

        /** Cancel the operation. Beware! This will not necessarily cancel the
        * execution of the operation on the server side. However it will make
        * sure that the callback associated with this operation will not be
        * called anymore, effectively disabling the operation from the client
        * side's view. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_operation*, void> pa_operation_cancel;

        // Return the current status of the operation
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_operation*, pa_operation_state_t> pa_operation_get_state;

        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_operation*,
            delegate* <pa_operation*, void*, void>,
            void*,
            void> pa_operation_set_state_callback;

    }
}
