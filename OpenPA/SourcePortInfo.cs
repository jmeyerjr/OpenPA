using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class SourcePortInfo
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public uint Priority { get; init; }
        public bool Available { get; init; }

        internal unsafe static SourcePortInfo Convert(pa_source_port_info info)
        {
            SourcePortInfo sourcePortInfo = new()
            {
                Name = Marshal.PtrToStringUTF8(info.name),
                Description = Marshal.PtrToStringUTF8(info.description),
                Priority = info.priority,
                Available = info.available == 1
            };

            return sourcePortInfo;
        }

    }
}
