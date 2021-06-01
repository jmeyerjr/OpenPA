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
        /// <summary>
        /// List of properties in this object
        /// </summary>
        public IReadOnlyList<String?>? Properties { get; init; }

        /// <summary>
        /// Copies an unmanaged pa_proplist structure to a PropList object
        /// </summary>
        /// <param name="proplist">Unmanaged pa_proplist structure</param>
        /// <returns>PropList object</returns>
        internal unsafe static PropList Convert(pa_proplist* proplist)
        {
            // Create a list to hold the properties
            List<String?> properties = new();

            // Allocate a state object and initialize it to NULL
            IntPtr state = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            Marshal.WriteIntPtr(state, IntPtr.Zero);

            // Create a pointer for property names
            IntPtr p = IntPtr.Zero;

            // Loop until a NULL value is found
            do
            {
                // Get the next propertie in the list
                p = pa_proplist.pa_proplist_iterate(proplist, (void**)state);

                // If there is a property name
                if (p != IntPtr.Zero)
                {
                    // Marshal the name into a string
                    string? propName = Marshal.PtrToStringUTF8(p);
                    // Add the property to the list
                    properties.Add(propName);
                }
            } while (p != IntPtr.Zero && Marshal.ReadIntPtr(state) != IntPtr.Zero);

            // Free the state object
            Marshal.FreeHGlobal(state);

            // Create and populate a PropList object with the property list
            PropList propList = new()
            {
                Properties = properties
            };

            // Return the PropList object
            return propList;
        }

        internal unsafe static pa_proplist* Convert(PropList propList)
        {
            pa_proplist* proplist;

            proplist = pa_proplist.pa_proplist_new();
            if (proplist == null)
            {
                throw new Exception("Could not create pa_proplist.");
            }

            if (propList.Properties != null)
            {
                foreach (string? prop in propList.Properties)
                {
                    IntPtr ptr = Marshal.StringToHGlobalAnsi(prop);

                    int result = pa_proplist.pa_proplist_setp(proplist, ptr);

                    Marshal.FreeHGlobal(ptr);

                    if (result != 0)
                    {
                        throw new Exception("Can't set property on pa_proplist");
                    }
                }
            }

            return proplist;
        }
    }
}
