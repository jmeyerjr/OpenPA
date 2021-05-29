using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class SinkPortInfo
    {
        /// <summary>
        /// Name of this port
        /// </summary>
        public String? Name { get; init; }
        /// <summary>
        /// Description of this port
        /// </summary>
        public String? Description { get; init; }
        /// <summary>
        ///  The higher this value is, the more useful this port is as a default.
        /// </summary>
        public uint Priority { get; init; }
        /// <summary>
        /// A flags indicating availability status of this port.
        /// </summary>
        public bool Available { get; init; }

        internal static SinkPortInfo Convert(pa_sink_port_info port_info)
        {
            SinkPortInfo info = new()
            {
                Name = port_info.name != IntPtr.Zero ? Marshal.PtrToStringUTF8(port_info.name) : String.Empty,
                Description = port_info.description != IntPtr.Zero ? Marshal.PtrToStringUTF8(port_info.description) : String.Empty,
                Priority = port_info.priority,
                Available = port_info.available == 1,
            };

            return info;
        }
    }
}
