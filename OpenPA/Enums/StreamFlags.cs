using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPA.Enums
{
    [Flags]
    public enum StreamFlags
    {
        /** Flag to pass when no specific options are needed (used to avoid casting)  \since 0.9.19 */
        NOFLAGS = 0x0000,

        /**< Create the stream corked, requiring an explicit 
         * pa_stream_cork() call to uncork it. */
        START_CORKED = 0x0001,

        /** Interpolate the latency for this stream. When enabled,
        * pa_stream_get_latency() and pa_stream_get_time() will try to
        * estimate the current record/playback time based on the local
        * time that passed since the last timing info update.  Using this
        * option has the advantage of not requiring a whole roundtrip
        * when the current playback/recording time is needed. Consider
        * using this option when requesting latency information
        * frequently. This is especially useful on long latency network
        * connections. It makes a lot of sense to combine this option
        * with PA_STREAM_AUTO_TIMING_UPDATE. */
        INTERPOLATE_TIMING = 0x0002,

        /**< Don't force the time to increase monotonically. If this
        * option is enabled, pa_stream_get_time() will not necessarily
        * return always monotonically increasing time values on each
        * call. This may confuse applications which cannot deal with time
        * going 'backwards', but has the advantage that bad transport
        * latency estimations that caused the time to jump ahead can
        * be corrected quickly, without the need to wait. (Please note
        * that this flag was named PA_STREAM_NOT_MONOTONOUS in releases
        * prior to 0.9.11. The old name is still defined too, for
        * compatibility reasons. */
        NOT_MONOTONIC = 0x0004,

        /** If set timing update requests are issued periodically
        * automatically. Combined with PA_STREAM_INTERPOLATE_TIMING you
        * will be able to query the current time and latency with
        * pa_stream_get_time() and pa_stream_get_latency() at all times
        * without a packet round trip.*/
        AUTO_TIMING_UPDATE = 0x0008,

        /**< Don't remap channels by their name, instead map them simply
        * by their index. Implies PA_STREAM_NO_REMIX_CHANNELS. Only
        * supported when the server is at least PA 0.9.8. It is ignored
        * on older servers.\since 0.9.8 */
        NO_REMAP_CHANNELS = 0x0010,

        /** When remapping channels by name, don't upmix or downmix them
        * to related channels. Copy them into matching channels of the
        * device 1:1. Only supported when the server is at least PA
        * 0.9.8. It is ignored on older servers. \since 0.9.8 */
        NO_REMIX_CHANNELS = 0x0020,

        /**< Use the sample format of the sink/device this stream is being
        * connected to, and possibly ignore the format the sample spec
        * contains -- but you still have to pass a valid value in it as a
        * hint to PulseAudio what would suit your stream best. If this is
        * used you should query the used sample format after creating the
        * stream by using pa_stream_get_sample_spec(). Also, if you
        * specified manual buffer metrics it is recommended to update
        * them with pa_stream_set_buffer_attr() to compensate for the
        * changed frame sizes. Only supported when the server is at least
        * PA 0.9.8. It is ignored on older servers.
        *
        * When creating streams with pa_stream_new_extended(), this flag has no
        * effect. If you specify a format with PCM encoding, and you want the
        * server to choose the sample format, then you should leave the sample
        * format unspecified in the pa_format_info object. This also means that
        * you can't use pa_format_info_from_sample_spec(), because that function
        * always sets the sample format.
        *
        * \since 0.9.8 */
        FIX_FORMAT = 0x0040,

        /** Use the sample rate of the sink, and possibly ignore the rate
        * the sample spec contains. Usage similar to
        * PA_STREAM_FIX_FORMAT. Only supported when the server is at least
        * PA 0.9.8. It is ignored on older servers.
        *
        * When creating streams with pa_stream_new_extended(), this flag has no
        * effect. If you specify a format with PCM encoding, and you want the
        * server to choose the sample rate, then you should leave the rate
        * unspecified in the pa_format_info object. This also means that you can't
        * use pa_format_info_from_sample_spec(), because that function always sets
        * the sample rate.
        *
        * \since 0.9.8 */
        FIX_RATE = 0x0080,

        /** Use the number of channels and the channel map of the sink,
        * and possibly ignore the number of channels and the map the
        * sample spec and the passed channel map contain. Usage similar
        * to PA_STREAM_FIX_FORMAT. Only supported when the server is at
        * least PA 0.9.8. It is ignored on older servers.
        *
        * When creating streams with pa_stream_new_extended(), this flag has no
        * effect. If you specify a format with PCM encoding, and you want the
        * server to choose the channel count and/or channel map, then you should
        * leave the channels and/or the channel map unspecified in the
        * pa_format_info object. This also means that you can't use
        * pa_format_info_from_sample_spec(), because that function always sets
        * the channel count (but if you only want to leave the channel map
        * unspecified, then pa_format_info_from_sample_spec() works, because it
        * accepts a NULL channel map).
        *
        * \since 0.9.8 */
        FIX_CHANNELS = 0x0100,

        /** Don't allow moving of this stream to another
        * sink/device. Useful if you use any of the PA_STREAM_FIX_ flags
        * and want to make sure that resampling never takes place --
        * which might happen if the stream is moved to another
        * sink/source with a different sample spec/channel map. Only
        * supported when the server is at least PA 0.9.8. It is ignored
        * on older servers. \since 0.9.8 */
        DONT_MOVE = 0x0200,

        /** Allow dynamic changing of the sampling rate during playback
        * with pa_stream_update_sample_rate(). Only supported when the
        * server is at least PA 0.9.8. It is ignored on older
        * servers. \since 0.9.8 */
        VARIABLE_RATE = 0x0400,

        /** Find peaks instead of resampling. \since 0.9.11 */
        PEAK_DETECT = 0x0800,

        /** Create in muted state. If neither PA_STREAM_START_UNMUTED nor
        * PA_STREAM_START_MUTED are set, it is left to the server to decide
        * whether to create the stream in muted or in unmuted
        * state. \since 0.9.11 */
        START_MUTED = 0x1000,

        /** Try to adjust the latency of the sink/source based on the
        * requested buffer metrics and adjust buffer metrics
        * accordingly. Also see pa_buffer_attr. This option may not be
        * specified at the same time as PA_STREAM_EARLY_REQUESTS. \since
        * 0.9.11 */
        ADJUST_LATENCY = 0x2000,

        /** Enable compatibility mode for legacy clients that rely on a
        * "classic" hardware device fragment-style playback model. If
        * this option is set, the minreq value of the buffer metrics gets
        * a new meaning: instead of just specifying that no requests
        * asking for less new data than this value will be made to the
        * client it will also guarantee that requests are generated as
        * early as this limit is reached. This flag should only be set in
        * very few situations where compatibility with a fragment-based
        * playback model needs to be kept and the client applications
        * cannot deal with data requests that are delayed to the latest
        * moment possible. (Usually these are programs that use usleep()
        * or a similar call in their playback loops instead of sleeping
        * on the device itself.) Also see pa_buffer_attr. This option may
        * not be specified at the same time as
        * PA_STREAM_ADJUST_LATENCY. \since 0.9.12 */
        EARLY_REQUESTS = 0x4000,

        /** If set this stream won't be taken into account when it is
        * checked whether the device this stream is connected to should
        * auto-suspend. \since 0.9.15 */
        INHIBIT_AUTO_SUSPEND = 0x8000,

        /** Create in unmuted state. If neither PA_STREAM_START_UNMUTED
        * nor PA_STREAM_START_MUTED are set it is left to the server to decide
        * whether to create the stream in muted or in unmuted
        * state. \since 0.9.15 */
        START_UNMUTED = 0x10000,

        /** If the sink/source this stream is connected to is suspended
        * during the creation of this stream, cause it to fail. If the
        * sink/source is being suspended during creation of this stream,
        * make sure this stream is terminated. \since 0.9.15 */
        FAIL_ON_SUSPEND = 0x20000,

        /** If a volume is passed when this stream is created, consider
        * it relative to the sink's current volume, never as absolute
        * device volume. If this is not specified the volume will be
        * consider absolute when the sink is in flat volume mode,
        * relative otherwise. \since 0.9.20 */
        RELATIVE_VOLUME = 0x40000,

        /** Used to tag content that will be rendered by passthrough sinks.
        * The data will be left as is and not reformatted, resampled.
        * \since 1.0 */
        PASSTHROUGH = 0x80000,
    }
}
