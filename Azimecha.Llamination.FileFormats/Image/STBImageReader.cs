using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azimecha.Llamination.FileFormats.Image {
    public class STBImageReader : IImageReader {
        public unsafe ImageData ReadImage(Stream stmCompressedData) {
            ImageData data = new ImageData();

            using (Native.STBIOWrapper wrapper = new Native.STBIOWrapper(stmCompressedData)) {
                IntPtr pImageData = IntPtr.Zero;
                int nChannels;

                try {
                    pImageData = Native.STBImage.StbiLoadFromCallbacks(wrapper.CallbacksStruct, wrapper.UserData, out data.Width, out data.Height, 
                        out nChannels, 4);

                    data.Pixels = new uint[data.Width * data.Height];
                    for (int nPixel = 0; nPixel < data.Width * data.Height; nPixel++)
                        data.Pixels[nPixel] = ((uint*)pImageData)[nPixel];
                } finally {
                    if (pImageData != IntPtr.Zero)
                        Native.STBImage.StbiImageFree(pImageData);
                }
            }

            return data;
        }
    }
}
