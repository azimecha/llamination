using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaManagedState : IDisposable {
        private float[] _arrLogits;

        public LlamaManagedState(LlamaModel mdl) {
            _arrLogits = new float[mdl.VocabularySize];
            IntPtr pLogits = Native.Functions.LlamaGetLogits(mdl.Context.Value);
            System.Runtime.InteropServices.Marshal.Copy(pLogits, _arrLogits, 0, _arrLogits.Length);
        }

        internal unsafe void WriteToModel(LlamaModel mdl) {
            int nModelLogitCt = mdl.VocabularySize;

            int nCopyLen = _arrLogits.Length;
            if (nCopyLen > nModelLogitCt)
                nCopyLen = nModelLogitCt;

            IntPtr pLogits = Native.Functions.LlamaGetLogits(mdl.Context.Value);
            System.Runtime.InteropServices.Marshal.Copy(_arrLogits, 0, pLogits, nCopyLen);

            float* parrLogits = (float*)pLogits;
            for (int nToZero = nCopyLen; nToZero < nModelLogitCt; nToZero++)
                parrLogits[nToZero] = 0.0f;
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
