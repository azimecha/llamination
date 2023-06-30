using System;
using System.Collections.Generic;
using System.IO;
using Azimecha.Llamination.LlamaCpp;
using Azimecha.Llamination.TextGeneration;

namespace Azimecha.Llamination.ConsoleUI {
    public static class ConsoleProgram {
        private const bool LLAMA_BUILT_WITH_CUDA = true;

        public static void Main(string[] arrArgs) {
            if (arrArgs.Length < 1) {
                Console.WriteLine("No model specified");
                Environment.Exit(-1);
            }

            try {
                Random rand = new();
                Dictionary<string, IPromptInterface> dicEndpointPIs = new Dictionary<string, IPromptInterface>();

                Console.Error.WriteLine(" ====> LOADING MODEL");
                LlamaModel mdl = LlamaLibrary.GetInstance(LLAMA_BUILT_WITH_CUDA).LoadModel(arrArgs[0]);
                TokenBasedMultiplexer<LlamaModel, LlamaState> mplex = new TokenBasedMultiplexer<LlamaModel, LlamaState>(mdl);
                Console.Error.WriteLine(" ====> LOADING DATA");
                mdl.WaitForPreload();
                Console.Error.WriteLine(" ====> RETRIEVING INTERFACE");
                IPromptInterface piCur = dicEndpointPIs[string.Empty] = mplex.CreateEndpoint().CreatePromptInterface();
                Console.Error.WriteLine(" ====> LOAD COMPLETED");
                Console.Error.WriteLine();

                while (true) {
                    Console.Error.Write("> ");
                    Console.Error.Flush();
                    string strLine = Console.ReadLine();

                    string[] arrPieces = strLine.Trim().ToLowerInvariant().Split(' ');

                    if (arrPieces.Length > 0) {
                        switch (arrPieces[0]) {
                            case "reset":
                                piCur.ResetState();
                                Console.Error.WriteLine("(model state reset)");
                                continue;

                            case "conv":
                                if (arrPieces.Length < 2)
                                    continue;
                                if (dicEndpointPIs.TryGetValue(arrPieces[1], out IPromptInterface piExisting))
                                    piCur = piExisting;
                                else
                                    piCur = dicEndpointPIs[arrPieces[1]] = mplex.CreateEndpoint().CreatePromptInterface();
                                Console.Error.WriteLine($"(switched to conversation {arrPieces[1]})");
                                continue;
                        }
                    }


                    piCur.ProvidePrompt(strLine);

                    for (int nSentencesToGenerate = rand.Next(1, 5); nSentencesToGenerate > 0; nSentencesToGenerate--) {
                        Console.Write(piCur.ReadSentence());
                        Console.Out.Flush();
                    }

                    Console.WriteLine();
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