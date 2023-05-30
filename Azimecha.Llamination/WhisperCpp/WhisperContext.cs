using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.WhisperCpp {
    internal class WhisperContext : Pointers.AutoPointer {
        protected override void Free(IntPtr pObject) {
            Native.WhisperFunctions.WhisperFree(pObject);
        }
    }
}
