using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.DeepSpeech.Native {
    internal static class Functions {
        public const string LIBRARY_NAME = "deepspeech";

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DS_CreateModel(byte[] arrModelPathUTF8, out IntPtr pModelState);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DS_FreeModel(IntPtr pModelState);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DS_GetModelSampleRate(IntPtr pModelState);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DS_SpeechToText(IntPtr pModelState, [In] short[] arrSamples, uint nSamples);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DS_SpeechToTextWithMetadata(IntPtr pModelState, [In] short[] arrSamples, uint nSamples, uint nMaxResultCt);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DS_FreeMetadata(IntPtr pMetadata);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DS_FreeString(IntPtr pString);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DS_Version();
    }
}
