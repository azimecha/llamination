using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.ImageGeneration {
    public interface ILocalImageGenerator : IImageGenerator {
        void LoadModel(string strFilePath);
        void LoadModel(System.IO.Stream stmModel);
    }
}
