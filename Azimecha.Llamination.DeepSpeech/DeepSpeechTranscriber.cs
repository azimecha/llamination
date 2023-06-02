using Azimecha.Llamination.Listening;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.DeepSpeech {
    public class DeepSpeechTranscriber : ITranscriptionInterface {
        private DeepSpeechRawTranscriber _tscRaw;
        private List<Segment> _lstSegments;

        public DeepSpeechTranscriber(DeepSpeechModel mdl) {
            _tscRaw = new DeepSpeechRawTranscriber(mdl);
            _lstSegments = new List<Segment>();

            SegmentBreakThreshold = TimeSpan.FromSeconds(1);
            SegmentEndMargin = TimeSpan.FromSeconds(1);
        }

        public TimeSpan SegmentBreakThreshold { get; set; }
        public TimeSpan SegmentEndMargin { get; set; }

        public void ProcessAudio(float[] arrSamples) {
            if (_tscRaw is null)
                throw new ObjectDisposedException(nameof(DeepSpeechTranscriber));

            _tscRaw.ProcessAudio(arrSamples);
            _lstSegments.Clear();

            Segment segCur = new Segment();

            for (int nRawSeg = 0; nRawSeg < _tscRaw.SegmentCount; nRawSeg++) {
                TimeSpan tsRawStart = _tscRaw.GetStartOffset(nRawSeg);
                string strRawText = _tscRaw.GetText(nRawSeg);

                if (nRawSeg == 0) {
                    segCur.Text = strRawText;
                    segCur.End = segCur.Start = tsRawStart;
                } else if ((tsRawStart - segCur.End) < SegmentBreakThreshold) {
                    segCur.Text += strRawText;
                    segCur.End = tsRawStart;
                } else {
                    segCur.End += SegmentEndMargin;
                    segCur.Text = segCur.Text.Trim();
                    _lstSegments.Add(segCur);
                    segCur.Text = strRawText;
                    segCur.End = segCur.Start = tsRawStart;
                }
            }

            if (segCur.Text.Length > 0) {
                segCur.End += SegmentEndMargin;
                segCur.Text = segCur.Text.Trim();
                _lstSegments.Add(segCur);
            }
        }

        public int SegmentCount => _lstSegments.Count;

        public TimeSpan GetStartOffset(int nSegment)
            => _lstSegments[nSegment].Start;

        public TimeSpan GetEndOffset(int nSegment)
            => _lstSegments[nSegment].End;

        public string GetText(int nSegment)
            => _lstSegments[nSegment].Text;

        public void Dispose() {
            Interlocked.Exchange(ref _tscRaw, null)?.Dispose();
            _lstSegments = null;
        }

        private struct Segment {
            public string Text;
            public TimeSpan Start;
            public TimeSpan End;
        }
    }
}
