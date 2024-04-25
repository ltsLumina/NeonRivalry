/// <summary>
/// The base class for all states in the game.
/// </summary>
public abstract class State
{
    // Cached References
    protected readonly PlayerController player;

    // -- State Related -- //
    
    // Constructor to set the player reference.
    protected State(PlayerController player)
    {
        this.player = player;
    }
    
    // -- Properties --
    
    // StateType is used to indicate the type of state that the player is in.
    public enum StateType
    {
        // The values of the enum are used to determine the priority of the state.
        // They should all be unique, and the higher the value, the higher the priority.
        Idle,
        Walk,
        Jump,
        Fall,
        Attack,
        AirborneAttack,
        Dash,
        Block,
        HitStun,    // HitStun indicates that the player has been hit and is unable to move or attack for a short period of time.
        Knockdown,  // Knockdown indicates that the player has been knocked down and is unable to move or attack for a short period of time.
        Dead,       // Dead indicates that the player has died and is unable to move or attack. This takes priority over all other states and should always be the highest priority.
        None,       // None is a special state that is used to indicate that there is no player, and therefore, no state.
    }
    
    // -- State Methods --

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public abstract void OnEnter();
    
    /// <summary>
    /// Called to update the state's logic.
    /// Run any logic that needs to be run every frame in this method.
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    ///     Called when the state is exited.
    ///     <remarks> Make sure to always run this method when exiting a state. </remarks>
    /// </summary>
    public abstract void OnExit();
}