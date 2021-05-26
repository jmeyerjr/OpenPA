using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace OpenPA.Interop
{
    public static class NativeLibraryLoader
    {
        public static unsafe void Load<T>()
        {
            Type type = typeof(T);
            Load(type);
        }
        public static void Load(Type type)
        { 
            NativeLibraryAttribute attr = (NativeLibraryAttribute)type.GetCustomAttributes(true).FirstOrDefault(o => o is NativeLibraryAttribute);
            if (attr != null)
            {
                
                string library = attr.LibraryName;
                if (NativeLibrary.TryLoad(library, out IntPtr libHandle))
                {
                    var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach(var field in fields)
                    {
                        if (field.GetCustomAttributes(typeof(NativeMethod), true).Length > 0)
                        {
                            string funcName = field.Name;
                            if (NativeLibrary.TryGetExport(libHandle, funcName, out IntPtr address))
                            {
                                Console.WriteLine("Func: {0} [0x{1:x}]", funcName, address);
                                field.SetValue(null, address);
                            }
                        }
                                
                    }
                }
            }
        }
    }
}
