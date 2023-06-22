using Azimecha.Llamination.Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    public class VforUVocaloid : ISpeechSynthesisModel {
        private VforUState _state;
        private Language _lang;

        public VforUVocaloid(string strINIFolderPath, Language lang) {
            _state = VforUState.GetInstance(strINIFolderPath);
            _lang = lang;
        }

        internal VforUState State => _state;

        public string Name => "Vocaloid for Unity";
        public ModelGender Gender => ModelGender.Unknown;
        public Language VocaloidLanguage => _lang;

        public IStatementSynthesizer CreateStatementSynthesizer(ref int nChannels, ref int nSampleRate) {
            nSampleRate = V4UAPI.SAMPLE_RATE;
            return new RealtimeSynthesizer(this, nChannels);
        }

        public void Dispose() {
            _state = null;
        }
    }
}
