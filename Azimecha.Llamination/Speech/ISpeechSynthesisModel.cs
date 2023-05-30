using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Speech {
    public interface ISpeechSynthesisModel : IDisposable {
        string Name { get; }
        ModelGender Gender { get; }

        IStatementSynthesizer CreateStatementSynthesizer(ref int nChannels, ref int nSampleRate);
    }

    public enum ModelGender {
        Unknown,
        Male,
        Female,
        Other
    }
}
