using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp.Native {
    internal static class Functions {
        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_init_backend")]
        public static extern void LlamaInitBackend();

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_free")]
        public static extern void LlamaFree(IntPtr pContext);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_context_default_params")]
        public static extern ContextParams<GenericTensorSplit> LlamaContextDefaultParams_Generic();

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_context_default_params")]
        public static extern ContextParams<CudaTensorSplit> LlamaContextDefaultParams_CUDA();

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_init_from_file")]
        public static extern IntPtr LlamaInitFromFile([In] byte[] arrPathUTF8, ContextParams<GenericTensorSplit> par);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_init_from_file")]
        public static extern IntPtr LlamaInitFromFile([In] byte[] arrPathUTF8, ContextParams<CudaTensorSplit> par);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_tokenize")]
        public static extern int LlamaTokenize(IntPtr pContext, [In] byte[] arrTextUTF8, [Out] int[] arrTokens, int nMaxTokens,
            bool bAddBOSToken);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_eval")]
        public static extern int LlamaEval(IntPtr pContext, [In] int[] arrTokens, int nTokens, int nPastTokens, int nThreads);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_token_to_str")]
        public static extern IntPtr LlamaTokenToStr(IntPtr pContext, int nToken);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_token_bos")]
        public static extern int LlamaTokenBOS();

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_token_eos")]
        public static extern int LlamaTokenEOS();

        /*[DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_sample_top_p_top_k")]
        public static extern int LlamaSampleTopPTopK(IntPtr pContext, [In] int[] arrTokens, int nTokens, int nTopK,
            float fTopP, float fRepeatPenalty);*/

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_sample_repetition_penalty")]
        public static extern void LlamaSampleRepetitionPenalty(IntPtr pContext, IntPtr pCandidateArray, [In] int[] arrPrevTokens,
            IntPtr nPrevTokens, float fPenalty);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_sample_token_mirostat_v2")]
        public static extern int LlamaSampleTokenMirostatV2(IntPtr pContext, IntPtr pCandidateArray, float fTau, float fEta, ref float fMu);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_n_vocab")]
        public static extern int LlamaGetNVocab(IntPtr pContext);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_get_logits")]
        public static extern IntPtr LlamaGetLogits(IntPtr pContext);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_get_state_size")]
        public static extern IntPtr LlamaGetStateSize(IntPtr pContext);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_copy_state_data")]
        public static extern IntPtr LlamaCopyStateData(IntPtr pContext, IntPtr pDest);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_set_state_data")]
        public static extern IntPtr LlamaSetStateData(IntPtr pContext, IntPtr pSource);

        [DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcpy")]
        public static extern IntPtr MemCpy(IntPtr pDest, IntPtr pSource, IntPtr nSize);
    }
}
