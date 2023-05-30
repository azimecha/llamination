using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Pointers {
    public unsafe abstract class TypedAutoPointer<T> : AutoPointer where T : unmanaged {
        public ref T Target => ref *(T*)Value;
    }
}
