using System.Collections.Generic;

namespace StateMachine.Example {

    static class EntityStates {

        #region Idle state

        private static State<Entity> idle = null;
        public static State<Entity> Idle {
            get {
                if (idle == null) {
                    idle = State<Entity>.Build(
                        null,
                        IdleUpdate,
                        null );
                }

                return idle;
            }
        }

        private static void IdleUpdate(Entity ett) {
            // Look for target
        }

        #endregion

        #region Walk state

        private static State<Entity> walk = null;
        public static State<Entity> Walk {
            get {
                if (walk == null) {
                    walk = State<Entity>.Build(
                        null,
                        (ett) => ett.Move( 2 ),
                        null );
                }

                return walk;
            }
        }

        #endregion

        #region Talk state

        private static State<Entity> talk = null;
        public static State<Entity> Talk {
            get {
                if (talk == null) {
                    talk = State<Entity>.Build(
                        (ett) => ett.PhrasesLeft = 10,
                        (ett) => --ett.PhrasesLeft,
                        null );
                }

                return talk;
            }
        }

        #endregion

    }

}
