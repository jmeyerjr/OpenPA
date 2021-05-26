using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using size_t = System.UInt32;

namespace OpenPA.Native
{
    [NativeLibrary("libpulse.so.0")]
    public static unsafe class xmalloc
    {
        // Allocate the specified number of bytes, just like malloc() does. However, in case of OOM, terminate
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<size_t, void*> pa_xmalloc;

        // Same as pa_xmalloc(), but initialize allocated memory to 0
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<size_t,void*> pa_xmalloc0;

        // The combination of pa_xmalloc() and realloc()
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<void*,size_t,void*> pa_xrealloc;

        // Free allocated memory
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<void*,void> pa_xfree;

        // Duplicate the specified string, allocating memory with pa_xmalloc()
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,IntPtr> pa_xstrdup;

        // Duplicate the specified string, but truncate aftee n characters
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<IntPtr,size_t,IntPtr> pa_xstrndup;

        // Duplicated the specified memory block;
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<void*, size_t, void*> pa_xmemdup;
    }
}
