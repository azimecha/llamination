using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.WhisperCpp.Native {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WhisperNewSegmentCallback(IntPtr pContext, IntPtr pState, int nSegment, IntPtr pUserData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WhisperProgressCallback(IntPtr pContext, IntPtr pState, int nProgress, IntPtr pUserData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate bool WhisperEncoderBeginCallback(IntPtr pContext, IntPtr pState, IntPtr pUserData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WhisperLogitsFilterCallback(IntPtr pContext, IntPtr pState, IntPtr pTokenData, int nTokens,
        IntPtr pLogits, IntPtr pUserData);
}
