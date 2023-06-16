using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Pointers {
    public class HGlobalPointer : AutoPointer {
        protected override void Free(IntPtr pObject) {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(pObject);
        }
    }
}
