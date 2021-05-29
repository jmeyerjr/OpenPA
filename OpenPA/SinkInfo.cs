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
    public class SinkInfo
    {
        /// <summary>
        /// Name of sink
        /// </summary>
        public String? Name { get; init; }
        /// <summary>
        /// Index of sink
        /// </summary>
        public uint Index { get; init; }
        /// <summary>
        /// Description of the sink
        /// </summary>
        public String? Description { get; init; }
        /// <summary>
        /// Sample spec of this sink
        /// </summary>
        public SampleSpec? SampleSpec { get; init; }
        /// <summary>
        /// Channel Map
        /// </summary>
        public ChannelMap? ChannelMap { get; init; }
        /// <summary>
        /// Index of the owning module of this sink or PA_INVALID_INDEX
        /// </summary>
        public uint OwnerModule { get; init; }
        //// <summary>
        //// Volume of this sink 
        //// </summary>
        public Volume? Volume { get; init; }        
        /// <summary>
        /// Mute switch of the sink
        /// </summary>
        public bool Mute { get; init; }
        /// <summary>
        /// Index of the monitor source connected to this sink
        /// </summary>
        public uint MonitorSource { get; init; }
        /// <summary>
        /// The name of the monitor source
        /// </summary>
        public String? MonitorSourceName { get; init; }
        /// <summary>
        /// Length of queued audio in the output buffer
        /// </summary>
        public ulong Latency { get; init; }
        /// <summary>
        /// Driver list
        /// </summary>
        public String? Driver { get; init; }
        /// <summary>
        /// Flags
        /// </summary>
        public SinkFlags Flags { get; init; }
        //public proplist
        /// <summary>
        /// The latency this device has been configured to.
        /// </summary>
        public ulong ConfiguredLatency { get; init; }       
        /// <summary>
        /// State
        /// </summary>
        public SinkState State { get; init; }
        /// <summary>
        /// Number of volume steps for sinks which do not support arbitrary volumes.
        /// </summary>
        public uint VolumeSteps { get; init; }
        /// <summary>
        /// Card index, or PA_INVALID_INDEX
        /// </summary>
        public uint Card { get; init; }
        /// <summary>
        /// List of available ports
        /// </summary>
        public IReadOnlyList<SinkPortInfo>? Ports { get; init; }
        /// <summary>
        /// Copy of active port
        /// </summary>
        public SinkPortInfo? ActivePort { get; init; }
        /// <summary>
        /// List of formats supported by the sink
        /// </summary>
        public IReadOnlyList<FormatInfo>? Formats { get; init; }


        internal unsafe static SinkInfo Convert(pa_sink_info sink_info)
        {
            // Create list of ports
            List<SinkPortInfo> ports = new();
            if (sink_info.n_ports > 0)
            {
                // Point to the first port in the list
                pa_sink_port_info* pi = *sink_info.ports;

                // Loop for n_ports times
                for (int i = 0; i < sink_info.n_ports; i++)
                {
                    // Copy the port into a managed structure and add it
                    // to the list
                    ports.Add(SinkPortInfo.Convert(*pi));

                    // Move to the next port in the list
                    pi++;
                }
            }

            // Create list of formats
            List<FormatInfo> formats = new();
            if (sink_info.n_formats > 0)
            {
                // Point to the first format in the list
                pa_format_info* fi = *sink_info.formats;

                // Loop for n_formats times
                for (int i = 0; i < sink_info.n_formats; i++)
                {
                    // Copy the format into a managed structure and add it
                    // to the list
                    formats.Add(FormatInfo.Convert(*fi));

                    // Move to the next format in the list
                    fi++;
                }
            }

            // Create a SinkInfo object and populate it with data from the unmanaged structure
            SinkInfo info = new()
            {
                Name = sink_info.name != IntPtr.Zero ? Marshal.PtrToStringUTF8(sink_info.name) : String.Empty,
                Description = sink_info.description != IntPtr.Zero ? Marshal.PtrToStringUTF8(sink_info.description) : String.Empty,
                Index = sink_info.index,
                SampleSpec = SampleSpec.Convert(sink_info.sample_spec),
                ChannelMap = ChannelMap.Convert(sink_info.channel_map),
                OwnerModule = sink_info.owner_module,
                Mute = sink_info.mute != 0,
                MonitorSource = sink_info.monitor_source,
                MonitorSourceName = sink_info.monitor_source_name != IntPtr.Zero ? Marshal.PtrToStringUTF8(sink_info.monitor_source_name) : String.Empty,
                Latency = sink_info.latency,
                Driver = sink_info.driver != IntPtr.Zero ? Marshal.PtrToStringUTF8(sink_info.driver) : String.Empty,
                Flags = sink_info.flags,
                ConfiguredLatency = sink_info.configured_latency,
                State = sink_info.state,
                VolumeSteps = sink_info.n_volume_steps,
                Card = sink_info.card,
                Volume = Volume.Convert(sink_info.volume),
                Ports = ports,
                ActivePort = sink_info.active_port != null ? SinkPortInfo.Convert(*sink_info.active_port) : null,
                Formats = formats,
            };

            // Return the created SinkInfo object
            return info;
        }
    }
}
