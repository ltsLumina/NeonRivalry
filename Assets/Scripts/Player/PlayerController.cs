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

    // Cached References
    Animator anim;

    public Rigidbody Rigidbody { get; private set; }
    public InputManager InputManager { get; private set; }
    public StateMachine StateMachine { get; private set; }

    void Awake()
    {
        StateMachine = GetComponent<StateMachine>();
        Rigidbody    = GetComponent<Rigidbody>();
        InputManager = GetComponentInChildren<InputManager>();
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
                StateMachine.TransitionToState(StateType.Idle);
            }
        }
        else { idleTime = 0; }
    }
}