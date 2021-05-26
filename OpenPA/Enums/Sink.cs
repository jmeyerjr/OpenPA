using System;

namespace OpenPA.Enums
{
    [Flags]
    public enum SinkFlags
    {
        NOFLAGS = 0x000,
        HW_VOLUME_CTRL = 0x001,
        LATENCY = 0x002,
        HARDWARE = 0x004,
        NETWORK = 0x008,
        MUTE_CTRL = 0x010,
        DECIBEL_VOLUME = 0x020,
        FLAT_VOLUME = 0x040,
        DYNAMIC_LATENCY = 0x080,
        SET_FORMATS = 0x100,
    }

    public enum SinkState : byte
    {
        INVALID_STATE = 0xFF,
        RUNNING = 0x00,
        IDLE = 0x01,
        SUSPENDED = 0x02,
    }

    [Flags]
    public enum SourceFlags
    {
        // Flag to pass when no specific options are needed (used to avoid casting)
        NOFLAGS = 0x0000,
        // Supports hardware volume control. This is a dynamic flag and may
        // change at runtime after the source has initialized
        HW_VOLUME_CTRL = 0x0001,
        // Supports latency querying
        LATENCY = 0x0002,
        // Is a hardware source of some kind, in contrast to
        // "virtual"/software source
        HARDWARE = 0x0004,
        // Is a networked source of some kind
        NETWORK = 0x0008,
        // Supports hardware mute control. This is a dynamic flage and may
        // change at runtime after the source has initialized
        HW_MUTE_CTRL = 0x0010,
        // Volume can be trandlated to dB with pa_sw_volume_to_dB(). This is a
        // dynamic flag and may chaned at runtime after the source has initialized.
        DECIBEL_VOLUME = 0x0020,
        // The latency can be adjusted dynamically depending on the
        // needs of the connected streams.
        DYNAMIC_LATENCY = 0x0040,
        // This source is in flat volume mode, i.e. always the maximum of
        // the volume of all connected outputs.
        FLAT_VOLUME = 0x0080,
    }

    public enum SourceState
    {
        // This state is used when the server does not support source state introspection
        INVALID_STATE = -1,
        // Running, source is recording and used by at least one non-corked source-output
        RUNNING = 0,
        // When idle, the source is still recording but there is no non-corked source-output
        IDLE = 1,
        // When suspended, actual source access can be closed, for instance
        SUSPENDED = 2,
    }
}