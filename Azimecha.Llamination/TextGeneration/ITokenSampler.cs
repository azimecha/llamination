using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.TextGeneration {
    public interface ITokenSampler {
        int Apply(SamplingCandidate[] arrCandidates, int[] arrPrevTokens);
    }

    public struct SamplingCandidate {
        public int TokenID;
        public float Logit;
        public float Probability;
    }
}
