using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pa_usec_t = System.UInt64;
using pa_volume_t = System.UInt32;
using pa_sink_flags_t = OpenPA.Enums.SinkFlags;
using pa_sink_state_t = OpenPA.Enums.SinkState;
using pa_source_flags_t = OpenPA.Enums.SourceFlags;
using pa_source_state_t = OpenPA.Enums.SourceState;
using System.Runtime.InteropServices;

namespace OpenPA.Native
{

    internal unsafe struct pa_sink_info
    {
        // Name of the sink
        public IntPtr name;
        // Index of the sink
        public uint index;
        // Description of this sink
        public IntPtr description;
        // Sample spec of this sink
        public pa_sample_spec sample_spec;
        // Channel map
        public pa_channel_map channel_map;
        // Index of the owning module of this sink or PA_INVALID_INDEX
        public uint owner_module;
        // Volume of the sink
        public pa_cvolume volume;
        // Mute switch of the sink
        public int mute;
        // Index of the monitor source connected to this sink
        public uint monitor_source;
        // The name of the monitor source
        public IntPtr monitor_source_name;
        // Length of queued audio in the output buffer
        public pa_usec_t latency;
        // Driver name
        public IntPtr driver;
        // Flags
        public pa_sink_flags_t flags;
        // Property list
        public pa_proplist* proplist;
        // The latency this device had been configured to.
        public pa_usec_t configured_latency;
        // Some kind of "base" volume that refers to unamplified/anattenuated volume in the context of the output device.
        public pa_volume_t base_volume;
        // State
        public pa_sink_state_t state;
        // Number of volume steps for sinks which do not support arbitrary volumes.
        public uint n_volume_steps;
        // Card index, or PA_INVALID_INDEX
        public uint card;
        // Number of entries in port array
        public uint n_ports;
        // Array of available ports, or NULL. Array is terminated by an entry set to NULL. The number of entries is stored in n_ports.
        public pa_sink_port_info** ports;
        // Pointer to active port in the array, or NULL.
        public pa_sink_port_info* active_port;
        // Number of formats supported by the sink.
        public byte n_formats;
        // Array of formats supported by the sink;
        public pa_format_info** formats;

    }
    // delegate* unmanaged<pa_context*, pa_sink_info*, int, void*, void>
    // pa_context bindings for sinks
    internal unsafe partial struct pa_context
    {
        // Get information about a sing by its name
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            delegate* unmanaged[Cdecl]<pa_context*, pa_sink_info*, int, void*, void>,
            void*,
            pa_operation*> pa_context_get_sink_info_by_name;

        // Get information about a sink by its index
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            delegate* unmanaged[Cdecl]<pa_context*, pa_sink_info*, int, void*, void>,            
            void*,
            pa_operation*> pa_context_get_sink_info_by_index;

        // Get the complete sink list
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            delegate* unmanaged[Cdecl]<pa_context*, pa_sink_info*, int, void*, void>,
            void*,
            pa_operation*> pa_context_get_sink_info_list;

        // Set the volume of a sink device specified by its index
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint, pa_cvolume*,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_sink_volume_by_index;

        // Set the volume of a sink device specified by its name
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            pa_cvolume*,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_sink_volume_by_name;

        // Set the mute switch of a sink device specified by its index
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_sink_mute_by_index;

        // Set the mute switch of a sink device specified by its name
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_sink_mute_by_name;

        // Suspend/Resume a sink.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_suspend_sink_by_name;

        // Suspend/Resume a sink. If idx is PA_INVALID_INDEX all sink will be suspended.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_suspend_sink_by_index;

        // Change the profile of a sink.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            IntPtr,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_sink_port_by_index;

        // Change the profile of a sink.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            IntPtr,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_sink_port_by_name;

    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void pa_sink_info_cb_t(pa_context* c, pa_sink_info* i, int eol, void* userdata);

    internal struct pa_sink_port_info
    {
        // Name of this port
        public IntPtr name;
        // Description of this port
        public IntPtr description;
        // The higher this value is, the more useful this port is as a default.
        public uint priority;
        // A flags (see pa_port_available), indicating availability status of this port.
        public int available;
    }

    /** Stores information about a specific port of a source.  Please
    * note that this structure can be extended as part of evolutionary
    * API updates at any time in any new release. \since 0.9.16 */
    internal struct pa_source_port_info
    {
        // Name of this port
        public IntPtr name;
        // Description of this port
        public IntPtr description;
        // The higher this value is, the more usefil this port is as a default.
        public uint priority;
        // A flags (see pa_port_available), indicating availability status of this port.
        public int available;
    }

    internal unsafe struct pa_source_info
    {
        // Name of source
        public IntPtr name;
        // Index of the source
        public uint index;
        // Description of this source
        public IntPtr description;
        //Sample spec of this source
        public pa_sample_spec sample_spec;
        // Channel map
        public pa_channel_map channel_map;
        // Owning module index, or PA_INVALID_INDEX
        public uint owner_module;
        // Volume of the source
        public pa_cvolume volume;
        // Mute switch of the sink
        public int mute;
        // If this is a monitor source, the index of the owning sink, otherwise PA_INVALID_INDEX.
        public uint monitor_of_sink;
        // Name of the owning sink, or NULL.
        public IntPtr monitor_of_sink_name;
        // Length of filled record buffer of this source.
        public pa_usec_t latency;
        // Drive name
        public IntPtr driver;
        // Flags
        public pa_source_flags_t flags;
        // Property list
        public pa_proplist* proplist;
        // The latency this device has been configured to.
        public pa_usec_t configured_latency;
        // Some kind of "base" volume that refers to unamplified/unattenuated volume in the context of the input device.
        public pa_volume_t base_volume;
        // State
        public pa_source_state_t state;
        // Number of volume steps for sources which do not support arbitrary volumes.
        public uint n_volume_steps;
        // Card index, or PA_INVALID_INDEX.
        public uint card;
        // Number of entries in port array
        public uint n_ports;
        // Array of available ports, or NULL. Array is terminated by an entry set to NULL. The number of entries is stored in n_ports.
        public pa_source_port_info** ports;
        // Pointer to active port in the array, or NULL.
        public pa_source_port_info* active_port;
        // Number of formats supported byte the source.
        public byte n_formats;
        // Array of formats supported by the source.
        public pa_format_info** formats;

    }

    // delegate* unmanaged<pa_context*, pa_sink_info*, int, void*, void>

    // pa_context bindings for sources
    internal unsafe partial struct pa_context
    {
        // Get information about a source by its name
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            delegate* unmanaged[Cdecl]<pa_context*, pa_source_info*, int, void*, void>,
            void*,
            pa_operation*> pa_context_get_source_info_by_name;

        // Get information about a source by its index
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            delegate* unmanaged[Cdecl]<pa_context*, pa_source_info*, int, void*, void>,            
            void*,
            pa_operation*> pa_context_get_source_info_by_index;

        // Get the complete source list
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            delegate* unmanaged[Cdecl]<pa_context*, pa_source_info*, int, void*, void>,
            void*,
            pa_operation*> pa_context_get_source_info_list;

        // Set the volume of a source device specified by its index
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            pa_cvolume*,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_source_volume_by_index;

        // Set the volume of a source device specified by its name
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            pa_cvolume*,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_source_volume_by_name;

        // Set the mute switch of a source device specified by its index
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_source_mute_by_index;

        // Set the mute switch of a source device specified by its name
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_source_mute_by_name;

        // Suspend/Resume a source.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_suspend_source_by_name;

        // Suspend/Resume a source. If idx is PA_INVALID_INDEX, all sources will be suspended.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            int,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_suspend_source_by_index;

        // Chaned the profile of a source.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            IntPtr,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*, pa_operation*> pa_context_set_source_port_by_index;

        // Chaned the profile of a source.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            IntPtr,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_context_set_source_port_by_name;

    }

    // Callback prototype for pa_context_get_source_info_by_name() and friends
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void pa_source_info_cb_t(pa_context* c, pa_source_info* i, int eol, void* userdata);

    internal struct pa_server_info
    {
        // User name of the daemon process
        public IntPtr user_name;
        // Host name the daemon is running on
        public IntPtr host_name;
        // Version string of the daemon
        public IntPtr server_version;
        // Server package name (usually "pulseaudio")
        public IntPtr server_name;
        // Default sample specification
        public pa_sample_spec sample_spec;
        // Name of default sink.
        public IntPtr default_sink_name;
        // Name of default source.
        public IntPtr default_source_name;
        // A random cookie for identifying this instance of PulseAudio.
        public uint cookie;
        // Default channel map.
        public pa_channel_map channel_map;
    }

    // Callback prototype for pa_context_get_server_info()
    unsafe delegate void pa_server_info_cb_t(pa_context* c, pa_server_info* i, void* userdata);

    // pa_context bindings for servers.
    internal unsafe partial struct pa_context
    {
        // Get some information about the server
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            delegate* unmanaged<pa_context*, pa_server_info*, void*, void>,
            void*,
            pa_operation*> pa_context_get_server_info;
    }

    internal unsafe struct pa_module_info
    {
        // Index of the module
        public uint index;
        // Name of the module
        public IntPtr name;
        // Argument string of the module
        public IntPtr argument;
        // Usage counter of PR_INVALID_INDEX
        public uint n_used;
        // deprecated Non-zero if this is an autoloaded module.
        public int auto_unload;
        // Property list
        public pa_proplist* proplist;
    }

    // Callback prototype for pa_context_get_module_info() and friends
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void pa_module_info_cb_t(pa_context* c, pa_module_info* i, int eol, void* userdata);

    // Callback prototype for pa_context_load_module()
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void pa_context_info_cb_t(pa_context* c, uint idx, void* userdata);

    internal unsafe partial struct pa_context
    {
        // Get some information about a module by its index
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            delegate* unmanaged<pa_context*, pa_module_info*, int, void*, void>,
            void*,
            pa_operation*> pa_context_get_module_info;

        // Get the complete list of currently loaded modules
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            delegate* unmanaged<pa_context*, pa_module_info*, int, void*, void>,
            void*,
            pa_operation*> pa_context_get_module_info_list;

        // Load a module.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            IntPtr,
            IntPtr,
            delegate* unmanaged<pa_context*, uint, void*, void>,
            void*,
            pa_operation*> pa_context_load_module;

        // Unload a module.        
        [NativeMethod]        
        public static delegate* unmanaged[Cdecl]<
            pa_context*,
            uint,
            delegate* unmanaged<pa_context*, int, void*, void>,
            void*,
            pa_operation*> pa_contet_unload_module;

    }
}
