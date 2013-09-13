using System;
using System.Collections.Generic;

namespace StateMachine {

    public struct StateTransition<T> {
        public Predicate<T> Condition { get; private set; }

        /// <summary>
        /// State this transition can start from.
        /// Leave null if it should apply to all states.
        /// </summary>
        public State<T> FromState { get; private set; }

        /// <summary>
        /// States this trancsition cannot start from.
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
            FromState = null;

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
            FromState = fromState;
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
            exceptionStates = new State<T>[1] { exceptionState };
            FromState = null;

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
            exceptionStates = new State<T>[exceptions.Length];

            Condition = condition;
            FromState = null;
            exceptions.CopyTo( exceptionStates, 0 );
            ToState = toState;
        }

        public bool IsViable(StateMachine<T> stateMachine) {
            bool stateMatches = false;

            if (stateMachine.State == FromState) {
                stateMatches = true;
            }

            if (FromState == null) {
                if (exceptionStates.Length > 0) {
                    for (int i = 0; i < exceptionStates.Length; ++i) {
                        if (stateMachine.State == exceptionStates[i]) {
                            return false;
                        }
                    }
                }

                stateMatches = true;
            }

            return stateMatches && Condition( stateMachine.Owner );
        }

        public override int GetHashCode() {
            return Condition.GetHashCode()
              + FromState.GetHashCode()
              + ToState.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (!(obj is StateTransition<T>)) {
                return false;
            }

            return Equals( (StateTransition<T>)obj );
        }

        public bool Equals(StateTransition<T> other) {
            return Condition == other.Condition
              && FromState == other.FromState
              && ToState == other.ToState
              && EqualsExceptions( other.ExceptionStates );
        }

        private bool EqualsExceptions(IEnumerable<State<T>> otherExceptions) {
            if (otherExceptions == null && exceptionStates == null) {
                return true;
            }

            if ((otherExceptions == null && exceptionStates != null)
              || (otherExceptions != null && exceptionStates == null)) {
                return false;
            }

            List<State<T>> allStates = new List<State<T>>( exceptionStates );
            foreach (var exception in otherExceptions) {
                if (allStates.Contains( exception )) {
                    allStates.Remove( exception );
                } else {
                    allStates.Add( exception );
                }
            }

            return allStates.Count == 0;
        }

    }

}
