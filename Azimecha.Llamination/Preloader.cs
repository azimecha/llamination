using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination {
    internal class Preloader : IDisposable {
        private object _objStartStopMutex = new object();
        private ManualResetEvent _evtStop = new ManualResetEvent(false);
        private ManualResetEvent _evtFinished = new ManualResetEvent(false);
        private bool _bStarted = false;
        private System.IO.Stream _stmToPreload;
        private Thread _thdPreload;

        public Preloader(string strFilePath) {
            _stmToPreload = System.IO.File.OpenRead(strFilePath);
        }

        public Preloader(System.IO.Stream stm) {
            _stmToPreload = stm;
        }

        private struct DataBag {
            public ManualResetEvent StopEvent;
            public System.IO.Stream StreamToPreload;
            public ManualResetEvent FinishedEvent;
        }

        public void Start() {
            lock (_objStartStopMutex) {
                if (_bStarted)
                    throw new InvalidOperationException("Preloader already started");

                Thread thdPreload = new Thread(PreloadThreadProc);
                thdPreload.Name = "File Preload Thread";
                thdPreload.Start(new DataBag() {
                    StopEvent = _evtStop,
                    StreamToPreload = _stmToPreload,
                    FinishedEvent = _evtFinished
                });

                _thdPreload = thdPreload;
                _bStarted = true;
            }
        }

        public WaitHandle FinishedEvent => _evtFinished;

        public void Stop() {
            lock (_objStartStopMutex) {
                _evtStop.Set();

#if NETFRAMEWORK
                if (!_thdPreload.Join(1000))
                    _thdPreload.Abort();
#else
                _thdPreload.Join(1000);
#endif

                _thdPreload = null;
            }
        }

        public void Dispose() {
            lock (_objStartStopMutex) {
                _evtStop.Set();

                if (!_bStarted)
                    _stmToPreload.Dispose();
            }
        }

        ~Preloader() {
            _evtStop.Set();
        }

        private static void PreloadThreadProc(object objData) {
            DataBag data = (DataBag)objData;

            try {
                byte[] arrBuffer = new byte[0xFFFFF];

                WaitHandle[] arrWaitHandles = new WaitHandle[2];
                arrWaitHandles[1] = data.StopEvent;

                bool bFinished = false;

                while (!bFinished) {
                    IAsyncResult result = data.StreamToPreload.BeginRead(arrBuffer, 0, arrBuffer.Length, null, null);
                    arrWaitHandles[0] = result.AsyncWaitHandle;

                    switch (WaitHandle.WaitAny(arrWaitHandles)) {
                        case 0:
                            bFinished = (data.StreamToPreload.EndRead(result) == 0);
                            break;

                        case 1:
                            data.StreamToPreload.Dispose();
                            return;
                    }
                }
            } catch (Exception) {}

            try {
                data.FinishedEvent.Set();
            } catch (Exception) {}
        }
    }
}
