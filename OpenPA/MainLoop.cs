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
        /// <summary>
        /// Unmanaged mainloop handle
        /// </summary>
        pa_threaded_mainloop* mainloop;

        /// <summary>
        /// Unmanaged mainloop api handle
        /// </summary>
        pa_mainloop_api* mainloop_api;

        private bool disposedValue;

        /// <summary>
        /// True if the mainloop has been started
        /// </summary>
        private bool running;

        /// <summary>
        /// True if the mainloop has been started
        /// </summary>
        public bool Running => running;

        /// <summary>
        /// Mainloop API handle
        /// </summary>
        internal pa_mainloop_api* API => mainloop_api;

        public MainLoop()
        {
            // Create the mainloop handle
            mainloop = pa_threaded_mainloop.pa_threaded_mainloop_new();

            // Store the mainloop api handle
            mainloop_api = pa_threaded_mainloop.pa_threaded_mainloop_get_api(mainloop);
        }

        /// <summary>
        /// Starts the mainloop
        /// </summary>
        public void Start()
        {
            if (mainloop == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                // Call the native function to start the mainloop
                pa_threaded_mainloop.pa_threaded_mainloop_start(mainloop);

                // Flag we are running
                running = true;
            }
        }

        /// <summary>
        /// Stop the mainloop
        /// </summary>
        public void Stop()
        {
            if (mainloop == null || running == false)
            {
                throw new InvalidOperationException();
            }
            else
            {            
                // Call the native function to stop the mainloop
                pa_threaded_mainloop.pa_threaded_mainloop_stop(mainloop);

                // Flag we are not running
                running = false;
            }
        }

        /// <summary>
        /// Locks the mainloop
        /// </summary>
        public void Lock() => pa_threaded_mainloop.pa_threaded_mainloop_lock(mainloop);
        
        /// <summary>
        /// Unlocks the main loop
        /// </summary>
        public void Unlock() => pa_threaded_mainloop.pa_threaded_mainloop_unlock(mainloop);
        
        /// <summary>
        /// Waits on the mainloop
        /// </summary>
        public void Wait() => pa_threaded_mainloop.pa_threaded_mainloop_wait(mainloop);
        
        /// <summary>
        /// Signals the mainloop
        /// </summary>
        /// <param name="i"></param>
        public void Signal(int i) => pa_threaded_mainloop.pa_threaded_mainloop_signal(mainloop, i);
        
        /// <summary>
        /// Accepts a signal
        /// </summary>
        public void Accept() => pa_threaded_mainloop.pa_threaded_mainloop_accept(mainloop);
        
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
