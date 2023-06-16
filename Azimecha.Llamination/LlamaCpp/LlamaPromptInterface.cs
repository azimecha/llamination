using Azimecha.Llamination.TextGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaPromptInterface : IPromptInterface {
        private LlamaModel _mdl;
        private List<int> _lstPrevTokens = new List<int>();
        private List<string> _lstSentenceTerminators = new List<string>() { ".", "?", "!" };
        private List<ITokenSampler> _lstSamplers = new List<ITokenSampler>();
        private LlamaState _stateOriginal;

        public LlamaPromptInterface(LlamaModel mdl) {
            _mdl = mdl;
            MemorySize = 256;

            _lstSamplers.Add(new Samplers.RepetitionPenalizer(mdl, 1.10f));
            _lstSamplers.Add(new Samplers.MirostatV2Sampler(mdl));

            _stateOriginal = mdl.GetCurrentState();
        }

        public int MemorySize { get; set; }
        public ICollection<string> SentenceTerminators => _lstSentenceTerminators;
        public IList<ITokenSampler> Samplers => _lstSamplers;

        public void ResetState() {
            _mdl.SetState(_stateOriginal);
            _lstPrevTokens.Clear();
        }

        public void ProvidePrompt(string strMessage) {
            int[] arrMessageTokens = _mdl.Tokenize(strMessage);

            int[] arrPromptTokensFull = new int[arrMessageTokens.Length + 1];
            arrPromptTokensFull[0] = Extras.BeginningOfStringToken;
            Extras.FastCopy(arrMessageTokens, 0, arrPromptTokensFull, 1, arrMessageTokens.Length);

            _mdl.Evaluate(arrPromptTokensFull, _lstPrevTokens.Count);

            foreach (int nPromptToken in arrPromptTokensFull)
                _lstPrevTokens.Insert(0, nPromptToken);
        }

        public int ReadToken() {
            while (_lstPrevTokens.Count > MemorySize)
                _lstPrevTokens.RemoveAt(0);

            int nToken = _mdl.Sample(_lstPrevTokens.ToArray(), _lstSamplers);
            _mdl.Evaluate(new int[] { nToken }, _lstPrevTokens.Count);

            _lstPrevTokens.Insert(0, nToken);

            return nToken;
        }

        public string ReadSentence(int nApproxSizeLimit = int.MaxValue) {
            if (nApproxSizeLimit == 0)
                return string.Empty;

            StringBuilder sbSentence = new StringBuilder();
            bool bSentenceIsEmpty = true;

            while (sbSentence.Length < nApproxSizeLimit) {
                int nToken = ReadToken();

                if (nToken == Extras.EndOfStringToken && !bSentenceIsEmpty)
                    break;

                string strCurToken = _mdl.TokenToString(nToken);
                string strTrimmedToken = strCurToken.TrimEnd();

                if (!bSentenceIsEmpty) {
                    foreach (string strTerminator in _lstSentenceTerminators) {
                        if (strTrimmedToken.EndsWith(strTerminator)) {
                            sbSentence.Append(strCurToken);
                            goto L_breakbreak;
                        }
                    }
                }

                if (strCurToken.EndsWith("\n")) {
                    sbSentence.Append(strCurToken.TrimEnd('\r', '\n'));

                    if (bSentenceIsEmpty)
                        continue;
                    else
                        break;
                }

                if (bSentenceIsEmpty && !string.IsNullOrEmpty(strTrimmedToken))
                    bSentenceIsEmpty = false;

                sbSentence.Append(strCurToken);
                System.Diagnostics.Debug.WriteLine($"{nToken} => {sbSentence}");
            }

            L_breakbreak:
            return sbSentence.ToString();
        }

        public void Dispose() {
            _mdl = null;
        }
    }
}
