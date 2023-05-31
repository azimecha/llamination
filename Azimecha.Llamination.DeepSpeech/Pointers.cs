using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.DeepSpeech {
    internal class ModelStatePointer : Pointers.AutoPointer {
        protected override void Free(IntPtr pObject) {
            Native.Functions.DS_FreeModel(pObject);
        }
    }

    internal class StringPointer : Pointers.AutoPointer {
        protected override void Free(IntPtr pObject) {
            Native.Functions.DS_FreeString(pObject);
        }
    }

    internal class MetadataPointer : Pointers.TypedAutoPointer<Native.Metadata> {
        protected override void Free(IntPtr pObject) {
            Native.Functions.DS_FreeMetadata(pObject);
        }
    }
}
