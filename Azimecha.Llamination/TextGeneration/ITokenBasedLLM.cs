using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.TextGeneration {
    public interface ITokenBasedLLM : ILanguageModel {
        int[] Tokenize(string strText);

        void Evaluate(int[] arrTokens, int nOldTokensToUse = 0);

        string TokenToString(int nToken);

        int Sample(int[] arrPrevTokens);
        int Sample(int[] arrPrevTokens, params ITokenSampler[] arrSamplers);
        int Sample(int[] arrPrevTokens, IEnumerable<ITokenSampler> enuSamplers);

        IEnumerable<ITokenSampler> DefaultSamplers { get; }

        int VocabularySize { get; }
        float GetLogit(int nToken);
        void SetLogit(int nToken, float fValue);

        int ContextSize { get; }

        int BeginningOfStringToken { get; }
        int EndOfStringToken { get; }
    }
}
