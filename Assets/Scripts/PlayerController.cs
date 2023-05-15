#region
using UnityEngine;
#endregion

/// <summary>
/// THIS IS NOT FINAL, I AM TESTING. NONE OF THIS WORKS OR IS MEANT TO WORK.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Cached References
    StateMachine stateMachine;
    InputManager input;

    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;

    public Rigidbody2D PlayerRB { get; private set; }

    void Start()
    {
        stateMachine = FindObjectOfType<StateMachine>();
        stateMachine.SetState(new IdleState());
        PlayerRB = GetComponent<Rigidbody2D>();
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