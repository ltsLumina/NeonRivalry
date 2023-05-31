using System.Collections.Generic;
using static State.StateType;

/// <summary>
/// The base class for all states in the game.
/// </summary>
public abstract class State
{
    protected readonly PlayerController player;

    protected State(PlayerController player)
    {
        this.player = player;
    }

    // StateType is used to indicate the type of state that the player is in.
    public enum StateType
    {
        // The values of the enum are used to determine the priority of the state.
        // They should all be unique, and the higher the value, the higher the priority.
        Idle,
        Walk,       // Walk indicates that the player is moving.
        Run,        // Run indicates that the player is moving faster than walking.
        Jump,
        Fall,
        Attack,
        Block,
        HitStun,    // HitStun indicates that the player has been hit and is unable to move or attack for a short period of time.
        Knockdown,  // Knockdown indicates that the player has been knocked down and is unable to move or attack for a short period of time.
        Dead,       // Dead indicates that the player has died and is unable to move or attack. This takes priority over all other states and should always be the highest priority.
        None,       // None is a special state that is used to indicate that there is no player, and therefore, no state.
    }

    // This dictionary is used to determine the priority of each state.
    protected readonly Dictionary<StateType, int> statePriorities = new()
    { //TODO: Adjust the system to allow for states with the same priority, allowing for more complex state transitions.
      { Idle, 1 },
      { Walk, 2 },
      { Run, 3 },
      { Jump, 4 },
      { Fall, 5},
      { Attack, 6 },
      { Block, 7 },
      { HitStun, 8 },
      { Knockdown, 10 },
      { Dead, 99 },
      { None, 100 }
    };

    /// <summary>
    /// The type of the state.
    /// </summary>
    public abstract StateType Type { get; }

    /// <summary>
    /// The priority of the state. The higher the value, the higher the priority.
    /// //TODO: Implement this again at a later date.
    /// </summary>
    public abstract int Priority { get; }

    // -- State Machine Methods --

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    ///     Called when the state is exited.
    ///     <remarks> Make sure to always run this method when exiting a state. </remarks>
    /// </summary>
    public abstract void OnExit();

    /// <summary>
    /// Called to update the state's logic.
    /// Run any logic that needs to be run every frame in this method.
    /// </summary>
    public abstract void UpdateState();
}