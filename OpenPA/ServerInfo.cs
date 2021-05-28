using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class ServerInfo
    {
        /// <summary>
        /// User name of the daemon process
        /// </summary>
        public String? UserName { get; init; }
        /// <summary>
        /// Host name of the daemon process
        /// </summary>
        public String? HostName { get; init; }
        /// <summary>
        /// Version string of the daemon
        /// </summary>
        public String? ServerVersion { get; init; }
        /// <summary>
        /// Server package name (usually "pulseaudio")
        /// </summary>
        public String? ServerName { get; init; }
        /// <summary>
        /// Name of the default sink
        /// </summary>
        public String? DefaultSinkName { get; init; }
        /// <summary>
        /// Name of the default source
        /// </summary>
        public String? DefaultSourceName { get; init; }
        /// <summary>
        /// A random cookie for identifying this instance of PulseAudio
        /// </summary>
        public uint Cookie { get; init; }
        // public SampleSpec SampleSpec { get; init; }
        // public ChannelMap ChannelMap { get; init; }
    }
}
