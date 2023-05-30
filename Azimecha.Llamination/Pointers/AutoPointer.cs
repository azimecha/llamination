using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.Pointers {
    public abstract class AutoPointer : IDisposable {
        private IntPtr _pObject;

        public IntPtr Value {
            get {
                IntPtr pObject = _pObject;
                if (pObject == IntPtr.Zero)
                    throw new InvalidOperationException("Pointer not initialized");
                return pObject;
            }
            set {
                if (Interlocked.CompareExchange(ref _pObject, value, IntPtr.Zero) != IntPtr.Zero)
                    throw new InvalidOperationException("Pointer already initialized");
            }
        }

        public bool Initialized => _pObject != IntPtr.Zero;

        protected abstract void Free(IntPtr pObject);

        private void DeletePointer() {
            try { } finally { // protection against asynchronous thread abort
                IntPtr pObject = Interlocked.Exchange(ref _pObject, IntPtr.Zero);
                if (pObject != IntPtr.Zero)
                    Free(pObject);
            }
        }

        public void Dispose() => DeletePointer();
        ~AutoPointer() => DeletePointer();

        public void Abandon() {
            _pObject = IntPtr.Zero;
        }
    }
}
