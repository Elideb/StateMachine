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
