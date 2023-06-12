using Azimecha.Llamination.Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    public class VforUSynthesizer : IStatementSynthesizer {
        private VforUVocaloid _voc;
        private int _nChannels;

        public VforUSynthesizer(VforUVocaloid voc, int nChannels) {
            if (nChannels < 1)
                throw new ArgumentOutOfRangeException(nameof(nChannels));

            _voc = voc;
            _nChannels = nChannels;

            DelayPerSyllable = TimeSpan.FromSeconds(0.30);
            DelayBetweenSyllables = TimeSpan.FromSeconds(0.01);
            DelayAfterStatement = TimeSpan.FromSeconds(0.50);
            Tone = 71;
        }

        public TimeSpan DelayPerSyllable { get; set; }
        public TimeSpan DelayBetweenSyllables { get; set; }
        public TimeSpan DelayAfterStatement { get; set; }
        public int Tone { get; set; }

        public int AudioChannels => _nChannels;
        public int AudioSampleRate => V4UAPI.SAMPLE_RATE;

        private void AddMIDIEvent(MIDIEvent nEvent, int nParam, TimeSpan tsLength, List<short[]> lstSampleBlocks) {
            DateTime dtStart = DateTime.UtcNow;
            _voc.State.AddMIDIEvent(nEvent, nParam);
            _voc.State.CommitMIDIEvents();

            while ((DateTime.UtcNow - dtStart) < tsLength) {
                short[] arrRawSamples = new short[_voc.State.BufferedSamples];
                _voc.State.ReadSamples(arrRawSamples, arrRawSamples.Length);
                lstSampleBlocks.Add(arrRawSamples);
                System.Threading.Thread.Sleep(arrRawSamples.Length * 1000 / AudioSampleRate);
            }
        }

        public float[] Synthesize(string strStatement) {
            _voc.State.SetLyrics(strStatement, _voc.VocaloidLanguage);
            int nSyllables = _voc.State.LyricSyllables;

            List<short[]> lstSampleBlocks = new List<short[]>();
            
            for (int nSyllable = 0; nSyllable < nSyllables; nSyllable++) {
                AddMIDIEvent(MIDIEvent.NoteOn, Tone, DelayPerSyllable, lstSampleBlocks);
                AddMIDIEvent(MIDIEvent.NoteOff, Tone, (nSyllable < (nSyllables - 1)) ? DelayBetweenSyllables : DelayAfterStatement, lstSampleBlocks);
            }

            int nRawSamples = 0;
            foreach (short[] arr in lstSampleBlocks)
                nRawSamples += arr.Length;

            float[] arrConvSamples = new float[nRawSamples * _nChannels];
            int nConvSample = 0;

            foreach (short[] arrBlock in lstSampleBlocks) {
                for (int nRawSample = 0; nRawSample < arrBlock.Length; nRawSample++) {
                    for (int nChannel = 0; nChannel < _nChannels; nChannel++) {
                        arrConvSamples[nConvSample] = (float)arrBlock[nRawSample] / short.MaxValue;
                        nConvSample++;
                    }
                }
            }

            return arrConvSamples;
        }

        public void Dispose() {
            _voc = null;
            _nChannels = 0;
        }
    }
}
