using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.WhisperCpp {
    internal class ModelSource : IDisposable {
        private Native.WhisperModelLoader _ldr;
        private System.IO.Stream _stmSource;
        private byte[] _arrBuffer;
        private bool _bReachedEOF, _bDisposeStream;

        public ModelSource(System.IO.Stream stmSource, bool bDisposeStream) {
            _stmSource = stmSource;
            _bDisposeStream = bDisposeStream;
            _ldr.Context = (IntPtr)GCHandle.Alloc(this, GCHandleType.Weak);
            _ldr.ReadFunction = Marshal.GetFunctionPointerForDelegate(_procRead);
            _ldr.EndOfFileCheckFunction = Marshal.GetFunctionPointerForDelegate(_procEOFCheck);
            _ldr.CloseFunction = Marshal.GetFunctionPointerForDelegate(_procClose);
        }

        public ref Native.WhisperModelLoader Loader => ref _ldr;

        private static Native.WhisperModelLoader.ReadDelegate _procRead = ReadWrapper;
        private static Native.WhisperModelLoader.EOFCheckDelegate _procEOFCheck = EOFCheckWrapper;
        private static Native.WhisperModelLoader.CloseDelegate _procClose = CloseWrapper;

        private static IntPtr ReadWrapper(IntPtr hSource, IntPtr pOutput, IntPtr nReadSize) {
            try {
                return FromHandle(hSource).Read(pOutput, nReadSize);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error in {nameof(ModelSource)} {nameof(ReadWrapper)}: {ex}");
                return IntPtr.Zero;
            }
        }

        private static bool EOFCheckWrapper(IntPtr hSource) {
            try {
                return FromHandle(hSource).IsAtEOF;
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error in {nameof(ModelSource)} {nameof(EOFCheckWrapper)}: {ex}");
                return true;
            }
        }

        private static void CloseWrapper(IntPtr hSource) {
            try {
                FromHandle(hSource).Dispose();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error in {nameof(ModelSource)} {nameof(CloseWrapper)}: {ex}");
            }
        }

        private static ModelSource FromHandle(IntPtr hSource)
            => (ModelSource)GCHandle.FromIntPtr(hSource).Target;

        private IntPtr Read(IntPtr pOutput, IntPtr nReadSize) {
            int nActualReadSize = (long)nReadSize < int.MaxValue ? (int)nReadSize : int.MaxValue;

            if (nActualReadSize > (_arrBuffer?.Length ?? 0))
                _arrBuffer = new byte[nActualReadSize];

            int nBytesRead = _stmSource.Read(_arrBuffer, 0, nActualReadSize);
            if (nBytesRead <= 0) {
                _bReachedEOF = true;
                return IntPtr.Zero;
            }

            Marshal.Copy(_arrBuffer, 0, pOutput, nActualReadSize);
            return (IntPtr)nBytesRead;
        }

        private bool IsAtEOF => _bReachedEOF;

        public void Dispose() {
            if (_bDisposeStream)
                System.Threading.Interlocked.Exchange(ref _stmSource, null)?.Dispose();

            FreeGCHandle();
            GC.SuppressFinalize(this);
        }

        ~ModelSource() => FreeGCHandle();

        private void FreeGCHandle() {
            IntPtr hSelf = System.Threading.Interlocked.Exchange(ref _ldr.Context, IntPtr.Zero);
            if (hSelf != IntPtr.Zero)
                GCHandle.FromIntPtr(hSelf).Free();
        }
    }
}
