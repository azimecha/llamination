using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.FileFormats {
    public interface IImageWriter {
        void WriteImage(ImageData img, System.IO.Stream stmWriteTo);
        string ContentType { get; }
        string FileExtension { get; }
    }
}
