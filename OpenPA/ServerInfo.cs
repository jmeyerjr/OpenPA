using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        /// <summary>
        /// Default sample specification
        /// </summary>
        public SampleSpec? SampleSpec { get; init; }
        /// <summary>
        /// Channel map
        /// </summary>
        public ChannelMap ChannelMap { get; init; }

        internal static ServerInfo Convert(pa_server_info server_info)
        {
            ServerInfo info = new()
            {
                ServerName = server_info.server_name != IntPtr.Zero ? Marshal.PtrToStringUTF8(server_info.server_name) : String.Empty,
                ServerVersion = server_info.server_version != IntPtr.Zero ? Marshal.PtrToStringUTF8(server_info.server_version) : String.Empty,
                HostName = server_info.host_name != IntPtr.Zero ? Marshal.PtrToStringUTF8(server_info.host_name) : String.Empty,
                UserName = server_info.user_name != IntPtr.Zero ? Marshal.PtrToStringUTF8(server_info.user_name) : String.Empty,
                DefaultSinkName = server_info.default_sink_name != IntPtr.Zero ? Marshal.PtrToStringUTF8(server_info.default_sink_name) : String.Empty,
                DefaultSourceName = server_info.default_source_name != IntPtr.Zero ? Marshal.PtrToStringUTF8(server_info.default_source_name) : String.Empty,
                Cookie = server_info.cookie,
                SampleSpec = SampleSpec.Convert(server_info.sample_spec),
                ChannelMap = ChannelMap.Convert(server_info.channel_map)
            };

            return info;
        }
    }
}
