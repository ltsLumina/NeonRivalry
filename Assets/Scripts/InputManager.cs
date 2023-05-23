using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static Essentials.Attributes;

public class InputManager : MonoBehaviour
{
    [Header("Read-Only Fields"), SerializeField, ReadOnly]
    Vector2 moveInput;

    PlayerController player;
    Rigidbody2D playerRB;
    StateMachine stateMachine;
    public InputAction action;
    bool isRunning;

    public Vector2 MoveInput
    {
        get => moveInput;
        private set => moveInput = value;
    }

    void OnEnable() => action.Enable();
    void OnDisable() => action.Disable();

    void Start()
    {
        player       = FindObjectOfType<PlayerController>();
        playerRB     = player.GetComponent<Rigidbody2D>();
        stateMachine = FindObjectOfType<StateMachine>();
    }

    void Update()
    {
        if (action.triggered)
        {
            isRunning = true;
            Debug.Log($"isRunning: {isRunning}");
        }
    }

    void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();

        if (player.IsGrounded())
        {
            // Regular Walk State
            stateMachine.HandleStateChange(State.StateType.Walk);
        }

        if (player.IsGrounded() && isRunning)
        {
            // Running State
            stateMachine.HandleStateChange(State.StateType.Run);
        }
    }

    void OnJump(InputValue value)
    {
        stateMachine.HandleStateChange(State.StateType.Jump);
    }

    void OnAttack(InputValue value)
    {
        stateMachine.HandleStateChange(State.StateType.Attack);
    }
}