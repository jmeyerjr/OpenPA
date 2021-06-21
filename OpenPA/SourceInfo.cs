using OpenPA.Enums;
using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA
{
    public class SourceInfo
    {
        /// <summary>
        /// Name of source
        /// </summary>
        public String? Name { get; init; }
        /// <summary>
        /// Index of the source
        /// </summary>
        public uint Index { get; init; }
        /// <summary>
        /// Description of this source
        /// </summary>
        public String? Description { get; init; }
        /// <summary>
        /// Sample spec of this source
        /// </summary>
        public SampleSpec? SampleSpec { get; init; }
        /// <summary>
        /// Channel map of this source
        /// </summary>
        public ChannelMap? ChannelMap { get; init; }
        /// <summary>
        /// Owning module index, or PA_INVALID_INDEX
        /// </summary>
        public uint OwnerModule { get; init; }
        /// <summary>
        /// Volume of the source
        /// </summary>
        public Volume? Volume { get; init; }
        /// <summary>
        /// Mute switch of the sink
        /// </summary>
        public bool Mute { get; init; }
        /// <summary>
        /// If this is a monitor source, the index of the owning sink, otherwise PA_INVALID_INDEX
        /// </summary>
        public uint MonitorOfSink { get; init; }
        /// <summary>
        /// Name of the owning sink
        /// </summary>
        public String? MonitorOfSinkName { get; init; }
        /// <summary>
        /// Length of filled record buffer of this source
        /// </summary>
        public ulong Latency { get; init; }
        /// <summary>
        /// Driver name
        /// </summary>
        public String? Driver { get; init; }
        /// <summary>
        /// Flags
        /// </summary>
        public SourceFlags Flags { get; init; }
        /// <summary>
        /// Property list
        /// </summary>
        public PropList? PropList { get; init; }
        /// <summary>
        /// The latency this device has been configured to
        /// </summary>
        public ulong ConfigfuredLatency { get; init; }
        /// <summary>
        /// Some kind of "base" volume that refers to unamplified/unattenuated volume in the context of the input device
        /// </summary>
        public uint BaseVolume { get; init; }
        /// <summary>
        /// State
        /// </summary>
        public SourceState State { get; init; }
        /// <summary>
        /// Number of volume steps for sources which do not support arbitrary volumes
        /// </summary>
        public uint VolumeSteps { get; init; }
        /// <summary>
        /// Card index or PA_INVALID_INDEX
        /// </summary>
        public uint Card { get; init; }
        /// <summary>
        /// Available profiles
        /// </summary>
        public IReadOnlyList<SourcePortInfo>? Ports { get; init; }
        /// <summary>
        /// Active profile
        /// </summary>
        public SourcePortInfo? ActivePort { get; init; }
        /// <summary>
        /// List of formats supported by this source
        /// </summary>
        public IReadOnlyList<FormatInfo>? Formats { get; init; }

        internal unsafe static SourceInfo Convert(pa_source_info source_Info)
        {
            // Create list of ports
            List<SourcePortInfo> ports = new();
            if (source_Info.n_ports > 0)
            {
                // Point to the first port in the list
                pa_source_port_info* pi = *source_Info.ports;

                // Loop for n_ports times
                for(int i = 0; i < source_Info.n_ports; i++)
                {
                    // Copy the port info into a managed class and add is
                    // to the list
                    ports.Add(SourcePortInfo.Convert(*pi));

                    // Move to the next port in the list
                    pi++;
                }
            }


            // Create list of formats
            List<FormatInfo> formats = new();
            if (source_Info.n_formats > 0)
            {
                // Point to the first format in the list
                pa_format_info* fi = *source_Info.formats;

                // Loop for n_formats times
                for (int i = 0; i < source_Info.n_formats; i++)
                {
                    // Copy the format into a managed class and add it
                    // to the list
                    formats.Add(FormatInfo.Convert(*fi));

                    // Move to the next format in the list
                    fi++;
                }
            }

            SourceInfo sourceInfo = new()
            {
                Name = source_Info.name != IntPtr.Zero ? Marshal.PtrToStringUTF8(source_Info.name) : String.Empty,
                Index = source_Info.index,
                Description = source_Info.description != IntPtr.Zero ? Marshal.PtrToStringUTF8(source_Info.description) : String.Empty,
                SampleSpec = SampleSpec.Convert(source_Info.sample_spec),
                ChannelMap = ChannelMap.Convert(source_Info.channel_map),
                OwnerModule = source_Info.owner_module,
                Volume = Volume.Convert(source_Info.volume),
                Mute = source_Info.mute != 0,
                MonitorOfSink = source_Info.monitor_of_sink,
                MonitorOfSinkName = source_Info.monitor_of_sink_name != IntPtr.Zero ? Marshal.PtrToStringUTF8(source_Info.monitor_of_sink_name) : String.Empty,
                Latency = source_Info.latency,
                Driver = source_Info.driver != IntPtr.Zero ? Marshal.PtrToStringUTF8(source_Info.driver) : String.Empty,
                Flags = source_Info.flags,
                PropList = PropList.Convert(source_Info.proplist),
                ConfigfuredLatency = source_Info.configured_latency,
                BaseVolume = source_Info.base_volume,
                State = source_Info.state,
                VolumeSteps = source_Info.n_volume_steps,
                Card = source_Info.card,    
                Formats = formats,
                Ports = ports,
                ActivePort = SourcePortInfo.Convert(*source_Info.active_port),
            };

            return sourceInfo;
        }
    }
}
