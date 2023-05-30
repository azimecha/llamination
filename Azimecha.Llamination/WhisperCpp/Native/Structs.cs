using Azimecha.Llamination.Listening;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.WhisperCpp.Native {
    [StructLayout(LayoutKind.Sequential)]
    internal struct WhisperFullParams_Managed {
        public SamplingStrategy Strategy;

        public int ThreadCount;
        public int MaxContextTokens;
        public int StartOffsetMillis;
        public int AudioDurationMillis;

        [MarshalAs(UnmanagedType.I1)] public bool Translate;
        [MarshalAs(UnmanagedType.I1)] public bool UseNoContext;
        [MarshalAs(UnmanagedType.I1)] public bool ForceSingleSegment;
        [MarshalAs(UnmanagedType.I1)] public bool PrintSpecialTokens;
        [MarshalAs(UnmanagedType.I1)] public bool PrintProgressInfo;
        [MarshalAs(UnmanagedType.I1)] public bool PrintRealtimeResults;
        [MarshalAs(UnmanagedType.I1)] public bool PrintTimestamps;

        [MarshalAs(UnmanagedType.I1)] public bool EnableTokenTimestamps;
        public float TimestampProbabilityThreshold;
        public float TimestampSumProbabilityThreshold;
        public int MaxSegmentLengthChars;
        [MarshalAs(UnmanagedType.I1)] public bool SplitSegmentsOnWords;
        public int MaxTokensPerSegment;

        [MarshalAs(UnmanagedType.I1)] public bool SpeedUpAudio;
        public int AudioContextSizeLimit;

        [MarshalAs(UnmanagedType.LPArray)] public byte[] InitialPrompt_UTF8;
        [MarshalAs(UnmanagedType.LPArray)] public int[] PromptTokens;
        int PromptTokenCount;

        [MarshalAs(UnmanagedType.LPStr)] public string Language;

        [MarshalAs(UnmanagedType.I1)] public bool SuppressBlankTokens;
        [MarshalAs(UnmanagedType.I1)] public bool SuppressNonSpeechTokens;

        public float InitialTemperature;
        public float MaxInitialTimestamp;
        public float LengthPenalty;

        public float FallbackTemperatureIncrement;
        public float FallbackEntropyThreshold;
        public float FallbackLogProbabilityThreshold;
        public float NoSpeechThreshold;

        public int GreedyBestOf;
        public int BeamSearchBeamSize;
        public float BeamSearchPatience;

        [MarshalAs(UnmanagedType.FunctionPtr)] public WhisperNewSegmentCallback NewSegmentCallback;
        public IntPtr NewSegmentCallbackData;

        [MarshalAs(UnmanagedType.FunctionPtr)] public WhisperProgressCallback ProgressCallback;
        public IntPtr ProgressCallbackData;

        [MarshalAs(UnmanagedType.FunctionPtr)] public WhisperEncoderBeginCallback EncoderBeginCallback;
        public IntPtr EncoderBeginCallbackData;

        [MarshalAs(UnmanagedType.FunctionPtr)] public WhisperLogitsFilterCallback LogitsFilterCallback;
        public IntPtr LogitsFilterCallbackData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WhisperFullParams_Unmanaged {
        public SamplingStrategy Strategy;

        public int ThreadCount;
        public int MaxContextTokens;
        public int StartOffsetMillis;
        public int AudioDurationMillis;

        public byte Translate;
        public byte UseNoContext;
        public byte ForceSingleSegment;
        public byte PrintSpecialTokens;
        public byte PrintProgressInfo;
        public byte PrintRealtimeResults;
        public byte PrintTimestamps;

        public byte EnableTokenTimestamps;
        public float TimestampProbabilityThreshold;
        public float TimestampSumProbabilityThreshold;
        public int MaxSegmentLengthChars;
        public byte SplitSegmentsOnWords;
        public int MaxTokensPerSegment;

        public byte SpeedUpAudio;
        public int AudioContextSizeLimit;

        public IntPtr InitialPrompt_UTF8;
        public IntPtr PromptTokens;
        int PromptTokenCount;

        public IntPtr Language;

        public byte SuppressBlankTokens;
        public byte SuppressNonSpeechTokens;

        public float InitialTemperature;
        public float MaxInitialTimestamp;
        public float LengthPenalty;

        public float FallbackTemperatureIncrement;
        public float FallbackEntropyThreshold;
        public float FallbackLogProbabilityThreshold;
        public float NoSpeechThreshold;

        public int BeamSearchBeamSize;
        public int GreedyBestOf;
        public float BeamSearchPatience;

        public IntPtr NewSegmentCallback;
        public IntPtr NewSegmentCallbackData;

        public IntPtr ProgressCallback;
        public IntPtr ProgressCallbackData;

        public IntPtr EncoderBeginCallback;
        public IntPtr EncoderBeginCallbackData;

        public IntPtr LogitsFilterCallback;
        public IntPtr LogitsFilterCallbackData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WhisperTokenData {
        public int TokenID;
        public int ForcedTimestampTokenID;

        public float TokenProbability;
        public float TokenProbabilityLog;

        public float TimestampProbability;
        public float TimestampProbabilityLog;

        public long StartTime;
        public long EndTime;

        public float VoiceLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WhisperModelLoader {
        public IntPtr Context;
        public IntPtr ReadFunction;
        public IntPtr EndOfFileCheckFunction;
        public IntPtr CloseFunction;

        public delegate IntPtr ReadDelegate(IntPtr pContext, IntPtr pOutBuf, IntPtr nReadSize);
        public delegate bool EOFCheckDelegate(IntPtr pContext);
        public delegate void CloseDelegate(IntPtr pContext);
    }
}
