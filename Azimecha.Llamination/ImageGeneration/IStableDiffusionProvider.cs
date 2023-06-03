using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.ImageGeneration {
    public interface IStableDiffusionProvider : IImageGenerator {
        int Steps { get; set; }
        int ConfigScale { get; set; }
    }
}
