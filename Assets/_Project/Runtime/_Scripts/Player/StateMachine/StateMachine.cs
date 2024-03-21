#region
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static State;
using static State.StateType;
#if UNITY_EDITOR
using static UnityEditor.EditorGUILayout;
#endif
#endregion

/// <summary>
/// The state machine that handles the player's state.
/// Allows for easy state transitions and state management.
/// </summary>
[RequireComponent(typeof(PlayerController)), DefaultExecutionOrder(-300)]
public class StateMachine : MonoBehaviour
{
    [SerializeField] StateMachineData stateData;
    [SerializeField] PlayerController player;

    // Cached References
    bool isTransitioning;
    
    public PlayerController Player
    {
        get => player;
        private set => player = value;
    }

    // -- State Related --
    public State CurrentState { get; private set; }

    void Awake()
    {
        Player = GetComponent<PlayerController>();

        // Set the default state.
        TransitionToState(None);
    }

    /// <summary>
    ///     Sets the state of the state machine.
    ///     <remarks> If you want to change the state, use TransitionToState() instead. </remarks>
    /// </summary>
    /// <param name="newState">The new state we transition into.</param>
    /// <seealso cref="TransitionToState"/>
    void SetState(State newState)
    {
        if (newState == null)
        {
            Debug.LogError("The new state is null. Please assign a valid state.");
            return;
        }
        
        // Checks if the current state is null, or if the new state has a higher priority than the current state.
        // If the new state has a lower or equal priority, the current state is entered like normal.
        if (CurrentState != null && newState.Priority > CurrentState.Priority)
        {
            // If the current state can be interrupted, we exit the current state.
            CurrentState?.OnExit();
        }
        
        // Set the new state and enter it.
        CurrentState = newState;

        //TODO: dont like this. find a better way to do this.
        Player.IdleTime = 0;
        
        CurrentState?.OnEnter();
    }

    // Runs the current state's update method. (Fixed interval of 60 calls per second)
    public void FixedUpdate()
    {
        if (!isTransitioning) CurrentState?.UpdateState();
    }

    /// <summary>
    /// Handles the transition between states.
    /// </summary>
    /// <param name="state"> The state to transition into. </param>
    public void TransitionToState(StateType state)
    {
        if (isTransitioning) return;
        
        isTransitioning = true;
        
        // Do NOT run any other code than the CheckStateDataThenExecute() method in this switch statement.
        switch (state)
        {
            // Note: The 'when X condition' checks that we perform on each case seem to be redundant.
            
            case Idle:
                SetState(new IdleState(Player)); //TODO: Add state data, potentially. (Such as idleTimeThreshold. Currently handled in the player controller.)
                break;
            
            case Walk when Player.IsGrounded():
                CheckStateDataThenExecute(stateData.moveStateData, data => SetState(new MoveState(Player, data)));
                break;
            
            case Jump when Player.CanJump():
                CheckStateDataThenExecute(stateData.jumpStateData, data => SetState(new JumpState(Player, data)));
                break;

            case Fall when Player.CanFall():
                CheckStateDataThenExecute(stateData.fallStateData, data => SetState(new FallState(Player, data)));
                break;

            case Attack when Player.CanAttack():
                CheckStateDataThenExecute(stateData.attackStateData, data => SetState(new AttackState(Player, data)));
                break;
            
            case AirborneAttack when Player.CanAirborneAttack():
                CheckStateDataThenExecute(stateData.airborneAttackStateData, data => SetState(new AirborneAttackState(Player, data)));
                break;

            // - Unused States -
            case Knockdown:
                break;

            case Dead:
                break;

            case Block:
                break;

            case HitStun:
                break;

            // -- Default State --

            case None: // The None state uses the defaultStateData which is primarily used for debugging. (It's a template for new state data)
                SetState(new NoneState(Player));
                break;

            // If you wish to add more states, make sure to run the CheckStateDataThenExecute method like all the other states.

            default:
                SetState(new NoneState(Player));
                throw new ArgumentOutOfRangeException
                    (nameof(state), state, "Fatal Error! "                                              +
                                           "The state you are trying to transition to does not exist! " +
                                           "\nLikely, the player got stuck and tried to transition to a state but was unable to.");
        }
        
        isTransitioning = false;
    }

    // I totally wrote this method myself and didn't copy it from the internet.
    // Checks if the state data is null or default, and if it is, it throws an error.
    // ReSharper disable Unity.PerformanceAnalysis
    static void CheckStateDataThenExecute<T>(T stateData, Action<T> executeCode)
    {
        if (EqualityComparer<T>.Default.Equals(stateData, default))
            Debug.LogError(
                $"The state data of type {typeof(T)} is null or default. " +
                           "Please assign the correct data in the inspector via the 'Systems' prefab.");
        else executeCode(stateData);
    }
}

/// <summary>
/// A struct that holds all the state data for the state machine.
/// State data is used to pass data from the state machine to the state.
/// </summary>
[Serializable] 
internal struct StateMachineData
{
    public MoveStateData moveStateData;
    public JumpStateData jumpStateData;
    public FallStateData fallStateData;
    public AttackStateData attackStateData;
    public AirborneAttackStateData airborneAttackStateData;
}

#if UNITY_EDITOR
/// <summary>
/// A custom editor for the state machine.
/// Displays the current state of the state machine in the inspector under the StateMachine component in the Systems prefab.
/// (Or under DontDestroyOnLoad > Systems > StateMachine in the hierarchy during runtime.)
/// </summary>
[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor
{
    // custom editor that displays the current state of the state machine in the inspector
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var stateMachine = (StateMachine) target;
        var player       =  stateMachine.Player;
        if (!player.Rigidbody) return;
        
        // Each bool represents a state. If the bool is true, then the state is active.
        // Used to display the state values in the inspector.
        Dictionary<string, bool> states = new ()
        {
            {"IsGrounded", player.IsGrounded()},
            {"IsMoving", player.IsMoving()},
            {"IsJumping", player.IsJumping()},
            {"IsFalling", player.IsFalling()},
            {"IsAttacking", player.IsAttacking()},
            {"IsAirborneAttacking", player.IsAirborneAttacking()}
        };

        LabelField("Current State", stateMachine.CurrentState?.GetType().Name);
        
        Space();
        
        LabelField("State Booleans", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(true))
        {
            foreach (var state in states)
            {
                var label = new GUIContent(state.Key, "Shows the StateChecks.cs value for the state, not the state machine CurrentState value.");
                
                Toggle(label, state.Value);
            }
        }

        // Update the inspector if the application is playing to view live results.
        if (Application.isPlaying) EditorUtility.SetDirty(stateMachine);
    }
}
#endif