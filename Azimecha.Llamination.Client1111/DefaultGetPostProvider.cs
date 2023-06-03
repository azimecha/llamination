using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Azimecha.Llamination.Client1111 {
    public class DefaultGetPostProvider : ISimpleGetPostProvider {
        public Stream PerformGetRequest(string strURL, byte[] arrPayload = null)
            => DoRequest("GET", strURL, arrPayload);

        public Stream PerformPostRequest(string strURL, byte[] arrPayload = null)
            => DoRequest("POST", strURL, arrPayload);

        private static Stream DoRequest(string strMethod, string strURL, byte[] arrPayload) {
            WebRequest req = WebRequest.Create(strURL);

            if (req is HttpWebRequest reqHTTP)
                reqHTTP.Method = strMethod;

            if (arrPayload != null)
                using (Stream stmReq = req.GetRequestStream())
                    stmReq.Write(arrPayload, 0, arrPayload.Length);

            WebResponse resp = req.GetResponse();

            if (resp is HttpWebResponse respHTTP)
                if ((int)respHTTP.StatusCode >= 300)
                    throw new WebException($"Server returned error {respHTTP.StatusCode}");

            return resp.GetResponseStream();
        }
    }
}
