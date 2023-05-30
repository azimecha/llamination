using SpeechLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.SAPI {
    internal static class StreamFormatWrapper {
        public static ISpStreamFormat Wrap(IStream stm, in WaveFormatEx fmt) {
            Guid idDataFormat = WaveFormatEx.SAPI_DATA_FORMAT;
            WAVEFORMATEX fmtNative = fmt.Native;

            ISpStream stmSAPI = new SpStream();
            stmSAPI.SetBaseStream(stm, ref idDataFormat, ref fmtNative);
            return stmSAPI;
        }
    }
}
