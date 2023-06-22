using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    internal static class V4UAPI {
        private const string LIBRARY_NAME = "VforU_Win";

        public const int SAMPLE_RATE = 44100;

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFStartup([In] byte[] arrTargetUTF8, [In] byte[] arrPathUTF8);

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFRealtimeSetStaticSetting(RealtimeMode mode);

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFRealtimeStart();

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFRealtimeSetLyrics([In] byte[] arrLyricsUTF8, Language lang);

        [DllImport(LIBRARY_NAME)]
        public static extern int YVFRealtimeGetSyllableCount();

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFRealtimeAddMidi(MIDIEvent nEventType, int nValue);

        [DllImport(LIBRARY_NAME)]
        public static extern uint YVFRealtimeGetAudioNumData();

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFRealtimePopAudio([Out] short[] arrBuffer, int nBufferSize);

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFRealtimeCommitMidi();

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFRealtimeStop();

        [DllImport(LIBRARY_NAME)]
        public static extern int YVFOpenSong();

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFAddTrack(int nHandle, out short nTrack);

        [DllImport(LIBRARY_NAME)]
        public static extern int YVFGetNumTracks(int nHandle);

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFEditPart(int nHandle, short nTrack, ref PartHead head, out int nPartHandle);

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFPourLyricsInPart(int nHandle, int nPartHandle, int nStartTime, byte[] arrLyricsUTF8, Language nLanguage);

        [DllImport(LIBRARY_NAME)]
        public static extern void YVFCloseSong(int nHandle);

        [DllImport(LIBRARY_NAME)]
        public static extern APIResult YVFShutdown();
    }
}
