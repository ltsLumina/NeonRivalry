#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using static State;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public partial class PlayerController : MonoBehaviour
{
    [Header("Read-Only Fields")]
    [SerializeField] float idleTimeThreshold;
    [SerializeField, ReadOnly] public float idleTime;

    [Header("Ground Check"), Tooltip("The minimum distance the raycast must be from the ground."), SerializeField, ReadOnly]
     float raycastDistance = 1.022f; //With the capsule collider, this is the minimum distance between player and ground.
    [SerializeField] LayerMask groundLayer;

    [Header("Player Input"), SerializeField]
     int playerIndex;

    // Cached References
    StateMachine stateMachine;
    Animator anim;

    public Rigidbody Rigidbody { get; private set; }
    public InputManager InputManager { get; private set; }
    public int PlayerIndex
    {
        get => playerIndex;
        set => playerIndex = value;
    }

    void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
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