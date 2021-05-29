namespace OpenPA.Enums
{
    public enum ChannelPosition : int
    {
        INVALID = -1,
        MONO = 0,
        FRONT_LEFT,
        FRONT_RIGHT,
        FRONT_CENTER,

        LEFT = FRONT_LEFT,
        RIGHT = FRONT_RIGHT,
        CENTER = FRONT_CENTER,

        REAR_CENTER,
        REAR_LEFT,
        REAR_RIGHT,

        LFE,
        SUBWOOFER = LFE,

        FRONT_LEFT_OF_CENTER,
        FRONT_RIGHT_OF_CENTER,

        SIDE_LEFT,
        SIDE_RIGHT,

        AUX0,
        AUX1,
        AUX2,
        AUX3,
        AUX4,
        AUX5,
        AUX6,
        AUX7,
        AUX8,
        AUX9,
        AUX10,
        AUX11,
        AUX12,
        AUX13,
        AUX14,
        AUX15,
        AUX16,
        AUX17,
        AUX18,
        AUX19,
        AUX20,
        AUX21,
        AUX22,
        AUX23,
        AUX24,
        AUX25,
        AUX26,
        AUX27,
        AUX28,
        AUX29,
        AUX30,
        AUX31,

        TOP_CENTER,

        TOP_FRONT_LEFT,
        TOP_FRONT_RIGHT,
        TOP_FRONT_CENTER,

        TOP_REAR_LEFT,
        TOP_REAR_RIGHT,
        TOP_REAR_CENTER,

        MAX = 32

    }

    public enum ChannelMapDef
    {
        // The mapping from RFC3551, which is base on AIFF-C
        MAP_AIFF,
        // The default mapping used by ALSA. This mapping is probably
        // not too useful since ALSA's default channel mapping depends on
        // the device string used.
        MAP_ALSA,
        // Only aux channels
        MAP_AUX,
        // Microsoft's WAVEFORMATEXTENSIBLE mapping. This mapping works
        // as if all LSBs of dwChannelMask are set.
        MAP_WAVEEX,
        // The default channel mapping used by OSS as defined in the OSS
        // 4.0 API specs. This mapping is probably not too useful since
        // the OSS API has changed in this respect and no longer knows a
        // default channel mapping based on the number of channels.
        MAP_OSS,
        // Upper limit of valid channel mapping definitions
        MAP_DEF_MAX,
        // The default channel map
        MAP_DEFAULT = MAP_AIFF,
    }
}