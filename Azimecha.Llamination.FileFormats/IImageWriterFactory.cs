using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.FileFormats {
    public interface IImageWriterFactory {
        IImageWriter TryCreateWriterByContentType(string strContentType);
        IImageWriter TryCreateWriterByExtension(string strExtension);
    }
}
