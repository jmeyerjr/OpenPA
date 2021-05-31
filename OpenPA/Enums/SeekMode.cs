using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    /** Seek type for pa_stream_write(). */
    public enum SeekMode
    {
        /** Seek relative to the write index. */
        RELATIVE = 0,

        /** Seek relative to the start of the buffer queue. */
        ABSOLUTE = 1,

        /** Seek relative to the read index. */
        RELATIVE_ON_READ = 2,

        /** Seek relative to the current end of the buffer queue. */
        RELATIVE_END = 3,
    }
}
