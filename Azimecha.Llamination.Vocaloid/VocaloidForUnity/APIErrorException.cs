using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    public class APIErrorException : Exception {
        public APIErrorException(APIResult result) : base($"Vocaloid for Unity returned error {result}") { }

        public static void Check(APIResult result) {
            if (result < 0)
                throw new APIErrorException(result);
        }
    }
}
