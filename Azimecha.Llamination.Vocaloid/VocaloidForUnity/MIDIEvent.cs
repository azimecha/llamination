using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    internal enum MIDIEvent {
        Begin, ProgramChange, BankSelect, PBSense, PitchBend, Dynamics, Breathiness,
        Brightness, Clarity, Opening, Gender, VibratoDebth, VibratoRate, NoteOff,
        NoteOn, End
    }
}
