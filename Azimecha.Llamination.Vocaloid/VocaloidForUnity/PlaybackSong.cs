using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    internal class PlaybackSong : IDisposable {
        private SongHandle _handle;

        public PlaybackSong() {
            _handle = new SongHandle();

            try { } finally {
                _handle.Value = V4UAPI.YVFOpenSong();
            }

            APIErrorException.Check((APIResult)_handle.RawValue);
        }

        public short AddTrack() {
            APIErrorException.Check(V4UAPI.YVFAddTrack(_handle.Value, out short nTrack));
            return nTrack;
        }

        public short TrackCount
            => (short)V4UAPI.YVFGetNumTracks(_handle.Value);

        public int AddPart(short nTrack) {
            PartHead head = new PartHead();
            APIErrorException.Check(V4UAPI.YVFEditPart(_handle.Value, nTrack, ref head, out int nPart));
            return nPart;
        }

        public void AddLyrics(int nPart, int nStartTime, string strLyrics, Language nLanguage)
            => APIErrorException.Check(V4UAPI.YVFPourLyricsInPart(_handle.Value, nPart, nStartTime, InteropUtils.ToNarrowString(strLyrics), nLanguage));

        public void Dispose() {
            System.Threading.Interlocked.Exchange(ref _handle, null)?.Dispose();
        }
    }
}
