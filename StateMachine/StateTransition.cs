using System;
using System.Collections.Generic;

namespace StateMachine {

    public struct StateTransition<T> {
        public Predicate<T> Condition { get; private set; }

        /// <summary>
        /// States this transition can start from.
        /// If none defined, start from any.
        /// </summary>
        private State<T>[] fromStates;
        public IEnumerable<State<T>> FromStates {
            get {
                return fromStates;
            }
        }

        /// <summary>
        /// States this transition cannot start from.
        /// </summary>
        private State<T>[] exceptionStates;
        public IEnumerable<State<T>> ExceptionStates {
            get {
                return exceptionStates;
            }
        }

        /// <summary>
        /// State this transition triggers when conditions are met.
        /// </summary>
        public State<T> ToState { get; private set; }

        /// <summary>
        /// Create a transition which can trigger from any state.
        /// </summary>
        /// <param name="condition">Condition which would trigger the transition.</param>
        /// <param name="toState">State to change the state machine to when triggered.</param>
        public StateTransition(Predicate<T> condition, State<T> toState)
            : this() {
            exceptionStates = null;
            fromStates = null;

            Condition = condition;
            ToState = toState;
        }

        /// <summary>
        /// Create a transition which can only trigger for a specific state.
        /// </summary>
        /// <param name="fromState">State the machine must be executing to trigger the transition.</param>
        /// <param name="condition">Condition which would trigger the transition.</param>
        /// <param name="toState">State to change the state machine to when triggered.</param>
        public StateTransition(State<T> fromState, Predicate<T> condition, State<T> toState)
            : this() {
            exceptionStates = null;

            Condition = condition;
            fromStates = new State<T>[1] { fromState };
            ToState = toState;
        }

        public StateTransition(State<T>[] fromStates, Predicate<T> condition, State<T> toState)
            : this() {
            exceptionStates = null;

            Condition = condition;
            this.fromStates = new State<T>[fromStates.Length];
            fromStates.CopyTo( this.fromStates, 0 );
            ToState = toState;
        }

        /// <summary>
        /// Create a transition which can trigger from any state but the one specified.
        /// </summary>
        /// <param name="condition">Condition which would trigger the transition.</param>
        /// <param name="exceptionState">State from which the transition cannot be triggered.</param>
        /// <param name="toState">State to change the state machine to when triggered.</param>
        public StateTransition(Predicate<T> condition, State<T> exceptionState, State<T> toState)
            : this() {
            fromStates = null;

            exceptionStates = new State<T>[1] { exceptionState };
            Condition = condition;
            ToState = toState;
        }

        /// <summary>
        /// Create a transition which can trigger from any state but the ones specified.
        /// </summary>
        /// <param name="condition">Condition which would trigger the transition.</param>
        /// <param name="exceptions">States from which the transition cannot be triggered.</param>
        /// <param name="toState">State to change the state machine to when triggered.</param>
        public StateTransition(Predicate<T> condition, State<T>[] exceptions, State<T> toState)
            : this() {
            fromStates = null;

            Condition = condition;
            exceptionStates = new State<T>[exceptions.Length];
            exceptions.CopyTo( exceptionStates, 0 );
            ToState = toState;
        }

        public bool IsViable(StateMachine<T> stateMachine) {
            bool stateMatches = false;

            if (fromStates != null) {
                for (int i = 0; i < fromStates.Length; ++i) {
                    if (stateMachine.State == fromStates[i]) {
                        stateMatches = true;
                        break;
                    }
                }
            } else {
                stateMatches = true;

                if (exceptionStates.Length > 0) {
                    for (int i = 0; i < exceptionStates.Length; ++i) {
                        if (stateMachine.State == exceptionStates[i]) {
                            stateMatches = false;
                            break;
                        }
                    }
                }
            }

            return stateMatches && Condition( stateMachine.Owner );
        }

        public override int GetHashCode() {
            int result = Condition.GetHashCode()
                + ToState.GetHashCode();

            if (null != fromStates) {
                foreach (var state in fromStates) {
                    result += state.GetHashCode();
                }
            } else if (null != exceptionStates) {
                foreach (var exception in exceptionStates) {
                    result += exception.GetHashCode();
                }
            }

            return result;
        }

        public override bool Equals(object obj) {
            if (!(obj is StateTransition<T>)) {
                return false;
            }

            return Equals( (StateTransition<T>)obj );
        }

        public bool Equals(StateTransition<T> other) {
            return Condition == other.Condition
              && ToState == other.ToState
              && CompareStateArrays( fromStates, other.fromStates )
              && CompareStateArrays( exceptionStates, other.exceptionStates );
        }

        private bool CompareStateArrays(State<T>[] array1, State<T>[] array2) {
            if (array1 == null && array2 == null) {
                return true;
            }

            if((array1 == null && array2 != null)
                || (array1 != null && array2 == null)) {
                return false;
            }

            if (array1.Length != array2.Length) {
                return false;
            }

            return _CompareStateArrays( array1, array2 );
        }

        private bool _CompareStateArrays(State<T>[] array1, State<T>[] array2) {
            List<State<T>> unmatchedStates = new List<State<T>>( array1 );
            foreach (var exception in array2) {
                if (unmatchedStates.Contains( exception )) {
                    unmatchedStates.Remove( exception );
                } else {
                    unmatchedStates.Add( exception );
                }
            }

            return unmatchedStates.Count == 0;
        }

    }

}
