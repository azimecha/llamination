using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Listening {
    public interface ISpeechRecognitionModel : IDisposable {
        int RequiredSampleRate { get; }
        int RequiredChannelCount { get; }
    }
}
