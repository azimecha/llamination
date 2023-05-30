using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.SAPI {
    internal class AutoHGlobal : Pointers.AutoPointer {
        // beware: Marshal.AllocHGlobal & Marshal.FreeHGlobal create HLOCALs, not HGLOBALs!

        public static AutoHGlobal Allocate(int nSize) {
            AutoHGlobal ptr = new AutoHGlobal();
            ptr.Value = GlobalAlloc(0, (IntPtr)nSize);
            return ptr;
        }

        protected override void Free(IntPtr pObject) {
            GlobalFree(pObject);
        }

        [DllImport("kernel32")]
        private static extern IntPtr GlobalAlloc(uint flags, IntPtr nBytes);

        [DllImport("kernel32")]
        private static extern IntPtr GlobalFree(IntPtr hGlobal);
    }
}
