using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    public enum ContextState
    {
        UNCONNECTED,
        CONNECTING,
        AUTHORIZING,
        SETTING_NAME,
        READY,
        FAILED,
        TERMINATED,
    }

    [Flags]
    public enum ContextFlags
    {
        // Flag to pass when no specific options are needed (used to avoid casting)
        NOFLAGS = 0x0,
        // Disabled autospawning of the PulseAudio daemon if required
        NOAUTOSPAWN = 0x0001,
        // Don't fail if the daemon is not available when pa_context_connect() is
        // called, instead enter PA_CONTEXT_CONNECTION state and wait for the daemon
        // to appear.
        NOFAIL = 0x0002,

    }
}
