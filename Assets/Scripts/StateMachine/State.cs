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
        Attack,
        Block,
        HitStun,    // HitStun indicates that the player has been hit and is unable to move or attack for a short period of time.
        Knockdown,  // Knockdown indicates that the player has been knocked down and is unable to move or attack for a short period of time.
        Dead,       // Dead indicates that the player has died and is unable to move or attack. This takes priority over all other states and should always be the highest priority.
        None,       // None is a special state that is used to indicate that there is no player, and therefore, no state.
    }

    // This dictionary is used to determine the priority of each state.
    // The higher the value, the higher the priority.
    protected readonly Dictionary<StateType, int> statePriorities = new()
    {
    //TODO: Adjust the system to allow for states with the same priority, allowing for more complex state transitions.
    { Idle, 1 },
    { Walk, 2 },
    { Run, 3 },
    { Jump, 4 },
    { Attack, 5 },
    { Block, 6 },
    { HitStun, 7 },
    { Knockdown, 8 },
    { Dead, 99 },
    { None, 0 }
    };

    /// <summary>
    /// The type of the state.
    /// </summary>
    public abstract StateType Type { get; }

    /// <summary>
    /// The priority of the state. The higher the value, the higher the priority.
    /// </summary>
    public abstract int Priority { get; }

    /// <summary>
    /// Indicates whether the state has been interrupted.
    /// </summary>
    public abstract bool Interrupted { get; set; }

    // -- State Machine Methods --

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public abstract void OnExit();

    /// <summary>
    /// Called to update the state's logic.
    /// Run any logic that needs to be run every frame in this method.
    /// </summary>
    public abstract void UpdateState();

    // -- State Transitions --

    /// <summary>
    ///     Called when the state is interrupted by a higher-priority state.
    ///     For example, if the player is in the Jump state, and then the player is hit, the Jump state will be interrupted by
    ///     the HitStun state.
    ///     This means that we run the OnInterrupt() method of the Jump state to do interrupt actions, and then the OnEnter()
    ///     method of the HitStun state.
    /// </summary>
    public abstract void OnInterrupt();
}