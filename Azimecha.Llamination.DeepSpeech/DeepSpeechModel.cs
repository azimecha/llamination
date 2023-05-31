using Azimecha.Llamination.Listening;
using System;

namespace Azimecha.Llamination.DeepSpeech {
    public class DeepSpeechModel : ISpeechRecognitionModel {
        private ModelStatePointer _mdl;

        internal DeepSpeechModel(ModelStatePointer mdl) {
            _mdl = mdl;
        }

        public static DeepSpeechModel LoadFromFile(string strFilePath) {
            try {
                byte[] arrPathUTF8 = System.Text.Encoding.UTF8.GetBytes(strFilePath);

                ModelStatePointer mdl = new ModelStatePointer();
                IntPtr pModel = IntPtr.Zero;

                try {
                    DeepSpeechException.Check(Native.Functions.DS_CreateModel(arrPathUTF8, out pModel));
                    mdl.Value = pModel;
                } finally {
                    if ((pModel != IntPtr.Zero) && !mdl.Initialized)
                        Native.Functions.DS_FreeModel(pModel);
                }

                return new DeepSpeechModel(mdl);
            } catch (Exception ex) {
                throw new ModelLoadException(strFilePath, ex);
            }
        }

        public int RequiredSampleRate => Native.Functions.DS_GetModelSampleRate(_mdl.Value);
        public int RequiredChannelCount => 1;

        internal ModelStatePointer StatePointer => _mdl;

        public void Dispose() {
            System.Threading.Interlocked.Exchange(ref _mdl, null)?.Dispose();
        }
    }
}
