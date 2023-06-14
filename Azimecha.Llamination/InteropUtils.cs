using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination {
    public static class InteropUtils {
        public static string ReadCString(IntPtr pUTF8String, Encoding enc)
            => enc.GetString(ReadUnmanagedArray<byte>(pUTF8String, GetNarrowStringLength(pUTF8String)));

        public static unsafe T[] ReadUnmanagedArray<T>(IntPtr pBase, int nLength) where T : unmanaged {
            T* parrUnmanaged = (T*)pBase;
            T[] arrManaged = new T[nLength];

            nLength--;
            while (nLength >= 0) {
                arrManaged[nLength] = parrUnmanaged[nLength];
                nLength--;
            }

            return arrManaged;
        }

        public static unsafe int GetNarrowStringLength(IntPtr pNarrowString) {
            int nLength = 0;
            for (byte* pszInput = (byte*)pNarrowString; *pszInput != 0; pszInput++)
                nLength++;
            return nLength;
        }

        // can be used as empty ascii, utf8, utf16, utf32, ucs2, ucs4... pointer
        public static readonly IntPtr EmptyStringPointer = AllocEmptyString();

        private static unsafe IntPtr AllocEmptyString() {
            IntPtr pEmptyString = Marshal.AllocHGlobal(4);
            *(int*)pEmptyString = 0;
            return pEmptyString;
        }

        public static readonly Encoding UTF8NoBOM = new UTF8Encoding(false, false);

        public static byte[] ToNarrowString(string str, Encoding enc = null) {
            if (enc is null)
                enc = UTF8NoBOM;

            byte[] arrData = new byte[enc.GetByteCount(str) + 1];
            enc.GetBytes(str, 0, str.Length, arrData, 0);
            return arrData;
        }
    }
}
