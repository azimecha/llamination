using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.FileFormats {
    public interface IImageReader {
        ImageData ReadImage(System.IO.Stream stmCompressedData);
    }

    public struct ImageData {
        public uint[] Pixels;
        public int Width;
        public int Height;
        public bool IsValid => (Width * Height) == Pixels.Length;
    }
}
