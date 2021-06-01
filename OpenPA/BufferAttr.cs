using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class BufferAttr
    {
        /// <summary>
        /// Maximum length of the buffer in bytes. recommended to set to (uint32_t)-1;
        /// </summary>
        public uint MaxLength { get; init; }

        /// <summary>
        /// Target length of the buffer (playback only)
        /// </summary>
        public uint TLength { get; init; }

        /// <summary>
        /// Pre-buffering (playback only) recommended to set to (uint32_t)-1
        /// </summary>
        public uint PreBuf { get; init; }

        /// <summary>
        /// Minimum request (playback only)
        /// </summary>
        public uint MinReq { get; init; }

        /// <summary>
        /// Fragment size (recording only)
        /// </summary>
        public uint FragSize { get; init; }

        internal static BufferAttr Convert(pa_buffer_attr buffer_attr)
        {
            BufferAttr attr = new()
            {
                MaxLength = buffer_attr.maxlength,
                TLength = buffer_attr.tlength,
                PreBuf = buffer_attr.prebuf,
                MinReq = buffer_attr.minreq,
                FragSize = buffer_attr.fragsize,
            };

            return attr;
        }

        internal static pa_buffer_attr Convert(BufferAttr attr)
        {
            pa_buffer_attr buffer_attr;

            buffer_attr.maxlength = attr.MaxLength;
            buffer_attr.tlength = attr.TLength;
            buffer_attr.prebuf = attr.PreBuf;
            buffer_attr.minreq = attr.MinReq;
            buffer_attr.fragsize = attr.FragSize;

            return buffer_attr;
        }
    }
}
