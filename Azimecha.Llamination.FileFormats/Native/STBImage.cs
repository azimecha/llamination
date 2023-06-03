using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.FileFormats.Native {
    internal static class STBImage {
        public delegate int ReadDelegate(IntPtr pUserData, IntPtr pBuffer, int nBufferSize);
        public delegate void SkipDelegate(IntPtr pUserData, int nToSkip);
        public delegate int EOFCheckDelegate(IntPtr pUserData);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct IOCallbacks {
            public ReadDelegate Read;
            public SkipDelegate Skip;
            public EOFCheckDelegate EOF;
        }

        private const string STB_IMAGE_LIBRARY = "stb_image";

        [DllImport(STB_IMAGE_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stbi_load_from_callbacks")]
        public static extern IntPtr StbiLoadFromCallbacks(in IOCallbacks callbacks, IntPtr pUserData, out int nWidth, out int nHeight,
            out int nChannelsInFile, int nDesiredChannels);

        [DllImport(STB_IMAGE_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stbi_image_free")]
        public static extern IntPtr StbiImageFree(IntPtr pBuffer);
    }
}
