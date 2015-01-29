StateMachine
============
Implementation of classic game state machine, with transitions isolated from
the states methods.

Description
-----------
The idea is to have the transitions defined separately from the states'
behaviour. This should help visualize and verify the state diagram by simply looking at
the transitions added to the state machine.

Example
-------
Creating and configuring a state machine (copied from the example Entity),
starting on IdleState:

```C#
stateMachine = new StateMachine<Entity>( this, IdleState.Instance )
    .AddTransitions(
        Transition.FromAny<Entity>()
                  .Except( IdleState.Instance, TalkState.Instance )
                  .To( IdleState.Instance )
                  .When( HasNoTarget ),
        Transition.From( IdleState.Instance )
                  .To( walkState )
                  .When( HasTarget ),
        Transition.From( walkState )
                  .To( TalkState.Instance )
                  .When( IsTargetInRange ),
        Transition.From( TalkState.Instance )
                  .To( IdleState.Instance )
                  .When( DoneTalking ) );
```


Defining a state on the fly:

```C#
State<Entity> walkState = State<Entity>.Build(
    null,
    (entity) => entity.Move( 2 ),
    null );
```

Defining the states as classic singletons:

```C#
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
```