using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaLibrary {
        private static LlamaLibrary _instance;
        private static object _objInitMutex = new object();

        public static LlamaLibrary GetInstance(bool bIsCUDABuild) {
            lock (_objInitMutex) {
                if (_instance is null)
                    _instance = new LlamaLibrary(bIsCUDABuild);
                else if (_instance.IsCUDABuild != bIsCUDABuild)
                    throw new InvalidOperationException($"LLaMA has already been initialized with CUDA set to {_instance.IsCUDABuild}, not {bIsCUDABuild}");
                return _instance;
            }
        }

        private bool _bCUDA;

        private LlamaLibrary(bool bIsCUDABuild) {
            _bCUDA = bIsCUDABuild;
            Native.Functions.LlamaInitBackend();
        }

        public bool IsCUDABuild => _bCUDA;

        public LlamaModel LoadModel(string strFileName, int nMaxGPULayers = int.MaxValue) {
            Pointers.ContextPointer ctx = new Pointers.ContextPointer();

            if (_bCUDA) {
                Native.ContextParams<Native.CudaTensorSplit> par = Native.Functions.LlamaContextDefaultParams_CUDA();
                par.GPULayers = nMaxGPULayers;
                ctx.Value = Native.Functions.LlamaInitFromFile(InteropUtils.ToNarrowString(strFileName), par);
            } else {
                Native.ContextParams<Native.GenericTensorSplit> par = Native.Functions.LlamaContextDefaultParams_Generic();
                par.GPULayers = nMaxGPULayers;
                ctx.Value = Native.Functions.LlamaInitFromFile(InteropUtils.ToNarrowString(strFileName), par);
            }

            if (!ctx.Initialized)
                throw new ModelLoadException(strFileName);

            return new LlamaModel(ctx, strFileName);
        }
    }
}
