using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp.Native {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void LlamaProgressCallback(float fProgress, IntPtr pUserData);
}
