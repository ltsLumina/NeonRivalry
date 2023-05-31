using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        // Get a reference to the StateMachine component.
        Instance = GetComponent<StateMachine>();

        player = FindObjectOfType<PlayerController>();

        // Set the default state.
        HandleStateChange(State.StateType.None);
    }

    /// <summary>
    ///     Sets the state of the state machine.
    ///     <remarks> If you want to change the state, use HandleStateChange() instead. </remarks>
    /// </summary>
    /// <param name="newState">The new state we transition into.</param>
    /// <seealso cref="HandleStateChange(State.StateType)"/>
    void SetState(State newState)
    {
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState?.OnEnter();
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
    /// <param name="stateAction"> A optional action to pass in to each state. </param>
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

            case State.StateType.Fall:
                CheckStateDataAndExecute(stateData.fallStateData, data => SetState(new FallState(player, data)));
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

            case State.StateType.None: // The None state uses the defaultStateData which is primarily used for debugging. (It's a template for new state data)
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
        EditorGUILayout.LabelField("Current State", stateMachine.CurrentState?.GetType().Name);

        EditorUtility.SetDirty(stateMachine);
    }
}
#endif