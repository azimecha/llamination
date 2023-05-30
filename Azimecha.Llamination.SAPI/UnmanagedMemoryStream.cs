using SpeechLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.SAPI {
    internal class UnmanagedMemoryStream : IDisposable {
        private IStream _stm;

        public UnmanagedMemoryStream() {
            _stm = CreateStream(IntPtr.Zero, true);
        }

        public UnmanagedMemoryStream(int nSize) {
            AutoHGlobal ptr = AutoHGlobal.Allocate(nSize);
            _stm = CreateStream(ptr.Value, true);
            ptr.Abandon();
        }

        public UnmanagedMemoryStream(byte[] arrData, int nStartIndex = 0, int nLength = -1) {
            AutoHGlobal ptr = AutoHGlobal.Allocate(arrData.Length);
            Marshal.Copy(arrData, nStartIndex, ptr.Value, (nLength < 0) ? (arrData.Length - nStartIndex) : nLength);
            _stm = CreateStream(ptr.Value, true);
            ptr.Abandon();
        }

        private static IStream CreateStream(IntPtr hGlobal, bool bDeleteOnRelease) {
            int nResult = CreateStreamOnHGlobal(hGlobal, bDeleteOnRelease, out IStream stm);
            if (nResult < 0)
                Marshal.ThrowExceptionForHR(nResult);
            return stm;
        }

        public IStream Stream => _stm;

        public void WithBufferPointer(Action<IntPtr> procCallback) {
            int nResult = GetHGlobalFromStream(_stm, out IntPtr hGlobal);
            if (nResult < 0)
                Marshal.ThrowExceptionForHR(nResult);

            IntPtr pData = IntPtr.Zero;

            try {
                pData = GlobalLock(hGlobal);

                if (pData == IntPtr.Zero) {
                    if (GlobalSize(hGlobal) == IntPtr.Zero)
                        return;

                    throw new InsufficientMemoryException("Unable to lock HGLOBAL in memory");
                }

                procCallback(pData);
            } finally {
                if (pData != IntPtr.Zero)
                    GlobalUnlock(hGlobal);
            }
        }

        public ulong CurrentBufferSize {
            get {
                _stm.Stat(out tagSTATSTG stat, (uint)StatFlags.NoName);
                return stat.cbSize.QuadPart;
            }
        }

        public ulong Position {
            get {
                _stm.RemoteSeek(new _LARGE_INTEGER(), (uint)StreamSeekOrigin.Current, out _ULARGE_INTEGER liPos);
                return liPos.QuadPart;
            }
            set {
                _stm.RemoteSeek(new _LARGE_INTEGER() { QuadPart = (long)value }, (uint)StreamSeekOrigin.Beginning, out _);
            }
        }

        public unsafe ulong GetElementCount<T>(bool bEntireBuffer = false) where T : unmanaged
            => (bEntireBuffer ? CurrentBufferSize : Position) / (ulong)sizeof(T);

        public unsafe T[] RetrieveContents<T>(bool bEntireBuffer = false) where T : unmanaged {
            ulong nFullSize = bEntireBuffer ? CurrentBufferSize : Position;
            if (nFullSize > Extras.IntPtrMax)
                throw new InsufficientMemoryException("The stream contains more data than can be stored in memory");

            ulong nFullCount = nFullSize / (ulong)sizeof(T);
            if (nFullCount > int.MaxValue)
                throw new InsufficientMemoryException("The stream contains more elements than can be held in an array");

            int nCount = (int)nFullCount;
            T[] arrData = new T[nCount];

            WithBufferPointer(pBuffer => {
                fixed (T* pData = arrData)
                    MemCpy((IntPtr)pData, pBuffer, (IntPtr)nFullSize);
            });

            return arrData;
        }

        public void Dispose() {
            _stm = null;
        }

        [DllImport("ole32")]
        private static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool bDeleteOnRelease, out IStream stm);

        [DllImport("ole32")]
        private static extern int GetHGlobalFromStream(IStream stm, out IntPtr hGlobal);

        [DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcpy")]
        public static extern IntPtr MemCpy(IntPtr pDest, IntPtr pSource, IntPtr nSize);

        [DllImport("kernel32")]
        public static extern IntPtr GlobalLock(IntPtr hGlobal);

        [DllImport("kernel32")]
        public static extern bool GlobalUnlock(IntPtr hGlobal);

        [DllImport("kernel32")]
        public static extern IntPtr GlobalSize(IntPtr hGlobal);

        [Flags]
        private enum StatFlags : uint {
            Default = 0,
            NoName = 1,
            NoOpen = 2
        }

        private enum StreamSeekOrigin : uint {
            Beginning = 0,
            Current = 1,
            End = 2
        }
    }
}
