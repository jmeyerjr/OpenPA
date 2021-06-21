using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class ModuleInfo
    {
        /// <summary>
        /// Index of module
        /// </summary>
        public uint Index { get; init; }
        /// <summary>
        /// Name of the module
        /// </summary>
        public String? Name { get; init; }
        /// <summary>
        /// Argument string of the module
        /// </summary>
        public String? Argument { get; init; }
        /// <summary>
        /// Property list
        /// </summary>
        public PropList? PropertyList { get; init; }

        internal unsafe static ModuleInfo Convert(pa_module_info module_info)
        {
            ModuleInfo moduleInfo = new()
            {
                Index = module_info.index,
                Name = Marshal.PtrToStringUTF8(module_info.name),
                Argument = Marshal.PtrToStringUTF8(module_info.argument),
                PropertyList = PropList.Convert(module_info.proplist)
            };

            return moduleInfo;
        }
    }
}
