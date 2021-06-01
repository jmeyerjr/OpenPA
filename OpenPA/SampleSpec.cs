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

        /// <summary>
        /// Copies an unmanaged pa_sample_spec structure to a SampleSpec object
        /// </summary>
        /// <param name="sample_spec">Unmanaged pa_sample_spec structure</param>
        /// <returns>SampleSpec object</returns>
        internal static SampleSpec Convert(pa_sample_spec sample_spec)
        {
            // Create a SampleSpec object and populate it with data
            // from the sample_spec structure
            SampleSpec sampleSpec = new()
            {
                Rate = sample_spec.rate,
                Channels = sample_spec.channels,
                Format = sample_spec.format
            };

            // Return the SampleSpec object
            return sampleSpec;
        }

        internal static pa_sample_spec Convert(SampleSpec sampleSpec)
        {
            pa_sample_spec sample_spec;

            sample_spec.rate = sampleSpec.Rate;
            sample_spec.channels = sampleSpec.Channels;
            sample_spec.format = sampleSpec.Format;

            return sample_spec;
        }

    }
}
