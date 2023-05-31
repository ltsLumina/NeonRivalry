#region
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;

    // Cached References
    StateMachine stateMachine;

    public Rigidbody2D PlayerRB { get; private set; }
    public InputManager InputManager { get; private set; }

    void Start()
    {
        stateMachine = StateMachine.Instance;
        PlayerRB     = GetComponent<Rigidbody2D>();
        InputManager = GetComponent<InputManager>();
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