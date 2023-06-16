using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.TextGeneration {
    public interface IPromptInterface : IDisposable {
        int MemorySize { get; set; }
        ICollection<string> SentenceTerminators { get; }
        IList<ITokenSampler> Samplers { get; }

        void ResetState();
        void ProvidePrompt(string strMessage);
        string ReadSentence(int nApproxSizeLimit = int.MaxValue);
    }

    public interface ITokenPromptInterface : IPromptInterface {
        int ReadToken();
    }
}
