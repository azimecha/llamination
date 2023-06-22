using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaNativeState : IDisposable {
        private Pointers.HGlobalPointer _ptrData;
        private long _nTotalBytes, _nValidBytes;

        internal LlamaNativeState(Pointers.HGlobalPointer ptrData, long nTotalBytes, long nValidBytes) {
            _ptrData = ptrData;
            _nTotalBytes = nTotalBytes;
            _nValidBytes = nValidBytes;
        }

        internal LlamaNativeState(IntPtr nSize) {
            _ptrData = new Pointers.HGlobalPointer();

            IntPtr pData = IntPtr.Zero;
            try {
                pData = Marshal.AllocHGlobal(nSize);
                _ptrData.Value = pData;
                pData = IntPtr.Zero;
            } finally {
                if (!_ptrData.Initialized && pData != IntPtr.Zero)
                    Marshal.FreeHGlobal(pData);
            }

            _nTotalBytes = (long)nSize;
        }

        public static LlamaNativeState ReadFromModel(LlamaModel mdl) {
            IntPtr nSizeEst = Native.Functions.LlamaGetStateSize(mdl.Context.Value);
            LlamaNativeState state = new LlamaNativeState(nSizeEst);
            state._nValidBytes = (long)Native.Functions.LlamaCopyStateData(mdl.Context.Value, state._ptrData.Value);
            return state;
        }

        internal long BufferSize => _nTotalBytes;
        internal long DataSize => _nValidBytes;
        internal IntPtr Buffer => _ptrData.Value;

        /*public unsafe void WriteTo(System.IO.Stream stmDest) {
            stmDest.Write(BitConverter.GetBytes(_nTotalBytes), 0, sizeof(long));
            stmDest.Write(BitConverter.GetBytes(_nValidBytes), 0, sizeof(long));

            byte[] arrBuffer = new byte[0xFFFF];
            byte* pCurSrc = (byte*)_ptrData.Value;
            long nBytesLeft = _nValidBytes;
            
            while (nBytesLeft > 0) {
                int nCurToWrite = nBytesLeft < arrBuffer.Length ? (int)nBytesLeft : arrBuffer.Length;

                Marshal.Copy((IntPtr)pCurSrc, arrBuffer, 0, nCurToWrite);
                stmDest.Write(arrBuffer, 0, nCurToWrite);

                nBytesLeft -= nCurToWrite;
                pCurSrc += nCurToWrite;
            }
        }*/

        public void Dispose() {
            System.Threading.Interlocked.Exchange(ref _ptrData, null)?.Dispose();
        }
    }
}
