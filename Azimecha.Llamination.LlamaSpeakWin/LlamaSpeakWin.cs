using Azimecha.Llamination;
using Azimecha.Llamination.SAPI;
using Azimecha.Llamination.Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaSpeakWin {
    internal static class LlamaSpeakWin {
        private static void Main(string[] args) {
            if (args.Length < 1) {
                Console.Error.WriteLine("Usage: LlamaSpeakWin <outputfile.raw>");
                Console.Error.WriteLine("       Provide input data on stdin");
                Environment.Exit(1);
            }

            try {
                using (System.IO.Stream stmAudioFile = System.IO.File.Open(args[0], System.IO.FileMode.Create)) {
                    SAPIVoice voice = new SAPIVoice();
                    Console.WriteLine($"Active model: {voice.Name} ({voice.Gender})");

                    IStatementSynthesizer synth = new SAPIStatementSynth(voice, 1, 16000);
                    string strCurStatement;

                    while ((strCurStatement = Console.In.ReadLine()) != null) {
                        float[] arrSamples = synth.Synthesize(strCurStatement);
                        byte[] arrData = Extras.GetBytes(arrSamples);
                        stmAudioFile.Write(arrData, 0, arrData.Length);
                    }
                }
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
                Environment.Exit(System.Runtime.InteropServices.Marshal.GetHRForException(ex));
            }
        }
    }
}
