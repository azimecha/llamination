using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    internal class VforUState {
        private static object _objMutex = new object();
        private static VforUState _instance = null;
        private static string _strPrevPath;

        public static VforUState GetInstance(string strINIFolderPath) {
            string strFullPath = System.IO.Path.GetFullPath(strINIFolderPath);

            lock (_objMutex) {
                if (_instance != null) {
                    if (_strPrevPath != strFullPath)
                        throw new NotSupportedException("Vocaloid for Unity can only be initialized once, with one INI path");
                    return _instance;
                }

                _instance = new VforUState(strINIFolderPath);
                _strPrevPath = strFullPath;
                return _instance;
            }
        }

        private VforUState(string strINIFolderPath) {
            APIErrorException.Check(V4UAPI.YVFStartup(InteropUtils.ToNarrowString("personal"), InteropUtils.ToNarrowString(strINIFolderPath)));
        }

        public void StartRealtime() {
            APIErrorException.Check(V4UAPI.YVFRealtimeSetStaticSetting(RealtimeMode.Mode2048));
            APIErrorException.Check(V4UAPI.YVFRealtimeStart());
        }

        public void SetLyrics(string strLyrics, Language lang)
            => APIErrorException.Check(V4UAPI.YVFRealtimeSetLyrics(InteropUtils.ToNarrowString(strLyrics), lang));

        public int LyricSyllables 
            => V4UAPI.YVFRealtimeGetSyllableCount();

        public void AddMIDIEvent(MIDIEvent nEventType, int nValue)
            => APIErrorException.Check(V4UAPI.YVFRealtimeAddMidi(nEventType, nValue));

        public void CommitMIDIEvents()
            => APIErrorException.Check(V4UAPI.YVFRealtimeCommitMidi());

        public int BufferedSamples {
            get {
                uint nSamples = V4UAPI.YVFRealtimeGetAudioNumData();
                if (nSamples > int.MaxValue)
                    nSamples = int.MaxValue;
                return (int)nSamples;
            }
        }

        public void ReadSamples(short[] arrBuffer, int nCount) {
            if ((nCount < 0) || (nCount > arrBuffer.Length))
                throw new ArgumentOutOfRangeException(nameof(nCount));
            APIErrorException.Check(V4UAPI.YVFRealtimePopAudio(arrBuffer, nCount));
        }

        ~VforUState() {
            V4UAPI.YVFRealtimeStop();
            V4UAPI.YVFShutdown();
        }
    }
}
