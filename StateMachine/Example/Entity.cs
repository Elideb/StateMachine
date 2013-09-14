using System.Collections.Generic;

namespace StateMachine.Example {

    class Entity : IStateful<Entity> {

        private StateMachine<Entity> stateMachine = null;

        private float distanceToTarget = 0;
        public float DistanceToTarget {
            get { return distanceToTarget; }
            private set { distanceToTarget = value; }
        }

        private Entity target = null;
        public Entity Target {
            get {
                return target;
            }
            private set {
                target = value;
                if (target != null) {
                    DistanceToTarget = DistanceTo( target );
                }
            }
        }

        private int phrasesLeft;

        public int PhrasesLeft {
            get { return phrasesLeft; }
            set { phrasesLeft = value; }
        }

        public void Initialize() {
            State<Entity> walkState = State<Entity>.Build(
                null,
                (entity) => entity.Move( 2 ),
                null );

            stateMachine = new StateMachine<Entity>( this, IdleState.Instance )
                .AddTransitions(
                    Transition<Entity>
                        .FromAny()
                        .Except( IdleState.Instance, TalkState.Instance )
                        .To( IdleState.Instance )
                        .When( HasNoTarget ),
                    Transition<Entity>
                        .From( IdleState.Instance )
                        .To( walkState )
                        .When( HasTarget ),
                    Transition<Entity>
                        .From( walkState )
                        .To( TalkState.Instance )
                        .When( IsTargetInRange ),
                    Transition<Entity>
                        .From( TalkState.Instance )
                        .To( IdleState.Instance )
                        .When( DoneTalking ) );

        }

        public void Update() {
            stateMachine.Update();
        }

        public float DistanceTo(Entity other) {
            return 15f; // Properly calculating distance requires positions...
        }

        public void Move(float distance) {
            distanceToTarget -= distance;
        }

        #region State condition methods

        public bool IsTargetOutOfRange() {
            return DistanceToTarget > 0.01f;
        }

        public bool IsTargetInRange() {
            return !IsTargetOutOfRange();
        }

        public bool HasNoTarget() {
            return Target == null;
        }

        public bool HasTarget() {
            return !HasNoTarget();
        }

        public bool DoneTalking() {
            return PhrasesLeft == 0;
        }

        #endregion

        #region IStateful<Entity> Members

        public StateMachine<Entity> GetStateMachine() {
            return stateMachine;
        }

        #endregion
    }

}
