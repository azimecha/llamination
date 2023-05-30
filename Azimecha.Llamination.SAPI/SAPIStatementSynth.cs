using Azimecha.Llamination.Speech;
using SpeechLib;
using System;
using System.Threading;

namespace Azimecha.Llamination.SAPI {
    public class SAPIStatementSynth : IStatementSynthesizer {
        private UnmanagedMemoryStream _stmMem = new UnmanagedMemoryStream();
        private WaveFormatEx _format;
        private ISpStreamFormat _stmWithFormat;
        private SAPIVoice _voice;

        public SAPIStatementSynth(SAPIVoice voice, int nChannels, int nSamplesPerSec) {
            _format = WaveFormatEx.CreateI16Format((ushort)nChannels, (uint)nSamplesPerSec);
            _stmWithFormat = StreamFormatWrapper.Wrap(_stmMem.Stream, _format);
            _voice = voice;
        }

        public int AudioChannels => _format.Channels;
        public int AudioSampleRate => (int)_format.SamplesPerSec;

        public float[] Synthesize(string strStatement) {
            _stmMem.Position = 0;
            _voice.Voice.SetOutput(_stmWithFormat, 0);
            _voice.Voice.Speak(strStatement, (uint)SpeechFlags.IsNotXML, out _);

            ulong nFullSampleCount = _stmMem.GetElementCount<short>();
            if (nFullSampleCount > int.MaxValue)
                throw new InsufficientMemoryException("Generated audio is too large");

            int nSampleCount = (int)nFullSampleCount;
            float[] arrF32Samples = new float[nSampleCount];
            short[] arrI16Samples = _stmMem.RetrieveContents<short>();

            for (int nSample = 0; nSample < nSampleCount; nSample++)
                arrF32Samples[nSample] = (float)arrI16Samples[nSample] / short.MaxValue;

            return arrF32Samples;
        }

        public void Dispose() {
            Interlocked.Exchange(ref _stmMem, null)?.Dispose();
            _stmWithFormat = null;
            _voice = null;
        }
    }
}
