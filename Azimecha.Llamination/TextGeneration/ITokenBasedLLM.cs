using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.TextGeneration {
    public interface ITokenBasedLLM : ILanguageModel {
        int[] Tokenize(string strText);

        void Evaluate(int[] arrTokens, int nOldTokensToUse = 0);

        string TokenToString(int nToken);

        int Sample(int[] arrPrevTokensToAvoid);

        float GetLogit(int nToken);
        void SetLogit(int nToken, float fValue);
    }
}
