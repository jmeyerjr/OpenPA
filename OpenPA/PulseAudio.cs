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
        public static void Init()
        {
            var types = typeof(PulseAudio).Assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(NativeLibraryAttribute), true).Length > 0);

            foreach(var type in types)
            {
                NativeLibraryLoader.Load(type);
            }
        }
    }
}
