using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Azimecha.Llamination.FileFormats.Native {
    [ComVisible(true)]
    internal class DotnetStream : IStream {
        private System.IO.Stream _stmInternal;

        public DotnetStream(System.IO.Stream stmInternal) {
            _stmInternal = stmInternal;
        }

        public unsafe void Read(byte[] pv, int cb, IntPtr pcbRead) {
            int nBytesRead = _stmInternal.Read(pv, 0, cb);

            if (pcbRead != IntPtr.Zero)
                *(int*)pcbRead = nBytesRead;
        }

        public unsafe void Write(byte[] pv, int cb, IntPtr pcbWritten) {
            _stmInternal.Write(pv, 0, cb);

            if (pcbWritten != IntPtr.Zero)
                *(int*)pcbWritten = cb;
        }

        public unsafe void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition) {
            long nNewPos = _stmInternal.Seek(dlibMove, (System.IO.SeekOrigin)dwOrigin);

            if (plibNewPosition != IntPtr.Zero)
                *(long*)plibNewPosition = nNewPos;
        }

        public void SetSize(long libNewSize) {
            _stmInternal.SetLength(libNewSize);
        }

        public unsafe void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten) {
            byte[] arrBuffer = new byte[0xFFFF];
            long nTotalRead = 0, nTotalWritten = 0;

            while (cb > 0) {
                int nBytesRead = _stmInternal.Read(arrBuffer, 0, (cb > arrBuffer.Length) ? arrBuffer.Length : (int)cb);
                nTotalRead += nBytesRead;

                if (nBytesRead == 0)
                    break; // EOF

                int nBytesWritten = 0;
                pstm.Write(arrBuffer, nBytesRead, (IntPtr)(&nBytesWritten));
                nTotalWritten += nBytesWritten;

                cb -= nBytesRead;
            }

            if (pcbRead != IntPtr.Zero)
                *(long*)pcbRead = nTotalRead;

            if (pcbWritten != IntPtr.Zero)
                *(long*)pcbWritten = nTotalWritten;
        }

        public void Commit(int grfCommitFlags) {
            _stmInternal.Flush();
        }

        public void Revert() { }
        public void LockRegion(long libOffset, long cb, int dwLockType) { }
        public void UnlockRegion(long libOffset, long cb, int dwLockType) { }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag) {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG() {
                type = 2, // stream
            };

            try {
                pstatstg.cbSize = _stmInternal.Length;
            } catch (Exception) { }
        }

        public void Clone(out IStream ppstm) {
            ppstm = null;
        }
    }
}
