using UnityEngine;

public class MoveState : State
{
    PlayerController player;
    Rigidbody2D playerRB;
    float speed;

    public MoveState(MoveStateData data)
    {
        // Initialize any properties or fields that require data

        // Access the data from the Scriptable Object
        player   = data.Player;
        playerRB = data.PlayerRB;
        speed    = data.moveSpeed;
    }

    public override void OnEnter()
    {
        // Play the move animation.
    }

    public override void UpdateState()
    {
        //Handle move logic, such as checking for input, etc.
       if (player.MoveInput != Vector2.zero)
           playerRB.MovePosition(playerRB.position + player.MoveInput * (speed * Time.fixedDeltaTime));
       else
           OnExit();
    }

    public override void OnExit()
    {
        // Perform any necessary cleanup or exit actions
    }
}