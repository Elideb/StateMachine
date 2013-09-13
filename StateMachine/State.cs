using System;
using System.Collections.Generic;

namespace StateMachine {

    public abstract class State<T> {

        public Action<T> OnEnter { get { return EnterMethod; } }
        public Action<T> OnUpdate { get { return UpdateMethod; } }
        public Action<T> OnExit { get { return ExitMethod; } }

        protected Action<T> EnterMethod;
        protected Action<T> UpdateMethod;
        protected Action<T> ExitMethod;

        public abstract IEnumerable<StateTransition<T>> GetTransitions();
    }

}
