using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPA.Enums;
using OpenPA.Interop;
using pa_volume_t = System.UInt32;
using size_t = System.UInt32;
using pa_channel_position_mask_t = System.UInt64;

namespace OpenPA.Native
{

    /// <summary>
    /// A Structure encapsulating a per-channel volume.
    /// </summary>
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_cvolume
    {
        public byte channels;      // Number of channels
        public fixed pa_volume_t values[(int)ChannelPosition.MAX];

        // Normal volume (100%, 0 dB)
        public const pa_volume_t PA_VOLUME_NORM = 0x10000;
        // Muted (minimal valid) volume (0%, -inf dB)
        public const pa_volume_t PA_VOLUME_MUTED = 0;
        // Maximum valid volume we can store.
        public const pa_volume_t PA_VOLUME_MAX = UInt32.MaxValue / 2;
        /** Recommended maximum volume to show in user facing UIs.
        * Note: UIs should deal gracefully with volumes greater than this value
        * and not cause feedback loops etc. - i.e. if the volume is more than
        * this, the UI should not limit it and push the limited value back to
        * the server. \since 0.9.23 */
        //public static readonly pa_volume_t PA_VOLUME_UI_MAX = pa_sw_volume_from_dB(+11.0f);

        // Special 'invalid' volume.
        public const pa_volume_t PA_VOLUME_INVALID = UInt32.MaxValue;

        // Check if volume is valid.
        public static bool PA_VOLUME_IS_VALID(pa_volume_t v) => v <= PA_VOLUME_MAX;



        // Return non-zero when *a == *b, checking that both a and b
        // have the same number of channels and that the volumes of
        // channels in a equal those in b.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_cvolume*,int> pa_cvolume_equal;

        // Initialize the specified volume and return a pointer to
        // it. The sample spec will have a defined state but
        // pa_cvolume_valid() will fail for it.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_cvolume*> pa_cvolume_init;

        // Set the volume of the first n channels to PA_VOLUME_NORM
        public static pa_cvolume* pa_cvolume_reset(pa_cvolume* a, uint n) => pa_cvolume_set(a, n, PA_VOLUME_NORM);

        // Set the volume of the first n channels to PA_VOLUME_MUTED
        public static pa_cvolume* pa_cvolume_mute(pa_cvolume* a, uint n) => pa_cvolume_set(a, n, PA_VOLUME_MUTED);

        // Set the volume of te specified number of channels to the volume v
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,uint,pa_volume_t,pa_cvolume*> pa_cvolume_set;

        /** Maximum length of the strings returned by
        * pa_cvolume_snprint(). Please note that this value can change with
        * any release without warning and without being considered API or ABI
        * breakage. You should not use this definition anywhere where it
        * might become part of an ABI.*/
        public const int PA_CVOLUME_SNPRINT_MAX = 320;

        // Pretty print a volume structure. Returns s.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,pa_cvolume*,IntPtr> pa_cvolume_snprint;

        /** Maximum length of the strings returned by
        * pa_sw_cvolume_snprint_dB(). Please note that this value can change with
        * any release without warning and without being considered API or ABI
        * breakage. You should not use this definition anywhere where it
        * might become part of an ABI. \since 0.9.13 */
        public const int PA_SW_CVOLUME_SNPRINT_DB_MAX = 448;

        // Pretty print a volume structure, showing dB values. Returns s.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,pa_cvolume*,IntPtr> pa_sw_cvolume_snprint_dB;

        /** Maximum length of the strings returned by pa_cvolume_snprint_verbose().
        * Please note that this value can change with any release without warning and
        * without being considered API or ABI breakage. You should not use this
        * definition anywhere where it might become part of an ABI. \since 5.0 */
        public const int PA_CVOLUME_SNPRINT_VERBOSE_MAX = 1984;

        /** Pretty print a volume structure in a verbose way. The volume for each
        * channel is printed in several formats: the raw pa_volume_t value,
        * percentage, and if print_dB is non-zero, also the dB value. If map is not
        * NULL, the channel names will be printed. Returns \a s. \since 5.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,pa_cvolume*,pa_channel_map*,int,IntPtr> pa_cvolume_snprint_verbose;

        /** Maximum length of the strings returned by
        * pa_volume_snprint(). Please note that this value can change with
        * any release without warning and without being considered API or ABI
        * breakage. You should not use this definition anywhere where it
        * might become part of an ABI. \since 0.9.15 */
        public const int PA_VOLUME_SNPRINT_MAX = 10;

        // Pretty print a volume. Returns s.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,pa_volume_t,IntPtr> pa_volume_snprint;

        /** Maximum length of the strings returned by
        * pa_sw_volume_snprint_dB(). Please note that this value can change with
        * any release without warning and without being considered API or ABI
        * breakage. You should not use this definition anywhere where it
        * might become part of an ABI. \since 0.9.15 */
        public const int PA_SW_VOLUME_SNPRINT_DB_MAX = 11;

        // Pretty print a volume but show dB values. Returns s.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,pa_volume_t,IntPtr> pa_sw_volume_snprint_dB;

        /** Maximum length of the strings returned by pa_volume_snprint_verbose().
        * Please note that this value can change with any release without warning and
        * withou being considered API or ABI breakage. You should not use this
        * definition anywhere where it might become part of an ABI. \since 5.0 */
        public const int PA_VOLUME_SNPRINT_VERBOSE_MAX = 35;

        /** Pretty print a volume in a verbose way. The volume is printed in several
        * formats: the raw pa_volume_t value, percentage, and if print_dB is non-zero,
        * also the dB value. Returns \a s. \since 5.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,pa_volume_t,int,IntPtr> pa_volume_snprint_verbose;

        // Return the average volume of all channels
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_volume_t> pa_cvolume_avg;

        /** Return the average volume of all channels that are included in the
        * specified channel map with the specified channel position mask. If
        * cm is NULL this call is identical to pa_cvolume_avg(). If no
        * channel is selected the returned value will be
        * PA_VOLUME_MUTED. \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*,pa_channel_position_mask_t,pa_volume_t> pa_cvolume_avg_mask;

        // Return the maximum volume of all channels.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_volume_t> pa_cvolume_max;

        /** Return the maximum volume of all channels that are included in the
        * specified channel map with the specified channel position mask. If
        * cm is NULL this call is identical to pa_cvolume_max(). If no
        * channel is selected the returned value will be PA_VOLUME_MUTED.
        * \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*,pa_channel_position_mask_t,pa_volume_t> pa_cvolume_max_mask;

        // Return the minimum volume of all channels.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_volume_t> pa_cvolume_min;

        /** Return the minimum volume of all channels that are included in the
        * specified channel map with the specified channel position mask. If
         * cm is NULL this call is identical to pa_cvolume_min(). If no
        * channel is selected the returned value will be PA_VOLUME_MUTED.
        * \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*,pa_channel_position_mask_t,pa_volume_t> pa_cvolume_min_mask;

        // Return non-zero when the passed cvolume structure is valid
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,int> pa_cvolume_valid;

        // Return non-zero if the volume of all channels is equal to the specified value
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_volume_t,int> pa_cvolume_channels_equal_to;

        // Return 1 if the specified volume has all channels muted
        public static int pa_cvolume_is_muted(pa_cvolume* a) => pa_cvolume_channels_equal_to(a, PA_VOLUME_MUTED);

        // Return 1 if the specified volume has all channels on normal level
        public static int pa_cvolume_is_norm(pa_cvolume* a) => pa_cvolume_channels_equal_to(a, PA_VOLUME_NORM);

        /** Multiply two volume specifications, return the result. This uses
        * PA_VOLUME_NORM as neutral element of multiplication. This is only
        * valid for software volumes! */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_volume_t, pa_volume_t,pa_volume_t> pa_sw_volume_multiply;

        /** Multiply two per-channel volumes and return the result in
        * *dest. This is only valid for software volumes! a, b and dest may
        * point to the same structure. Returns dest, or NULL on error. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_cvolume*,pa_cvolume*,pa_cvolume*> pa_sw_cvolume_multiply;

        /** Multiply a per-channel volume with a scalar volume and return the
        * result in *dest. This is only valid for software volumes! a
        * and dest may point to the same structure. Returns dest, or NULL on error.
        * \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_cvolume*,pa_volume_t,pa_cvolume*> pa_sw_cvolume_multiply_scalar;

        /** Divide two volume specifications, return the result. This uses
        * PA_VOLUME_NORM as neutral element of division. This is only valid
        * for software volumes! If a division by zero is tried the result
        * will be 0. \since 0.9.13 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_volume_t,pa_volume_t,pa_volume_t> pa_sw_volume_divide;

        /** Divide two per-channel volumes and return the result in
        * *dest. This is only valid for software volumes! a, b
        * and dest may point to the same structure. Returns dest,
        * or NULL on error. \since 0.9.13 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_cvolume*,pa_cvolume*,pa_cvolume*> pa_sw_cvolume_divide;

        /** Divide a per-channel volume by a scalar volume and return the
        * result in *dest. This is only valid for software volumes! a
        * and dest may point to the same structure. Returns dest,
        * or NULL on error. \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_cvolume*, pa_volume_t, pa_cvolume*> pa_sw_cvolume_divide_scalar;

        // Convert a decibel value to a volume (amplitude, not power). This is only valid for software volumes!
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<double, pa_volume_t> pa_sw_volume_from_dB;

        // Convert a volume to a decibel value (aplitude, not power). This is only valid for software volumes!
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_volume_t,double> pa_sw_volume_to_dB;

        // Convert a linear factor to a volume. 0.0 and less is muted while
        // 1.0 is PA_VOLUME_NORM. This is only valid for software volumes!
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<double, pa_volume_t> pa_sw_volume_from_linear;

        // Convert a volume to a linear factor. This is only vlid for software volumes!
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_volume_t, double> pa_sw_volume_to_linear;

        // This floor value is used as minus infinity when using pa_sw_volume_to_dB() / pa_sw_volume_from_dB().
        public const double PA_DECIBEL_MININFTY = -200.0;

        // Remap a volume from one channel mapping to a different channel mapping.
        // Returns v.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_channel_map*, pa_channel_map*> pa_cvolume_remap;

        // Return non-zero if the specified volume is compatible with the
        // specified sample spec.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_sample_spec*,int> pa_cvolume_compatible;

        // Return non-zero if the specified volume is compatible with the
        // specified sample spec.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_channel_map*,int> pa_cvolume_compatible_with_channel_map;

        /** Calculate a 'balance' value for the specified volume with the
        * specified channel map. The return value will range from -1.0f
        * (left) to +1.0f (right). If no balance value is applicable to this
        * channel map the return value will always be 0.0f. See
        * pa_channel_map_can_balance(). \since 0.9.15 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*,float> pa_cvolume_get_balance;

        /** Adjust the 'balance' value for the specified volume with the
        * specified channel map. v will be modified in place and
        * returned. The balance is a value between -1.0f and +1.0f. This
        * operation might not be reversible! Also, after this call
        * pa_cvolume_get_balance() is not guaranteed to actually return the
        * requested balance value (e.g. when the input volume was zero anyway for
        * all channels). If no balance value is applicable to
        * this channel map the volume will not be modified. See
        * pa_channel_map_can_balance(). Will return NULL on error. \since 0.9.15 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*,float,pa_cvolume*> pa_cvolume_set_balance;

        /** Calculate a 'fade' value (i.e.\ 'balance' between front and rear)
        * for the specified volume with the specified channel map. The return
        * value will range from -1.0f (rear) to +1.0f (left). If no fade
        * value is applicable to this channel map the return value will
        * always be 0.0f. See pa_channel_map_can_fade(). \since 0.9.15 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*> pa_cvolume_get_fade;

        /** Adjust the 'fade' value (i.e.\ 'balance' between front and rear)
        * for the specified volume with the specified channel map. v will be
        * modified in place and returned. The balance is a value between
        * -1.0f and +1.0f. This operation might not be reversible! Also,
        * after this call pa_cvolume_get_fade() is not guaranteed to actually
        * return the requested fade value (e.g. when the input volume was
        * zero anyway for all channels). If no fade value is applicable to
        * this channel map the volume will not be modified. See
        * pa_channel_map_can_fade(). Will return NULL on error. \since 0.9.15 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*,float,pa_cvolume*> pa_cvolume_set_fade;

        /** Calculate a 'lfe balance' value for the specified volume with
        * the specified channel map. The return value will range from
        * -1.0f (no lfe) to +1.0f (only lfe), where 0.0f is balanced.
        * If no value is applicable to this channel map the return value
        * will always be 0.0f. See pa_channel_map_can_lfe_balance(). \since 8.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*> pa_cvolume_get_lfe_balance;

        /** Adjust the 'lfe balance' value for the specified volume with
        * the specified channel map. v will be modified in place and returned.
        * The balance is a value between -1.0f (no lfe) and +1.0f (only lfe).
        * This operation might not be reversible! Also, after this call
        * pa_cvolume_get_lfe_balance() is not guaranteed to actually
        * return the requested value (e.g. when the input volume was
        * zero anyway for all channels). If no lfe balance value is applicable to
        * this channel map the volume will not be modified. See
        * pa_channel_map_can_lfe_balance(). Will return NULL on error. \since 8.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_channel_map*,float,pa_cvolume*> pa_cvolume_set_lfe_balance;

        /** Scale the passed pa_cvolume structure so that the maximum volume
        * of all channels equals max. The proportions between the channel
        * volumes are kept. Returns \a v, or NULL on error. \since 0.9.15 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_volume_t,pa_cvolume*> pa_cvolume_scale;

        /** Scale the passed pa_cvolume structure so that the maximum volume
        * of all channels selected via cm/mask equals max. This also modifies
        * the volume of those channels that are unmasked. The proportions
        * between the channel volumes are kept. If cm is NULL this call is
        * identical to pa_cvolume_scale(). Returns \a v, or NULL on error.
        * \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*,pa_volume_t,pa_channel_map*,pa_channel_position_mask_t,pa_cvolume*> pa_cvolume_scale_mask;

        /** Set the passed volume to all channels at the specified channel
        * position. Will return the updated volume struct, or NULL if there
        * is no channel at the position specified. You can check if a channel
        * map includes a specific position by calling
        * pa_channel_map_has_position(). Returns \a cv, or NULL on error.
        * \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_channel_map*, ChannelPosition, pa_volume_t, pa_cvolume*> pa_cvolume_set_position;

        /** Get the maximum volume of all channels at the specified channel
        * position. Will return 0 if there is no channel at the position
        * specified. You can check if a channel map includes a specific
        * position by calling pa_channel_map_has_position(). \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_channel_map*, ChannelPosition, pa_volume_t> pa_cvolume_get_position;

        /** This goes through all channels in a and b and sets the
        * corresponding channel in dest to the greater volume of both. a, b
        * and dest may point to the same structure. Returns \a dest, or NULL
        * on error. \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_cvolume*, pa_cvolume*, pa_cvolume*> pa_cvolume_merge;

        /** Increase the volume passed in by 'inc', but not exceeding 'limit'.
        * The proportions between the channels are kept.
        * Returns \a v, or NULL on error. \since 0.9.19 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume, pa_volume_t, pa_volume_t, pa_cvolume*> pa_cvolume_inc_clamp;

        /** Increase the volume passed in by 'inc'. The proportions between
        * the channels are kept. Returns \a v, or NULL on error. \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_volume_t, pa_cvolume*> pa_cvolume_inc;

        /** Decrease the volume passed in by 'dec'. The proportions between
        * the channels are kept. Returns \a v, or NULL on error. \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_cvolume*, pa_volume_t, pa_cvolume*> pa_cvolume_dec;

    }
}
