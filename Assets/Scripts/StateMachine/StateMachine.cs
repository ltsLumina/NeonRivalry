using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] public StateData stateData;

    // Cached References
    public PlayerController Player { get; private set; }
    public Rigidbody2D PlayerRB { get; private set; }
    public InputManager PlayerInput { get; private set; }

    public State CurrentState { get; private set; }

    void Start()
    {
        Player      = FindObjectOfType<PlayerController>();
        PlayerRB    = Player.GetComponent<Rigidbody2D>();
        PlayerInput = FindObjectOfType<InputManager>();
    }

    public void SetState(State newState)
    {
        CurrentState?.OnExit(this);
        CurrentState = newState;
        CurrentState.OnEnter(this);
    }

    public void Update()
    {
        CurrentState?.UpdateState(this);
    }

    /// <summary>
    /// Handles changing the state of the state machine.
    /// </summary>
    /// <param name="stateType"> The state to transition into. </param>
    public void HandleStateChange(State.StateType stateType)
    {
        switch (stateType)
        {
            case State.StateType.Idle:
                SetState(new IdleState());
                Debug.Log("Idle");
                break;

            case State.StateType.Walk:
                CheckStateDataAndExecute(stateData.moveStateData, data => SetState(new MoveState(data)));
                break;

            case State.StateType.Jump:
                CheckStateDataAndExecute(stateData.jumpStateData, data => SetState(new JumpState(data)));
                break;

            case State.StateType.Attack:
                CheckStateDataAndExecute(stateData.attackStateData, data => SetState(new AttackState(data)));
                break;

            case State.StateType.Knockdown:
                break;

            case State.StateType.Dead:
                break;

            // - Unused States -
            case State.StateType.Run:
                Debug.Log("RUNNING");
                break;

            case State.StateType.Block:
            case State.StateType.HitStun:

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
    static void CheckStateDataAndExecute<T>(T stateData, Action<T> executeCode)
    {
        if (EqualityComparer<T>.Default.Equals(stateData, default))
            Debug.LogError(
                $"The state data of type {typeof(T)} is null or default. " +
                           "Please assign the correct data in the inspector via the 'Systems' prefab.");
        else executeCode(stateData);
    }

    bool CheckIdle()
    {
        return PlayerInput.MoveInput == Vector2.zero && PlayerRB.velocity == Vector2.zero && Player.IsGrounded();
    }

    public void EnterIdleState()
    {
        StartCoroutine(EnterIdle());
    }

    IEnumerator EnterIdle()
    {
        if (CheckIdle())
        {
            yield return new WaitForSeconds(0.5f);

            if (CheckIdle())
            {
                HandleStateChange(State.StateType.Idle);
            }
        }
    }
}

[Serializable]
public struct StateData
{
    public MoveStateData moveStateData;
    public JumpStateData jumpStateData;
    public AttackStateData attackStateData;
}

#if UNITY_EDITOR
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