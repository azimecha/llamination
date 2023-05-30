using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.SAPI {
    [StructLayout(LayoutKind.Sequential)]
    internal struct WaveFormatEx {
        public ushort FormatTag;
        public ushort Channels;
        public uint SamplesPerSec;
        public uint AvgBytesPerSec;
        public ushort BlockAlign;
        public ushort BitsPerSample;
        public uint ExtraInfoSize;

        public static WaveFormatEx CreateI16Format(ushort nChannels, uint nSamplesPerSec) {
            WaveFormatEx fmt = new WaveFormatEx() {
                FormatTag = FORMAT_RAW_INTEGER,
                Channels = nChannels,
                SamplesPerSec = nSamplesPerSec,
                BitsPerSample = 16
            };

            fmt.CalculateBitrateAndAlignment();
            return fmt;
        }

        public void CalculateBitrateAndAlignment() {
            AvgBytesPerSec = (uint)(BitsPerSample * Channels * SamplesPerSec / 8);

            int nCrossChannelBits = BitsPerSample * Channels;
            BlockAlign = (ushort)(nCrossChannelBits / 8 + ((nCrossChannelBits % 8 == 0) ? 0 : 1));
        }

        public const int FORMAT_RAW_INTEGER = 1;
        public const int FORMAT_RAW_FLOAT = 3;

        public static readonly Guid SAPI_DATA_FORMAT = new Guid("{C31ADBAE-527F-4ff5-A230-F62BB61FF70C}");

        public unsafe SpeechLib.WAVEFORMATEX Native {
            get {
                WaveFormatEx fmt = this;
                return *(SpeechLib.WAVEFORMATEX*)&fmt;
            }
            set {
                this = *(WaveFormatEx*)&value;
            }
        }
    }
}
