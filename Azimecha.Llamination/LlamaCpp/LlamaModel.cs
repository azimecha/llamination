using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaModel : TextGeneration.ITokenBasedLLM {
        private Pointers.ContextPointer _ctx;
        private Preloader _pldModelFile;

        internal LlamaModel(Pointers.ContextPointer ctx, string strFileForPreload = null) {
            _ctx = ctx;

            Threads = Environment.ProcessorCount - 2;
            if (Threads < 1) Threads = 1;

            TokenConsiderationCount = 40;
            MinimumTokenProbability = 0.95f;
            ParamTemp = 0.80f;
            RepeatPenalty = 1.10f;

            if (strFileForPreload != null) {
                _pldModelFile = new Preloader(strFileForPreload);
                _pldModelFile.Start();
            }
        }

        public int Threads { get; set; }

        public int TokenConsiderationCount { get; set; }
        public float MinimumTokenProbability { get; set; }
        public float ParamTemp { get; set; }
        public float RepeatPenalty { get; set; }

        public static LlamaModel LoadFromFile(string strFileName) {
            Native.ContextParams par = Native.Functions.LlamaContextDefaultParams();

            Pointers.ContextPointer ctx = new Pointers.ContextPointer();
            ctx.Value = Native.Functions.LlamaInitFromFile(Encoding.UTF8.GetBytes(strFileName), par);
            if (!ctx.Initialized)
                throw new ModelLoadException(strFileName);

            return new LlamaModel(ctx, strFileName);
        }

        public void WaitForPreload(WaitHandle whCancel = null, int nTimeout = -1) {
            if (_pldModelFile != null) {
                if (whCancel is null)
                    _pldModelFile.FinishedEvent.WaitOne(nTimeout);
                else
                    WaitHandle.WaitAny(new WaitHandle[] { whCancel, _pldModelFile.FinishedEvent }, nTimeout);
            }
        }

        public int[] Tokenize(string strText) {
            byte[] arrTextUTF8 = Encoding.UTF8.GetBytes(strText);

            int nTokens = -Native.Functions.LlamaTokenize(_ctx.Value, arrTextUTF8, null, 0, false);
            int[] arrTokens = new int[nTokens];

            int nResult = Native.Functions.LlamaTokenize(_ctx.Value, arrTextUTF8, arrTokens, nTokens, false);
            if (nResult < 0)
                throw new TokenizationException(strText, nResult);

            return arrTokens;
        }

        // for some reason llama uses limited "scratch space" instead of malloc
        // if the scratch space fills, it will know but access invalid memory anyway (very "good" design)
        // 1024 tokens appears to be OK, but over ~1500 will cause a problem
        private const int MAX_TOKEN_COUNT = 1024;

        public void Evaluate(int[] arrTokens, int nOldTokensToUse = 0) {
            if (arrTokens.Length > MAX_TOKEN_COUNT)
                throw new OversizedInputException($"Token count {arrTokens.Length} exceeds maximum of {MAX_TOKEN_COUNT}");

            int nResult = Native.Functions.LlamaEval(_ctx.Value, arrTokens, arrTokens.Length, nOldTokensToUse, Threads);
            if (nResult < 0)
                throw new EvaluationException(nResult, arrTokens.Length);
        }

        public void Evaluate(string strInput, int nOldTokensToUse = 0) {
            int[] arrTokens = Tokenize(strInput);

            try {
                Evaluate(arrTokens, nOldTokensToUse);
            } catch (EvaluationException exEval) {
                throw new EvaluationException(strInput, exEval);
            }
        }

        public string TokenToString(int nToken) {
            IntPtr pszTokenUTF8 = Native.Functions.LlamaTokenToStr(_ctx.Value, nToken);
            if (pszTokenUTF8 == IntPtr.Zero)
                throw new InvalidTokenException(nToken);

            return InteropUtils.ReadCString(pszTokenUTF8, Encoding.UTF8);
        }

        public int Sample(int[] arrPrevTokensToAvoid)
            => Native.Functions.LlamaSampleTopPTopK(_ctx.Value, arrPrevTokensToAvoid, arrPrevTokensToAvoid.Length,
                TokenConsiderationCount, MinimumTokenProbability, RepeatPenalty);

        public unsafe ref float Logit(int nToken) {
            // FIXME - this is bad, no checking of argument or ptr validity
            return ref *((float*)Native.Functions.LlamaGetLogits(_ctx.Value) + nToken);
        }

        public void Dispose() {
            Pointers.ContextPointer ctx = _ctx;
            _ctx = null;
            ctx?.Dispose();

            Preloader pldModelFile = _pldModelFile;
            _pldModelFile = null;
            pldModelFile?.Dispose();
        }

        public float GetLogit(int nToken)
            => Logit(nToken);

        public void SetLogit(int nToken, float fValue)
            => Logit(nToken) = fValue;
    }
}
