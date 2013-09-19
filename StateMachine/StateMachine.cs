using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Current executing state.
        /// </summary>
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

        /// <summary>
        /// Transitions which determine this state machine's behaviour.
        /// </summary>
        private List<Transition<T>> transitions = new List<Transition<T>>();

        /// <summary>
        /// Create a new state machine.
        /// </summary>
        /// <param name="owner">Whose behaviour is controlled by this state machine.</param>
        /// <param name="initialState">State the state machine starts from.</param>
        public StateMachine(T owner, State<T> initialState) {
            Owner = owner;
            State = initialState;
        }

        public StateMachine<T> AddTransition(Transition<T> transition) {
            transitions.Add( transition );

            return this;
        }

        public StateMachine<T> AddTransitions(params Transition<T>[] trans) {
            foreach (var transition in trans) {
              transitions.Add( transition );
            }

            return this;
        }

        public StateMachine<T> AddTransitions(IEnumerable<Transition<T>> trans) {
            foreach (var transition in trans) {
                transitions.Add( transition );
            }

            return this;
        }

        public void RemoveTransition(Transition<T> toBeRemoved) {
            for (int i = 0; i < transitions.Count; ++i) {
                if (toBeRemoved.Equals( transitions[i] )) {
                    transitions.RemoveAt( i );
                    break;
                }
            }
        }

        public void RemoveTransitions(IEnumerable<Transition<T>> toBeRemoved) {
            foreach (var transition in toBeRemoved) {
                RemoveTransition( transition );
            }
        }

        /// <summary>
        /// Check transitions and update the current state.
        /// </summary>
        public void Update() {
            // Check if a new state should be executed
            var toStates = from transition in transitions
                           where transition.IsApplicable( State )
                           select transition.ToState == null ? prevState : transition.ToState;

            if (toStates.Any()) {
                State = toStates.First();
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
