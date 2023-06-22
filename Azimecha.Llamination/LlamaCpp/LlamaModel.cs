using Azimecha.Llamination.TextGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaModel : ITokenBasedLLM, ISelectableStateModel<LlamaState> {
        private Pointers.ContextPointer _ctx;
        private Preloader _pldModelFile;
        private ITokenSampler[] _arrDefaultSampleSteps;

        internal LlamaModel(Pointers.ContextPointer ctx, string strFileForPreload = null) {
            _ctx = ctx;

            Threads = Environment.ProcessorCount - 2;
            if (Threads < 1) Threads = 1;

            if (strFileForPreload != null) {
                _pldModelFile = new Preloader(strFileForPreload);
                _pldModelFile.Start();
            }

            _arrDefaultSampleSteps = new ITokenSampler[] {
                new Samplers.RepetitionPenalizer(this, 1.10f),
                new Samplers.MirostatV2Sampler(this)
            };
        }

        public int Threads { get; set; }

        internal Pointers.ContextPointer Context => _ctx;

        [Obsolete("Do not use. Call LlamaLibrary.GetInstance().LoadModel() instead. Otherwise, an access violation will occur if llama.cpp was built with CUDA.")]
        public static LlamaModel LoadFromFile(string strFileName)
            => LlamaLibrary.GetInstance(false).LoadModel(strFileName);

        public void WaitForPreload(WaitHandle whCancel = null, int nTimeout = -1) {
            if (_pldModelFile != null) {
                if (whCancel is null)
                    _pldModelFile.FinishedEvent.WaitOne(nTimeout);
                else
                    WaitHandle.WaitAny(new WaitHandle[] { whCancel, _pldModelFile.FinishedEvent }, nTimeout);
            }
        }

        public int[] Tokenize(string strText) {
            byte[] arrTextUTF8 = InteropUtils.ToNarrowString(strText);

            int nTokens = -Native.Functions.LlamaTokenize(_ctx.Value, arrTextUTF8, null, 0, false);
            int[] arrTokens = new int[nTokens];

            int nResult = Native.Functions.LlamaTokenize(_ctx.Value, arrTextUTF8, arrTokens, nTokens, false);
            if (nResult < 0)
                throw new TokenizationException(strText, nResult);

            return arrTokens;
        }


        public int Sample(int[] arrPrevTokens) 
            => Sample(arrPrevTokens, (IEnumerable<ITokenSampler>)_arrDefaultSampleSteps);

        public int Sample(int[] arrPrevTokens, params ITokenSampler[] arrSamplers)
            => Sample(arrPrevTokens, (IEnumerable<ITokenSampler>)arrSamplers);

        public unsafe int Sample(int[] arrPrevTokens, IEnumerable<ITokenSampler> enuSamplers) {
            SamplingCandidate[] arrCandidates = new SamplingCandidate[VocabularySize];
            float* pLogits = GetLogitsPointer();

            for (int nToken = 0; nToken < arrCandidates.Length; nToken++) {
                arrCandidates[nToken].TokenID = nToken;
                arrCandidates[nToken].Logit = pLogits[nToken];
            }

            int nResult = -1;
            foreach (ITokenSampler samp in enuSamplers)
                nResult = samp.Apply(arrCandidates, arrPrevTokens);

            return nResult;
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

        public int VocabularySize {
            get {
                int nValue = Native.Functions.LlamaGetNVocab(_ctx.Value);
                if (nValue < 0)
                    throw new LlamaException($"llama_n_vocab returned {nValue}");
                return nValue;
            }
        }

        public unsafe ref float Logit(int nToken) {
            if (nToken < 0)
                throw new ArgumentOutOfRangeException(nameof(nToken));

            int nVocabSize = VocabularySize;
            if (nToken >= nVocabSize)
                throw new IndexOutOfRangeException($"Token index {nToken} is invalid for model with vocabulary size {nVocabSize}");
            
            return ref *(GetLogitsPointer() + nToken);
        }

        private unsafe float* GetLogitsPointer()
            => (float*)Native.Functions.LlamaGetLogits(_ctx.Value);

        internal unsafe IntPtr LogitBuffer
            => (IntPtr)GetLogitsPointer();

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

        public LlamaState GetCurrentState()
            => new LlamaState(this);

        public void SetState(LlamaState state)
            => state.WriteToModel(this);

        public int ContextSize 
            => Native.Functions.LlamaGetNCtx(_ctx.Value);

        public int BeginningOfStringToken => Native.Functions.LlamaTokenBOS();
        public int EndOfStringToken => Native.Functions.LlamaTokenEOS();

        private unsafe float* GetEmbeddingsPointer()
            => (float*)Native.Functions.LlamaGetEmbeddings(_ctx.Value);
        internal unsafe IntPtr EmbeddingBuffer
            => (IntPtr)GetEmbeddingsPointer();

        internal int EmbeddingBufferSize
            => Native.Functions.LlamaGetNEmbed(_ctx.Value);

        public LlamaPromptInterface CreatePromptInterface()
            => new LlamaPromptInterface(this);

        IPromptInterface ILanguageModel.CreatePromptInterface()
            => new LlamaPromptInterface(this);
    }
}
