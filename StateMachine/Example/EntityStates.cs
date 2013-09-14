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
            OnEnter = null;
            OnUpdate = Update;
            OnExit = null;
        }

        private void Update(Entity entity) {
            // Look for target
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
            OnEnter = null;
            OnUpdate = (entity) => entity.Move( 2 );
            OnExit = null;
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
            OnEnter = (entity) => entity.PhrasesLeft = 10;
            OnUpdate = (entity) => --entity.PhrasesLeft;
            OnExit = null;
        }

    }

}
