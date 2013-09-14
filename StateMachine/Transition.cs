﻿using System;
using System.Collections.Generic;

namespace StateMachine {

    public class Transition<T> {
        public Func<bool> Condition { get; private set; }

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

        public static ConfigFrom From(params State<T>[] states) {
            if (states == null) { throw new ArgumentNullException( "states", "At least an origin state must be defined" ); }
            if (states.Length == 0) { throw new ArgumentException( "states", "At least an origin state must be defined" ); }

            return new ConfigFrom( states );
        }

        public static ConfigFromAny FromAny() {
            return new ConfigFromAny();
        }

        /// <summary>
        /// Cannot build a StateTransition, except from the static configurators.
        /// </summary>
        private Transition() {

        }

        public bool Evaluate(State<T> currentState) {
            bool stateMatches = false;

            if (fromStates != null) {
                for (int i = 0; i < fromStates.Length; ++i) {
                    if (currentState == fromStates[i]) {
                        stateMatches = true;
                        break;
                    }
                }
            } else {
                stateMatches = true;

                if (exceptionStates.Length > 0) {
                    for (int i = 0; i < exceptionStates.Length; ++i) {
                        if (currentState == exceptionStates[i]) {
                            stateMatches = false;
                            break;
                        }
                    }
                }
            }

            return stateMatches && Condition();
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
            if (!(obj is Transition<T>)) {
                return false;
            }

            return Equals( (Transition<T>)obj );
        }

        public bool Equals(Transition<T> other) {
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

        public class Config {
            protected State<T>[] Origins { get; private set; }
            protected State<T>[] Exceptions { get; private set; }

            internal Config(State<T>[] origins, State<T>[] exceptions) {
                Origins = origins;
                Exceptions = exceptions;
            }

            public ConfigFromTo To(State<T> state) {
                if (state == null) { throw new ArgumentNullException( "state", "Transition must define a target state." ); }

                return new ConfigFromTo( Origins, Exceptions, state );
            }

            public ConfigFromTo ToPrev() {
                return new ConfigFromTo( Origins, Exceptions, null );
            }

        }

        public class ConfigFrom : Config {

            internal ConfigFrom(State<T>[] origins)
                : base( origins, null ) {
            }

            public ConfigFrom From(params State<T>[] states) {
                if (states == null) { throw new ArgumentNullException( "states", "At least one origin state required." ); }
                if (states.Length == 0) { throw new ArgumentException( "At least one origin state required.", "states" ); }

                State<T>[] newOrigins = null;
                if (Origins != null) {
                    newOrigins = new State<T>[Origins.Length + states.Length];
                    Origins.CopyTo( newOrigins, 0 );
                    states.CopyTo( newOrigins, Origins.Length );
                } else {
                    newOrigins = new State<T>[states.Length];
                    states.CopyTo( newOrigins, 0 );
                }

                return new ConfigFrom( newOrigins );
            }

        }

        public class ConfigFromAny : Config {

            internal ConfigFromAny()
                : base( null, null ) {
            }

            internal ConfigFromAny(State<T>[] exceptions)
                : base( null, exceptions ) {
            }

            public ConfigFromAny Except(params State<T>[] states) {
                if(states == null) { throw new ArgumentNullException("states", "At least one exceptional state required."); }
                if(states.Length == 0) { throw new ArgumentException( "At least one exceptional state required.", "states" ); }

                State<T>[] newExceptions = null;
                if (Exceptions != null) {
                    newExceptions = new State<T>[Exceptions.Length + states.Length];
                    Exceptions.CopyTo( newExceptions, 0 );
                    states.CopyTo( newExceptions, Exceptions.Length );
                } else {
                    newExceptions = new State<T>[states.Length];
                    states.CopyTo( newExceptions, 0 );
                }

                return new ConfigFromAny( newExceptions );
            }

        }

        public class ConfigFromTo {

            private State<T>[] Origins { get; set; }
            private State<T>[] Exceptions { get; set;}
            private State<T> Target { get; set;}

            internal ConfigFromTo(State<T>[] origins, State<T>[] exceptions, State<T> target) {
                Origins = origins;
                Exceptions = exceptions;
                Target = target;
            }

            public Transition<T> When(Func<bool> condition) {
                if (condition == null) { throw new ArgumentNullException( "condition", "A condition must be configured" ); }

                return new Transition<T>() {
                    fromStates = Origins,
                    exceptionStates = Exceptions,
                    ToState = Target,
                    Condition = condition
                };
            }

        }
    }

}
