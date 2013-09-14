using System;
using System.Collections.Generic;

namespace StateMachine {

    public abstract class State<T> {

        public Action<T> OnEnter { get; protected set; }
        public Action<T> OnUpdate { get; protected set; }
        public Action<T> OnExit { get; protected set; }

    }

}
