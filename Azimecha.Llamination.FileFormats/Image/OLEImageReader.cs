#if false
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azimecha.Llamination.FileFormats.Image {
    public class OLEImageReader : IImageReader {
        public OLEImageReader() {
            if (!SmartImageReader.IsWindows)
                throw new PlatformNotSupportedException($"{nameof(OLEImageReader)} requires Windows");
        }

        public ImageData ReadImage(Stream stmCompressedData) {
            Native.DotnetStream stmWrapped = new Native.DotnetStream(stmCompressedData);

        }
    }
}
#endif
