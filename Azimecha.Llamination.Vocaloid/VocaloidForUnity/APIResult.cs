using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.Vocaloid.VocaloidForUnity {
    public enum APIResult {
        Success = 0,
        Error = -1,
        NotEnoughNotes = -2,
        NoDatabase = -3,
        SkipWaveRead = -4,
        InvalidHandle = -5,
        InvalidPartHandle = -6,
        InvalidNoteHandle = -7,
        InvalidParameter = -8,
        InvalidTrack = -9,
        CannotOpenFile = -10,
        NoEvent = -11,
        Uninitialized = -12,
        AlreadyInitialized = -13,
        LyricsEmpty = -14,
        TracksFull = -15,
        InvalidSingerIndex = -16,
        InvalidLanguageIndex = -17,
        InvalidAudioSequenceNumber = -18,
        InternalError = -19,
        RenderInterrupted = -20,
        AuthenticationFailed = -21,
        DatabaseLoadFailed = -22,
        CannotOpenVoiceDatabaseINI = -23,
        InvalidVoiceDatabaseINI = -24,
        NullPointerError = -25,
        InvalidRequestSize = -26,
        InvalidMIDIEventType = -27,
        InvalidTime = -28,
        InvalidString = -29,
        OverCapacity = -30
    }
}
