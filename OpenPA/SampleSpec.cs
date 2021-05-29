using OpenPA.Enums;
using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class SampleSpec
    {
        /// <summary>
        /// Maximum number of channels supported by PulseAudio
        /// </summary>
        public const byte MaxChannels = 32;
        /// <summary>
        /// Maximum sample rate supported by PulseAudio
        /// </summary>
        public const uint MaxRate = 48000 * 8;
        /// <summary>
        /// Sample rate
        /// </summary>
        public uint Rate { get; init; }

        /// <summary>
        /// Number of channels
        /// </summary>
        public byte Channels { get; init; }

        /// <summary>
        /// Audio sample format
        /// </summary>
        public SampleFormat Format { get; init; }

        internal static SampleSpec Convert(pa_sample_spec pa)
        {
            SampleSpec sampleSpec = new()
            {
                Rate = pa.rate,
                Channels = pa.channels,
                Format = pa.format
            };

            return sampleSpec;
        }
    }
}
