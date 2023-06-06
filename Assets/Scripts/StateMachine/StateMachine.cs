using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static State;
using static State.StateType;
using static UnityEditor.EditorGUILayout;

/// <summary>
/// The state machine that handles the player's state.
/// Allows for easy state transitions and state management.
/// </summary>
public class StateMachine : MonoBehaviour
{
    [SerializeField] public StateData stateData;

    // Cached References
    //TODO: I am considering using a different approach than using a singleton reference for all states to access the player.
    PlayerController player;

    // -- State Related --
    public static StateMachine Instance { get; private set; }
    public State CurrentState { get; private set; }


    void Start()
    {
        // Get a reference to the StateMachine.
        Instance = this;

        player = FindObjectOfType<PlayerController>();

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
            //Debug.Log("Interrupting current state and exiting!");
            CurrentState?.OnExit();
        }

        //Debug.Log("Entering: " + newState);
        CurrentState = newState;
        CurrentState?.OnEnter();
    }

    // Runs the current state's update method, allowing for the state to run its logic.
    public void Update() => CurrentState?.UpdateState();

    /// <summary>
    /// Handles changing the state of the state machine.
    /// </summary>
    /// <param name="stateType"> The state to transition into. </param>
    public void TransitionToState(StateType stateType)
    {
        // Do NOT run any other code than the CheckStateDataAndExecute() method in this switch statement.
        // Handle any other logic in the state itself.
        switch (stateType)
        {
            case Idle:
                SetState(new IdleState(player)); //TODO: Add state data, potentially. (Such as idleTimeThreshold)
                break;

            case Walk:
                CheckStateDataAndExecute(stateData.moveStateData, data => SetState(new MoveState(player, data)));
                break;

            case Jump:
                CheckStateDataAndExecute(stateData.jumpStateData, data => SetState(new JumpState(player, data)));
                break;

            case Fall:
                CheckStateDataAndExecute(stateData.fallStateData, data => SetState(new FallState(player, data)));
                break;

            case Attack:
                CheckStateDataAndExecute(stateData.attackStateData, data => SetState(new AttackState(player, data)));
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
                SetState(new NoneState(player));
                break;

            // If you wish to add more states, make sure to run the CheckStateDataAndExecute method like all the other states.

            default:
                throw new ArgumentOutOfRangeException(nameof(stateType), stateType, "The state type is not valid.");
        }
    }

    // I totally wrote this method myself and didn't copy it from the internet.
    // Checks if the state data is null or default, and if it is, it throws an error.
    // ReSharper disable Unity.PerformanceAnalysis
    static void CheckStateDataAndExecute<T>(T stateData, Action<T> executeCode)
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

        LabelField("Current State", stateMachine.CurrentState?.GetType().Name);
        
            Space();
        
        // For debugging purposes, to see if the related bool is true or false, even if the state is not the current state.
        LabelField("State Booleans", EditorStyles.boldLabel);
        LabelField("IsMoving: ", stateMachine.CurrentState is MoveState {IsMoving: true } ? "True" : "False"); 
        LabelField("IsJumping: ", stateMachine.CurrentState is JumpState {IsJumping: true } ? "True" : "False");
        LabelField("IsFalling: ", stateMachine.CurrentState is FallState {IsFalling: true } ? "True" : "False");
        LabelField("IsAttacking: ", stateMachine.CurrentState is AttackState {IsAttacking: true } ? "True" : "False");

        EditorUtility.SetDirty(stateMachine);
    }
}
#endif