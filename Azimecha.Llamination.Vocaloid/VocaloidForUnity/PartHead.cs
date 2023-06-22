using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PartHead {
        public int PartHandle;
        public int TickPos;
        public int PlayTime;
        public fixed byte PartName[257];
        public fixed byte PartComment[257];
        public int Plane;
        public int UserData;
    }
}
