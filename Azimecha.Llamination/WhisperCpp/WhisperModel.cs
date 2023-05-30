using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.WhisperCpp {
    public class WhisperModel : Listening.ISpeechRecognitionModel {
        private WhisperContext _ctx;

        internal WhisperModel(WhisperContext ctx) { _ctx = ctx; }

        public const int REQUIRED_AUDIO_SAMPLE_RATE = 16000;
        public const byte REQUIRED_AUDIO_CHANNEL_COUNT = 1;

        public int RequiredSampleRate => REQUIRED_AUDIO_SAMPLE_RATE;
        public int RequiredChannelCount => REQUIRED_AUDIO_CHANNEL_COUNT;

        public static WhisperModel Load(System.IO.Stream stmModelData) {
            WhisperContext ctx = new WhisperContext();

            using (ModelSource ms = new ModelSource(stmModelData, false)) {
                ctx.Value = Native.WhisperFunctions.WhisperInit(ms.Loader);
                GC.KeepAlive(ms);
            }

            if (!ctx.Initialized)
                throw new ModelLoadException();

            return new WhisperModel(ctx);
        }

        public static WhisperModel Load(string strFilePath) {
            using (System.IO.Stream stmFile = System.IO.File.OpenRead(strFilePath))
                return Load(stmFile);
        }

        /*
        public static SpeechRecognitionModel Load(string strFilePath) {
            WhisperContext ctx = new WhisperContext();

            ctx.Value = Native.WhisperFunctions.WhisperInitFromFile(strFilePath);

            if (!ctx.Initialized)
                throw new ModelLoadException();

            return new SpeechRecognitionModel(ctx);
        }
        */

        internal WhisperContext Context => _ctx;

        public string GetTokenString(int nTokenID) {
            // TODO: check ID?
            IntPtr pString = Native.WhisperFunctions.WhisperTokenToString(_ctx.Value, nTokenID);
            if (pString == IntPtr.Zero)
                throw new KeyNotFoundException($"Token {nTokenID} not found");
            return InteropUtils.ReadCString(pString, Encoding.UTF8);
        }

        public void Dispose() {
            System.Threading.Interlocked.Exchange(ref _ctx, null)?.Dispose();
        }
    }
}
