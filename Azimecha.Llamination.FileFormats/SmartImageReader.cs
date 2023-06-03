using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azimecha.Llamination.FileFormats {
    public class SmartImageReader : IImageReader {
        private Image.STBImageReader _reader = new Image.STBImageReader();

        public ImageData ReadImage(Stream stmCompressedData)
            => _reader.ReadImage(stmCompressedData);

        public static bool IsWindows {
            get {
                switch (Environment.OSVersion.Platform) {
                    #if NETFRAMEWORK
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                    case PlatformID.Xbox:
                    #endif
                    case PlatformID.Win32NT:
                        return true;

                    default:
                        return false;
                }
            }
        }
    }
}
