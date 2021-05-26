using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPA.Interop;
using pa_usec_t = System.UInt64;

namespace OpenPA.Native
{
    // Turn off warning for never-set fields and naming violations.
#pragma warning disable CS0169, CS0649, IDE1006, IDE0051

    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct timeval
    {
        // The number of milliseconds in a second
        public const ulong MSEC_PER_SEC = 1000;

        // The number of microseconds in a second
        public const ulong USEC_PER_SEC = 1000000;

        // The number of nanoseconds in a second
        public const ulong NSEC_PER_SEC = 1000000000;

        // The number of microseconds in a millisecond
        public const ulong USEC_PER_MSEC = 1000;

        // The number of nanoseconds in a millisecond
        public const ulong NSEC_PER_MSEC = 1000000;

        // The number of nanoseconds in a microsecond
        public const ulong NSEC_PER_USEC = 1000;

        // Invalid time in usec.
        public const ulong USEC_INVALID = ulong.MaxValue;

        // Biggest time in usec
        public const ulong USEC_MAX = ulong.MaxValue - 1;

        // Return the current wallclock timestamp, just like UNIX gettimeofday().
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*> pa_gettimeofday;

        // Calculate the difference between the two specified timeval structs.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*, timeval*, pa_usec_t> pa_timeval_diff;

        // Compare the two timeval structs and return 0 when equal, negative when a < b, positive otherwise
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*, timeval*, int> pa_timeval_cmp;

        // Return the time difference between now and the specified timestamp
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*, pa_usec_t> pa_timeval_age;

        //Add the specified time in microseconds to the specified timeval structure
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*, pa_usec_t, timeval*> pa_timeval_add;

        // Subtract the specified time in microseconds to the specified timeval structure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*, pa_usec_t, timeval*> pa_timeval_sub;

        // Store the specified usec value in the timeval struct.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*, pa_usec_t, timeval*> pa_timeval_store;

        // Load the specified tv value and return it in usec.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<timeval*, pa_usec_t> pa_timeval_load;
    }

}
// Turn off warning for never-set fields and naming violations.
#pragma warning restore CS0169,CS0649, IDE1006, IDE0051
