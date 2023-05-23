#region
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;

    // Cached References
    StateMachine stateMachine;
    InputManager input;

    public Rigidbody2D PlayerRB { get; private set; }

    void Start()
    {
        stateMachine = FindObjectOfType<StateMachine>();
        input        = GetComponent<InputManager>();
        PlayerRB     = GetComponent<Rigidbody2D>();

        // Set the default state.
        stateMachine.HandleStateChange(State.StateType.None);
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundDistance);
    }
}