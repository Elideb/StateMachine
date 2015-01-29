
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

            stateMachine = new StateMachine<Entity>( this, EntityStates.Idle )
                .AddTransitions(
                    Transition.FromAny<Entity>()
                              .Except( EntityStates.Idle, EntityStates.Talk )
                              .To( EntityStates.Idle )
                              .When( HasNoTarget ),
                    Transition.From( EntityStates.Idle )
                              .To( EntityStates.Walk )
                              .When( HasTarget ),
                    Transition.From( EntityStates.Walk )
                              .To( EntityStates.Talk )
                              .When( IsTargetInRange ),
                    Transition.From( EntityStates.Talk )
                              .To( EntityStates.Idle )
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
