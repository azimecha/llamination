using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Client1111 {
    public interface ISimpleGetPostProvider {
        System.IO.Stream PerformGetRequest(string strURL, byte[] arrPayload = null);
        System.IO.Stream PerformPostRequest(string strURL, byte[] arrPayload = null);
    }
}
