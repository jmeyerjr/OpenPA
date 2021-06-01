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
        /// <summary>
        /// Number of channels in this map
        /// </summary>
        public byte NumChannels { get; init; }
        /// <summary>
        /// List of channels
        /// </summary>
        public IReadOnlyList<ChannelPosition>? Map { get; init; }

        /// <summary>
        /// Creates a ChannelMap object from a pa_channel_map structure
        /// </summary>
        /// <param name="channel_map">Unmanaged pa_channel_map structure</param>
        /// <returns>Channel map object</returns>
        internal unsafe static ChannelMap Convert(pa_channel_map channel_map)
        {
            // Create list of channel positions
            List<ChannelPosition> channelPositions = new();

            // Point to the first channel position in the unmanaged list
            int* channel = channel_map.map;

            // Loop through each channel
            for(int i = 0; i < channel_map.channels; i++)
            {
                // Copy the channel position to a local variable
                int pos = *channel;

                // Cast it to a ChannelPosition
                ChannelPosition channelPosition = (ChannelPosition)pos;

                // Add the ChannelPosition to the list
                channelPositions.Add(channelPosition);

                // Move to the next channel position in the list
                channel++;
            }

            // Create a new ChannelMap object and populate it with the data
            // from the unmanaged structure
            ChannelMap channelMap = new()
            {
                NumChannels = channel_map.channels,
                Map = channelPositions
            };

            // Return the ChannelMap object
            return channelMap;
        }

        internal unsafe static pa_channel_map Convert(ChannelMap channelMap)
        {
            pa_channel_map channel_map;

            channel_map.channels = channelMap.NumChannels;

            for (int i = 0; i < channelMap.NumChannels; i++)
            {
                if (channelMap.Map != null)
                {
                    channel_map.map[i] = (int)channelMap.Map[i];
                }
            }

            return channel_map;
        }
    }
}
