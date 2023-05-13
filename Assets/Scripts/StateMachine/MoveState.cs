using UnityEngine;

public class MoveState : StateMachine.IState
{
    public PlayerController Player { get; }
    public Rigidbody2D PlayerRB { get; }

    float speed;

    public MoveState(PlayerController player, Rigidbody2D playerRB, float moveSpeed)
    {
        Player        = player;
        PlayerRB      = playerRB;
        speed         = moveSpeed;
    }

    public void OnEnter()
    {
        // Play the move animation.
        Debug.Log("Current State: " + GetType().Name);
    }

    public void UpdateState()
    {
       // Handle move logic, such as checking for input, etc.
       if (Player.MoveInput != Vector2.zero)
           PlayerRB.MovePosition(PlayerRB.position + Player.MoveInput * (speed * Time.fixedDeltaTime));
       else
           OnExit();
    }

    public void OnExit()
    {
        // Perform any necessary cleanup or exit actions
    }
}