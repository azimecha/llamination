using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination.DeepSpeech {
    public class DeepSpeechException : LlamaException {
        public DeepSpeechException(int nErrorCode) {
            LibraryErrorCode = nErrorCode;
        }

        public int LibraryErrorCode { get; private set; }

        public override string Message {
            get {
                string strMsg = $"DeepSpeech error 0x{LibraryErrorCode:X4}";
                if (_dicErrorMessages.TryGetValue(LibraryErrorCode, out string strDesc))
                    strMsg += " - " + strDesc;
                return strMsg;
            }
        }

        internal static void Check(int nErrorCode) {
            if (nErrorCode != 0)
                throw new DeepSpeechException(nErrorCode);
        }

        private static readonly Dictionary<int, string> _dicErrorMessages = new Dictionary<int, string>() {
            { 0x0000, "No error" },
            { 0x1000, "Missing model information" },
            { 0x2000, "Invalid alphabet embedded in model (data corruption?)" },
            { 0x2001, "Invalid model shape" },
            { 0x2002, "Invalid scorer file" },
            { 0x2003, "Incompatible model" },
            { 0x2004, "External scorer is not enabled" },
            { 0x2005, "Could not read scorer file" },
            { 0x2006, "Could not recognize language model header in scorer" },
            { 0x2007, "Reached end of scorer file before loading vocabulary trie" },
            { 0x2008, "Invalid magic in trie header" },
            { 0x2009, "Scorer file version does not match expected version" },
            { 0x3000, "Failed to initialize memory mapped model" },
            { 0x3001, "Failed to initialize the session" },
            { 0x3002, "Interpreter failed" },
            { 0x3003, "Failed to run the session" },
            { 0x3004, "Error creating the stream" },
            { 0x3005, "Error reading the proto buffer model file" },
            { 0x3006, "Failed to create session" },
            { 0x3007, "Could not allocate model state" },
            { 0x3008, "Could not insert hot-word" },
            { 0x3009, "Could not clear hot-words" },
            { 0x3010, "Could not erase hot-word" }
        };
    }
}
