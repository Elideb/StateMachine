using System;
using System.Collections.Generic;

namespace StateMachine {

    public class StateTransition<T> {
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

        #region Static builders

        /// <summary>
        /// Create a transition from any state to the given one.
        /// </summary>
        public static StateTransition<T> FromAnyTo(Predicate<T> condition, State<T> toState) {
            if (null == toState) { throw new ArgumentNullException( "toState", "FromAnyTo requires a to state." ); }

            return new StateTransition<T>() {
                fromStates = null,
                exceptionStates = null,
                Condition = condition,
                ToState = toState
            };
        }

        /// <summary>
        /// Create a transition from any state to the previous one.
        /// </summary>
        public static StateTransition<T> FromAnyToPrev(Predicate<T> condition) {
            return new StateTransition<T>() {
                fromStates = null,
                exceptionStates = null,
                Condition = condition,
                ToState = null
            };
        }

        /// <summary>
        /// Create a transition from a state to the given one.
        /// </summary>
        public static StateTransition<T> FromTo(State<T> fromState, Predicate<T> condition, State<T> toState) {
            if (null == fromState) { throw new ArgumentNullException( "fromState", "FromTo requires a from state." ); }
            if (null == toState) { throw new ArgumentNullException( "toState", "FromTo requires a to state." ); }

            return new StateTransition<T>() {
                fromStates = new State<T>[] { fromState },
                exceptionStates = null,
                Condition = condition,
                ToState = toState
            };
        }

        /// <summary>
        /// Create a transition from several states to the given one.
        /// </summary>
        public static StateTransition<T> FromTo(State<T>[] fromStates, Predicate<T> condition, State<T> toState) {
            if (null == fromStates) { throw new ArgumentNullException( "fromStates", "FromTo requires from states." ); }
            if (null == toState) { throw new ArgumentNullException( "toState", "FromTo requires a to state." ); }

            return new StateTransition<T>() {
                fromStates = fromStates,
                exceptionStates = null,
                Condition = condition,
                ToState = toState
            };
        }

        /// <summary>
        /// Create a transition from a state to the previous one.
        /// </summary>
        public static StateTransition<T> FromToPrev(State<T> fromState, Predicate<T> condition) {
            if (null == fromState) { throw new ArgumentNullException( "fromState", "FromToPrev requires a from state." ); }

            return new StateTransition<T>() {
                fromStates = new State<T>[] { fromState },
                exceptionStates = null,
                Condition = condition,
                ToState = null
            };
        }

        /// <summary>
        /// Create a transition from several states to the previous one.
        /// </summary>
        public static StateTransition<T> FromToPrev(State<T>[] fromStates, Predicate<T> condition) {
            if (null == fromStates) { throw new ArgumentNullException( "fromStates", "FromToPrev requires from states." ); }

            return new StateTransition<T>() {
                fromStates = fromStates,
                exceptionStates = null,
                Condition = condition,
                ToState = null
            };
        }

        /// <summary>
        /// Create a transition from any state but the exception to the given one.
        /// </summary>
        public static StateTransition<T> FromAnyButTo(State<T> exception, Predicate<T> condition, State<T> toState) {
            if (null == exception) { throw new ArgumentNullException( "exception", "NotFromTo requires an exception state." ); }
            if (null == toState) { throw new ArgumentNullException( "toState", "NotFromTo requires a to state." ); }

            return new StateTransition<T>() {
                fromStates = null,
                exceptionStates = new State<T>[] { exception },
                Condition = condition,
                ToState = toState
            };
        }

        /// <summary>
        /// Create a transition from any state but the exceptions to the given one.
        /// </summary>
        public static StateTransition<T> FromAnyButTo(State<T>[] exceptions, Predicate<T> condition, State<T> toState) {
            if (null == exceptions) { throw new ArgumentNullException( "exceptions", "NotFromTo requires exception states." ); }
            if (null == toState) { throw new ArgumentNullException( "toState", "NotFromTo requires a to state." ); }

            return new StateTransition<T>() {
                fromStates = null,
                exceptionStates = exceptions,
                Condition = condition,
                ToState = toState
            };
        }

        /// <summary>
        /// Create a transition from any state but the exception to the previous one.
        /// </summary>
        public static StateTransition<T> FromAnyButToPrev(State<T> exception, Predicate<T> condition) {
            if (null == exception) { throw new ArgumentNullException( "exception", "NotFromToPrev requires an exception state." ); }

            return new StateTransition<T>() {
                fromStates = null,
                exceptionStates = new State<T>[] { exception },
                Condition = condition,
                ToState = null
            };
        }

        /// <summary>
        /// Create a transition from any state but the exceptions to the previous one.
        /// </summary>
        public static StateTransition<T> FromAnyButToPrev(State<T>[] exceptions, Predicate<T> condition) {
            if (null == exceptions) { throw new ArgumentNullException( "exceptions", "NotFromToPrev requires exception states." ); }

            return new StateTransition<T>() {
                fromStates = null,
                exceptionStates = exceptions,
                Condition = condition,
                ToState = null
            };
        }

        #endregion

        /// <summary>
        /// Cannot build a StateTransition, except from the static builders.
        /// </summary>
        private StateTransition() {

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
            int result = Condition.GetHashCode();

            if (ToState != null) {
                result += ToState.GetHashCode();
            }

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

            if ((array1 == null && array2 != null)
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
