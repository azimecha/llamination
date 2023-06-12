using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    internal static class V4UAPI {
        private const string LIBRARY_NAME = "VforU_Win";

        public const int SAMPLE_RATE = 44100;
        public static readonly Encoding TEXT_ENCODING = new UTF8Encoding(false);

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
        public static extern APIResult YVFShutdown();
    }
}
