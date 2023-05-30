using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Speech {
    public interface IStatementSynthesizer : IDisposable {
        int AudioChannels { get; }
        int AudioSampleRate { get; }
        float[] Synthesize(string strStatement);
    }
}
