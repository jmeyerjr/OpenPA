using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class PropList
    {
        public IReadOnlyList<String?>? Properties { get; init; }

        internal unsafe static PropList Convert(pa_proplist* proplist)
        {
            List<String?> properties = new();

            IntPtr state = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            Marshal.WriteIntPtr(state, IntPtr.Zero);

            do
            {
                IntPtr p = pa_proplist.pa_proplist_iterate(proplist, (void**)state);

                if (p != IntPtr.Zero)
                {
                    string? propName = Marshal.PtrToStringUTF8(p);
                    properties.Add(propName);
                }
                else
                {
                    break;
                }
            } while (Marshal.ReadIntPtr(state) != IntPtr.Zero);

            Marshal.FreeHGlobal(state);

            PropList propList = new()
            {
                Properties = properties
            };

            return propList;
        }
    }
}
