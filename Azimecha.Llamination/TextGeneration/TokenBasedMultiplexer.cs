using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Llamination.TextGeneration {
    public class TokenBasedMultiplexer<TModel, TState> where TModel : ITokenBasedLLM, ISelectableStateModel<TState> where TState : IDisposable {
        private TModel _model;
        private Endpoint _epLast;
        private object _objEndpointMutex = new object();
        private TState _stateDefault;

        public TokenBasedMultiplexer(TModel model) {
            _model = model;
            _stateDefault = model.GetCurrentState();
        }

        private delegate void EndpointSubroutineProc(TModel model);
        private delegate TResult EndpointFunctionProc<TResult>(TModel model);

        private void WithSelected(Endpoint ep, EndpointSubroutineProc proc)
            => WithSelected(ep, model => { proc(model); return 0; });

        private TResult WithSelected<TResult>(Endpoint ep, EndpointFunctionProc<TResult> proc) {
            lock (_objEndpointMutex) {
                if (!ReferenceEquals(ep, _epLast)) {
                    if (_epLast != null) {
                        Endpoint epLast = _epLast;
                        _epLast = null;
                        epLast.Deselect();
                    }

                    ep.Select();
                    _epLast = ep;
                }

                return proc(_model);
            }
        }

        public IEndpoint CreateEndpoint() 
            => new Endpoint(this);

        public interface IEndpoint : ITokenBasedLLM, ISelectableStateModel<TState> { }

        private class Endpoint : IEndpoint {
            private TokenBasedMultiplexer<TModel, TState> _mplex;
            private TState _state;
            private List<int> _lstContextTokens;

            public Endpoint(TokenBasedMultiplexer<TModel, TState> mplex) {
                _mplex = mplex;
                _state = mplex._stateDefault;
                _lstContextTokens = new List<int>();
            }

            public void Select() {
                if (_lstContextTokens.Count < 1 || _lstContextTokens[0] != _mplex._model.BeginningOfStringToken)
                    _lstContextTokens.Insert(0, _mplex._model.BeginningOfStringToken);

                _mplex._model.SetState(_state);
                _mplex._model.Evaluate(_lstContextTokens.ToArray(), 0);
            }

            public void Deselect() {
                while (_lstContextTokens.Count > _mplex._model.ContextSize)
                    _lstContextTokens.RemoveAt(0);

                TState stateNew = _mplex._model.GetCurrentState();
                TState stateOld = _state;

                _state = stateNew;

                if (!ReferenceEquals(stateOld, _mplex._stateDefault))
                    stateOld.Dispose();
            }

            public int Threads { 
                get => _mplex._model.Threads; 
                set => _mplex._model.Threads = value;
            }

            public IEnumerable<ITokenSampler> DefaultSamplers
                => _mplex._model.DefaultSamplers;

            public int VocabularySize 
                => _mplex._model.VocabularySize;

            public int ContextSize
                => _mplex._model.ContextSize;

            public int BeginningOfStringToken
                => _mplex._model.BeginningOfStringToken;

            public int EndOfStringToken
                => _mplex._model.EndOfStringToken;

            public IPromptInterface CreatePromptInterface()
                => new TokenBasedPromptInterface<Endpoint, TState>(this);

            public TState GetCurrentState()
                => _mplex.WithSelected(this, model => model.GetCurrentState());

            public void SetState(TState state)
                => _mplex.WithSelected(this, model => model.SetState(state));

            public void WaitForPreload(WaitHandle whCancel = null, int nTimeout = -1)
                => _mplex._model.WaitForPreload(whCancel, nTimeout);

            public void Dispose() {
                _mplex = null;

                TState stateOld = _state;
                _state = default;

                if (!ReferenceEquals(stateOld, _mplex._stateDefault))
                    stateOld.Dispose();
            }

            public int[] Tokenize(string strText)
                => _mplex.WithSelected(this, model => model.Tokenize(strText));

            public void Evaluate(int[] arrTokens, int nOldTokensToUse = 0) {
                while (_lstContextTokens.Count > nOldTokensToUse)
                    _lstContextTokens.RemoveAt(0);

                _mplex.WithSelected(this, model => model.Evaluate(arrTokens, nOldTokensToUse));
                _lstContextTokens.AddRange(arrTokens);
            }

            public string TokenToString(int nToken)
                => _mplex.WithSelected(this, model => model.TokenToString(nToken));

            public int Sample(int[] arrPrevTokens)
                => _mplex.WithSelected(this, model => model.Sample(arrPrevTokens));

            public int Sample(int[] arrPrevTokens, params ITokenSampler[] arrSamplers)
                => _mplex.WithSelected(this, model => model.Sample(arrPrevTokens, arrSamplers));

            public int Sample(int[] arrPrevTokens, IEnumerable<ITokenSampler> enuSamplers)
                => _mplex.WithSelected(this, model => model.Sample(arrPrevTokens, enuSamplers));

            public float GetLogit(int nToken)
                => _mplex.WithSelected(this, model => model.GetLogit(nToken));

            public void SetLogit(int nToken, float fValue)
                => _mplex.WithSelected(this, model => model.SetLogit(nToken, fValue));
        }
    }
}
