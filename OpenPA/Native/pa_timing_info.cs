using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pa_usec_t = System.UInt32;

namespace OpenPA.Native
{
    /** A structure for all kinds of timing information of a stream. See
    * pa_stream_update_timing_info() and pa_stream_get_timing_info(). The
    * total output latency a sample that is written with
    * pa_stream_write() takes to be played may be estimated by
    * sink_usec+buffer_usec+transport_usec (where buffer_usec is defined
    * as pa_bytes_to_usec(write_index-read_index)). The output buffer
    * which buffer_usec relates to may be manipulated freely (with
    * pa_stream_write()'s seek argument, pa_stream_flush() and friends),
    * the buffers sink_usec and source_usec relate to are first-in
    * first-out (FIFO) buffers which cannot be flushed or manipulated in
    * any way. The total input latency a sample that is recorded takes to
    * be delivered to the application is:
    * source_usec+buffer_usec+transport_usec-sink_usec. (Take care of
    * sign issues!) When connected to a monitor source sink_usec contains
    * the latency of the owning sink. The two latency estimations
    * described here are implemented in pa_stream_get_latency().
    *
    * All time values are in the sound card clock domain, unless noted
    * otherwise. The sound card clock usually runs at a slightly different
    * rate than the system clock.
    *
    * Please note that this structure can be extended as part of evolutionary
    * API updates at any time in any new release.
    * */
    internal struct pa_timing_info
    {
        /** The system clock time when this timing info structure was
        * current. */
        public timeval timestamp;

        /** Non-zero if the local and the remote machine have
        * synchronized clocks. If synchronized clocks are detected
        * transport_usec becomes much more reliable. However, the code
        * that detects synchronized clocks is very limited and unreliable
        * itself. */
        public int synchronized_clocks;

        /** Time in usecs a sample takes to be played on the sink. For
        * playback streams and record streams connected to a monitor
        * source. */
        public pa_usec_t sink_usec;

        /** Time in usecs a sample takes from being recorded to being
        * delivered to the application. Only for record streams. */
        public pa_usec_t source_usec;

        /** Estimated time in usecs a sample takes to be transferred
        * to/from the daemon. For both playback and record streams. */
        public pa_usec_t transport_usec;

        /** Non-zero when the stream is currently not underrun and data
        * is being passed on to the device. Only for playback
        * streams. This field does not say whether the data is actually
        * already being played. To determine this check whether
        * since_underrun (converted to usec) is larger than sink_usec.*/
        public int playing;

        /** Non-zero if write_index is not up-to-date because a local
        * write command that corrupted it has been issued in the time
        * since this latency info was current . Only write commands with
        * SEEK_RELATIVE_ON_READ and SEEK_RELATIVE_END can corrupt
        * write_index. */
        public int write_index_corrupt;

        /** Current write index into the playback buffer in bytes. Think
        * twice before using this for seeking purposes: it might be out
        * of date at the time you want to use it. Consider using
        * PA_SEEK_RELATIVE instead. */
        public long write_index;

        /** Non-zero if read_index is not up-to-date because a local
        * pause or flush request that corrupted it has been issued in the
        * time since this latency info was current. */
        public int read_index_corrupt;

        /** Current read index into the playback buffer in bytes. Think
        * twice before using this for seeking purposes: it might be out
        * of date at the time you want to use it. Consider using
        * PA_SEEK_RELATIVE_ON_READ instead. */
        public long read_index;

        /** The configured latency for the sink. \since 0.9.11 */
        public pa_usec_t configured_sink_usec;

        /** The configured latency for the source. \since 0.9.11 */
        public pa_usec_t configured_source_usec;

        /** Bytes that were handed to the sink since the last underrun
        * happened, or since playback started again after the last
        * underrun. playing will tell you which case it is. \since
        * 0.9.11 */
        public long since_underrun;

    }
}
