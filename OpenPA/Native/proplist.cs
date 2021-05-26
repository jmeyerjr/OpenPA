using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using size_t = System.UInt32;

// Turn off warning for never-set fields and naming violations.
#pragma warning disable CS0169,CS0649, IDE1006, IDE0051



namespace OpenPA.Native
{
    [NativeLibrary("libpulse.so.0")]
    internal unsafe struct pa_proplist
    {
        // Allocate a property list. Free with pa_proplist_free.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*> pa_proplist_new;

        // Free the property list.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, void> pa_proplist_free;

        // Returns a non-zero value is the key is valid.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr, int> pa_proplist_key_valid;

        // Append a new string entry to the property list, possibly
        // overwriting an already existing entry with the same key. An
        // internal copy of the data passed is made. Will accept only valid
        // UTF-8. Returns zero on success.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, IntPtr, int> pa_proplist_sets;

        // Append a new string entry to the property list, possibly
        // overwriting an already existing entry with the same key. An
        // internal copy of the data passed is made. Will accept only valid
        // UTF-8. The string passed in must contain a '='. Left hand side of
        // the '=' is used as key name, the right hand side as string
        // data. Returns zero on success.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, int> pa_proplist_setp;

        // Append a new string entry to the property list, possibly
        // overwriting an already existing entry with the same key. An
        // internal copy of the data passed is made. Will accept only valid
        // UTF-8. The data can be passed as printf()-style format string with
        // arguments. Returns zero on success.
        //public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, IntPtr, int> pa_proplist_setf;

        // Append a new arbitrary data entry to the property list, possibly
        // overwriting an already existing entry with the same key. An
        // internal copy of the data passed is made.
        // Returns zero on success.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, void*, size_t> pa_proplist_set;

        // Return a string entry for the specified key. Will return NULL if
        // the data is not valid UTF-8. Will return a NUL-terminated string in
        // an internally allocated buffer. The caller should make a copy of
        // the data before accessing the property list again.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, IntPtr> pa_proplist_gets;

        // Store the value for the specified key in data. Will store a
        // NUL-terminated string for string entries. The data pointer returned will
        // point to an internally allocated buffer. The caller should make a
        // copy of the data before the property list is accessed again.
        // Returns zero on success, negative on error.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, void**, size_t> pa_proplist_get;


        // Merge property list "other" into "p", adhereing the merge mode as
        // specified in "mode".        
        [NativeMethod]        
        public static delegate* unmanaged[Cdecl]<pa_proplist*, UpdateMode, pa_proplist*, void> pa_proplist_update;

        // Removes a single entry from the property list, identified be the
        // specified key name. Returns zero on success, negative on error.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, int> pa_proplist_unset;

        // Similar to pa_proplist_unset() but takes an array of keys to
        // remove. The array should be terminated byte a NULL pointer. Returns -1
        // on failure, otherwise the number of entries actually removed (which
        // might even be 0, if there were no matching entruess to 
        // remove).
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr[]> pa_proplist_unset_many;

        // Iterate through the property list. The user should allocate a
        // state variable of type void* and initialize it with NULL. A posinter
        // to this variable should then be passed to pa_proplist_iterate()
        // which should be called in a loop until it returns NULL which
        // signifies EOL. The property list should not be modified during
        // iteration through the list -- with the exception of deleting the
        // current entry. On each incovation this function will return the
        // key string for the next entry. The keys in the property list do not
        // have any particular order.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, void**, IntPtr> pa_proplist_iterate;

        // Format the property list nicely as a human readable string. This
        // workds very much list pa_proplist_to_string_sep() and uses a newline
        // as a separator and appends one final one. Call pa_xfree() on the
        // result.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr> pa_proplist_to_string;

        // Format the property list nicely as a human readable string and
        // choose the separator. Call pa_xfree() on the result.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, IntPtr> pa_proplist_to_string_sep;

        // Allocate a new property list and assign key/value from a human
        // readable string.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr, pa_proplist*> pa_proplist_from_string;

        // Returns 1 if an entry for the specified key exists in the
        // property list. Returns negative on error.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, IntPtr, int> pa_proplist_contains;

        // Remove all entries from the property list object.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, void> pa_proplist_clear;

        // Allocate a new property list and copy over every single entry from
        // the specified list.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, pa_proplist*> pa_proplist_copy;

        // Return the number of entries in the property list.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, uint> pa_proplist_size;

        // Returns 0 when the proplist is empty, positive otherwise
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist, int> pa_proplist_isempty;

        // Returns non-zero when a and b have the same keys and values
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_proplist*, pa_proplist*, int> pa_proplist_equal;
    }

    internal enum UpdateMode
    {
        UPDATE_SET,
        UPDATE_MERGE,
        UPDATE_REPLACE,
    }

    // Turn off warning for never-set fields and naming violations.
#pragma warning restore CS0169,CS0649, IDE1006, IDE0051


}
