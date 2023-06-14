using Azimecha.Llamination.TextGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp.Samplers {
    public abstract class LlamaSampler : ITokenSampler {
        private LlamaModel _mdl;

        public LlamaSampler(LlamaModel mdl) {
            _mdl = mdl;
        }

        public unsafe int Apply(SamplingCandidate[] arrCandidates, int[] arrPrevTokens) {
            fixed (SamplingCandidate* pCandidates = arrCandidates) {
                Native.TokenDataArray tda = new Native.TokenDataArray() {
                    Data = pCandidates,
                    Size = (IntPtr)arrCandidates.Length
                };

                int nResult = CallSamplerFunction(_mdl.Context.Value, &tda, arrPrevTokens);
                GC.KeepAlive(_mdl.Context);

                return nResult;
            }
        }

        protected abstract unsafe int CallSamplerFunction(IntPtr pContext, Native.TokenDataArray* ptda, int[] arrPrevTokens);
    }
}
