using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    internal class SongHandle : IDisposable {
        private int _nHandle = -1;

        public int RawValue 
            => _nHandle;

        public int Value {
            get {
                int nHandle = _nHandle;
                if (nHandle < 0)
                    throw new InvalidOperationException("Song handle is invalid");
                return nHandle;
            }
            set {
                if (System.Threading.Interlocked.CompareExchange(ref _nHandle, value, -1) != -1)
                    throw new InvalidOperationException("Song handle already initialized");
            }
        }

        private void CloseHandle() {
            try { } finally {
                int nHandle = System.Threading.Interlocked.Exchange(ref _nHandle, -1);
                if (nHandle > 0)
                    V4UAPI.YVFCloseSong(nHandle);
            }
        }

        public void Dispose() {
            CloseHandle();
            GC.SuppressFinalize(this);
        }

        ~SongHandle() {
            CloseHandle();
        }
    }
}
