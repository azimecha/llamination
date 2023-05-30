using Azimecha.Llamination.Speech;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.SAPI {
    public class SAPIVoice : ISpeechSynthesisModel {
        private SpeechLib.ISpVoice _voice;

        public SAPIVoice() {
            _voice = (SpeechLib.ISpVoice)new SpVoiceClass();
        }

        internal SpeechLib.ISpVoice Voice => _voice;

        public string Name {
            get {
                try {
                    return GetAttribute("Name");
                } catch (Exception) {
                    VoiceToken.GetStringValue(null, out string strName);
                    return strName;
                }
            }
        }

        public ModelGender Gender {
            get {
                try {
                    return (ModelGender)Enum.Parse(typeof(ModelGender), GetAttribute("Gender"));
                } catch (Exception) {
                    return ModelGender.Unknown;
                }
            }
        }

        internal SpeechLib.ISpObjectToken VoiceToken {
            get {
                _voice.GetVoice(out SpeechLib.ISpObjectToken tok);
                return tok;
            }
        }

        public string GetAttribute(string strAttribName) {
            VoiceToken.OpenKey("Attributes", out SpeechLib.ISpDataKey key);
            key.GetStringValue(strAttribName, out string strAttribValue);
            return strAttribValue;
        }

        public IStatementSynthesizer CreateStatementSynthesizer(ref int nChannels, ref int nSampleRate)
            => new SAPIStatementSynth(this, nChannels, nSampleRate);

        public void Dispose() {
            _voice = null;
        }

        [ComImport]
        [Guid("96749377-3391-11D2-9EE3-00C04F797396")]
        private class SpVoiceClass { }
    }
}
