using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;

    // Cached References
    public Rigidbody2D PlayerRB { get; private set; }

    void Start() => PlayerRB = GetComponent<Rigidbody2D>();

    void Update()
    {
        Move();
        
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float speed = moveInput * moveSpeed;

        PlayerRB.velocity = new Vector2(speed, PlayerRB.velocity.y);
    }

    void Jump()
    {
        PlayerRB.velocity = new Vector2(PlayerRB.velocity.x, jumpForce);
    }
}