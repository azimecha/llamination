using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.FileFormats {
    public class EncodingException : Exception {
        public EncodingException(string strMessage) : base(strMessage) { }
        public EncodingException(string strMessage, Exception ex) : base(strMessage, ex) { }
    }
}
