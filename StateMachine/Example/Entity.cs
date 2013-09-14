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
            State<Entity>[] idleExceptions = new State<Entity>[] {
                IdleState.Instance,
                TalkState.Instance };

            stateMachine = new StateMachine<Entity>( this, IdleState.Instance )
                .AddTransitions(
                    StateTransition<Entity>.NotFromTo( idleExceptions, HasNoTarget, IdleState.Instance ),
                    StateTransition<Entity>.FromTo( IdleState.Instance, HasTarget, WalkState.Instance ),
                    StateTransition<Entity>.FromTo( WalkState.Instance, IsTargetInRange, TalkState.Instance ),
                    StateTransition<Entity>.FromTo( TalkState.Instance, DoneTalking, IdleState.Instance ) );
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

        public static bool IsTargetOutOfRange(Entity entity) {
            return entity.DistanceToTarget > 0.01f;
        }

        public static bool IsTargetInRange(Entity entity) {
            return !Entity.IsTargetOutOfRange( entity );
        }

        public static bool HasNoTarget(Entity entity) {
            return entity.Target == null;
        }

        public static bool HasTarget(Entity entity) {
            return !Entity.HasNoTarget( entity );
        }

        public static bool DoneTalking(Entity entity) {
            return entity.PhrasesLeft == 0;
        }

        #endregion

        #region IStateful<Entity> Members

        public StateMachine<Entity> GetStateMachine() {
            return stateMachine;
        }

        #endregion
    }

}
