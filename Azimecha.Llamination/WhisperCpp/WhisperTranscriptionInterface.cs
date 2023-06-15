using Azimecha.Llamination.Listening;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Azimecha.Llamination.WhisperCpp {
    public class WhisperTranscriptionInterface : ITokenTranscriptionInterface {
        private WhisperModel _mdl;
        private Native.WhisperFullParams_Unmanaged _par;
        private GCHandle _gchWeakSelf;
        private Exception _exEncodeBegin;

        public WhisperTranscriptionInterface(WhisperModel mdl, SamplingStrategy nStrat) {
            _mdl = mdl;
            _par = Native.WhisperFunctions.WhisperFullDefaultParams(nStrat);
            _par.PrintProgressInfo = 0;
            _par.PrintRealtimeResults = 0;
            _par.PrintSpecialTokens = 0;
            _par.PrintTimestamps = 0;
            _par.InitialPrompt_UTF8 = InteropUtils.EmptyStringPointer;

            _gchWeakSelf = GCHandle.Alloc(this, GCHandleType.Weak);

            unsafe {
                fixed (Native.WhisperFullParams_Unmanaged* ppar = &_par) {
                    byte[] arrData = new byte[sizeof(Native.WhisperFullParams_Unmanaged)];
                    Marshal.Copy((IntPtr)ppar, arrData, 0, arrData.Length);
                    System.Diagnostics.Debug.WriteLine(BitConverter.ToString(arrData).Replace('-', ' '));
                }
            }

            _par.NewSegmentCallback = Marshal.GetFunctionPointerForDelegate(_procNewSegment);
            _par.NewSegmentCallbackData = (IntPtr)_gchWeakSelf;
            _par.LogitsFilterCallback = Marshal.GetFunctionPointerForDelegate(_procLogitsFilter);
            _par.LogitsFilterCallbackData = (IntPtr)_gchWeakSelf;
            _par.EncoderBeginCallback = Marshal.GetFunctionPointerForDelegate(_procEncoderBegin);
            _par.EncoderBeginCallbackData = (IntPtr)_gchWeakSelf;
        }

        private static int MillisFromSampleCount(int nSamples)
            => nSamples / 16; // 16000 samples per second, and 1000 ms per second -> 16 samples per ms

        // must be 16khz mono pcm
        public virtual void ProcessAudio(float[] arrSamples) {
            int nResult = Native.WhisperFunctions.WhisperFull(_mdl.Context.Value, _par, arrSamples, arrSamples.Length);

            if (nResult < 0)
                throw new EvaluationException($"Evaluation function returned {nResult} processing {arrSamples.Length} samples");
            else if (_exEncodeBegin != null)
                throw new System.Reflection.TargetInvocationException($"An exception was thrown from {nameof(OnEncodeBeginning)}",
                    _exEncodeBegin);
        }

        private static Native.WhisperNewSegmentCallback _procNewSegment = NewSegmentCallback;
        private static Native.WhisperLogitsFilterCallback _procLogitsFilter = LogitsFilterCallback;
        private static Native.WhisperEncoderBeginCallback _procEncoderBegin = EncoderBeginCallback;

        private static void NewSegmentCallback(IntPtr pContext, IntPtr pState, int nNewSeg, IntPtr pUserData) {
            try {
                FromIntPtr(pUserData).OnSegmentGenerated();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Exception in {nameof(WhisperTranscriptionInterface)} {nameof(NewSegmentCallback)}: {ex}");
            }
        }

        private static void LogitsFilterCallback(IntPtr pContext, IntPtr pState, IntPtr pTokens, int nTokens, IntPtr pLogits,
            IntPtr pUserData) {
            // TODO
        }

        private static bool EncoderBeginCallback(IntPtr pContext, IntPtr pState, IntPtr pUserData) {
            WhisperTranscriptionInterface iface = null;

            try {
                iface = FromIntPtr(pUserData);
                iface._exEncodeBegin = null;

                /*if (iface.SegmentCount > 0) {
                    iface._par.StartOffsetMillis += (int)iface.GetEndTime(0) * 10;
                    return false;
                }*/

                iface.OnEncodeBeginning();

                return true;
            } catch (Exception ex) {
                if (iface != null)
                    iface._exEncodeBegin = ex;

                System.Diagnostics.Debug.WriteLine($"Exception in {nameof(WhisperTranscriptionInterface)} {nameof(EncoderBeginCallback)}: {ex}");

                return false;
            }
        }

        private static WhisperTranscriptionInterface FromIntPtr(IntPtr pUserData)
            => (WhisperTranscriptionInterface)GCHandle.FromIntPtr(pUserData).Target;

        protected virtual void OnEncodeBeginning() { }

        protected virtual void OnSegmentGenerated() {
            int nSegment = SegmentCount - 1;
            System.Diagnostics.Debug.WriteLine($"S[{nSegment}] = " + GetText(nSegment));
        }

        public int SegmentCount
            => Native.WhisperFunctions.WhisperFullNumSegments(_mdl.Context.Value);

        private void CheckSegmentIndex(int nSegment) {
            if (nSegment < 0 || nSegment >= SegmentCount)
                throw new ArgumentOutOfRangeException(nameof(nSegment));
        }

        public long GetStartTime(int nSegment) {
            CheckSegmentIndex(nSegment);
            return Native.WhisperFunctions.WhisperFullGetSegmentStartTime(_mdl.Context.Value, nSegment);
        }

        public long GetEndTime(int nSegment) {
            CheckSegmentIndex(nSegment);
            return Native.WhisperFunctions.WhisperFullGetSegmentEndTime(_mdl.Context.Value, nSegment);
        }

        public string GetText(int nSegment) {
            CheckSegmentIndex(nSegment);
            return InteropUtils.ReadCString(Native.WhisperFunctions.WhisperFullGetSegmentText(_mdl.Context.Value, nSegment), Encoding.UTF8);
        }

        public int GetTokenCount(int nSegment) {
            CheckSegmentIndex(nSegment);
            return Native.WhisperFunctions.WhisperFullNumTokens(_mdl.Context.Value, nSegment);
        }

        public TokenInfo GetToken(int nSegment, int nToken) {
            if (nToken < 0 || nToken >= GetTokenCount(nSegment))
                throw new ArgumentOutOfRangeException(nameof(nToken));

            Native.WhisperTokenData data = Native.WhisperFunctions.WhisperFullGetTokenData(_mdl.Context.Value, nSegment, nToken);

            return new TokenInfo() {
                TokenID = data.TokenID,
                TokenConfidence = data.TokenProbability,
                TimestampConfidence = data.TimestampProbability,
                StartTime = data.StartTime,
                EndTime = data.EndTime
            };
        }

        private const int TIME_MULTIPLIER = 180; // undocumented, determined empirically, not perfect

        public TimeSpan GetStartOffset(int nSegment)
            => TimeSpan.FromSeconds((float)GetStartTime(nSegment) * TIME_MULTIPLIER / WhisperModel.REQUIRED_AUDIO_SAMPLE_RATE);

        public TimeSpan GetEndOffset(int nSegment)
            => TimeSpan.FromSeconds((float)GetEndTime(nSegment) * TIME_MULTIPLIER / WhisperModel.REQUIRED_AUDIO_SAMPLE_RATE);

        public void Dispose() {
            _mdl = null;
        }

        ~WhisperTranscriptionInterface() {
            try { _gchWeakSelf.Free(); } catch (Exception) { }
        }
    }
}
