using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct FullNote {
        public int NoteHandle;
        public NoteBasicSettings BasicSettings;
        public fixed byte LyricString[64];
        public fixed byte PhoneticString[213];
        public NoteAdvancedSettings AdvancedSettings;
        public int UserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NoteBasicSettings {
        public int NoteTime;
        public int NoteLength;
        public int NoteNumber;
        public int NoteVelocity;
        public int VibTypeNo;
        public int VibDelay;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NoteAdvancedSettings {
        public bool Protect; // Bend depth [%] (magnitude of pitch change occurring at the beginning of a note) [0, 100] default: 0
        public int BendDepth; // Bend depth [%] (magnitude of pitch change occurring at the beginning of a note) [0, 100] default: 0
        public int BendLength; // Bend length [%] (length of pitch change that occurs at the beginning of a note) [0, 100] default: 0
        public int UpPortamento; // Add portamento in ascending form (when the pitch of a note is higher than the pitch of the previous note, it produces a smooth upward "portamento" pitch slide at the beginning of the note) 0=off 1=on default: 0
        public int DownPortamento; // Adds portamento in descending form (when a note's pitch is lower than the pitch of the previous note, it produces a smooth descending "portamento-like" pitch slide at the beginning of the note) 0=off 1=on default: 0
        public int Decay; // Decay [%] (Amplitude attenuation at beginning of note) [0, 100] default: 0
        public int Accent; // accent [%] (magnitude of amplitude at beginning of note) [0, 100] default: 0
        public int Opening; // Mouth opening [0: closed, 127: open] default: 127
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Note {
        public NoteBasicSettings BasicSettings;
        public string Lyrics;
        public string PhoneticLyrics;
        public NoteAdvancedSettings AdvancedSettings;
    }
}
