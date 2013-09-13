

namespace StateMachine {

    public interface IStateful<T> {

        StateMachine<T> GetStateMachine();

    }

}
