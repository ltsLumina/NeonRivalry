#region
using UnityEngine;
using Logger = Lumina.Debugging.Logger;
#endregion

public class AirborneAttackState : State
{
    readonly Animator animator;
    readonly AttackHandler attackHandler;
    readonly Moveset moveset;

    float timeAirborneAttacking;
    float timeAirborne;
    
    readonly float requiredAirTime;
    
    public AirborneAttackState(PlayerController player, AirborneAttackStateData stateData) : base(player)
    {
        animator = player.GetComponentInChildren<Animator>();

        // Falling variables
        fallGravityMultiplier = stateData.FallGravityMultiplier;
        jumpHaltForce         = stateData.JumpHaltForce;
        
        // Attack variables
        timeAirborne    = player.AirborneTime;
        requiredAirTime = stateData.RequiredAirTime;
        moveset = player.Character.Moveset;
        
        // Create a new instance of the attack handler.
        attackHandler = new (moveset, animator, player);
    }

    public bool IsAirborneAttacking { get; private set; }
    public bool IsAirborne { get; private set; }

    public override StateType Type => StateType.AirborneAttack;
    public override int Priority => statePriorities[Type];

    // -- State Specific Variables --
    readonly float fallGravityMultiplier;
    readonly float jumpHaltForce;

    #region State Methods
    public override void OnEnter()
    {
        IsAirborneAttacking = false;
        IsAirborne          = true; // Check whether the player was in air when attack started.
        
        player.GetComponentInChildren<SpriteRenderer>().color = new (1f, 0.21f, 0.38f, 0.75f);
        
        timeAirborneAttacking = 0;
    }

    public override void UpdateState()
    {   
        if (!IsAirborne)
        {
            OnExit();
            return;
        }

        // Only attack if the player has been airborne for a certain amount of time.
        if (timeAirborne >= requiredAirTime && !IsAirborneAttacking && !player.HasAirborneAttacked)
        {
            IsAirborneAttacking = true;
            player.HasAirborneAttacked = true;
        }

        if (!IsAirborneAttacking)
        {
            player.StateMachine.TransitionToState(StateType.Fall);
            return;
        }
        
        PerformAttack();

        // If the attack animation is still playing, run logic.
        if (timeAirborneAttacking < animator.GetCurrentAnimatorStateInfo(0).length)
        {
            timeAirborneAttacking += Time.fixedDeltaTime;
                
            // Perform the airborne attack logic.
            if (!player.IsGrounded())
            {
                Logger.Debug("Attacking in the air!", LogType.Log, StateType.AirborneAttack);
                
                // Stop the player once they attack in the air, if the moveData says so.
                //player.Rigidbody.velocity = new Vector2(player.Rigidbody.velocity.x, 0);

                // Apply gravity
                if (player.Rigidbody.velocity.y < 0) player.Rigidbody.AddForce(fallGravityMultiplier * Vector3.down, ForceMode.Acceleration);

                // Apply jump halt force
                if (player.Rigidbody.velocity.y > 0) player.Rigidbody.AddForce(jumpHaltForce * Vector3.down);

                player.Animator.SetBool("IsFalling", true);
            }
            // If the player lands, cancel the attack.
            else
            {
                player.GlobalGravity = player.DefaultGravity;
                player.GravityScale  = 1;
                OnExit();
            }
        }
        else { OnExit(); }
    }

    public override void OnExit()
    {
        // Cancel the attack animation by starting the idle animation.
        //animator.Play("Idle");
        
        IsAirborneAttacking = false;
        IsAirborne          = false;

        player.StateMachine.TransitionToState(player.IsGrounded() ? StateType.Idle : StateType.Fall);

        if (player.IsGrounded() && player.InputManager.MoveInput.x != 0) player.StateMachine.TransitionToState(StateType.Walk);
        else if (player.IsGrounded()) player.StateMachine.TransitionToState(StateType.Idle);

        // Play land animation.
        player.Animator.SetBool("IsFalling", false);

        if (player.IsGrounded()) player.HasAirborneAttacked = false;
    }
    #endregion

    void PerformAttack()
    {
        // Select the attack type.
        InputManager.AttackType attackType = player.InputManager.LastAttackPressed;

        // If the attack type is not None, select the attack.
        if (attackType == InputManager.AttackType.Airborne)
        {
            attackHandler.SelectAttack(attackType);
            player.InputManager.LastAttackPressed = InputManager.AttackType.None; // Reset after usage
        }
        else { Logger.Debug("No \"(AttackType.None)\" attack type was selected. Something went wrong.", LogType.Error); }
    }
}