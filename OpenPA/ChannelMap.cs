using OpenPA.Enums;
using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class ChannelMap
    {
        public byte NumChannels { get; init; }
        public IReadOnlyList<ChannelPosition> Map { get; init; }

        internal unsafe static ChannelMap Convert(pa_channel_map channel_map)
        {
            List<ChannelPosition> channelPositions = new();


            int* channel = channel_map.map;
            for(int i = 0; i < channel_map.channels; i++)
            {
                int pos = *channel;
                ChannelPosition channelPosition = (ChannelPosition)pos;
                channelPositions.Add(channelPosition);
                channel++;
            }

            ChannelMap channelMap = new()
            {
                NumChannels = channel_map.channels,
                Map = channelPositions
            };

            return channelMap;
        }
    }
}
