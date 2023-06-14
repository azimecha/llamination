using Azimecha.Llamination.LlamaCpp.Native;
using Azimecha.Llamination.TextGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp.Samplers {
    public class RepetitionPenalizer : LlamaSampler {
        public RepetitionPenalizer(LlamaModel mdl, float fPenalty) : base(mdl) {
            Penalty = fPenalty;
        }

        public float Penalty { get; private set; }

        protected override unsafe int CallSamplerFunction(IntPtr pContext, TokenDataArray* ptda, int[] arrPrevTokens) {
            Functions.LlamaSampleRepetitionPenalty(pContext, (IntPtr)ptda, arrPrevTokens, (IntPtr)arrPrevTokens.Length, Penalty);
            return 0;
        }
    }
}
