using Azimecha.Llamination.Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    internal class PlaybackSynthesizer : IStatementSynthesizer {
        public int AudioChannels => throw new NotImplementedException();

        public int AudioSampleRate => throw new NotImplementedException();

        public void Dispose() {
            throw new NotImplementedException();
        }

        public float[] Synthesize(string strStatement) {
            throw new NotImplementedException();
        }
    }
}
