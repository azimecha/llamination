using Azimecha.Llamination.Listening;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.WhisperCpp.Native {
    internal static class WhisperFunctions {
        public const string WHISPER_LIBRARY = "whisper";

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_init_from_file")]
        public static extern IntPtr WhisperInitFromFile([MarshalAs(UnmanagedType.LPStr)] string strPath);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_init_from_buffer")]
        public static extern IntPtr WhisperInitFromBuffer(IntPtr pBuffer, IntPtr nBufferSize);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_init")]
        public static extern IntPtr WhisperInit(in WhisperModelLoader loader);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_token_to_str")]
        public static extern IntPtr WhisperTokenToString(IntPtr pContext, int nTokenID);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full")]
        public static extern int WhisperFull(IntPtr pContext, WhisperFullParams_Managed par, [In] float[] arrSamples,
            int nSamples);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full")]
        public static extern int WhisperFull(IntPtr pContext, WhisperFullParams_Unmanaged par, [In] float[] arrSamples,
            int nSamples);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_default_params")]
        public static extern WhisperFullParams_Unmanaged WhisperFullDefaultParams(SamplingStrategy nStrat);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_n_segments")]
        public static extern int WhisperFullNumSegments(IntPtr pContext);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_lang_id")]
        public static extern int WhisperFullLangID(IntPtr pContext);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_get_segment_t0")]
        public static extern int WhisperFullGetSegmentStartTime(IntPtr pContext, int nSegment);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_get_segment_t1")]
        public static extern int WhisperFullGetSegmentEndTime(IntPtr pContext, int nSegment);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_get_segment_text")]
        public static extern IntPtr WhisperFullGetSegmentText(IntPtr pContext, int nSegment);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_n_tokens")]
        public static extern int WhisperFullNumTokens(IntPtr pContext, int nSegment);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_get_token_text")]
        public static extern IntPtr WhisperFullGetTokenText(IntPtr pContext, int nSegment, int nToken);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_get_token_id")]
        public static extern int WhisperFullGetTokenID(IntPtr pContext, int nSegment, int nToken);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_get_token_data")]
        public static extern WhisperTokenData WhisperFullGetTokenData(IntPtr pContext, int nSegment, int nToken);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_full_get_token_p")]
        public static extern float WhisperFullGetTokenProbability(IntPtr pContext, int nSegment, int nToken);

        [DllImport(WHISPER_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "whisper_free")]
        public static extern void WhisperFree(IntPtr pContext);
    }
}
