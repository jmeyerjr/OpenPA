using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    /** Direction bitfield - while we currently do not expose anything bidirectional,
    one should test against the bit instead of the value (e.g.\ if (d & PA_DIRECTION_OUTPUT)),
    because we might add bidirectional stuff in the future. \since 2.0
    */
    [Flags]
    public enum Direction : int
    {
        OUTPUT = 1,
        INPUT = 2
    }
}
