using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPA.Enums;
using OpenPA.Interop;
using size_t = System.UInt32;
using pa_usec_t = System.UInt64;
using pa_sample_format_t = OpenPA.Enums.SampleFormat;

namespace OpenPA.Native
{
    // Turn off warning for never-set fields and naming violations.
#pragma warning disable CS0169,CS0649, IDE1006, IDE0051

    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_sample_spec
    {
        // Maximum number of channels
        public const uint CHANNELS_MAX = 32;

        // Maximum allowed sample rate
        public const ulong RATE_MAX = 48000 * 8;

        // The sample format
        public pa_sample_format_t format;

        // The sample rate
        public uint rate;

        // Audio channels (1 for mono, 2 for stereo, ...)
        public byte channels;

        // Return the amount of bytes that constitute playback of one second of
        // audio, with the specified sample spec.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_spec*, size_t> pa_bytes_per_second;

        // Return the size of a frame with the specific sample type
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_spec*, size_t> pa_frame_size;

        // Return the size of a sample with the specific sample type
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_spec*, size_t> pa_sample_size;

        // Similar to pa_sample_size() but take a sample format instead of a
        // full sample spec.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_format_t, size_t> pa_sample_size_of_format;

        // Calculate the time it would take to play a buffer od the specified
        // size with the specified sample type. The return value will always
        // be rounded down for non-integral return values.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<ulong, pa_sample_spec*, pa_usec_t> pa_bytes_to_usec;

        // Calculates the size of a buffer required, for playback duration
        // of the time specified, with the specified sample type. The
        // return value will always be rounded down for non-integral
        // return values.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_usec_t, pa_sample_spec*, size_t> pa_usec_to_bytes;

        // Initialize the specified sample spec and return a pointer to
        // it. The sample spec will have a defined state but
        // pa_sample_spec_valid() will fail for it.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_spec*, pa_sample_spec*> pa_sample_spec_init;

        // Return non-zero if the given integer is a valid sample format.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<uint, int> pa_sample_format_valid;

        // Return non-zero if the rate is withing the supported range.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<UInt32, int> pa_sample_rate_valid;

        // Returns non-zero if the channel cound is withing the supported range.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<byte, int> pa_channels_valid;

        // Returns non-zero whem the rample type specification is value
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_spec*, int> pa_sample_spec_valid;

        // Returns non-zero when the two sample type specifications match
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_spec*, pa_sample_spec*, int> pa_sample_spec_equal;

        // Returns a descriptive string for the specified sample format.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_format_t, void*> pa_sample_format_to_string;

        // Parse a sample format text. Inverse of pa_sample_format_to_string()
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<void*, pa_sample_format_t> pa_parse_sample_format;

        // Mazimum required string length dor
        // pa_sample_spec_snprint(). Please not that this value can changed
        // with any release without warning and without being considered API
        // pr ABI breakage. You should not user this definition anywhere where
        // it might become part of an ABI.
        public const int PA_SAMPLE_SPEC_SNPRINT_MAX = 32;

        // Pretty print a sample type specification to a string.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr, size_t, pa_sample_spec*, IntPtr> pa_sample_spec_snprint;

        // Maximum required string length for pa_bytes_snprint(). Please note
        // that this value can change with any release without warning and
        // without being considered API or ABI breakage. You should not use
        // this definition anywhere it might become part of an ABI.
        public const int PA_BYTES_SNPRINT_MAX = 11;

        // Pretty print a byte size value (i.e. "2.5 MiB")
        public static delegate* unmanaged[Cdecl]<IntPtr, size_t, uint, IntPtr> pa_bytes_snprint;

        // Returns 1 when the specified format is little endian, 0 when
        // big endian. Return -1 when endianess does not apply to the
        // specified format, or endianess is unknown.
        public static delegate* unmanaged[Cdecl]<pa_sample_format_t, int> pa_sample_format_is_le;

        // Returns 1 when the specified format is big endian, 0 when
        // little endian. Returns -1 when endianess does not apply to the
        // specified format, or endianess is unknown.
        public static delegate* unmanaged[Cdecl]<pa_sample_format_t, int> pa_sample_format_is_be;
    }



    // Turn off warning for never-set fields and naming violations.
#pragma warning restore CS0169,CS0649, IDE1006, IDE0051

}
