using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] public StateData stateData;

    // Cached References
    //TODO: I am considering using a different approach than using a singleton reference for all states to access the player.what is
    PlayerController player;

    // -- State Related --
    public static StateMachine Instance { get; private set; }
    public State CurrentState { get; private set; }

    void Start()
    {
        // Get a reference to the StateMachine component.
        Instance = GetComponent<StateMachine>();

        player = FindObjectOfType<PlayerController>();

        // Set the default state.
        HandleStateChange(State.StateType.None);
    }

    /// <summary>
    ///     Sets the state of the state machine.
    ///     <remarks>If you want to change the state, use HandleStateChange() instead. </remarks>
    /// </summary>
    /// <param name="newState">The new state we transition into.</param>
    /// <seealso cref="HandleStateChange(State.StateType)"/>
    void SetState(State newState)
    {
        // Checks if the current state is null, or if the new state has a higher priority than the current state.
        // If the new state has a lower or equal priority, the current state is entered like normal.
        // If the new state has a higher priority, the current state is interrupted and the interrupt logic is handled.
        if (CurrentState == null || newState.Priority <= CurrentState.Priority)
        {
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();
        }
        // Checks if the new state has a higher priority than the current state.
        else if (newState.Priority > CurrentState.Priority)
        {
            // Interrupt the current state and handle the interrupt logic.
            CurrentState.Interrupted = true;
            CurrentState?.OnInterrupt();
            CurrentState?.OnExit();

            // Set the new state and enter it.
            CurrentState = newState;
            CurrentState.OnEnter();
        }
    }

    // Runs the current state's update method, allowing for the state to run its logic.
    public void Update()
    {
        CurrentState?.UpdateState();
        Debug.Log(CurrentState);
    }

    /// <summary>
    /// Handles changing the state of the state machine.
    /// </summary>
    /// <param name="stateType"> The state to transition into. </param>
    public void HandleStateChange(State.StateType stateType)
    {
        // Do NOT run any other code than the CheckStateDataAndExecute() method in this switch statement.
        // Handle any other logic in the state itself.
        switch (stateType)
        {
            case State.StateType.Idle:
                SetState(new IdleState(player)); //TODO: Add state data, potentially. (Such as idleTimeThreshold)
                break;

            case State.StateType.Walk:
                CheckStateDataAndExecute(stateData.moveStateData, data => SetState(new MoveState(player, data)));
                break;

            case State.StateType.Jump:
                CheckStateDataAndExecute(stateData.jumpStateData, data => SetState(new JumpState(player, data)));
                break;

            case State.StateType.Attack:
                CheckStateDataAndExecute(stateData.attackStateData, data => SetState(new AttackState(player, data)));
                break;

            // - Unused States -
            case State.StateType.Knockdown:
                break;

            case State.StateType.Dead:
                break;

            case State.StateType.Run:
                Debug.Log("RUNNING");
                // technically unused (only used for debugging)
                break;

            case State.StateType.Block:
                break;

            case State.StateType.HitStun:
                break;

            // -- Default State --

            case State.StateType.None:
                Debug.Log(
                    "'None' state selected. This state is used when there is no state to transition to, or there is no player.");
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
        EditorGUILayout.LabelField("Current State", stateMachine.CurrentState?.GetType().Name);

        EditorUtility.SetDirty(stateMachine);
    }
}
#endif