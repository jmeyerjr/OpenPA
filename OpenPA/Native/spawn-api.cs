using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Native
{
    internal unsafe struct pa_spawn_api
    {
        // Is called just before the fork in the parent process. May be
        // NULL.
        public delegate* unmanaged[Cdecl]<void> prefork;
        // Is called immediately after the fork in the parent
        // process. May be NULL.
        public delegate* unmanaged[Cdecl]<void> postfork;
        // Is called immediately after the fork in the child
        // process. May be NULL. It is not safe to close all file
        // descriptors in this function unconditionally, since a UNIX
        // socket (created using socketpair()) is passed to the new
        // process/
        public delegate* unmanaged[Cdecl]<void> atfork;
    }
}
