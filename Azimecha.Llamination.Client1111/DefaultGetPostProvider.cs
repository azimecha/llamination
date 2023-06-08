using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Azimecha.Llamination.Client1111 {
    public class DefaultGetPostProvider : ISimpleGetPostProvider {
        private int _nTimeoutMillis;

        public DefaultGetPostProvider() {
            _nTimeoutMillis = 2 * 60 * 1000; // 2 minutes
        }

        public DefaultGetPostProvider(TimeSpan tsTimeout) {
            double fTotalMillis = tsTimeout.TotalMilliseconds;

            if ((fTotalMillis < 0.0) || (fTotalMillis > int.MaxValue))
                throw new ArgumentOutOfRangeException(nameof(tsTimeout));

            _nTimeoutMillis = (int)fTotalMillis;

            if (_nTimeoutMillis < 1)
                _nTimeoutMillis = 1;
        }

        public Stream PerformGetRequest(string strURL, byte[] arrPayload = null)
            => DoRequest("GET", strURL, arrPayload);

        public Stream PerformPostRequest(string strURL, byte[] arrPayload = null)
            => DoRequest("POST", strURL, arrPayload);

        private Stream DoRequest(string strMethod, string strURL, byte[] arrPayload) {
            WebRequest req = WebRequest.Create(strURL);
            req.Timeout = _nTimeoutMillis;

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
