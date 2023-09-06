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
public class StateMachine : SingletonPersistent<StateMachine>
{
    [SerializeField] public StateData stateData;

    // Cached References
    //TODO: Make this private. It is used to reference the player from the editor for debugging purposes.
    public PlayerController Player;

    // -- State Related --
    public State CurrentState { get; private set; }
    
    void Start()
    {
        Player = FindObjectOfType<PlayerController>();

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
        // Checks if the current state is null, or if the new state has a higher priority than the current state.
        // If the new state has a lower or equal priority, the current state is entered like normal.
        if (CurrentState != null && newState.Priority > CurrentState.Priority && CurrentState.CanBeInterrupted())
        {
            // If the current state can be interrupted, we exit the current state.
            CurrentState?.OnExit();
        }
        
        // Set the new state and enter it.
        CurrentState = newState;

        //TODO: dont like this. find a better way to do this.
        Player.idleTime = 0;
        
        CurrentState?.OnEnter();
    }

    // Runs the current state's update method.
    public void Update() => CurrentState?.UpdateState();

    /// <summary>
    /// Handles the transition between states.
    /// </summary>
    /// <param name="stateType"> The state to transition into. </param>
    public void TransitionToState(StateType stateType)
    {
        // Do NOT run any other code than the CheckStateDataThenExecute() method in this switch statement.
        switch (stateType)
        {
            case Idle:
                SetState(new IdleState(Player)); //TODO: Add state data, potentially. (Such as idleTimeThreshold. Currently handled in the player controller.)
                break;

            case Walk when Player.IsGrounded():
                CheckStateDataThenExecute(stateData.moveStateData, data => SetState(new MoveState(Player, data)));
                break;

            case Walk when !Player.IsGrounded():
                // Fallback state for whenever the player tries to move after jumping or attacking so they can keep moving after landing or finishing an attack.
                break;
            
            case Jump when Player.CanJump():
                CheckStateDataThenExecute(stateData.jumpStateData, data => SetState(new JumpState(Player, data)));
                break;

            case Fall:
                CheckStateDataThenExecute(stateData.fallStateData, data => SetState(new FallState(Player, data)));
                break;

            case Attack:
                CheckStateDataThenExecute(stateData.attackStateData, data => SetState(new AttackState(Player, data)));
                break;

            // - Unused States -
            case Knockdown:
                break;

            case Dead:
                break;

            case Run:
                Debug.Log("RUNNING");
                // technically unused (only used for debugging)
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
                throw new ArgumentOutOfRangeException(nameof(stateType), stateType, "The state type is not valid.");
        }
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
public struct StateData
{
    public MoveStateData moveStateData;
    public JumpStateData jumpStateData;
    public FallStateData fallStateData;
    public AttackStateData attackStateData;
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

        LabelField("Current State", stateMachine.CurrentState?.GetType().Name);
        
            Space();
        
        // For debugging purposes, to see if the related bool is true or false, even if the state is not the current state.
        LabelField("State Booleans", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(true))
        {
            Toggle("IsGrounded", player.IsGrounded());
            Toggle("IsMoving", stateMachine.CurrentState is MoveState {IsMoving: true });
            Toggle("IsJumping", stateMachine.CurrentState is JumpState {IsJumping: true });
            Toggle("IsFalling", stateMachine.CurrentState is FallState {IsFalling: true });
            Toggle("IsAttacking", stateMachine.CurrentState is AttackState {IsAttacking: true });
        }

        EditorUtility.SetDirty(stateMachine);
    }
}
#endif