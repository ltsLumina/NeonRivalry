#region
using System;
using UnityEngine;
using static Lumina.Essentials.Attributes;
using static State;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(InputManager))]
public partial class PlayerController : MonoBehaviour
{
    [Header("Read-Only Fields")]
    [SerializeField] float idleTimeThreshold;
    [SerializeField, ReadOnly] float idleTime;

    [Header("Ground Check")]
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    // Cached References
    StateMachine stateMachine;
    Animator anim;
    
    public event Action OnEnterIdleState;

    public Rigidbody2D PlayerRB { get; private set; }
    public InputManager InputManager { get; private set; }

    void Awake()
    {
        OnEnterIdleState += () => stateMachine.TransitionToState(StateType.Idle);
    }

    void Start()
    {
        stateMachine = StateMachine.Instance;
        PlayerRB     = GetComponent<Rigidbody2D>();
        InputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        idleTime += Time.deltaTime;

        CheckIdle();
    }

    // -- State Checks --
    // Related: StateChecks.cs

    void CheckIdle()
    {
        // Check if the player is idle.
        if (IsIdle())
        {
            // If the player has been idle for longer than the threshold, trigger the idle state.
            if (idleTime > idleTimeThreshold)
            {
                OnEnterIdleState?.Invoke();
                idleTime = 0;
            }
        }
        else { idleTime = 0; }
    }

    // -- Gizmos --

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, Vector3.one * groundDistance);
    }
}