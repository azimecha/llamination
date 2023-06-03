using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.FileFormats.Native {
    internal class STBIOWrapper : IDisposable {
        private System.IO.Stream _stmInner;
        private IntPtr _hSelf;
        private Exception _exFromStream;
        private byte[] _arrReadBuffer;
        private bool _bAtEOF = false;

        public STBIOWrapper(System.IO.Stream stm) {
            try { } finally {
                _hSelf = (IntPtr)GCHandle.Alloc(this, GCHandleType.Weak);
            }

            _stmInner = stm;
        }

        public Exception IOError => _exFromStream;
        public STBImage.IOCallbacks CallbacksStruct => CALLBACKS;
        public IntPtr UserData => _hSelf;

        private unsafe int Read(IntPtr pBuffer, int nBufferSize) {
            if (nBufferSize == 1) {
                int nByteVal = _stmInner.ReadByte();
                _bAtEOF = nByteVal < 0;

                if (_bAtEOF) {
                    return 0;
                } else {
                    *(byte*)pBuffer = (byte)nByteVal;
                    return 1;
                }
            } else {
                if (nBufferSize > (_arrReadBuffer?.Length ?? -1))
                    _arrReadBuffer = new byte[nBufferSize];

                int nBytesRead = _stmInner.Read(_arrReadBuffer, 0, nBufferSize);
                Marshal.Copy(_arrReadBuffer, 0, pBuffer, nBytesRead);

                if ((nBufferSize > 0) && (nBytesRead == 0))
                    _bAtEOF = true;

                return nBytesRead;
            }
        }

        private void Skip(int nBytes) {
            if (nBytes >= 0) {
                while (nBytes > 0) {
                    _bAtEOF = _stmInner.ReadByte() < 0;
                    nBytes--;
                    if (_bAtEOF) return;
                }
            } else {
                _stmInner.Seek(nBytes, System.IO.SeekOrigin.Current);
                _bAtEOF = false;
            }
        }

        private int CheckEOF()
            => _bAtEOF ? 1 : 0;

        private static readonly STBImage.IOCallbacks CALLBACKS = new STBImage.IOCallbacks() {
            Read = StaticRead,
            Skip = StaticSkip,
            EOF = StaticCheckEOF
        };

        private delegate int NonstaticDelegate(STBIOWrapper wrapper);

        private static int RunWithInstance(IntPtr hSelf, NonstaticDelegate proc) {
            STBIOWrapper wrapper = null;

            try {
                wrapper = (STBIOWrapper)GCHandle.FromIntPtr(hSelf).Target;
            } catch (Exception) {
                return 0;
            }

            try {
                return proc(wrapper);
            } catch (Exception ex) {
                wrapper._exFromStream = ex;
                return 0;
            }
        }

        private static int StaticRead(IntPtr hSelf, IntPtr pBuffer, int nBufferSize)
            => RunWithInstance(hSelf, wrapper => wrapper.Read(pBuffer, nBufferSize));

        private static void StaticSkip(IntPtr hSelf, int nBytes)
            => RunWithInstance(hSelf, wrapper => { wrapper.Skip(nBytes); return 0; });

        private static int StaticCheckEOF(IntPtr hSelf)
            => RunWithInstance(hSelf, wrapper => wrapper.CheckEOF());

        private void FreeGCHandle() {
            try { } finally {
                IntPtr hSelf = Interlocked.Exchange(ref _hSelf, IntPtr.Zero);
                if (hSelf != IntPtr.Zero)
                    GCHandle.FromIntPtr(hSelf).Free();
            }
        }

        public void Dispose() {
            _arrReadBuffer = null;
            FreeGCHandle();
            GC.SuppressFinalize(this);
        }

        ~STBIOWrapper() {
            FreeGCHandle();
        }
    }
}
