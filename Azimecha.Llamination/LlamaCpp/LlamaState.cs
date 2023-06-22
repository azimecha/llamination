using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaState : IDisposable {
        private float[] _arrLogits;
        private float[] _arrEmbeddings;

        public LlamaState(LlamaModel mdl) {
            _arrLogits = ReadData(mdl.LogitBuffer, mdl.VocabularySize);
            _arrEmbeddings = ReadData(mdl.EmbeddingBuffer, mdl.EmbeddingBufferSize);
        }

        internal void WriteToModel(LlamaModel mdl) {
            WriteData(_arrLogits, mdl.LogitBuffer, mdl.VocabularySize);
            WriteData(_arrEmbeddings, mdl.EmbeddingBuffer, mdl.EmbeddingBufferSize);
        }

        private static float[] ReadData(IntPtr pData, int nCount) {
            if ((pData == IntPtr.Zero) || (nCount == 0))
                return new float[0];

            float[] arrCopy = new float[nCount];
            System.Runtime.InteropServices.Marshal.Copy(pData, arrCopy, 0, nCount);
            return arrCopy;
        }

        private static unsafe void WriteData(float[] arrSource, IntPtr pDest, int nDestSize) {
            if ((pDest == IntPtr.Zero) || (nDestSize == 0) || (arrSource is null))
                return;

            int nCopyCt = arrSource.Length;
            if (nCopyCt > nDestSize)
                nCopyCt = nDestSize;

            System.Runtime.InteropServices.Marshal.Copy(arrSource, 0, pDest, nCopyCt);

            float* parrDest = (float*)pDest;
            for (int nToZero = nCopyCt; nToZero < nDestSize; nToZero++)
                parrDest[nToZero] = 0.0f;
        }

        public void Dispose() {
            _arrLogits = null;
            _arrEmbeddings = null;
        }
    }
}
