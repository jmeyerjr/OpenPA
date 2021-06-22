using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class ClientInfo
    {
        public uint Index { get; init; }
        public String? Name { get; init; }
        public uint OwnerModule { get; init; }
        public String? Driver { get; init; }
        public PropList? PropertyList { get; init; }

        internal unsafe static ClientInfo Convert(pa_client_info client_info)
        {
            ClientInfo clientInfo = new()
            {
                Index = client_info.index,
                Name = client_info.name != IntPtr.Zero ? Marshal.PtrToStringUTF8(client_info.name) : null,
                OwnerModule = client_info.owner_module,
                Driver = client_info.driver != IntPtr.Zero ? Marshal.PtrToStringUTF8(client_info.driver) : null,
                PropertyList = client_info.proplist != null ? PropList.Convert(client_info.proplist) : null
            };

            return clientInfo;
        }
    }
}
