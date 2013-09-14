using System;
using System.Collections.Generic;

namespace StateMachine {

    public class State<T> {

        public Action<T> OnEnter { get; protected set; }
        public Action<T> OnUpdate { get; protected set; }
        public Action<T> OnExit { get; protected set; }

        protected State() { }

        public static State<T> Build(Action<T> onEnter, Action<T> onUpdate, Action<T> onExit) {
          return new State<T>() {
            OnEnter = onEnter,
            OnUpdate = onUpdate,
            OnExit = onExit
          };
        }
    }

}
