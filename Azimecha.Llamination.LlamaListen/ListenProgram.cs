using Azimecha.Llamination.Listening;
using Azimecha.Llamination.WhisperCpp;
using System;

namespace Azimecha.Llamination.LlamaListen {
    public static class ListenProgram {
        public static void Main(string[] arrArgs) {
            if (arrArgs.Length < 2) {
                Console.WriteLine("Usage: llamalisten <model> <audio>");
                Console.WriteLine("       <model>   Model file");
                Console.WriteLine("       <audio>   File containing raw 16kHz mono PCM audio in F32 format");
                Environment.Exit(-1);
            }

            string strModelPath = arrArgs[0];
            string strAudioPath = arrArgs[1];

            try {
                //ISpeechRecognitionModel mdl = DeepSpeech.DeepSpeechModel.LoadFromFile(strModelPath);
                //ITranscriptionInterface tsc = new DeepSpeech.DeepSpeechTranscriber((DeepSpeech.DeepSpeechModel)mdl);
                ISpeechRecognitionModel mdl = WhisperModel.Load(strModelPath);
                ITranscriptionInterface tsc = new WhisperTranscriptionInterface((WhisperModel)mdl, SamplingStrategy.BeamSearch);

                byte[] arrData = System.IO.File.ReadAllBytes(strAudioPath);
                float[] arrSamples = new float[arrData.Length / sizeof(float)];
                Extras.InterpetAsSamples(arrData, arrSamples);

                DateTime dtStart = DateTime.UtcNow;
                tsc.ProcessAudio(arrSamples);
                Console.WriteLine("Processing took {0:F2} seconds", (DateTime.UtcNow - dtStart).TotalSeconds);

                for (int nSegment = 0; nSegment < tsc.SegmentCount; nSegment++)
                    Console.WriteLine("[{0:hh\\:mm\\:ss\\.fff}]  {1}", tsc.GetStartOffset(nSegment), tsc.GetText(nSegment));

            } catch (Exception ex) {
                ConsoleColor ccOld = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.ToString());
                Console.ForegroundColor = ccOld;
                Environment.Exit(ex.HResult);
            }
        }
    }
}
