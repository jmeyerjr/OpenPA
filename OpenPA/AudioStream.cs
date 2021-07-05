using OpenPA.Enums;
using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OpenPA.Native.pa_stream;
using static OpenPA.Native.pa_operation;

using StreamRequestCallback = System.Action<OpenPA.AudioStream, uint>;

namespace OpenPA
{
    public class AudioBuffer
    { 
        public IntPtr Ptr { get; init; }
        public uint nSize { get; init; }  
        
        public void Copy(byte[] buffer)
        {
            Marshal.Copy(buffer, 0, Ptr, (int)nSize);
        }
    }

    public delegate void StreamNotifyCallback(AudioStream stream);
    public delegate void StreamEventCallback(AudioStream stream, string? e, PropList propList);
    public delegate void StreamRequestCallback(AudioStream stream, uint i);

    public unsafe class AudioStream : IDisposable
    {
        pa_stream* stream;
        private bool disposedValue;

        internal AudioStream()
        { }

        public AudioStream(PAContext context, string name, SampleSpec sampleSpec, ChannelMap channelMap)
        {
            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);            

            pa_sample_spec sample_spec = SampleSpec.Convert(sampleSpec);


            pa_channel_map channel_map = ChannelMap.Convert(channelMap);
                        

            pa_context* ctx = context.GetContext();
            

            MainLoop.Instance.Lock();
            stream = pa_stream_new(ctx, ptrName, &sample_spec, &channel_map);
            MainLoop.Instance.Unlock();

            
            
            Marshal.FreeHGlobal(ptrName);
        }

        public static AudioStream CreateMonoStream(PAContext context, string name, SampleSpec sampleSpec)
        {
            AudioStream audioStream = new();

            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);

            pa_sample_spec sample_spec = SampleSpec.Convert(sampleSpec);


            pa_channel_map channel_map = ChannelMap.MonoChannelMap;


            pa_context* ctx = context.GetContext();


            MainLoop.Instance.Lock();
            audioStream.stream = pa_stream_new(ctx, ptrName, &sample_spec, &channel_map);
            MainLoop.Instance.Unlock();



            Marshal.FreeHGlobal(ptrName);

            return audioStream;
        }

        public static AudioStream CreateStereoStream(PAContext context, string name, SampleSpec sampleSpec)
        {
            AudioStream audioStream = new();

            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);

            pa_sample_spec sample_spec = SampleSpec.Convert(sampleSpec);


            pa_channel_map channel_map = ChannelMap.StereoChannelMap;


            pa_context* ctx = context.GetContext();


            MainLoop.Instance.Lock();
            audioStream.stream = pa_stream_new(ctx, ptrName, &sample_spec, &channel_map);
            MainLoop.Instance.Unlock();



            Marshal.FreeHGlobal(ptrName);

            return audioStream;
        }

        //public AudioStream(PAContext context, string name, SampleSpec sampleSpec, ChannelMap channelMap, PropList propList)
        //{
        //    IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);

        //    pa_sample_spec* sample_spec = SampleSpec.Convert(sampleSpec);
        //    pa_channel_map* channel_map = ChannelMap.Convert(channelMap);
        //    pa_proplist* proplist = PropList.Convert(propList);

        //    stream = pa_stream_new_with_proplist(context.GetContext(), ptrName, sample_spec, channel_map, proplist);

        //    pa_proplist.pa_proplist_free(proplist);

        //    Marshal.FreeHGlobal((IntPtr)channel_map);
        //    Marshal.FreeHGlobal((IntPtr)sample_spec);
        //    Marshal.FreeHGlobal(ptrName);
        //}

        internal AudioStream(pa_stream* ptr)
        {
            stream = ptr;

            Ref();
        }

        internal void Ref()
        {
            Monitor.Enter(this);

            pa_stream_ref(stream);

            Monitor.Exit(this);
        }

        internal void Unref()
        {
            Monitor.Enter(this);

            pa_stream_unref(stream);

            Monitor.Exit(this);
        }

        public Task<StreamState> GetStateAsync() => Task.Run(GetState);

        public StreamState GetState()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            StreamState state = pa_stream_get_state(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return state;

        }

        public Task<uint> GetIndexAsync() => Task.Run(GetIndex);

        public uint GetIndex()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            uint index = pa_stream_get_index(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return index;
        }

        public Task<uint> GetDeviceIndexAsync() => Task.Run(GetDeviceIndex);

        public uint GetDeviceIndex()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            uint device = pa_stream_get_device_index(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return device;
        }

        public Task<string?> GetDeviceNameAsync() => Task.Run(GetDeviceName);

        public string? GetDeviceName()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            IntPtr ptr = pa_stream_get_device_name(stream);

            MainLoop.Instance.Unlock();

            string? deviceName = Marshal.PtrToStringUTF8(ptr);
            Marshal.FreeHGlobal(ptr);

            Monitor.Exit(this);

            return deviceName;
        }

        public Task<bool> GetIsSuspendedAsync() => Task.Run(GetIsSuspended);

        public bool GetIsSuspended()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            int sus = pa_stream_is_suspended(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return sus == 1;
        }

        public Task<bool> GetIsCorkedAsync() => Task.Run(GetIsCorked);

        public bool GetIsCorked()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            int corked = pa_stream_is_corked(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return corked == 1;
        }

        public Task<bool> ConnectPlaybackAsync(string sink, BufferAttr? attr, StreamFlags flags, Volume? volume = null, AudioStream? sync = null)
            => Task.Run(() => ConnectPlayback(sink, attr, flags, volume, sync));

        public bool ConnectPlayback(string sink, BufferAttr? attr, StreamFlags flags, Volume? volume = null, AudioStream? sync = null)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(sink);

            pa_buffer_attr* buffer_attr = null;

            if (attr != null)
                *buffer_attr = BufferAttr.Convert(attr);

            pa_cvolume cvolume;
            pa_cvolume* ptrVol;
            if (volume != null)
            {
                cvolume = Volume.Convert(volume);
                ptrVol = &cvolume;
            }
            else
            {
                ptrVol = null;
            }

            pa_stream* sync_stream;
            if (sync != null)
                sync_stream = sync.stream;
            else
                sync_stream = null;


            Monitor.Enter(this);
            MainLoop.Instance.Lock();
            int result = pa_stream_connect_playback(stream, ptr, buffer_attr, flags, ptrVol, sync_stream);
            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            Marshal.FreeHGlobal(ptr);

            return result == 0;
        }

        public Task<bool> ConnectRecordAsync(string source, BufferAttr attr, StreamFlags flags) => Task.Run(() => ConnectRecord(source, attr, flags));

        public bool ConnectRecord(string source, BufferAttr attr, StreamFlags flags)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(source);

            pa_buffer_attr buffer_attr = BufferAttr.Convert(attr);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();
            int result = pa_stream_connect_record(stream, ptr, &buffer_attr, flags);
            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return result == 0;
        }

        public bool Disconnect()
        {
            int result = pa_stream_disconnect(stream);

            return result == 0;
        }

        public Task<AudioBuffer?> BeginWriteAsync(uint size) => Task.Run(() => BeginWrite(size));

        public AudioBuffer? BeginWrite(uint size)
        {

            void* ptr = null;

            uint nsize = size;

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            int result = pa_stream_begin_write(stream, &ptr, &nsize);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            AudioBuffer? audioBuffer = default;
            if (result == 0)
            {
                audioBuffer = new()
                {
                    Ptr = (IntPtr)ptr,
                    nSize = nsize
                };
            }
            return audioBuffer;
        }        

        public Task<bool> CancelWriteAsync() => Task.Run(CancelWrite);

        public bool CancelWrite()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();
            int result = pa_stream_cancel_write(stream);
            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
            return result == 0;
        }

        public Task<bool> WriteAsync(AudioBuffer audioBuffer) => Task.Run(() => Write(audioBuffer));

        public bool Write(AudioBuffer audioBuffer)
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            int result = pa_stream_write(stream, (void*)audioBuffer.Ptr, audioBuffer.nSize, null, 0, SeekMode.RELATIVE);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return result == 0;

        }

        public Task<AudioBuffer?> PeekAsync() => Task.Run(Peek);

        public AudioBuffer? Peek()
        {
            Monitor.Enter(this);


            IntPtr ptr = IntPtr.Zero;
            void* p = &ptr;

            uint nSize = 0;

            MainLoop.Instance.Lock();
            int result = pa_stream_peek(stream, &p, &nSize);
            MainLoop.Instance.Unlock();

            Monitor.Exit(this);

            AudioBuffer? audioBuffer = default;
            if (result == 0)
            {
                audioBuffer = new()
                {
                    Ptr = ptr,
                    nSize = nSize
                };
            }

            return audioBuffer;
        }

        public Task<bool> DropAsync() => Task.Run(Drop);

        public bool Drop()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            int result = pa_stream_drop(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return result == 0;
        }

        public Task<uint> GetWritableSizeAsync() => Task.Run(GetWritableSize);

        public uint GetWritableSize()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            uint nsize = pa_stream_writable_size(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return nsize;

        }

        public Task<uint> GetReadableSizeAsync() => Task.Run(GetReadableSize);

        public uint GetReadableSize()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            uint nsize = pa_stream_readable_size(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return nsize;
        }

        public Task DrainAsync() => Task.Run(Drain);

        public void Drain()
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, int success, void* userdata)
            {
                OperationState* state = (OperationState*)userdata;
                *state = OperationState.DONE;

                MainLoop.Instance.Signal(0);
            }

            Monitor.Enter(this);
            OperationState op_state = OperationState.RUNNING;
            void* pstate = &op_state;

            MainLoop.Instance.Lock();
            pa_operation* op = pa_stream_drain(stream, &Callback, pstate);

            while (op_state == OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task UpdateTimingInfoAsync() => Task.Run(UpdateTimingInfo);

        public void UpdateTimingInfo()
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, int success, void* userdata)
            {
                OperationState* state = (OperationState*)userdata;
                *state = OperationState.DONE;

                MainLoop.Instance.Signal(0);
            }

            Monitor.Enter(this);
            OperationState op_state = OperationState.RUNNING;
            void* pstate = &op_state;

            MainLoop.Instance.Lock();
            pa_operation* op = pa_stream_update_timing_info(stream, &Callback, pstate);

            while (op_state == OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

        }

        public Task SetStateCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetStartedCallback(callback));

        public void SetStateCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_state_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetWriteCallbackAsync(StreamRequestCallback callback) => Task.Run(() => SetWriteCallback(callback));

        public void SetWriteCallback(StreamRequestCallback callback)
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, uint nsize, void* userdata)
            {
                StreamRequestCallback action = Marshal.GetDelegateForFunctionPointer<StreamRequestCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream, nsize);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_write_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetReadCallbackAsync(StreamRequestCallback callback) => Task.Run(() => SetReadCallback(callback));

        public void SetReadCallback(StreamRequestCallback callback)
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, uint nsize, void* userdata)
            {
                StreamRequestCallback action = Marshal.GetDelegateForFunctionPointer<StreamRequestCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream, nsize);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_read_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetOverflowCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetOverflowCallback(callback));

        public void SetOverflowCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_overflow_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task<long> GetUnderflowIndexAsync() => Task.Run(GetUnderflowIndex);

        public long GetUnderflowIndex()
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            long result = pa_stream_get_underflow_index(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return result;
        }

        public Task SetUnderflowCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetUnderflowCallback(callback));

        public void SetUnderflowCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_underflow_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetStartedCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetStartedCallback(callback));

        public void SetStartedCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_started_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetLatencyUpdateCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetLatencyUpdateCallback(callback));

        public void SetLatencyUpdateCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_latency_update_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetMovedCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetMovedCallback(callback));

        public void SetMovedCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_moved_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetSuspendedCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetSuspendedCallback(callback));

        public void SetSuspendedCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_suspended_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetEventCallbackAsync(StreamEventCallback callback) => Task.Run(() => SetEventCallback(callback));

        public void SetEventCallback(StreamEventCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, IntPtr ptrName, pa_proplist* pl, void* userdata)
            {
                string? name = Marshal.PtrToStringUTF8(ptrName);
                PropList propList = PropList.Convert(pl);

                StreamEventCallback action = Marshal.GetDelegateForFunctionPointer<StreamEventCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream, name, propList);
                }


            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_event_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task SetBufferAttrCallbackAsync(StreamNotifyCallback callback) => Task.Run(() => SetBufferAttrCallback(callback));

        public void SetBufferAttrCallback(StreamNotifyCallback callback)
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                StreamNotifyCallback action = Marshal.GetDelegateForFunctionPointer<StreamNotifyCallback>((IntPtr)userdata);

                using (AudioStream audioStream = new(stream))
                {
                    action(audioStream);
                }

            }

            IntPtr func = Marshal.GetFunctionPointerForDelegate(callback);

            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            pa_stream_set_buffer_attr_callback(stream, &Callback, (void*)func);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task CorkAsync(bool pause) => Task.Run(() => Cork(pause));

        private void Cork(bool pause)
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, int success, void* userdata)
            {
                OperationState* state = (OperationState*)userdata;
                *state = OperationState.DONE;

                MainLoop.Instance.Signal(0);
            }

            Monitor.Enter(this);
            OperationState op_state = OperationState.RUNNING;
            void* pstate = &op_state;

            int b = pause ? 1 : 0;

            MainLoop.Instance.Lock();
            pa_operation* op = pa_stream_cork(stream, b, &Callback, pstate);

            while (op_state == OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        public Task FlushAsync => Task.Run(Flush);

        public void Flush()
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, int success, void* userdata)
            {
                OperationState* state = (OperationState*)userdata;
                *state = OperationState.DONE;

                MainLoop.Instance.Signal(0);
            }

            Monitor.Enter(this);
            OperationState op_state = OperationState.RUNNING;
            void* pstate = &op_state;

            MainLoop.Instance.Lock();
            pa_operation* op = pa_stream_flush(stream, &Callback, pstate);

            while (op_state == OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }


        public Task PreBufAsync() => Task.Run(PreBuf);

        public void PreBuf()
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, int success, void* userdata)
            {
                OperationState* state = (OperationState*)userdata; 
                *state = OperationState.DONE;

                MainLoop.Instance.Signal(0);
            }

            Monitor.Enter(this);
            OperationState op_state = OperationState.RUNNING;
            void* pstate = &op_state;

            MainLoop.Instance.Lock();
            pa_operation* op = pa_stream_prebuf(stream, &Callback, pstate);

            while (op_state == OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                Unref();

                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~AudioStream()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
