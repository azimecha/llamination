using Azimecha.Llamination.TextGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaPromptInterface : IPromptInterface {
        private LlamaModel _mdl;
        private List<List<int>> _lstPrevTokens = new List<List<int>>();
        private List<string> _lstSentenceTerminators = new List<string>() { ".", "?", "!" };
        private List<ITokenSampler> _lstSamplers = new List<ITokenSampler>();
        private LlamaManagedState _stateOriginal;

        public LlamaPromptInterface(LlamaModel mdl) {
            _mdl = mdl;

            _lstSamplers.Add(new Samplers.RepetitionPenalizer(mdl, 1.10f));
            _lstSamplers.Add(new Samplers.MirostatV2Sampler(mdl));

            _stateOriginal = mdl.GetCurrentState();
            _lstPrevTokens.Add(new List<int>());
        }

        public ICollection<string> SentenceTerminators => _lstSentenceTerminators;
        public IList<ITokenSampler> Samplers => _lstSamplers;

        public void ResetState() {
            _mdl.SetState(_stateOriginal);
            _lstPrevTokens.Clear();
        }

        public void ProvidePrompt(string strMessage) {
            // tokenize prompt
            int[] arrMessageTokens = _mdl.Tokenize(strMessage);

            // add BOS token
            int[] arrPromptTokensFull = new int[arrMessageTokens.Length + 1];
            arrPromptTokensFull[0] = _mdl.BeginningOfStringToken;
            Extras.FastCopy(arrMessageTokens, 0, arrPromptTokensFull, 1, arrMessageTokens.Length);

            // get trimmed prev token count
            TrimPrevTokens(arrPromptTokensFull.Length);
            int nPrevTokens = GetPrevTokensCount();

            // evaluate prompt
            _mdl.Evaluate(arrPromptTokensFull, nPrevTokens);

            // terminate existing prev token sequence
            InsertMemoryMark();

            // add new tokens to prev tokens list
            foreach (int nPromptToken in arrPromptTokensFull) {
                InsertIntoMemory(nPromptToken);

                // check if the token terminates a sentence - if so, terminate current prev token sequence
                string strToken = _mdl.TokenToString(nPromptToken).Trim();
                foreach (string strTerminator in SentenceTerminators) {
                    if (strToken.EndsWith(strTerminator)) {
                        InsertMemoryMark();
                        break;
                    }
                }
            }
        }

        private int GetPrevTokensCount() {
            int nTotal = 0;

            foreach (List<int> lst in _lstPrevTokens)
                nTotal += lst.Count;

            return nTotal;
        }

        private void TrimPrevTokens(int nExtraSpace = 1) {
            bool bDidRemoveAny = false;

            while (GetPrevTokensCount() > (_mdl.ContextSize - nExtraSpace)) {
                _lstPrevTokens.RemoveAt(_lstPrevTokens.Count - 1);
                bDidRemoveAny = true;
            }

            if (bDidRemoveAny) {
                if (_lstPrevTokens.Count < 1)
                    _lstPrevTokens.Add(new List<int>() { _mdl.BeginningOfStringToken });
                else {
                    List<int> lstLast = _lstPrevTokens[_lstPrevTokens.Count - 1];
                    if (lstLast.Count == 0 || lstLast[lstLast.Count - 1] != _mdl.BeginningOfStringToken)
                        lstLast.Add(_mdl.BeginningOfStringToken);
                }

                int[] arrTokens = GetPrevTokens();
                Array.Reverse(arrTokens);

                _mdl.SetState(_stateOriginal);
                _mdl.Evaluate(arrTokens);
            }
        }

        private int[] GetPrevTokens(int[] arrPrepend = null) {
            int nPrependLength = arrPrepend?.Length ?? 0;
            int[] arrTokens = new int[nPrependLength + GetPrevTokensCount()];
            int nIndex = 0;

            for (nIndex = 0; nIndex < nPrependLength; nIndex++)
                arrTokens[nIndex] = arrPrepend[nIndex];

            foreach (List<int> lst in _lstPrevTokens) {
                foreach (int nToken in lst) {
                    arrTokens[nIndex] = nToken;
                    nIndex++;
                }
            }

            return arrTokens;
        }

        private void InsertIntoMemory(int nToken) {
            if (_lstPrevTokens.Count == 0)
                InsertMemoryMark();
            _lstPrevTokens[0].Insert(0, nToken);
        }

        public void InsertMemoryMark() {
            _lstPrevTokens.Insert(0, new List<int>());
        }

        public int ReadToken() {
            // retrieve prev tokens
            TrimPrevTokens();
            int[] arrPrevTokens = GetPrevTokens();

            // sample new token and feed it back into the model
            int nToken = _mdl.Sample(arrPrevTokens, _lstSamplers);
            _mdl.Evaluate(new int[] { nToken }, arrPrevTokens.Length);

            // add new token to prev tokens list
            InsertIntoMemory(nToken);

            return nToken;
        }

        public string ReadSentence(int nApproxSizeLimit = int.MaxValue) {
            if (nApproxSizeLimit == 0)
                return string.Empty;

            StringBuilder sbSentence = new StringBuilder();
            bool bSentenceIsEmpty = true;

            while (sbSentence.Length < nApproxSizeLimit) {
                // read one token
                int nToken = ReadToken();

                // check if the special EOS token was read
                if (nToken == _mdl.EndOfStringToken && !bSentenceIsEmpty)
                    break;

                // convert token to string and trim whitespace
                string strCurToken = _mdl.TokenToString(nToken);
                string strTrimmedToken = strCurToken.TrimEnd();

                // check if any terminator exists at the end of the token
                if (!bSentenceIsEmpty) {
                    foreach (string strTerminator in _lstSentenceTerminators) {
                        if (strTrimmedToken.EndsWith(strTerminator)) {
                            sbSentence.Append(strCurToken);
                            goto L_breakbreak;
                        }
                    }
                }

                // check for newlines (handled specially)
                if (strCurToken.EndsWith("\n")) {
                    sbSentence.Append(strCurToken.TrimEnd('\r', '\n') + " ");

                    if (bSentenceIsEmpty)
                        continue;
                    else
                        break;
                }

                // update sentence-is-empty flag
                if (bSentenceIsEmpty && !string.IsNullOrEmpty(strTrimmedToken))
                    bSentenceIsEmpty = false;

                // add token to sentence
                sbSentence.Append(strCurToken);
                System.Diagnostics.Debug.WriteLine($"{nToken} => {sbSentence}");
            }

            // after we read the sentence, terminate the current prev token sequence
            L_breakbreak:
            InsertMemoryMark();
            return sbSentence.ToString();
        }

        public void Dispose() {
            _mdl = null;
        }
    }
}
