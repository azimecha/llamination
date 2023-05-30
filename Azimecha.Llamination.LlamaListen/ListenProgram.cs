using Azimecha.Llamination.Listening;
using Azimecha.Llamination.WhisperCpp;
using System;

namespace Azimecha.Llamination.LlamaListen {
    public static class ListenProgram {
        public static void Main(string[] arrArgs) {
            if (arrArgs.Length < 2) {
                Console.WriteLine("Usage: llamalisten <model> <audio>");
                Console.WriteLine("       <model>   GGML model file");
                Console.WriteLine("       <audio>   File containing raw 16kHz mono PCM audio in F32 format");
                Environment.Exit(-1);
            }

            string strModelPath = arrArgs[0];
            string strAudioPath = arrArgs[1];

            try {
                WhisperModel mdl;
                WhisperTranscriptionInterface iface;
                int nSegment = 0;

                /*
                using (System.IO.Stream stmModelData = System.IO.File.OpenRead(strModelPath))
                    mdl = SpeechRecognitionModel.Load(stmModelData);
                */

                mdl = WhisperModel.Load(strModelPath);
                iface = new WhisperTranscriptionInterface(mdl, SamplingStrategy.Greedy);

                using (System.IO.Stream stmAudioData = System.IO.File.OpenRead(strAudioPath)) {
                    long nTotalInFile = stmAudioData.Length;
                    long nTotalRead = 0;
                    byte[] arrData = new byte[0xFFFFFF];
                    float[] arrSamples = null;

                    while (true) {
                        int nBytesRead = stmAudioData.Read(arrData);
                        if (nBytesRead <= 0)
                            break;

                        int nSamples = nBytesRead / sizeof(float);
                        if ((arrSamples?.Length ?? -1) != nSamples)
                            arrSamples = new float[nSamples];
                        Extras.InterpetAsSamples(arrData, nBytesRead, arrSamples);

                        iface.ProcessAudio(arrSamples);

                        while (nSegment < iface.SegmentCount) {
                            Console.Out.WriteLine(iface.GetText(nSegment));
                            nSegment++;
                        }

                        nTotalRead += nBytesRead;
                        //Console.Error.Write(((float)nTotalRead / nTotalInFile * 100).ToString("P") + "\r");
                        //Console.Error.Flush();
                    }
                }

                while (nSegment < iface.SegmentCount) {
                    Console.Out.WriteLine(iface.GetText(nSegment));
                    nSegment++;
                }

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
