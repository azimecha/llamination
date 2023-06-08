using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azimecha.Llamination.FileFormats.Image {
    internal class STBGeneralWriter : STBBaseWriter {
        private EncodingDelegate _procEncode;

        public STBGeneralWriter(string strContentType, string strExtension, EncodingDelegate procEncode) : base(strContentType, strExtension) {
            _procEncode = procEncode;
        }

        protected override int CallEncodingFunction(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData)
            => _procEncode(pWriteFunc, pContext, nWidth, nHeight, nComponents, pData);

        public delegate int EncodingDelegate(IntPtr pWriteFunc, IntPtr pContext, int nWidth, int nHeight, int nComponents, IntPtr pData);
    }
}
