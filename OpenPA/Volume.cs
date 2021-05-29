using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class Volume
    {
        /// <summary>
        /// Number of channels
        /// </summary>
        public byte Channels { get; init; }
        /// <summary>
        /// Volume of each channel
        /// </summary>
        public IReadOnlyList<uint>? Values { get; init; }

        /// <summary>
        /// Copies an unmanaged pa_cvolume structure into a Volume object
        /// </summary>
        /// <param name="cvolume">Unmanaged structure</param>
        /// <returns>Volume object</returns>
        internal unsafe static Volume Convert(pa_cvolume cvolume)
        {
            // Create list of volume values for each channel
            List<uint> vals = new();

            // Get a pointer to the first value in the volume array
            uint* vol = cvolume.values;

            // Iterate each channel volume
            for(int i = 0; i < cvolume.channels; i++)
            {
                // Get the value of the volume
                uint v = *vol;
                // Add it to the list
                vals.Add(v);
                // Move the pointer to the next value in the array
                vol++;
            }

            // Create the Volume object and populate it
            Volume volume = new()
            {
                Channels = cvolume.channels,
                Values = vals
            };

            // Return the Volume object
            return volume;
        }
    }
}
