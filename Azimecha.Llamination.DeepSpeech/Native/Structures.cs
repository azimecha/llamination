using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.DeepSpeech.Native {
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Metadata {
        public CandidateTranscript* Transcripts;
        public uint NumTranscripts;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CandidateTranscript {
        public TokenMetadata* Tokens;
        public uint NumTokens;
        public double Confidence;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct TokenMetadata {
        public byte* Text;
        public uint Timestep;
        public float StartTime;
    }
}
