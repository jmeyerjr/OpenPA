using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using OpenPA.Interop;

namespace OpenPA
{
    public static class PulseAudio
    {        
        /// <summary>
        /// Initializes the PulseAudio native library functions
        /// </summary>
        public static void Init()
        {
            // Find all types marked with a NativeLibrary attribute
            var types = typeof(PulseAudio).Assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(NativeLibraryAttribute), true).Length > 0);

            // For each type, load the library and map the function calls
            foreach(var type in types)
            {
                NativeLibraryLoader.Load(type);
            }
        }
    }
}
