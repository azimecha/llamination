using Azimecha.Llamination;
using Azimecha.Llamination.ImageGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.DiffusionCommand {
    internal static class DiffusionCommand {
        private static void Main(string[] args) {
            if (args.Length < 3) {
                Console.Error.WriteLine("Usage: DiffusionCommand <image.raw> <url> <prompt> [negative] [sampler] [model]");
                Environment.Exit(1);
            }

            string strImagePath = args[0];
            string strBaseURL = args[1];
            string strPrompt = args[2];
            string strNegativePrompt = (args.Length >= 4) ? args[3] : null;
            string strDesiredSampler = (args.Length >= 5) ? args[4].ToUpper() : null;
            string strDesiredModel = (args.Length >= 6) ? args[5].ToUpper() : null;

            try {
                using (System.IO.Stream stmRawImageFile = System.IO.File.Open(strImagePath, System.IO.FileMode.Create)) {
                    IRemoteImageGenerator gen = new Client1111.Automatic1111ImageGenerator(strBaseURL);

                    foreach (ISamplerInfo samp in gen.Samplers) {
                        Console.WriteLine($"Sampler: {samp.Name}");

                        if (samp.Name.ToUpper() == strDesiredSampler) {
                            Console.WriteLine("\t(selecting)");
                            samp.Select();
                        }
                    }

                    Console.WriteLine();

                    foreach (IRemoteModel mdl in gen.Models) {
                        Console.WriteLine($"Model: {mdl.Name} / {mdl.DisplayName} / " + Convert.ToBase64String(mdl.Hash));

                        if ((strDesiredModel != null) && (mdl.Name.ToUpper() == strDesiredModel)) {
                            Console.WriteLine("\t(selecting)");
                            mdl.Select();
                        }
                    }

                    Console.WriteLine();

                    Console.WriteLine($"Provided prompt: {strPrompt}");
                    Console.WriteLine($"Provided negative prompt: {strNegativePrompt ?? string.Empty}");

                    Console.WriteLine();

                    Console.Out.Write("Generating... ");
                    Console.Out.Flush();
                    IGeneratedImage img = gen.GenerateImage(strPrompt, strNegativePrompt);
                    Console.Out.WriteLine("done.");

                    Console.WriteLine();

                    Console.WriteLine($"Width: {img.Width}");
                    Console.WriteLine($"Height: {img.Height}");

                    Console.WriteLine();

                    Console.Out.Write($"Saving {strImagePath}... ");
                    Console.Out.Flush();
                    byte[] arrData = Extras.GetBytes(img.PixelData);
                    stmRawImageFile.Write(arrData, 0, arrData.Length);
                    Console.Out.WriteLine("done.");
                }
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
                Environment.Exit(System.Runtime.InteropServices.Marshal.GetHRForException(ex));
            }
        }
    }
}

