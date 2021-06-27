using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pa_direction_t = System.UInt32;

namespace OpenPA.Native
{
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_direction
    {
        public const uint PA_DIRECTION_OUTPUT = 0x0001;
        public const uint PA_DIRECTION_INPUT = 0x0002;

        /** Return non-zero if the given value is a valid direction (either input,
        * output or bidirectional). \since 6.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_direction_t, int> func_name;

        // Return a textual representation of the direction.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_direction_t, IntPtr> pa_direction_to_string;

    }
}
