using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.LlamaCpp {
    public class LlamaPromptInterface : TextGeneration.TokenBasedPromptInterface<LlamaModel, LlamaState> {
        public LlamaPromptInterface(LlamaModel mdl) : base(mdl) { }
    }
}
