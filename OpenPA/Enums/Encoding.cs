using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    public enum Encoding
    {
        // Any encoding format, PCM or compressed
        ANY,
        // Any PCM format
        PCM,
        // AC3 data encapsulated in IEC 61937 header/padding
        AC3_IEC61937,
        // EAC3 data encapsulated in IEC 61937 header/padding
        EAC3_IEC61937,
        // MPEG-1 or MPEG-2 (Part 3, not AAC) data encapsulated in IEC 61937 header/padding
        MPEG_IEC61937,
        // DTS data encapsulated in IEC 61937 header/padding
        DTS_IEC61937,
        // MPEG-2 AAC data encapsulated in IEC 61937 header/padding
        MPEG2_AAC_IEC61937,
        // Dolby TrueHD data encapsulated in IEC 61937 header/padding
        TRUEHD_IEC61937,
        // DTS-HD Master Audio encapsulated in IEC 61937 header/padding
        DTSHD_IEC61937,
        // Valid encoding types must be less than this value
        MAX,
        // Represents an invalid encoding
        INVALID = -1
    }
}
