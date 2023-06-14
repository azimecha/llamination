using System;
using System.Collections.Generic;
using System.IO;
using Azimecha.Llamination.LlamaCpp;

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

                Console.Error.WriteLine(" ====> LOADING MODEL");
                LlamaModel mdl = LlamaLibrary.GetInstance(LLAMA_BUILT_WITH_CUDA).LoadModel(arrArgs[0]);
                Console.Error.WriteLine(" ====> LOADING DATA");
                mdl.WaitForPreload();
                Console.Error.WriteLine(" ====> LOAD COMPLETED");
                Console.Error.WriteLine();

                LlamaPromptInterface pi = new LlamaPromptInterface(mdl);

                while (true) {
                    Console.Error.Write("> ");
                    Console.Error.Flush();
                    pi.ProvidePrompt(Console.ReadLine());

                    for (int nSentencesToGenerate = rand.Next(1, 5); nSentencesToGenerate > 0; nSentencesToGenerate--) {
                        Console.Write(pi.ReadSentence());
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