using System;
using System.Collections.Generic;

namespace StateMachine {

    public class StateMachine<T> {

        public T Owner { get; private set; }

        private State<T> prevState = null;

        public State<T> PrevState {
            get { return prevState; }
            private set {
                prevState = value;

                if (null != prevState && null != prevState.OnExit) {
                    prevState.OnExit( Owner );
                }
            }
        }

        private State<T> state = null;

        public State<T> State {
            get { return state; }
            private set {
                PrevState = state;
                state = value;

                if (null != state && null != state.OnEnter) {
                    state.OnEnter( Owner );
                }
            }
        }

        private List<StateTransition<T>> transitions = new List<StateTransition<T>>();

        public StateMachine(T owner, State<T> initialState) {
            Owner = owner;
            State = initialState;
        }

        public StateMachine<T> AddTransition(StateTransition<T> transition) {
            transitions.Add( transition );

            return this;
        }

        public StateMachine<T> AddTransition(Predicate<T> condition, State<T> fromState, State<T> toState) {
            return AddTransition( new StateTransition<T>( condition, fromState, toState ) );
        }

        public StateMachine<T> AddTransitions(IEnumerable<StateTransition<T>> trans) {
            foreach (var transition in trans) {
                transitions.Add( transition );
            }

            return this;
        }

        public void RemoveTransition(Predicate<T> condition, State<T> fromState, State<T> toState) {
            for (int i = 0; i < transitions.Count; ++i) {
                var transition = transitions[i];
                if (transition.Condition == condition
                  && transition.FromState == fromState
                  && transition.ToState == toState) {

                    transitions.RemoveAt( i );
                    break;
                }
            }
        }

        public void Update() {
            // Check if a new state should be executed
            foreach (var transition in transitions) {
                if (transition.IsViable( this )) {
                    State = transition.ToState;
                    break;
                }
            }

            if (null != State.OnUpdate) {
                State.OnUpdate( Owner );
            }
        }

        public bool IsCurrentState(State<T> state) {
            return State == state;
        }

    }

}
