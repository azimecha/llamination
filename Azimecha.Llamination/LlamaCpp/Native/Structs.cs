using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp.Native {
    [StructLayout(LayoutKind.Sequential)]
    internal struct TokenData {
        public int TokenID;
        public float Probability;
        public float LogProbability;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ContextParams {
        public int ContextNumber;
        public int PartCount;
        public int Seed;
        public byte UseF16ForKVCache; // bool
        public byte EvalAllLogits; // bool
        public byte VocabOnly; // bool
        public byte LockModelInMemory; // bool
        public byte EmbeddingMode; // bool
        public IntPtr ProgressCallback;
        public IntPtr ProgressCallbackUserData;
    }
}
