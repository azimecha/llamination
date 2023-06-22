using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.TextGeneration {
    public interface ILanguageModel : IDisposable {
        int Threads { get; set; }
        void WaitForPreload(WaitHandle whCancel = null, int nTimeout = -1);
        IPromptInterface CreatePromptInterface();
    }
}
