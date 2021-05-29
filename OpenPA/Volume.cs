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

        internal unsafe static Volume Convert(pa_cvolume cvolume)
        {
            List<uint> vals = new();

            uint* vol = cvolume.values;
            for(int i = 0; i < cvolume.channels; i++)
            {
                uint v = *vol;
                vals.Add(v);
                vol++;
            }

            Volume volume = new()
            {
                Channels = cvolume.channels,
                Values = vals
            };

            return volume;
        }
    }
}
