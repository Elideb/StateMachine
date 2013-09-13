StateMachine
============

Implementation of classic game state machine, with transitions isolated from
the states methods.

Description
-----------

The idea is to have the transitions defined separately from the states'
behaviour.

This should help visualize and verify the state diagram by simply looking at
the transitions. These can be defined when constructing the state machine
itself or from quarying the states for their transitions (I defined in them the
exit transitions and the bulk entry transitions, like exceptions).

Check the example to get an idea of how to implement your one states and use it.
