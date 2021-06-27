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
    public class CardInfo
    {
        /// <summary>
        /// Index of this card
        /// </summary>
        public uint Index { get; init; }
        /// <summary>
        /// Name of this card
        /// </summary>
        public string? Name { get; init; }
        /// <summary>
        /// Index of the owning module, or PA_INVALID_INDEX
        /// </summary>
        public uint OwnerModule { get; init; }
        /// <summary>
        /// Drive name
        /// </summary>
        public string? Driver { get; init; }
        /// <summary>
        /// Available profiles
        /// </summary>
        public IReadOnlyList<CardProfileInfo>? Profiles { get; init; }
        /// <summary>
        /// Active profile
        /// </summary>
        public CardProfileInfo? ActiveProfile { get; init; }
        /// <summary>
        /// Property list
        /// </summary>
        public PropList? PropertyList { get; init; }
        /// <summary>
        /// List of ports
        /// </summary>
        public IReadOnlyList<CardPortInfo>? Ports { get; init; }

        internal static unsafe CardInfo Convert(pa_card_info card_info)
        {
            List<CardProfileInfo> profiles = new();
            for(int i = 0; i < card_info.n_profiles; i++)
            {
                pa_card_profile_info2* p = card_info.profiles2[i];
                if (p != null)
                    profiles.Add(CardProfileInfo.Convert(*p));
                else
                    break;
            }

            List<CardPortInfo> ports = new();
            for (int i = 0; i < card_info.n_ports; i++)
            {
                pa_card_port_info* p = card_info.ports[i];
                if (p != null)
                    ports.Add(CardPortInfo.Convert(*p));
                else
                    break;
            }

            CardInfo cardInfo = new()
            {
                Index = card_info.index,
                Name = Marshal.PtrToStringUTF8(card_info.name),
                OwnerModule = card_info.owner_module,
                Driver = Marshal.PtrToStringUTF8(card_info.driver),
                Profiles = profiles,
                ActiveProfile = CardProfileInfo.Convert(*card_info.active_profile2),
                PropertyList = PropList.Convert(card_info.proplist),
                Ports = ports,
            };

            return cardInfo;
        }
    }

    public class CardPortInfo
    {
        /// <summary>
        /// Name of this port
        /// </summary>
        public string? Name { get; init; }
        /// <summary>
        /// Description of this port
        /// </summary>
        public string? Description { get; init; }
        /// <summary>
        /// The higher this value is, the more useful this port is as default.
        /// </summary>
        public uint Priority { get; init; }
        /// <summary>
        /// Port is available
        /// </summary>
        public bool IsAvailable { get; init; }
        /// <summary>
        /// Direction of this port
        /// </summary>
        public Direction Direction { get; init; }
        /// <summary>
        /// Available profiles
        /// </summary>
        public IReadOnlyList<CardProfileInfo>? Profiles { get; init; }
        /// <summary>
        /// Property list
        /// </summary>
        public PropList? PropertyList { get; init; }
        /// <summary>
        /// Latency offset of the port that gets added to the sink/source latency when the port is active.
        /// </summary>
        public long LatencyOffset { get; init; }

        internal static unsafe CardPortInfo Convert(pa_card_port_info port_info)
        {
            List<CardProfileInfo> profiles = new();

            for(int i = 0; i < port_info.n_profiles; i++)
            {
                pa_card_profile_info2* profile = port_info.profiles2[i];
                if (profile != null)
                    profiles.Add(CardProfileInfo.Convert(*profile));
                else
                    break;
            }

            CardPortInfo cardPortInfo = new()
            {
                Name = Marshal.PtrToStringUTF8(port_info.name),
                Description = Marshal.PtrToStringUTF8(port_info.description),
                Priority = port_info.priority,
                IsAvailable = port_info.available == 1,
                Direction = (Direction)port_info.direction,
                Profiles = profiles,
                PropertyList = PropList.Convert(port_info.proplist),
                LatencyOffset = port_info.latency_offset,
            };

            return cardPortInfo;
        }
    }

    public class CardProfileInfo
    {
        /// <summary>
        /// Name of this profile
        /// </summary>
        public string? Name { get; init; }
        /// <summary>
        /// Description of this profile
        /// </summary>
        public string? Description { get; init; }
        /// <summary>
        /// Number of sinks this profile would create
        /// </summary>
        public uint Sinks { get; init; }
        /// <summary>
        /// Number of sources this profile would create
        /// </summary>
        public uint Sources { get; init; }
        /// <summary>
        /// The higher this value is, the more useful this profile is as a default.
        /// </summary>
        public uint Priority { get; init; }
        /// <summary>
        /// Profile is available
        /// </summary>
        public bool IsAvailable { get; init; }

        internal static CardProfileInfo Convert(pa_card_profile_info2 profile_info)
        {
            CardProfileInfo cardProfileInfo = new()
            {
                Name = Marshal.PtrToStringUTF8(profile_info.name),
                Description = Marshal.PtrToStringUTF8(profile_info.description),
                Sinks = profile_info.n_sinks,
                Sources = profile_info.n_sources,
                Priority = profile_info.priority,
                IsAvailable = profile_info.available == 1,
            };

            return cardProfileInfo;
        }
    }
}
