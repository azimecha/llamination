using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.FileFormats.Image {
    public class STBWriterFactory : IImageWriterFactory {
        private Dictionary<string, IImageWriter> _dicByContentType = new Dictionary<string, IImageWriter>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string, IImageWriter> _dicByExtension = new Dictionary<string, IImageWriter>(StringComparer.InvariantCultureIgnoreCase);

        public STBWriterFactory() {
            IImageWriter[] arrWriters = new IImageWriter[] { PNGWriter, JPEGWriter, BMPWriter, TGAWriter };
            foreach (IImageWriter writer in arrWriters) {
                _dicByContentType.Add(writer.ContentType, writer);
                _dicByExtension.Add(writer.FileExtension, writer);
            }
        }

        public IImageWriter TryCreateWriterByContentType(string strContentType) {
            if (_dicByContentType.TryGetValue(strContentType, out IImageWriter writer))
                return writer;
            else
                return null;
        }

        public IImageWriter TryCreateWriterByExtension(string strExtension) {
            while (strExtension.StartsWith("."))
                strExtension = strExtension.Substring(1);

            if (_dicByExtension.TryGetValue(strExtension, out IImageWriter writer))
                return writer;
            else
                return null;
        }

        public static readonly IImageWriter PNGWriter = new STBPNGWriter();
        public static readonly IImageWriter JPEGWriter = new STBJPEGWriter(0.95f);
        public static readonly IImageWriter BMPWriter = new STBGeneralWriter("image/bmp", "bmp", StbiWriteBMPToFunc);
        public static readonly IImageWriter TGAWriter = new STBGeneralWriter("image/x-targa", "tga", StbiWriteTGAToFunc);

        public static IImageWriter CreateJPEGWriter(float fQuality)
            => new STBJPEGWriter(fQuality);

        internal const string LIBRARY_NAME = "stb_image_write";

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stbi_write_bmp_to_func")]
        private static extern int StbiWriteBMPToFunc(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData);

        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stbi_write_tga_to_func")]
        private static extern int StbiWriteTGAToFunc(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData);
    }

    internal class STBPNGWriter : STBBaseWriter {
        public STBPNGWriter() : base("image/png", "png") { }

        [DllImport(STBWriterFactory.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stbi_write_png_to_func")]
        private static extern int StbiWritePNGToFunc(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData,
            int nStride);

        protected override int CallEncodingFunction(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData)
            => StbiWritePNGToFunc(pWriteFunc, pContext, nWidth, nHeight, nComponents, pData, nWidth * nHeight * nComponents);
    }

    internal class STBJPEGWriter : STBBaseWriter {
        private int _nQualityPct;

        public STBJPEGWriter(float fQuality) : base("image/jpeg", "jpg") {
            if ((fQuality > 1.0f) || (fQuality < 0.0f))
                throw new ArgumentOutOfRangeException(nameof(fQuality));

            _nQualityPct = (int)(fQuality * 100);
            if (_nQualityPct < 0)
                _nQualityPct = 0;
            else if (_nQualityPct > 100)
                _nQualityPct = 100;
        }

        [DllImport(STBWriterFactory.LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stbi_write_jpg_to_func")]
        private static extern int StbiWriteJPGToFunc(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData,
            int nQuality);

        protected override int CallEncodingFunction(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData)
            => StbiWriteJPGToFunc(pWriteFunc, pContext, nWidth, nHeight, nComponents, pData, _nQualityPct);
    }
}
