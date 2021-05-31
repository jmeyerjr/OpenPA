using OpenPA.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using size_t = System.UInt32;
using pa_usec_t = System.UInt32;
using pa_stream_state_t = OpenPA.Enums.StreamState;
using pa_stream_flags_t = OpenPA.Enums.StreamFlags;
using pa_seek_mode_t = OpenPA.Enums.SeekMode;
using pa_update_mode_t = OpenPA.Enums.UpdateMode;

namespace OpenPA.Native
{
    // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
    // void (*pa_stream_request_cb_t)(pa_stream* p, size_t nbytes, void* userdata)
    // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
    // void (*pa_stream_event_cb_t)(pa_stream* p, const char* name, pa_proplist* pl, void* userdata)
    // void (*pa_free_cb_t)(void* p)

    internal unsafe struct pa_stream
    {
        /** Create a new, unconnected stream with the specified name and
        * sample type. It is recommended to use pa_stream_new_with_proplist()
        * instead and specify some initial properties. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, IntPtr, pa_sample_spec*, pa_channel_map*, pa_stream*> pa_stream_new;

        /** Create a new, unconnected stream with the specified name and
        * sample type, and specify the initial stream property
        * list. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, IntPtr, pa_sample_spec*, pa_channel_map*, pa_proplist*, pa_stream*> pa_stream_new_with_proplist;

        /** Create a new, unconnected stream with the specified name, the set of formats
        * this client can provide, and an initial list of properties. While
        * connecting, the server will select the most appropriate format which the
        * client must then provide. \since 1.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_context*, IntPtr, pa_format_info*, uint, pa_proplist*, pa_stream*> pa_stream_new_extended;

        // Decrease the reference counter by one.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, void> pa_stream_unref;

        // Increase the reference counter by one.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_stream*> pa_stream_ref;

        // Return the current state of the stream.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_stream_state_t> pa_stream_get_state;

        // Return the context this stream is attached to.
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_context*> pa_stream_get_context;

        /** Return the sink input resp.\ source output index this stream is
        * identified in the server with. This is useful with the
        * introspection functions such as pa_context_get_sink_input_info()
        * or pa_context_get_source_output_info(). This returns PA_INVALID_INDEX
        * on failure. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*,uint> pa_stream_get_index;

        /** Return the index of the sink or source this stream is connected to
        * in the server. This is useful with the introspection
        * functions such as pa_context_get_sink_info_by_index() or
        * pa_context_get_source_info_by_index().
        *
        * Please note that streams may be moved between sinks/sources and thus
        * it is recommended to use pa_stream_set_moved_callback() to be notified
        * about this. This function will return with PA_INVALID_INDEX on failure,
        * including the being server older than 0.9.8. \since 0.9.8 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, uint> pa_stream_get_device_index;

        /** Return the name of the sink or source this stream is connected to
        * in the server. This is useful with the introspection
        * functions such as pa_context_get_sink_info_by_name()
        * or pa_context_get_source_info_by_name().
        *
        * Please note that streams may be moved between sinks/sources and thus
        * it is recommended to use pa_stream_set_moved_callback() to be notified
        * about this. This function will fail when the server is older than
        * 0.9.8. \since 0.9.8 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, IntPtr> pa_stream_get_device_name;

        /** Return 1 if the sink or source this stream is connected to has
        * been suspended. This will return 0 if not, and a negative value on
        * error. This function will return with -PA_ERR_NOTSUPPORTED when the
        * server is older than 0.9.8. \since 0.9.8 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, int> pa_stream_is_suspended;

        /** Return 1 if the this stream has been corked. This will return 0 if
        * not, and a negative value on error. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, int> pa_stream_is_corked;

        /** Connect the stream to a sink. It is strongly recommended to pass
        * NULL in both \a dev and \a volume and to set neither
        * PA_STREAM_START_MUTED nor PA_STREAM_START_UNMUTED -- unless these
        * options are directly dependent on user input or configuration.
        *
        * If you follow this rule then the sound server will have the full
        * flexibility to choose the device, volume and mute status
        * automatically, based on server-side policies, heuristics and stored
        * information from previous uses. Also the server may choose to
        * reconfigure audio devices to make other sinks/sources or
        * capabilities available to be able to accept the stream.
        *
        * Before 0.9.20 it was not defined whether the \a volume parameter was
        * interpreted relative to the sink's current volume or treated as
        * an absolute device volume. Since 0.9.20 it is an absolute volume when
        * the sink is in flat volume mode, and relative otherwise, thus
        * making sure the volume passed here has always the same semantics as
        * the volume passed to pa_context_set_sink_input_volume(). It is possible
        * to figure out whether flat volume mode is in effect for a given sink
        * by calling pa_context_get_sink_info_by_name().
        *
        * Since 5.0, it's possible to specify a single-channel volume even if the
        * stream has multiple channels. In that case the same volume is applied to all
        * channels.
        *
        * Returns zero on success. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, IntPtr, pa_buffer_attr*, pa_stream_flags_t, pa_cvolume*, pa_stream*, int> pa_stream_connect_playback;

        /** Connect the stream to a source. Returns zero on success. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, IntPtr, pa_buffer_attr*, pa_stream_flags_t, int> pa_stream_connect_record;

        /** Disconnect a stream from a source/sink. Returns zero on success. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, int> pa_stream_disconnect;

        /** Prepare writing data to the server (for playback streams). This
        * function may be used to optimize the number of memory copies when
        * doing playback ("zero-copy"). It is recommended to call this
        * function before each call to pa_stream_write().
        *
        * Pass in the address to a pointer and an address of the number of
        * bytes you want to write. On return the two values will contain a
        * pointer where you can place the data to write and the maximum number
        * of bytes you can write. \a *nbytes can be smaller or have the same
        * value as you passed in. You need to be able to handle both cases.
        * Accessing memory beyond the returned \a *nbytes value is invalid.
        * Accessing the memory returned after the following pa_stream_write()
        * or pa_stream_cancel_write() is invalid.
        *
        * On invocation only \a *nbytes needs to be initialized, on return both
        * *data and *nbytes will be valid. If you place (size_t) -1 in *nbytes
        * on invocation the memory size will be chosen automatically (which is
        * recommended to do). After placing your data in the memory area
        * returned, call pa_stream_write() with \a data set to an address
        * within this memory area and an \a nbytes value that is smaller or
        * equal to what was returned by this function to actually execute the
        * write.
        *
        * An invocation of pa_stream_write() should follow "quickly" on
        * pa_stream_begin_write(). It is not recommended letting an unbounded
        * amount of time pass after calling pa_stream_begin_write() and
        * before calling pa_stream_write(). If you want to cancel a
        * previously called pa_stream_begin_write() without calling
        * pa_stream_write() use pa_stream_cancel_write(). Calling
        * pa_stream_begin_write() twice without calling pa_stream_write() or
        * pa_stream_cancel_write() in between will return exactly the same
        * \a data pointer and \a nbytes values.
        *
        * On success, will return zero and a valid (non-NULL) pointer. If the
        * return value is non-zero, or the pointer is NULL, this indicates an
        * error. Callers should also pay careful attention to the returned
        * length, which may not be the same as that passed in, as mentioned above.
        *
        * \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, void**, size_t, int> pa_stream_begin_write;

        /** Reverses the effect of pa_stream_begin_write() dropping all data
        * that has already been placed in the memory area returned by
        * pa_stream_begin_write(). Only valid to call if
        * pa_stream_begin_write() was called before and neither
        * pa_stream_cancel_write() nor pa_stream_write() have been called
        * yet. Accessing the memory previously returned by
        * pa_stream_begin_write() after this call is invalid. Any further
        * explicit freeing of the memory area is not necessary.
        * Returns zero on success. \since 0.9.16 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, int> pa_stream_cancel_write;

        /** Write some data to the server (for playback streams).
        * If \a free_cb is non-NULL this routine is called when all data has
        * been written out. An internal reference to the specified data is
        * kept, the data is not copied. If NULL, the data is copied into an
        * internal buffer.
        *
        * The client may freely seek around in the output buffer. For
        * most applications it is typical to pass 0 and PA_SEEK_RELATIVE
        * as values for the arguments \a offset and \a seek respectively.
        * After a successful write call the write index will be at the
        * position after where this chunk of data has been written to.
        *
        * As an optimization for avoiding needless memory copies you may call
        * pa_stream_begin_write() before this call and then place your audio
        * data directly in the memory area returned by that call. Then, pass
        * a pointer to that memory area to pa_stream_write(). After the
        * invocation of pa_stream_write() the memory area may no longer be
        * accessed. Any further explicit freeing of the memory area is not
        * necessary. It is OK to write to the memory area returned by
        * pa_stream_begin_write() only partially with this call, skipping
        * bytes both at the end and at the beginning of the reserved memory
        * area.
        *
        * Returns zero on success. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            void*,
            size_t,
            // void (*pa_free_cb_t)(void* p)
            delegate* unmanaged[Cdecl]<void*, void>,
            long,
            pa_seek_mode_t,
            int> pa_stream_write;

        /** Function does exactly the same as pa_stream_write() with the difference
        *  that free_cb_data is passed to free_cb instead of data. \since 6.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            void*,
            size_t,
            // void (*pa_free_cb_t)(void* p)
            delegate* unmanaged[Cdecl]<void*, void>,
            void*,
            long,
            pa_seek_mode_t,
            int> pa_stream_write_ext_free;

        /** Read the next fragment from the buffer (for recording streams).
        * If there is data at the current read index, \a data will point to
        * the actual data and \a nbytes will contain the size of the data in
        * bytes (which can be less or more than a complete fragment).
        *
        * If there is no data at the current read index, it means that either
        * the buffer is empty or it contains a hole (that is, the write index
        * is ahead of the read index but there's no data where the read index
        * points at). If the buffer is empty, \a data will be NULL and
        * \a nbytes will be 0. If there is a hole, \a data will be NULL and
        * \a nbytes will contain the length of the hole.
        *
        * Use pa_stream_drop() to actually remove the data from the buffer
        * and move the read index forward. pa_stream_drop() should not be
        * called if the buffer is empty, but it should be called if there is
        * a hole.
        *
        * Returns zero on success, negative on error. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, void**, size_t, int> pa_stream_peek;

        /** Remove the current fragment on record streams. It is invalid to do this without first
        * calling pa_stream_peek(). Returns zero on success. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, int> pa_stream_drop;

        /** Return the number of bytes requested by the server that have not yet
        * been written.
        *
        * It is possible to write more than this amount, up to the stream's
        * buffer_attr.maxlength bytes. This is usually not desirable, though, as
        * it would increase stream latency to be higher than requested
        * (buffer_attr.tlength).
        *
        * (size_t) -1 is returned on error.
        */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, size_t> pa_stream_writable_size;

        /** Return the number of bytes that may be read using pa_stream_peek().
        *
        * (size_t) -1 is returned on error. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, size_t> pa_stream_readable_size;

        /** Drain a playback stream.  Use this for notification when the
        * playback buffer is empty after playing all the audio in the buffer.
        * Please note that only one drain operation per stream may be issued
        * at a time. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_drain;

        /** Request a timing info structure update for a stream. Use
        * pa_stream_get_timing_info() to get access to the raw timing data,
        * or pa_stream_get_time() or pa_stream_get_latency() to get cleaned
        * up values. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_update_timing_info;

        /** Set the callback function that is called whenever the state of the stream changes. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_state_callback;

        /** Set the callback function that is called when new data may be
        * written to the stream. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_request_cb_t)(pa_stream* p, size_t nbytes, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, size_t, void*, void>,
            void*,
            void> pa_stream_set_write_callback;

        /** Set the callback function that is called when new data is available from the stream. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_request_cb_t)(pa_stream* p, size_t nbytes, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, size_t, void*, void>,
            void*,
            void> pa_stream_set_read_callback;

        /** Set the callback function that is called when a buffer overflow happens. (Only for playback streams) */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_overflow_callback;

        /** Return at what position the latest underflow occurred, or -1 if this information is not
        * known (e.g.\ if no underflow has occurred, or server is older than 1.0).
        * Can be used inside the underflow callback to get information about the current underflow.
        * (Only for playback streams) \since 1.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, long> pa_stream_get_underflow_index;

        /** Set the callback function that is called when a buffer underflow happens. (Only for playback streams) */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_underflow_callback;

        /** Set the callback function that is called when the server starts
        * playback after an underrun or on initial startup. This only informs
        * that audio is flowing again, it is no indication that audio started
        * to reach the speakers already. (Only for playback streams) \since
        * 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_started_callback;

        /** Set the callback function that is called whenever a latency
        * information update happens. Useful on PA_STREAM_AUTO_TIMING_UPDATE
        * streams only. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_latency_update_callback;

        /** Set the callback function that is called whenever the stream is
        * moved to a different sink/source. Use pa_stream_get_device_name() or
        * pa_stream_get_device_index() to query the new sink/source. This
        * notification is only generated when the server is at least
        * 0.9.8. \since 0.9.8 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_moved_callback;

        /** Set the callback function that is called whenever the sink/source
        * this stream is connected to is suspended or resumed. Use
        * pa_stream_is_suspended() to query the new suspend status. Please
        * note that the suspend status might also change when the stream is
        * moved between devices. Thus if you call this function you very
        * likely want to call pa_stream_set_moved_callback() too. This
        * notification is only generated when the server is at least
        * 0.9.8. \since 0.9.8 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_suspended_callback;

        /** Set the callback function that is called whenever a meta/policy
        * control event is received. \since 0.9.15 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_event_cb_t)(pa_stream* p, const char* name, pa_proplist* pl, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, IntPtr, pa_proplist*, void*, void>,
            void*,
            void> pa_stream_set_event_callback;

        /** Set the callback function that is called whenever the buffer
        * attributes on the server side change. Please note that the buffer
        * attributes can change when moving a stream to a different
        * sink/source too, hence if you use this callback you should use
        * pa_stream_set_moved_callback() as well. \since 0.9.15 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream,
            // void (*pa_stream_notify_cb_t)(pa_stream* p, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, void*, void>,
            void*,
            void> pa_stream_set_buffer_attr_callback;

        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            int,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_cork;

        /** Flush the playback or record buffer of this stream. This discards any audio data
        * in the buffer.  Most of the time you're better off using the parameter
        * \a seek of pa_stream_write() instead of this function. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_flush;

        /** Reenable prebuffering if specified in the pa_buffer_attr
        * structure. Available for playback streams only. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_prebuf;

        /** Request immediate start of playback on this stream. This disables
        * prebuffering temporarily if specified in the pa_buffer_attr structure.
        * Available for playback streams only. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_trigger;

        /** Rename the stream. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            IntPtr,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_set_name;

        /** Return the current playback/recording time. This is based on the
        * data in the timing info structure returned by
        * pa_stream_get_timing_info(). The returned time is in the sound card
        * clock domain, which usually runs at a slightly different rate than
        * the system clock.
        *
        * This function will usually only return new data if a timing info
        * update has been received. Only if timing interpolation has been
        * requested (PA_STREAM_INTERPOLATE_TIMING) the data from the last
        * timing update is used for an estimation of the current
        * playback/recording time based on the local time that passed since
        * the timing info structure has been acquired.
        *
        * The time value returned by this function is guaranteed to increase
        * monotonically (the returned value is always greater
        * or equal to the value returned by the last call). This behaviour
        * can be disabled by using PA_STREAM_NOT_MONOTONIC. This may be
        * desirable to better deal with bad estimations of transport
        * latencies, but may have strange effects if the application is not
        * able to deal with time going 'backwards'.
        *
        * The time interpolator activated by PA_STREAM_INTERPOLATE_TIMING
        * favours 'smooth' time graphs over accurate ones to improve the
        * smoothness of UI operations that are tied to the audio clock. If
        * accuracy is more important to you, you might need to estimate your
        * timing based on the data from pa_stream_get_timing_info() yourself
        * or not work with interpolated timing at all and instead always
        * query the server side for the most up to date timing with
        * pa_stream_update_timing_info().
        *
        * If no timing information has been
        * received yet this call will return -PA_ERR_NODATA. For more details
        * see pa_stream_get_timing_info().
        *
        * Returns zero on success, negative on error. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_usec_t*, int> pa_stream_get_time;

        /** Determine the total stream latency. This function is based on
        * pa_stream_get_time(). The returned time is in the sound card clock
        * domain, which usually runs at a slightly different rate than the
        * system clock.
        *
        * The latency is stored in \a *r_usec. In case the stream is a
        * monitoring stream the result can be negative, i.e. the captured
        * samples are not yet played. In this case \a *negative is set to 1.
        *
        * If no timing information has been received yet, this call will
        * return -PA_ERR_NODATA. On success, it will return 0.
        *
        * For more details see pa_stream_get_timing_info() and
        * pa_stream_get_time(). */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_usec_t*, int*, int> pa_stream_get_latency;

        /** Return the latest raw timing data structure. The returned pointer
        * refers to an internal read-only instance of the timing
        * structure. The user should make a copy of this structure if
        * wanting to modify it. An in-place update to this data structure
        * may be requested using pa_stream_update_timing_info().
        *
        * If no timing information has been received before (i.e. by
        * requesting pa_stream_update_timing_info() or by using
        * PA_STREAM_AUTO_TIMING_UPDATE), this function will return NULL.
        *
        * Please note that the write_index member field (and only this field)
        * is updated on each pa_stream_write() call, not just when a timing
        * update has been received. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_timing_info*> pa_stream_get_timing_info;

        /** Return a pointer to the stream's sample specification. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_sample_spec*> pa_stream_get_sample_spec;

        /** Return a pointer to the stream's channel map. */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_channel_map*> pa_stream_get_channel_map;

        /** Return a pointer to the stream's format. \since 1.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_format_info*> pa_stream_get_format_info;

        /** Return the per-stream server-side buffer metrics of the
        * stream. Only valid after the stream has been connected successfully
        * and if the server is at least PulseAudio 0.9. This will return the
        * actual configured buffering metrics, which may differ from what was
        * requested during pa_stream_connect_record() or
        * pa_stream_connect_playback(). This call will always return the
        * actual per-stream server-side buffer metrics, regardless whether
        * PA_STREAM_ADJUST_LATENCY is set or not. \since 0.9.0 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, pa_buffer_attr*> pa_stream_get_buffer_attr;

        /** Change the buffer metrics of the stream during playback. The
        * server might have chosen different buffer metrics than
        * requested. The selected metrics may be queried with
        * pa_stream_get_buffer_attr() as soon as the callback is called. Only
        * valid after the stream has been connected successfully and if the
        * server is at least PulseAudio 0.9.8. Please be aware of the
        * slightly different semantics of the call depending whether
        * PA_STREAM_ADJUST_LATENCY is set or not. \since 0.9.8 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            pa_buffer_attr*,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_set_buffer_attr;

        /** Change the stream sampling rate during playback. You need to pass
        * PA_STREAM_VARIABLE_RATE in the flags parameter of
        * pa_stream_connect_playback() if you plan to use this function. Only valid
        * after the stream has been connected successfully and if the server
        * is at least PulseAudio 0.9.8. \since 0.9.8 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            uint,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_update_sample_rate;

        /** Update the property list of the sink input/source output of this
        * stream, adding new entries. Please note that it is highly
        * recommended to set as many properties initially via
        * pa_stream_new_with_proplist() as possible instead a posteriori with
        * this function, since that information may be used to route
        * this stream to the right device. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            pa_update_mode_t,
            pa_proplist*,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_proplist_update;

        /** Update the property list of the sink input/source output of this
        * stream, remove entries. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<
            pa_stream*,
            IntPtr,
            // void (*pa_stream_success_cb_t)(pa_stream* s, int success, void* userdata)
            delegate* unmanaged[Cdecl]<pa_stream*, int, void*, void>,
            void*,
            pa_operation*> pa_stream_proplist_remove;

        /** For record streams connected to a monitor source: monitor only a
        * very specific sink input of the sink. This function needs to be
        * called before pa_stream_connect_record() is called.
        * Returns zero on success, negative on error. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, uint, int> pa_stream_set_monitor_stream;

        /** Return the sink input index previously set with
        * pa_stream_set_monitor_stream(). Returns PA_INVALID_INDEX
        * on failure. \since 0.9.11 */
        [NativeMethod]
        public static delegate* unmanaged[Cdecl]<pa_stream*, uint> pa_stream_get_monitor_stream;
    }
}
