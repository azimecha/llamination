using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp.Native {
    internal static class Functions {
        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_free")]
        public static extern void LlamaFree(IntPtr pContext);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_context_default_params")]
        public static extern ContextParams LlamaContextDefaultParams();

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_init_from_file")]
        public static extern IntPtr LlamaInitFromFile([In] byte[] arrPathUTF8, ContextParams par);

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

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_sample_top_p_top_k")]
        public static extern int LlamaSampleTopPTopK(IntPtr pContext, [In] int[] arrTokens, int nTokens, int nTopK,
            float fTopP, float fRepeatPenalty);

        [DllImport(Constants.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_get_logits")]
        public static extern IntPtr LlamaGetLogits(IntPtr pContext);

        [DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcpy")]
        public static extern IntPtr MemCpy(IntPtr pDest, IntPtr pSource, IntPtr nSize);
    }
}
