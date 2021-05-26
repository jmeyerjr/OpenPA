using System;
using System.Collections.Generic;
using System.Text;

namespace OpenPA.Interop
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NativeLibraryAttribute : Attribute
    {
        public string LibraryName { get; }
        public NativeLibraryAttribute(string libraryName)
        {
            LibraryName = libraryName;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NativeMethod : Attribute
    {        
    }
}
