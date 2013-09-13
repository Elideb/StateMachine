using System.Collections.Generic;

namespace StateMachine.Example {

    class IdleState : State<Entity> {

        #region Singleton

        private static IdleState instance;

        public static IdleState Instance {
            get {
                if (IdleState.instance == null) {
                    IdleState.instance = new IdleState();
                }

                return IdleState.instance;
            }
        }

        #endregion

        private IdleState() {
            EnterMethod = null;
            UpdateMethod = Update;
            ExitMethod = null;
        }

        private void Update(Entity entity) {
            // Look for target
        }

        public override IEnumerable<StateTransition<Entity>> GetTransitions() {
            return new StateTransition<Entity>[] {
                StateTransition<Entity>.NotFromTo( TalkState.Instance, Entity.HasNoTarget, IdleState.Instance ),
                StateTransition<Entity>.FromTo( IdleState.Instance, Entity.HasTarget, WalkState.Instance )
            };
        }

    }

    class WalkState : State<Entity> {

        #region Singleton

        private static WalkState instance;

        public static WalkState Instance {
            get {
                if (WalkState.instance == null) {
                    WalkState.instance = new WalkState();
                }

                return WalkState.instance;
            }
        }

        #endregion

        private WalkState() {
            EnterMethod = null;
            UpdateMethod = Update;
            ExitMethod = null;
        }

        private void Update(Entity entity) {
            entity.Move( 2 );
        }

        public override IEnumerable<StateTransition<Entity>> GetTransitions() {
            return new StateTransition<Entity>[] {
                StateTransition<Entity>.FromTo( WalkState.Instance, Entity.IsTargetInRange, TalkState.Instance )
            };
        }

    }

    class TalkState : State<Entity> {

        #region Singleton

        private static TalkState instance;

        public static TalkState Instance {
            get {
                if (TalkState.instance == null) {
                    TalkState.instance = new TalkState();
                }

                return TalkState.instance;
            }
        }

        #endregion

        private TalkState() {
            EnterMethod = Enter;
            UpdateMethod = Update;
            ExitMethod = null;
        }

        public void Enter(Entity entity) {
            entity.PhrasesLeft = 10;
        }

        public void Update(Entity entity) {
            --entity.PhrasesLeft;
        }

        public override IEnumerable<StateTransition<Entity>> GetTransitions() {
            return new StateTransition<Entity>[] {
                StateTransition<Entity>.FromTo( TalkState.Instance, Entity.DoneTalking, IdleState.Instance )
            };
        }

    }

}
