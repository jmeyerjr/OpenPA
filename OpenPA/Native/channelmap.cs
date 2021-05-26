using OpenPA.Enums;
using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pa_channel_position_t = System.Int32;
using pa_channel_position_mask_t = System.UInt64;
using size_t = System.UInt32;

namespace OpenPA.Native
{
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_channel_map
    {
        public byte channels;
        public fixed pa_channel_position_t map[(int)ChannelPosition.MAX];

        // Initialize the specified channel map and return a pointer to
        // it. The channel map will have a defined state but
        // pa_channel_map_valid() will fail for it.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, pa_channel_map*> pa_channel_map_init;

        // Initialize the specified channel map for monaural audio and return a pointer to it
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, pa_channel_map*> pa_channel_map_init_mono;

        // Initialize the specified channel map for stereophonic audio and return a pointer to it
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, pa_channel_map*> pa_channel_map_init_stereo;

        // Initialize the specified channel map for the specified number of
        // channels using default lables and return a pointer to it. This call
        // will fail (return NULL) if there is no default channel map known for this
        // specific number of channels and mapping.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, uint, ChannelMapDef, pa_channel_map*> pa_channel_map_init_auto;

        // Similar to pa_channel_map_init_auto() but instead of failed if no
        // default mapping is known with the specified parameters it will
        // synthesize a mapping based on a known mapping with fewer channels
        // and fill up the rest with AUX0..AUX31 channels
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,uint,ChannelMapDef,pa_channel_map*> pa_channel_map_init_extend;

        // Return a text label for the specified channel position
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_position_t, IntPtr> pa_channel_position_to_string;

        // The inverse of pa_channel_position_to_string().
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr, pa_channel_position_t> pa_channel_position_from_string;

        // Return a human readable text label for the specified channel position.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_position_t, IntPtr> pa_channel_position_to_pretty_string;

        /** The maximum length of strings returned by
        * pa_channel_map_snprint(). Please note that this value can change
        * with any release without warning and without being considered API
        * or ABI breakage. You should not use this definition anywhere where
        * it might become part of an ABI. */
        public const int PA_CHANNEL_MAP_SNPRINT_MAX = 336;

        // Make a human readable string from the specified channel map.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,pa_channel_map*,IntPtr> pa_channel_map_snprint;

        // Parse a channel position list or well-known mapping name into a
        // channel map structure. This turns the output of
        // pa_channel_map_snprint() and pa_channel_map_to_name() back into a
        // pa_channel_map
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, IntPtr, pa_channel_map*> pa_channel_map_parse;

        // Compare two channel maps. Return 1 if both match.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, pa_channel_map*, int> pa_channel_map_equal;

        // Return non-zero if this specified channel map is considered valid
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, int> pa_channel_map_valid;

        // Return non-zero if the specified channel map is compatible with
        // the specified sample spec.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,pa_sample_spec*,int> pa_channel_map_compatible;

        // Returns non-zero if every channel defined in b is also defined in a.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,pa_channel_map*,int> pa_channel_map_superset;

        // Returns non-zero if it makes sense to apply a volume 'balance'
        // with this mapping. i.e. if there are left/right channels
        // available.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,int> pa_channel_map_can_balance;

        // Returns non-zero if it makes sense to apply a volume 'fade'
        // (i.e. 'balance' between front and rear) with this mapping. i.e. if
        // there are front/rear channels available.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,int> pa_channel_map_can_fade;

        // Returns non-zero if it makes sense to apply a volume 'lfe balance'
        // (i.e. 'balance' between LFE and non-LFE channels) with this mapping,
        // i.e. if there are LFE and non-LFE channels available.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,int> pa_channel_map_can_lfe_balance;

        // Tries to find a well-known channel mapping name for this channel
        // mapping, i.e. "stereo", "surround-71" and so on. If the channel
        // mapping is unknown NULL will be returned. This name can be parsed
        // with pa_channel_map_parse()
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*, IntPtr> pa_channel_map_to_name;

        // Tries to find a human readable text label for this channel
        // mapping, i.e. "Stereo", "Surround 7.1" and so on. If the channel
        // mapping is unknown NULL will be returned.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,IntPtr> pa_channel_map_to_pretty_name;

        // Returns non-zero if the specified channel position is available at
        // least once in the channel map.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,pa_channel_position_t,int> pa_channel_map_has_position;

        // Generates a bit mask from a channel map.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_channel_map*,pa_channel_position_mask_t> pa_channel_map_mask;

    }
}
