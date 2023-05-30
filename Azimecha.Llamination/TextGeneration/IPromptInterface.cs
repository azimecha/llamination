using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.TextGeneration {
    public interface IPromptInterface : IDisposable {
        int MemorySize { get; set; }
        int MaxRepeatCount { get; set; }
        ICollection<string> SentenceTerminators { get; }

        void ProvidePrompt(string strMessage, bool bReset = false);
        void ReadResponseTokens(ResponseReceiver procReceiver);
        string ReadSentence(int nMaxSize = -1);
    }

    public delegate bool ResponseReceiver(int nToken);
}
