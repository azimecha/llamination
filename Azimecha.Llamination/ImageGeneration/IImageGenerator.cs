using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.ImageGeneration {
    public interface IImageGenerator : IDisposable {
        IEnumerable<ISamplerInfo> Samplers { get; }
        float Quality { get; set; }
        float Accuracy { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        string NegativePrompt { get; set; }

        IGeneratedImage GenerateImage(string strPrompt, string strAdditionalNegative = null, int nSeed = -1);
    }

    public interface ISamplerInfo {
        string Name { get; }
        void Select();
    }

    public interface IGeneratedImage : IDisposable {
        byte[] CompressedData { get; }
        uint[] PixelData { get; }
        int Width { get; }
        int Height { get; }
    }
}
