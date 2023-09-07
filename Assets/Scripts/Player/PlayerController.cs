#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using static State;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(InputManager))]
public partial class PlayerController : MonoBehaviour
{
    [Header("Read-Only Fields")]
    [SerializeField] float idleTimeThreshold;
    [SerializeField, ReadOnly] public float idleTime;

    [Header("Ground Check")]
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    // Cached References
    StateMachine stateMachine;
    Animator anim;

    public Rigidbody Rigidbody { get; private set; }
    public InputManager InputManager { get; private set; }

    void Start()
    {
        stateMachine = StateMachine.Instance;
        Rigidbody    = GetComponent<Rigidbody>();
        InputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        CheckIdle();
    }

    // -- State Checks --
    // Related: StateChecks.cs

    void CheckIdle()
    {
        // Check if the player is idle.
        if (IsIdle())
        {
            // If the player is idle, we add to the idle time.
            idleTime += Time.deltaTime;

            // If the idle time is greater than the threshold, we transition to the idle state.
            if (idleTime >= idleTimeThreshold)
            {
                stateMachine.TransitionToState(StateType.Idle);
            }
        }
        else { idleTime = 0; }
    }
}