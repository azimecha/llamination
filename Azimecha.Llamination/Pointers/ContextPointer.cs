using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Pointers {
    internal class ContextPointer : AutoPointer {
        protected override void Free(IntPtr pObject) {
            LlamaCpp.Native.Functions.LlamaFree(pObject);
        }
    }
}
