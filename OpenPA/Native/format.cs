﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenPA.Enums;
using OpenPA.Interop;
using size_t = System.UInt32;

// Turn off warning for never-set fields and naming violations.
#pragma warning disable CS0169,CS0649, IDE1006, IDE0051



namespace OpenPA.Native
{
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_format_info
    {
        // The encoding used for the format
        public Encoding encoding;

        // Additional encoding-specifi properties such as sample rate, bitreate, etc.
        public pa_proplist* plist;

        // Allocates a new pa_format_info structure. Clients must initialise at
        // least the encoding field themselves. Free with pa_format_info_free.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*> pa_format_info_new;

        // Returns a new pa_format_info struct and repersentring the same format as src.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, pa_format_info*> pa_format_info_copy;

        // Frees a pa_format_info structure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, void> pa_format_info_free;

        // Returns non-zero when the format ingo structure is valid.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, int> pa_format_info_valid;

        // Returns non-zero when the format info structure represents a PCM
        // (i.e. uncompressed data) format.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, int> pa_format_info_is_pcm;

        // Returns non-zero if the format represented by first is a subset of
        // the format represented by second. This means that second must
        // have all the fields that first does, but the reverse need not
        // ne true. This is typically expected to be used to check if a
        // stream's format is compatible with a give sink. In sudh a case,
        // first would be the sink's format and second would be the
        // stream's.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, pa_format_info*, int> pa_format_info_is_compatible;

        // Maximum required string length for
        // pa_format_info_snprint(). Please note that this valude can change
        // with any release without warning and without being considered API
        // or ABI breakage. You should not use this definition anywhere where
        // it might become part of an ABI.
        public const int FORMAT_INFO_SNPRINT_MAX = 256;

        // Make a human-readble string representing the given format. Rerturns s.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr, size_t, pa_format_info*, IntPtr> pa_format_info_snprint;

        // Parse a human-readable string of the form generated by
        // pa_format_info_snprint() into a pa_format_info structure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr, pa_format_info*> pa_format_info_from_string;

        /** Utility function to take a \a pa_sample_spec and generate the corresponding
        * \a pa_format_info.
        *
        * Note that if you want the server to choose some of the stream parameters,
        * for example the sample rate, so that they match the device parameters, then
        * you shouldn't use this function. In order to allow the server to choose
        * a parameter value, that parameter must be left unspecified in the
        * pa_format_info object, and this function always specifies all parameters. An
        * exception is the channel map: if you pass NULL for the channel map, then the
        * channel map will be left unspecified, allowing the server to choose it.
        */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_sample_spec*, pa_channel_map*, pa_format_info*> pa_format_info_from_sample_spec;

        /** Utility function to generate a \a pa_sample_spec and \a pa_channel_map corresponding to a given \a pa_format_info. The
        * conversion for PCM formats is straight-forward. For non-PCM formats, if there is a fixed size-time conversion (i.e. all
        * IEC61937-encapsulated formats), a "fake" sample spec whose size-time conversion corresponds to this format is provided and
        * the channel map argument is ignored. For formats with variable size-time conversion, this function will fail. Returns a
        * negative integer if conversion failed and 0 on success. \since 2.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, pa_channel_map*, int> pa_format_info_to_sample_spec;

        // Gets the type of property key in a give pa_format_info.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, PropType> pa_format_info_get_prop_type;

        // Gets an integer property from the given format info. Returns 0 on success and a negative integer on failure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, int*, int> pa_format_info_get_prop_int;

        // Gets an integer range property from the given format info. Returns 0 on success and a negative integer on failure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, int*, int*, int> pa_format_info_get_prop_int_range;

        // Gets and integer array property from the given format info. values contains the values and n_values contains the
        // number of elements. The caller must free values using pa_xfree. Returns 0 on success and a negative integer on
        // failure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, int**, int*, int> pa_format_info_get_prop_int_array;

        // Get a string property from the given format info. The caller must free the returned string using pa_xfree. Returns
        // 0 on success and a negative integer on failure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, IntPtr*, int> pa_format_info_get_prop_string;

        // Gets a string array property from the given format info. values contains the values and n_values contains
        // the number of elements. The caller must free values using pa_format_info_free_string_array. Returns 0 on success and
        // a negative integer on failure.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, IntPtr**, int*, int> pa_format_info_get_prop_string_array;

        // Free a string array returned by pa_format_info_get_prop_string_array.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr*, int, void> pa_format_info_free_string_array;

        // Gets the sample format stored in the format info. Returns a negative error
        // code on failure. If the sample format property is not set at all, returns a
        // negative integer.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, SampleFormat*, int> pa_format_info_get_sample_format;

        // Gets the sample rate store in the format info. Returns a negative error
        // code on failure. If the sample rate property is not set at all, returns a
        // negative integer.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*,uint*,int> ps_format_info_sample_rate;

        // Gets the channel cound store in the format info. Returns a negative error
        // code on failure. If the channels property is not set at all, returns a
        // negative integer.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, int> pa_format_info_get_channels;

        // Gets the channel map stored in the format info. Returns a negative error
        // code on failure. If the channel map property is not
        // set at all, returns a negative integer.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*,pa_channel_map*,int> pa_format_info_get_channel_map;

        // Sets an integer property on the given format info.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, int, void> ps_format_info_set_prop_int;

        // Sets a property with a list of integer values ont the given format info.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, int*, int, void> pa_format_info_set_prop_int_array;

        // Sets a property which can have any value in a give integer range on the give format info.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*,IntPtr, int, int, void> pa_format_info_set_prop_int_range;

        // Sets a string property on the given format info.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, IntPtr, IntPtr, void> pa_format_info_set_prop_string;

        // Sets a propert with a list of string values on the given format info.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*,IntPtr, IntPtr*, int, void> pa_format_info_set_prop_string_array;

        /** Convenience method to set the sample format as a property on the given
        * format.
        *
        * Note for PCM: If the sample format is left unspecified in the pa_format_info
        * object, then the server will select the stream sample format. In that case
        * the stream sample format will most likely match the device sample format,
        * meaning that sample format conversion will be avoided.
        *  
        * \since 1.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, SampleFormat> pa_format_info_set_sample_format;

        /** Convenience method to set the sampling rate as a property on the given
        * format.
        *
        * Note for PCM: If the sample rate is left unspecified in the pa_format_info
        * object, then the server will select the stream sample rate. In that case the
        * stream sample rate will most likely match the device sample rate, meaning
        * that sample rate conversion will be avoided.
        *
        * \since 1.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, int> pa_format_info_set_rate;

        /** Convenience method to set the number of channels as a property on the given
         * format.
        *  
        * Note for PCM: If the channel count is left unspecified in the pa_format_info
        * object, then the server will select the stream channel count. In that case
        * the stream channel count will most likely match the device channel count,
        * meaning that up/downmixing will be avoided.
        *
        * \since 1.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, int, void> pa_format_info_set_channels;

        /** Convenience method to set the channel map as a property on the given
        * format.
        *
        * Note for PCM: If the channel map is left unspecified in the pa_format_info
        * object, then the server will select the stream channel map. In that case the
        * stream channel map will most likely match the device channel map, meaning
        * that remixing will be avoided.
        *
        * \since 1.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_format_info*, pa_channel_map*, void> pa_format_info_set_channel_map;
    }

    // Turn off warning for never-set fields and naming violations.
#pragma warning restore CS0169,CS0649, IDE1006, IDE0051


}
