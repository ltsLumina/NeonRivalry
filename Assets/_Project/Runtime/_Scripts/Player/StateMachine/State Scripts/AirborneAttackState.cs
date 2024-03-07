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
        moveset = stateData.Moveset;

        // Falling variables
        fallGravityMultiplier = stateData.FallGravityMultiplier;
        jumpHaltForce         = stateData.JumpHaltForce;
        
        // Attack variables
        requiredAirTime = stateData.RequiredAirTime;
        
        Debug.Assert(moveset != null, "Moveset is null in the AttackStateData. Please assign it in the inspector.");

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

        timeAirborne = 0f;
        timeAirborneAttacking = 0;
    }

    public override void UpdateState()
    {   
        if (!IsAirborne)
        {
            OnExit();
            return;
        }

        timeAirborne += Time.deltaTime; // bug: timeAirborne is always one frame step. (0.016 something)

        // Only attack if the player has been airborne for a certain amount of time.
        if(timeAirborne >= requiredAirTime && !IsAirborneAttacking) IsAirborneAttacking = true;

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
                
                // Stop the player once they attack in the air.
                player.Rigidbody.velocity = new Vector2(player.Rigidbody.velocity.x, 0);

                if (player.IsGrounded())
                {
                    OnExit();
                    return;
                }

                // Applies gravity to the player to make them fall faster.
                if (player.Rigidbody.velocity.y < 0) player.Rigidbody.AddForce(fallGravityMultiplier * Vector3.down);
                    
                // Applies a halt force to the player's upward momentum to smooth out the jump.
                if (player.Rigidbody.velocity.y > 0) player.Rigidbody.AddForce(jumpHaltForce * Vector3.down);
            }
            // If the player lands, cancel the attack.
            else
            {
                Logger.Debug("Airborne Attack cancelled!", LogType.Log, StateType.AirborneAttack);
                OnExit();
            }
        }
        else { OnExit(); }
    }

    public override void OnExit()
    {
        // Cancel the attack animation by starting the idle animation.
        animator.Play("Idle");

        player.StateMachine.TransitionToState(player.IsGrounded() ? StateType.Idle : StateType.Fall);

        if (player.InputManager.MoveInput.x != 0 && player.IsGrounded())
            // If the player is moving, transition to the walk state.
            player.StateMachine.TransitionToState(StateType.Walk);
        
        else // If the player is not moving, transition to the idle state. 
            player.StateMachine.TransitionToState(StateType.Idle);

        IsAirborneAttacking = false;
        IsAirborne = false;
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
        else { Logger.Debug("No \"(None)\" attack type was selected. Something went wrong.", LogType.Error); }
    }
}