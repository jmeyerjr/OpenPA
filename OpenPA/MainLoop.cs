using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPA.Native;

namespace OpenPA
{
    public unsafe class MainLoop : IDisposable
    {
        pa_threaded_mainloop* mainloop;
        pa_mainloop_api* mainloop_api;

        private bool disposedValue;
        private bool running;

        public bool Running => running;
        internal pa_mainloop_api* API => mainloop_api;

        public MainLoop()
        {
            mainloop = pa_threaded_mainloop.pa_threaded_mainloop_new();
            mainloop_api = pa_threaded_mainloop.pa_threaded_mainloop_get_api(mainloop);
        }

        public void Start()
        {
            if (mainloop == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                pa_threaded_mainloop.pa_threaded_mainloop_start(mainloop);
                running = true;
            }
        }

        public void Stop()
        {
            if (mainloop == null || running == false)
            {
                throw new InvalidOperationException();
            }
            else
            {                
                pa_threaded_mainloop.pa_threaded_mainloop_stop(mainloop);
                running = false;
            }
        }

        public void Lock()
        {
            pa_threaded_mainloop.pa_threaded_mainloop_lock(mainloop);
        }

        public void Unlock()
        {
            pa_threaded_mainloop.pa_threaded_mainloop_unlock(mainloop);
        }

        public void Wait()
        {
            pa_threaded_mainloop.pa_threaded_mainloop_wait(mainloop);
        }

        public void Signal(int i)
        {
            pa_threaded_mainloop.pa_threaded_mainloop_signal(mainloop, i);
        }

        public void Accept()
        {
            pa_threaded_mainloop.pa_threaded_mainloop_accept(mainloop);
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
                if (mainloop != null)
                {
                    if (running)
                        Stop();

                    pa_threaded_mainloop.pa_threaded_mainloop_free(mainloop);
                    mainloop = null;
                }
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MainLoop()
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
