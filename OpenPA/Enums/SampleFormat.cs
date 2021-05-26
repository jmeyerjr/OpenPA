using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    public enum SampleFormat
    {
        // Unsigned 8 bit PCM
        U8 = 0,
        // 8 Bit a-law
        ALAW = 1,
        // 8 Bit mu-law
        ULAW = 2,
        // Signed 16 Bit PCM, little endian (PC)
        S16LE = 3,
        // Signed 16 Bit PCM, big endian
        S16BE = 4,
        // 32 Bit IEEE floating point, little endian (PC), range -1.0 to 1.0
        FLOAT32LE = 5,
        // 32 Bit IEEE floating point, big endian, rand -1.0 to 1.0
        FLOAT32BE = 6,
        // Signed 32 Bit PCM, little endian (PC)
        S32LE = 7,
        // Signed 32 Bit PCM, big endian
        S32BE = 8,
        // Signed 24 Bit PCM packed, little endian (PC)
        S24LE = 9,
        // Signed 24 Bit PCM packed, big endian
        S24BE = 10,
        // Signed 24 Bit PCM in LSB of 32 Bit words, little endian (PC)
        S24_32LE = 11,
        // Signed 24 Bit PCM in LSB of 32 Bit words, big endian
        S24_32BE = 12,
        // Upper limit of valid sample types
        SAMPLE_MAX = 13,
        // An invalid valud
        SAMPLE_INVALID = -1,
        // Signed 16 Bit PCM native endian
        S16NE = S16LE,
        // 32 Bit IEEE floating point, native endian
        FLOAT32NE = FLOAT32LE,
        // Signed 32 Bit PCM, native endian
        S32NE = S32LE,
        // Signed 24 Bit PCM packed, native endian
        S24NE = S24LE,
        // Signed 24 Bit PCM in LSB of 32 Bit words, native endian
        S24_32NE = S24_32LE,
        // Signed 16 Bit PCM, reverse endian
        S16RE = S16BE,
        // 32 Bit IEEE floating point, reverse endian
        FLOAT32RE = FLOAT32BE,
        // Signed 32 Bit PCM, reverse endian
        S32RE = S32BE,
        // Signed 24 Bit PCM, packed reverse endian
        S24RE = S24BE,
        // Signed 24 Bit PCM, in LSB of 32 Bit words, reverse endian
        S24_32RE = S24_32BE,
        // Shortcut for PA_SAMPLE_FLOAT32NE
        FLOAT32 = FLOAT32NE
    }

   
}
