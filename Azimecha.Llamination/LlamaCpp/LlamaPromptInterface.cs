using Azimecha.Llamination.TextGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaPromptInterface : IPromptInterface {
        private LlamaModel _mdl;
        private List<int> _lstPrevTokens = new List<int>();
        private bool _bIsReadingResponse = false;
        private List<string> _lstSentenceTerminators = new List<string>() { ".", "?", "!" };

        public LlamaPromptInterface(LlamaModel mdl) {
            _mdl = mdl;
            MemorySize = 256;
            MaxRepeatCount = 3;
        }

        public int MemorySize { get; set; }
        public int MaxRepeatCount { get; set; }
        public ICollection<string> SentenceTerminators => _lstSentenceTerminators;

        public void ProvidePrompt(string strMessage, bool bReset = false) {
            if (_bIsReadingResponse)
                throw new InvalidOperationException("Cannot send message while reading response");

            int[] arrMessageTokens = _mdl.Tokenize(strMessage);

            int[] arrPromptTokensFull = new int[arrMessageTokens.Length + 1];
            arrPromptTokensFull[0] = Extras.BeginningOfStringToken;
            Extras.FastCopy(arrMessageTokens, 0, arrPromptTokensFull, 1, arrMessageTokens.Length);
            _mdl.Evaluate(arrPromptTokensFull, bReset ? 0 : _lstPrevTokens.Count);

            if (bReset)
                _lstPrevTokens.Clear();
            _lstPrevTokens.AddRange(arrPromptTokensFull);
        }

        private int LastToken => _lstPrevTokens[_lstPrevTokens.Count - 1];

        private bool DidExceedRepeatCount() {
            if (MaxRepeatCount <= 0)
                return false;

            if (_lstPrevTokens.Count <= MaxRepeatCount)
                return false;

            int nLastToken = LastToken;
            for (int nIndexToCheck = _lstPrevTokens.Count - 2; nIndexToCheck > _lstPrevTokens.Count - (MaxRepeatCount + 1); nIndexToCheck--)
                if (_lstPrevTokens[nIndexToCheck] != nLastToken)
                    return false;

            return true;
        }

        public void ReadResponseTokens(ResponseReceiver procReceiver) {
            int nToken = 0;

            do {
                while (_lstPrevTokens.Count > MemorySize)
                    _lstPrevTokens.RemoveAt(0);

                if (DidExceedRepeatCount())
                    _mdl.Logit(LastToken) = 0.0f;

                int[] arrPrevTokens = _lstPrevTokens.ToArray();

                Array.Reverse(arrPrevTokens);
                nToken = _mdl.Sample(arrPrevTokens);
                _mdl.Evaluate(new int[] { nToken }, _lstPrevTokens.Count);

                _lstPrevTokens.Add(nToken);
            } while (procReceiver(nToken));
        }

        public string ReadSentence(int nMaxTokensToRead = -1) {
            if (nMaxTokensToRead == 0)
                return string.Empty;

            StringBuilder sbSentence = new StringBuilder();
            bool bSentenceIsEmpty = true;

            ReadResponseTokens(nToken => {
                if (nToken == Extras.EndOfStringToken && !bSentenceIsEmpty)
                    return false;

                string strCurToken = _mdl.TokenToString(nToken);
                string strTrimmedToken = strCurToken.TrimEnd();

                if (!bSentenceIsEmpty) {
                    foreach (string strTerminator in _lstSentenceTerminators) {
                        if (strTrimmedToken.EndsWith(strTerminator)) {
                            sbSentence.Append(strCurToken);
                            return false;
                        }
                    }
                }

                if (strCurToken.EndsWith("\n")) {
                    if (bSentenceIsEmpty)
                        return true; // ignore & continue
                    else
                        return false; // stop
                }

                if (bSentenceIsEmpty && !string.IsNullOrEmpty(strTrimmedToken))
                    bSentenceIsEmpty = false;

                sbSentence.Append(strCurToken);
                System.Diagnostics.Debug.WriteLine($"{nToken} => {sbSentence}");

                if (nMaxTokensToRead > 0)
                    nMaxTokensToRead -= 1;
                return nMaxTokensToRead != 0; // continue
            });

            return sbSentence.ToString();
        }

        public void Dispose() {
            _mdl = null;
        }
    }
}
