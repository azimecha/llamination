using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Listening {
    public interface ITranscriptionInterface : IDisposable {
        void ProcessAudio(float[] arrSamples);
        int SegmentCount { get; }
        TimeSpan GetStartOffset(int nSegment);
        TimeSpan GetEndOffset(int nSegment);
        string GetText(int nSegment);
    }

    public interface ITokenTranscriptionInterface : ITranscriptionInterface {
        int GetTokenCount(int nSegment);
        TokenInfo GetToken(int nSegment, int nToken);
    }
}
