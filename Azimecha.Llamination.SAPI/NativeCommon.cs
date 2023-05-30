using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.SAPI {
    [Flags]
    internal enum SpeechFlags {
        Default = 0,
        Async = 1 << 0,
        PurgeBeforeSpeak = 1 << 1,
        IsFilename = 1 << 2,
        IsXML = 1 << 3,
        IsNotXML = 1 << 4,
        PersistXML = 1 << 5,
        SpeakPunctuation = 1 << 6,
        XMLIsSAPIFormat = 1 << 7,
        XMLIsSSMLFormat = 1 << 8
    }
}
