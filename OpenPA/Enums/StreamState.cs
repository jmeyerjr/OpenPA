using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    /// <summary>
    /// The state of a stream
    /// </summary>
    public enum StreamState
    {
        UNCONNECTED,
        CREATING,
        READY,
        FAILED,
        TERMINATED,
    }
}
