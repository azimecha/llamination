using Azimecha.Llamination.Listening;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.DeepSpeech {
    public class DeepSpeechTranscriber : ITranscriptionInterface {
        private DeepSpeechModel _mdl;
        private MetadataPointer _metaCur;

        public DeepSpeechTranscriber(DeepSpeechModel mdl) {
            _mdl = mdl;
        }

        public void ProcessAudio(float[] arrSamples) {
            short[] arrI16Samples = new short[arrSamples.Length];

            for (int i = 0; i < arrSamples.Length; i++)
                arrI16Samples[i] = (short)(arrSamples[i] * short.MaxValue);

            MetadataPointer metaNew = new MetadataPointer();
            IntPtr pMeta = IntPtr.Zero;

            try {
                pMeta = Native.Functions.DS_SpeechToTextWithMetadata(_mdl.StatePointer.Value, arrI16Samples, (uint)arrI16Samples.Length, 1);

                if (pMeta == IntPtr.Zero)
                    throw new EvaluationException($"Error transcribing {arrSamples.Length} samples");

                metaNew.Value = pMeta;
            } finally {
                if ((pMeta != IntPtr.Zero) && !metaNew.Initialized)
                    Native.Functions.DS_FreeMetadata(pMeta);
            }

            Interlocked.Exchange(ref _metaCur, metaNew)?.Dispose();
        }

        public unsafe int SegmentCount {
            get {
                if (_metaCur.Target.NumTranscripts == 0)
                    return 0;
                return (int)_metaCur.Target.Transcripts->NumTokens;
            }
        }

        private unsafe void CheckSegmentIndex(int nSegment) {
            if (_metaCur.Target.NumTranscripts == 0)
                throw new IndexOutOfRangeException($"No segment {nSegment} (there was no successful transcript)");

            if (nSegment >= _metaCur.Target.Transcripts->NumTokens)
                throw new IndexOutOfRangeException($"No segment {nSegment} (there are only {_metaCur.Target.Transcripts->NumTokens})");
        }

        public TimeSpan GetEndOffset(int nSegment) {
            throw new NotSupportedException($"DeepSpeech does not provide segment length");
        }

        public unsafe TimeSpan GetStartOffset(int nSegment) {
            CheckSegmentIndex(nSegment);
            return TimeSpan.FromSeconds(_metaCur.Target.Transcripts->Tokens[nSegment].StartTime);
        }

        public unsafe string GetText(int nSegment) {
            CheckSegmentIndex(nSegment);

            byte* pText = _metaCur.Target.Transcripts->Tokens[nSegment].Text;
            if (pText == (byte*)0) return null; // can this actually happen?

            List<byte> lstUTF8Bytes = new List<byte>();
            while (*pText != 0) {
                lstUTF8Bytes.Add(*pText);
                pText++;
            }

            return Encoding.UTF8.GetString(lstUTF8Bytes.ToArray());
        }

        public void Dispose() {
            Interlocked.Exchange(ref _metaCur, null)?.Dispose();
            _mdl = null;
        }
    }
}
