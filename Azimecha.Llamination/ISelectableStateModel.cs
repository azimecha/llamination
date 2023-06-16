using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Llamination {
    public interface ISelectableStateModel<TState> : IDisposable where TState : IDisposable {
        TState GetCurrentState();
        void SetState(TState state);
    }
}
