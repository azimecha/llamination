using Azimecha.Llamination.TextGeneration;
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
    internal struct ContextParams<TTensorSplit> where TTensorSplit : unmanaged {
        public int ContextNumber;
        public int BatchSize;
        public int GPULayers;
        public int MainGPU;
        public TTensorSplit TensorSplit;
        public int Seed;

        public byte UseF16ForKVCache; // bool
        public byte EvalAllLogits; // bool
        public byte VocabOnly; // bool
        public byte MapModelInMemory; // bool
        public byte LockModelInMemory; // bool
        public byte EmbeddingMode; // bool

        public IntPtr ProgressCallback;
        public IntPtr ProgressCallbackUserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GenericTensorSplit {
        public float Device00;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CudaTensorSplit {
        public float Device00;
        public float Device01;
        public float Device02;
        public float Device03;
        public float Device04;
        public float Device05;
        public float Device06;
        public float Device07;
        public float Device08;
        public float Device09;
        public float Device10;
        public float Device11;
        public float Device12;
        public float Device13;
        public float Device14;
        public float Device15;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TokenDataArray {
        public SamplingCandidate* Data;
        public IntPtr Size;
        public bool IsSorted;
    }
}
