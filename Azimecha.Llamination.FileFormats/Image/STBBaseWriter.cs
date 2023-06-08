using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.FileFormats.Image {
    internal abstract class STBBaseWriter : IImageWriter {
        protected STBBaseWriter(string strContentType, string strExtension) {
            ContentType = strContentType;
            FileExtension = strExtension;
        }

        public string ContentType { get; private set; }
        public string FileExtension { get; private set; }

        private class WritingContext {
            public Exception Error;
            public Stream Destination;
            public byte[] TempBuffer;
        }

        private const int ARGB_CHANNEL_COUNT = 4;

        public unsafe void WriteImage(ImageData img, Stream stmWriteTo) {
            if (!img.IsValid)
                throw new ArgumentException($"Image width {img.Width} and height {img.Height} does not match data size {img.Pixels.Length}");

            IntPtr pWriteFunc = Marshal.GetFunctionPointerForDelegate(_procWrite);
            WritingContext ctx = new WritingContext() { Destination = stmWriteTo };
            GCHandle gchContext = new GCHandle();

            try {
                gchContext = GCHandle.Alloc(ctx);

                fixed (uint* pData = img.Pixels)
                    CallEncodingFunction(pWriteFunc, (IntPtr)gchContext, img.Width, img.Height, ARGB_CHANNEL_COUNT, (IntPtr)pData);
            } finally {
                if (gchContext.IsAllocated)
                    gchContext.Free();
            }

            if (ctx.Error != null)
                throw new EncodingException($"Error writing image data", ctx.Error);
        }

        private delegate void WriteFunctionDelegate(IntPtr pContext, IntPtr pData, int nDataSize);
        private static readonly WriteFunctionDelegate _procWrite = WriteFunction;

        private static void WriteFunction(IntPtr pContext, IntPtr pData, int nDataSize) {
            try {
                WritingContext ctx = (WritingContext)GCHandle.FromIntPtr(pContext).Target;

                if (ctx.Error != null)
                    return;

                try {
                    if ((ctx.TempBuffer?.Length ?? -1) < nDataSize)
                        ctx.TempBuffer = new byte[nDataSize];

                    Marshal.Copy(pData, ctx.TempBuffer, 0, nDataSize);

                    ctx.Destination.Write(ctx.TempBuffer, 0, nDataSize);
                } catch (Exception exError) {
                    ctx.Error = exError;
                }
            } catch (Exception exAny) {
                System.Diagnostics.Debug.WriteLine($"Exception in {nameof(STBBaseWriter)} {nameof(WriteFunction)}:\r\n{exAny}");
            }
        }

        protected abstract int CallEncodingFunction(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData);
    }
}
