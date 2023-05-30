using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination {
    public class LlamaException : Exception {
        public LlamaException(string strMessage) : base(strMessage) { }
        public LlamaException(string strMessage, Exception exInner) : base(strMessage, exInner) { }
    }

    public class ModelLoadException : LlamaException {
        public ModelLoadException() : base($"Error loading model") { }
        public ModelLoadException(string strFileName) : base($"Error loading model {strFileName}") { }
    }

    public class TokenizationException : LlamaException {
        public TokenizationException(string strInput, int nResult) 
            : base($"Tokenization function returned {nResult} for string \"{strInput}\"") { }
    }

    public class EvaluationException : LlamaException {
        public EvaluationException(string strMessage)
            : base(strMessage) { }
        public EvaluationException(int nResult, int nTokens) 
            : base($"Evaluation function returned {nResult} processing {nTokens} tokens") { }
        public EvaluationException(string strInput, Exception exInner)
            : base($"Error evaluating string {strInput}", exInner) { }
    }

    public class InvalidTokenException : LlamaException {
        public InvalidTokenException(int nToken) : base($"Token {nToken} not found") { }
    }

    public class OversizedInputException : LlamaException {
        public OversizedInputException(string strMessage) : base(strMessage) { }
    }
}
