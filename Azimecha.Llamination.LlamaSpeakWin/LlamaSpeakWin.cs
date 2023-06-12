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
                    //ISpeechSynthesisModel voice = new SAPIVoice();
                    ISpeechSynthesisModel voice = new Vocaloid.VocaloidForUnity.VforUVocaloid(
                        @"E:\Projects\AI\Chadbot\VOCALOID_SDK_for_Unity\Assets\StreamingAssets\VOCALOID\DB_ini", 
                        Vocaloid.VocaloidForUnity.Language.Japanese);
                    Console.WriteLine($"Active model: {voice.Name} ({voice.Gender})");

                    int nChannels = 1, nSampleRate = 16000;
                    IStatementSynthesizer synth = voice.CreateStatementSynthesizer(ref nChannels, ref nSampleRate);
                    string strCurStatement;

                    Console.WriteLine($"Channels: {nChannels}");
                    Console.WriteLine($"Sample rate: {nSampleRate}");

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
