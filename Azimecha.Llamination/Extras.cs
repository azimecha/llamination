using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination {
    public static class Extras {
        public static int BeginningOfStringToken => LlamaCpp.Native.Functions.LlamaTokenBOS();
        public static int EndOfStringToken => LlamaCpp.Native.Functions.LlamaTokenEOS();

        public static ulong IntPtrMax => (IntPtr.Size < 8) ? 1UL << (IntPtr.Size) : ulong.MaxValue;

        private const int MEMCPY_COUNT_THRESHOLD = 256;
        private const int MEMCPY_SIZE_THRESHOLD = 1024;

        public static unsafe void FastCopy<T>(T[] arrSource, int nSourceOffset, T[] arrDest, int nDestOffset, int nCount)
            where T : unmanaged
        {
            if (nCount < 0)
                throw new ArgumentOutOfRangeException(nameof(nCount));

            if ((nSourceOffset + nCount) > arrSource.Length)
                throw new IndexOutOfRangeException($"Attempted to copy {nCount} elements from offset {nSourceOffset} "
                    + $"in {arrSource.Length} element array");

            if ((nDestOffset + nCount) > arrDest.Length)
                throw new IndexOutOfRangeException($"Attempted to copy {nCount} elements to offset {nSourceOffset} "
                    + $"in {arrSource.Length} element array");

            ulong nSize = (ulong)nCount * (ulong)sizeof(T);
            if (nSize > IntPtrMax)
                throw new ArgumentException($"Cannot copy {nCount} items of type {typeof(T).FullName} - size too large");

            if (((nSize < MEMCPY_SIZE_THRESHOLD) && (nCount < MEMCPY_COUNT_THRESHOLD))
                || Environment.OSVersion.Platform != PlatformID.Win32NT) 
            {
                for (int nElem = 0; nElem < nCount; nElem++)
                    arrDest[nDestOffset + nElem] = arrSource[nSourceOffset + nElem];
            } else {
                fixed (T* pSource = arrSource)
                fixed (T* pDest = arrDest)
                    LlamaCpp.Native.Functions.MemCpy((IntPtr)(pDest + nDestOffset), (IntPtr)(pSource + nSourceOffset),
                        (IntPtr)nSize);
            }
        }

        public static unsafe void InterpetAsSamples<T>(byte[] arrData, int nBytes, T[] arrSamplesOut) where T : unmanaged {
            int nSamples = nBytes / sizeof(T);
            if (nSamples > arrSamplesOut.Length)
                throw new IndexOutOfRangeException($"Cannot fit {arrData.Length} bytes in {arrSamplesOut.Length} sample array");

            fixed (T* pSamplesOut = arrSamplesOut)
                Marshal.Copy(arrData, 0, (IntPtr)pSamplesOut, nSamples * sizeof(T));
        }

        public static unsafe byte[] GetBytes<T>(T[] arrOrig, int nOffset = 0, int nCount = -1) where T : unmanaged {
            if (nCount < 0)
                nCount = arrOrig.Length - nOffset;

            if (nOffset + nCount > arrOrig.Length)
                throw new IndexOutOfRangeException("Offset plus count exceeds array bounds");

            if (arrOrig.Length == 0)
                return new byte[0];

            byte[] arrData = new byte[nCount * sizeof(T)];
            fixed (T* pOrig = arrOrig)
                Marshal.Copy((IntPtr)(pOrig + nOffset), arrData, 0, nCount * sizeof(T));

            return arrData;
        }
    }
}
