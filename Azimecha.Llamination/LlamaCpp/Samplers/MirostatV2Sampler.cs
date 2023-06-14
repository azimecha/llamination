using Azimecha.Llamination.LlamaCpp.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp.Samplers {
    public class MirostatV2Sampler : LlamaSampler {
        private float _fMu;

        public MirostatV2Sampler(LlamaModel mdl, float fTau = 5.0f, float fEta = 0.10f) : base(mdl) {
            Tau = fTau;
            Eta = fEta;
            _fMu = Tau * 2.0f;
        }

        public float Tau { get; private set; }
        public float Eta { get; private set; }
        public float Mu => _fMu;

        protected override unsafe int CallSamplerFunction(IntPtr pContext, TokenDataArray* ptda, int[] arrPrevTokens)
            => Functions.LlamaSampleTokenMirostatV2(pContext, (IntPtr)ptda, Tau, Eta, ref _fMu);
    }
}
