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

namespace OpenPA
{
    public class AudioBuffer
    {
        public IntPtr Ptr { get; init; }
        public uint nSize { get; init; }
    }

    public unsafe class AudioStream : IDisposable
    {
        pa_stream* stream;
        private bool disposedValue;

        public AudioStream(PAContext context, string name, SampleSpec sampleSpec, ChannelMap channelMap)
        {
            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);

            pa_sample_spec sample_spec = SampleSpec.Convert(sampleSpec);
            pa_channel_map channel_map = ChannelMap.Convert(channelMap);

            stream = pa_stream_new(context.GetContext(), ptrName, &sample_spec, &channel_map);

            Marshal.FreeHGlobal(ptrName);
        }

        public AudioStream(PAContext context, string name, SampleSpec sampleSpec, ChannelMap channelMap, PropList propList)
        {
            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);

            pa_sample_spec sample_spec = SampleSpec.Convert(sampleSpec);
            pa_channel_map channel_map = ChannelMap.Convert(channelMap);
            pa_proplist* proplist = PropList.Convert(propList);

            stream = pa_stream_new_with_proplist(context.GetContext(), ptrName, &sample_spec, &channel_map, proplist);

            pa_proplist.pa_proplist_free(proplist);

            Marshal.FreeHGlobal(ptrName);
        }

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

        public Task<StreamState> GetStateAsync()
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();

                StreamState state = pa_stream_get_state(stream);

                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                return state;
            });
        }

        public Task<uint> GetIndexAsync()
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();

                uint index = pa_stream_get_index(stream);

                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                return index;
            });
        }

        public Task<uint> GetDeviceIndexAsync()
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();

                uint device = pa_stream_get_device_index(stream);

                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                return device;
            });
        }

        public Task<string?> GetDeviceNameAsync()
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();

                IntPtr ptr = pa_stream_get_device_name(stream);

                MainLoop.Instance.Unlock();

                string? deviceName = Marshal.PtrToStringUTF8(ptr);
                Marshal.FreeHGlobal(ptr);

                Monitor.Exit(this);

                return deviceName;
            });
        }

        public Task<bool> GetIsSuspendedAsync()
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();

                int sus = pa_stream_is_suspended(stream);

                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                return sus == 1;
            });
        }

        public Task<bool> GetIsCorkedAsync()
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();

                int corked = pa_stream_is_corked(stream);

                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                return corked == 1;
            });
        }

        public unsafe Task<bool> ConnectPlaybackAsync(string sink, BufferAttr attr, StreamFlags flags, Volume? volume = null, AudioStream? sync = null)
        {
            return Task.Run(() =>
            {
                IntPtr ptr = Marshal.StringToHGlobalAnsi(sink);

                pa_buffer_attr buffer_attr = BufferAttr.Convert(attr);

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
                int result = pa_stream_connect_playback(stream, ptr, &buffer_attr, flags, ptrVol, sync_stream);
                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                Marshal.FreeHGlobal(ptr);

                return result == 0;
            });
        }

        public unsafe Task<bool> ConnectRecordAsync(string source, BufferAttr attr, StreamFlags flags)
        {
            return Task.Run(() =>
            {
                IntPtr ptr = Marshal.StringToHGlobalAnsi(source);

                pa_buffer_attr buffer_attr = BufferAttr.Convert(attr);

                Monitor.Enter(this);
                MainLoop.Instance.Lock();
                int result = pa_stream_connect_record(stream, ptr, &buffer_attr, flags);
                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                return result == 0;
            });
        }

        public bool Disconnect()
        {
            int result = pa_stream_disconnect(stream);

            return result == 0;
        }

        public Task<AudioBuffer?> BeginWriteAsync(uint size)
        {
            return Task.Run(() =>
            {

                IntPtr ptrBuffer = IntPtr.Zero;
                void* ptr = (void*)ptrBuffer;

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
                        Ptr = ptrBuffer,
                        nSize = nsize
                    };
                }
                return audioBuffer;
            });
        }

        public Task<bool> CancelWriteAsync()
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();
                int result = pa_stream_cancel_write(stream);
                MainLoop.Instance.Unlock();
                Monitor.Exit(this);
                return result == 0;
            });
        }

        public Task<bool> WriteAsync(AudioBuffer audioBuffer)
        {
            return Task.Run(() =>
            {
                Monitor.Enter(this);
                MainLoop.Instance.Lock();

                int result = pa_stream_write(stream, (void*)audioBuffer.Ptr, audioBuffer.nSize, null, 0, SeekMode.RELATIVE);

                MainLoop.Instance.Unlock();
                Monitor.Exit(this);

                return result == 0;
            });
        }

        public Task<AudioBuffer?> PeekAsync() => Task.Run(() =>
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
        });

        public Task<bool> DropAsync() => Task.Run(() =>
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            int result = pa_stream_drop(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return result == 0;
        });

        public Task<uint> GetWritableSizeAsync() => Task.Run(() =>
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            uint nsize = pa_stream_writable_size(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return nsize;

        });

        public Task<uint> GetReadableSizeAsync() => Task.Run(() =>
        {
            Monitor.Enter(this);
            MainLoop.Instance.Lock();

            uint nsize = pa_stream_readable_size(stream);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

            return nsize;
        });

        static OperationState drain_state;
        public Task DrainAsync() => Task.Run(() =>
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, int success, void* userdata)
            {
                drain_state = OperationState.DONE;

                MainLoop.Instance.Signal(0);
            }

            Monitor.Enter(this);
            drain_state = OperationState.RUNNING;

            MainLoop.Instance.Lock();
            pa_operation* op = pa_stream_drain(stream, &Callback, null);

            while (drain_state == OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);
        });

        static OperationState update_timing_state;
        public Task UpdateTimingInfoAsync() => Task.Run(() =>
        {
            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, int success, void* userdata)
            {
                update_timing_state = OperationState.DONE;

                MainLoop.Instance.Signal(0);
            }

            Monitor.Enter(this);
            update_timing_state = OperationState.RUNNING;

            MainLoop.Instance.Lock();
            pa_operation* op = pa_stream_update_timing_info(stream, &Callback, null);

            while (update_timing_state == OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();
            Monitor.Exit(this);

        });

        public Task SetStateCallbackAsync(Action<AudioStream> callback) => Task.Run(() =>
        {

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
            static void Callback(pa_stream* stream, void* userdata)
            {
                Action<AudioStream> action = Marshal.GetDelegateForFunctionPointer<Action<AudioStream>>((IntPtr)userdata);

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
        });

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
