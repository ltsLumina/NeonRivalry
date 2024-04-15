#region
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using VInspector;
using static State;
#endregion

/// <summary>
/// The PlayerController is intended to be used as a base class for the player.
/// It is used to manage the individual player's input, movement, and state.
/// On the other hand, the PlayerManager is used to manage all players in the game including their settings, properties, and actions.
/// <seealso cref="PlayerManager"/>
/// </summary>
[RequireComponent(typeof(Rigidbody)), HelpURL("https://www.youtube.com/watch?v=wJWksPWDKOc")]
public partial class PlayerController : MonoBehaviour
{
    [Tab("Player Stats")]
    [Header("Player Info")]
    [SerializeField] Character character;
    [SerializeField, ReadOnly] int playerID;
    [SerializeField] float idleTimeThreshold;
    [SerializeField, ReadOnly] float IdleTime; //TODO: Make this private, but I should probably move it to a different script.
    [SerializeField, ReadOnly] public float AirborneTime;

    [Space]
    
    [Header("Ground Check"), Tooltip("The minimum distance the ray-cast must be from the ground.")]
    [SerializeField] float raycastDistance;
    [SerializeField] LayerMask groundLayer;

    [Space] 
    [Header("VFX")] 
    [SerializeField] VisualEffect playerLandVFX;
    
    [HideInInspector]
    public float GlobalGravity = -35f;
    [HideInInspector]
    public float GravityScale = 1.0f;
    [HideInInspector]
    public float DefaultGravity;

    [SerializeField] private GameObject playerShadow;
    private float playerShadowStartingHeight;
    private Vector3 playerShadowStartingScale;

    [Tab("Settings")]
    [Header("Debug")]
    [SerializeField] bool godMode;
    [Space(5)]
    [Header("Player Components")]
    [SerializeField, ReadOnly] Healthbar healthbar;
    [SerializeField, ReadOnly] Animator animator;
    
    // Cached References
    readonly static int Speed = Animator.StringToHash("Speed");

    float moveSpeed = 3f;
    float backwardSpeedFactor = 0.5f;
    float acceleration = 8f;
    float deceleration = 10f;
    float velocityPower = 1.4f;

    bool timerDeath;
    
    // -- Properties --  
    public Rigidbody Rigidbody { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public InputManager InputManager { get; private set; }
    public PlayerInput PlayerInput { get; private set; }
    public InputDevice Device => InputDeviceManager.GetDevice(PlayerInput);
    public HitBox HitBox { get; private set; }
    public HurtBox HurtBox { get; private set; }
    public bool IsInvincible { get; set; }
    public bool ActivateTrail { get; set; }
    public bool IsAbleToDash { get; set; }

    string ThisPlayer => $"Player {PlayerID}";
    public bool IsCrouching => Animator.GetBool("IsCrouching");
    
    /// <summary>
    /// <returns> 1 if the player is facing right, -1 if the player is facing left. </returns>
    /// </summary>
    public int FacingDirection => (int) transform.GetChild(0).localScale.x;

    // -- Serialized Properties --

    public Character Character
    {
        get => character;
        set => character = value;
    }

    public int PlayerID
    {
        get => playerID;
        private set => 
            // Clamp the playerID between 1 and 2.
            playerID = Mathf.Clamp(value, 1, 2);
    }
    
    public Healthbar Healthbar
    {
        get => healthbar;
        set => healthbar = value;
    }

    public Animator Animator
    {
        get => animator;
        private set => animator = value;
    }
    
    void Awake()
    {
        Rigidbody    = GetComponent<Rigidbody>();
        StateMachine = GetComponent<StateMachine>();
        Rigidbody    = GetComponent<Rigidbody>();
        InputManager = GetComponentInChildren<InputManager>();
        PlayerInput  = GetComponentInChildren<PlayerInput>();
        HitBox       = GetComponentInChildren<HitBox>();
        HurtBox      = GetComponentInChildren<HurtBox>();
        Animator     = GetComponentInChildren<Animator>();
        
        Rigidbody.useGravity = false;
        DefaultGravity = GlobalGravity;
        IsAbleToDash = true;
    }

    void OnDestroy()
    {
        Healthbar.OnPlayerDeath -= Death;
        PlayerInput.actions.FindAction("Unique").performed -= SubscribeOnUnique;
        RoundTimer.OnTimerEnded -= TimerDeath;
    }

    void Start() => Initialize();

#if UNITY_EDITOR
    void OnValidate() => GetMovementValues();
#endif
    
    void FixedUpdate()
    {
        // Player cannot block while airborne.
        if (Rigidbody.velocity.y > 0 || Rigidbody.velocity.y < 0) IsBlocking = false;

        #region THE "I GIVE UP" REGION
        PlayerShadow();
        
        // Animation bug: If the player attacks and jumps on the same frame, the player will be stuck in the jump animation.
        // Exit the jump animation if the player is grounded.
        if (IsGrounded() && Animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) Animator.Play("Idle");
        if (IsGrounded()) HasAirborneAttacked = false;
        if (IsGrounded()) IsAbleToDash = true;

        if (!IsGrounded() && !IsJumping()) Animator.SetBool("IsFalling", true);
        else if (IsGrounded()) Animator.SetBool("IsFalling", false);

        // Check if the player's grounded state has changed
        if (IsGrounded() != wasGroundedLastFrame)
        {
            // If the player has just landed, flip the model
            if (IsGrounded())
            {
                FlipModel(); 
                
                if (!SettingsManager.ShowParticles) return;
                playerLandVFX.Play(); 
            }
            wasGroundedLastFrame = IsGrounded();
        }
        #endregion

        #region Gravity
        Vector3 gravity = GlobalGravity * GravityScale * Vector3.up;
        Rigidbody.AddForce(gravity, ForceMode.Acceleration);
        #endregion

        CheckIdle();

        // Get the other player's position
        var otherPlayer = PlayerManager.OtherPlayer(this);
        if (otherPlayer != null)
        {
            FlipModel();
        }

        //TODO: Temporary fix to test new state machine.
        if (StateMachine.CurrentState is not AttackState or AirborneAttackState)
        {
            if (IsGrounded())
            {
                if (!InputManager.Enabled) return;
                
                // Reset the airborne time if the player is grounded.
                AirborneTime = 0;
                
                Movement();

                // Set player as crouching if the player is moving down and is grounded.
                Animator.SetBool("IsCrouching", InputManager.MoveInput.y < 0);
            }
            else
            {
                // If the player is airborne, we add to the airborne time.
                AirborneTime += Time.deltaTime;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 direction = transform.position - collision.transform.position;
            direction.Normalize();

            Rigidbody.AddForce(direction * 10, ForceMode.Acceleration);
        }
    }

    void Movement()
    {
        if (!InputManager.Enabled) return;
        
        // Getting the move input from the player's input manager.
        Vector2 moveInput = InputManager.MoveInput;
    
        // Fix rigidbody velocity issue (velocity is absurdly low when standing still)
        if (moveInput == Vector2.zero) Rigidbody.velocity = Vector3.zero;
        
        // Set the animator based on the player's facing direction.
        Animator.SetInteger(Speed, (int) moveInput.x * FacingDirection);
        
        // Check if the player is blocking.
        IsBlocking = Blocking();
    
        // If crouching, reduce movement speed to zero.
        if (IsCrouching) return;

        // Get the other player's position
        var otherPlayer = PlayerManager.OtherPlayer(this);

        bool movingAway;
        if (otherPlayer != null)
            // Check if the player is moving away from the other player
            movingAway  = (otherPlayer.transform.position.x > transform.position.x && moveInput.x < 0) || (otherPlayer.transform.position.x < transform.position.x && moveInput.x > 0);
        else
            // If there is only one player, moving left is considered as moving away
            movingAway = moveInput.x < 0;

        int   moveDirection   = (int) moveInput.x;
        float targetSpeed     = moveDirection * (movingAway ? moveSpeed * backwardSpeedFactor : moveSpeed);
        float speedDifference = targetSpeed - Rigidbody.velocity.x;
        float accelRate       = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement        = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);
        
        // Apply force.
        Rigidbody.AddForce(movement * Vector3.right);
    
        // TODO: ????????????????????
        // I think this is a fix to transition to move/idle. Refer to the OnExit method in just about any state.
        StateMachine.CurrentState.OnExit();
    }

    /// <summary>
    ///     Initialize the player to the correct state.
    ///     This includes setting the player's color and spawn point as well as the player's ID.
    /// </summary>
    void Initialize()
    {
        PlayerManager.AddPlayer(this);
        
        StateMachine.TransitionToState(StateType.Idle);

        // Update the player's ID.
        PlayerID = PlayerInput.playerIndex + 1;

        // The character is set when the player is loaded from a previous scene,
        // meaning if the player was instantiated directly into the scene, the character will be null.
        if (Character != null)
        {
            // Set the name of the player to "Player 1" or "Player 2".
            gameObject.name = $"Player {PlayerID}" + $" ({Character.characterName})";
            
            // Update all stats with the character's stats.
            GetMovementValues();
        }
        else
        {
            gameObject.name = $"Player {PlayerID}";
            Debug.LogWarning("Character is null. Please assign a character to the player.");
        }

        playerShadowStartingScale = playerShadow.transform.localScale;
        playerShadowStartingHeight = playerShadow.transform.position.y;

        PlayerInput.defaultControlScheme = Device is Keyboard ? "Keyboard" : "Gamepad";
        PlayerInput.defaultActionMap     = $"Player {PlayerID}";
        
        var player1actions = Resources.Load<InputActionAsset>("Input Management/Player 1 Input Actions");
        var player2actions = Resources.Load<InputActionAsset>("Input Management/Player 2 Input Actions");
        
        // Assign the player input actions asset to the player input.
        PlayerInput.actions = PlayerID == 1 
            ? player1actions
            : player2actions;
        
        // Switch the player's action map to the correct player.
        PlayerInput.actions.Disable();
        PlayerInput.SwitchCurrentActionMap($"Player {PlayerID}");
        
        // HAVE TO DO THIS MANUALLY BECAUSE UNITY IS INCAPABLE OF DOING IT AUTOMATICALLY ANYMORE
        PlayerInput.actions.FindAction("Unique").performed += SubscribeOnUnique;
        PlayerInput.actions.Enable();

        // Switch the UI input module to the UI input actions.
        PlayerInput.uiInputModule.actionsAsset = PlayerInput.actions;

        var move = InputActionReference.Create(PlayerInput.actions.FindAction("Navigate"));
        var submit = InputActionReference.Create(PlayerInput.actions.FindAction("Submit"));
        var cancel = InputActionReference.Create(PlayerInput.actions.FindAction("Cancel"));
        
        PlayerInput.uiInputModule.move = move;
        PlayerInput.uiInputModule.submit = submit;
        PlayerInput.uiInputModule.cancel = cancel;
        
        // -- Player Light fix -- \\
        // Must disable the player light if there are two players.
        var playerLight = gameObject.GetComponentInChildren<Light>();
        if (PlayerManager.PlayerCount == 2) playerLight.gameObject.SetActive(false);
        
        // -- Healthbar stuff -- \\
        
        PlayerManager.AssignHealthbarToPlayer(this, PlayerID);
        Healthbar.OnPlayerDeath += Death;
        RoundTimer.OnTimerEnded += TimerDeath;

        // Player has been fully initialized.
        // Invoke the OnPlayerJoin event from the InputDeviceManager.
        InputDeviceManager.TriggerPlayerJoin(PlayerInput, PlayerID);

        SetSpawnPosition();

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    void TimerDeath() => timerDeath = true;

    void SubscribeOnUnique(InputAction.CallbackContext ctx) => InputManager.OnUnique(ctx);

    void SetSpawnPosition()
    {
        const float player1X = -2.5f;
        const float player1Z = -2.5f;
        const float player2X = 2.5f;
        const float player2Z = -2.5f;
        
        Action action = playerID switch
        { 1 => () => transform.position = new (player1X, 1, player1Z),
          2 => () => transform.position = new (player2X, 1, player2Z),
          _ => () => Debug.LogError($"Invalid PlayerID: {playerID}. Expected either 1 or 2.") };

        action();
    }

    void GetMovementValues()
    {
        moveSpeed           = Character.moveSpeed;
        backwardSpeedFactor = Character.backwardSpeedFactor;
        acceleration        = Character.acceleration;
        deceleration        = Character.deceleration;
        velocityPower       = Character.velocityPower;
    }

    bool wasGroundedLastFrame;
    
    void PlayerShadow()
    {
        //Locks the shadows position into its initial height when the game starts
        playerShadow.transform.position = new (playerShadow.transform.position.x, playerShadowStartingHeight, playerShadow.transform.position.z);

        //Calculate the scaleFactor based on the players height
        float scaleFactor = Mathf.Clamp01((transform.position.y - 0.3f) / (2.9f - 0.3f));

        //Updates the starting scale for the shadow object
        Vector3 initialShadowScale = playerShadow.transform.localScale;

        //Sets how fast the shadow scales up and down
        float scaleSpeed = 15f;

        //If the player is above the minimum height and is moving upwards the shadow will begin to scale down until hitting the minimum size cap
        if(transform.position.y > 0.3f && Rigidbody.velocity.y > 0.01f)
        {
            //Calculates the new size for the shadow
            Vector3 newScale = new Vector3(
                (playerShadowStartingScale.x - scaleFactor) * initialShadowScale.x, 
                (playerShadowStartingScale.y - scaleFactor) * initialShadowScale.y,
                (playerShadowStartingScale.z - scaleFactor) * initialShadowScale.z);
            //Uses Mathf.Lerp to scale down the shadow according to the scaleSpeed
            playerShadow.transform.localScale = Vector3.Lerp(playerShadow.transform.localScale, newScale, scaleSpeed * Time.deltaTime);

            //Clamps the value so the shadow does not scale down infinitly if the scaleSpeed is at higher values
            playerShadow.transform.localScale = new Vector3(
                Mathf.Clamp(playerShadow.transform.localScale.x, playerShadow.transform.localScale.x * .25f, playerShadowStartingScale.x),
                Mathf.Clamp(playerShadow.transform.localScale.y, playerShadow.transform.localScale.y * .25f, playerShadowStartingScale.y),
                Mathf.Clamp(playerShadow.transform.localScale.z, playerShadow.transform.localScale.z * .25f, playerShadowStartingScale.z));
        }

        //If the player is below the maximum height and is falling the shadow will begin to scale down until reaching its default size
        if(transform.position.y < 2.9f && Rigidbody.velocity.y < -0.01f)
        {
            //Calculates the new size for the shadow
            Vector3 newScale = new Vector3(
                playerShadowStartingScale.x * initialShadowScale.x,
                playerShadowStartingScale.y * initialShadowScale.y,
                playerShadowStartingScale.z * initialShadowScale.z);
            //Uses Mathf.Lerp to scale up the shadow according to the scaleSpeed
            playerShadow.transform.localScale = Vector3.Lerp(playerShadow.transform.localScale, newScale, scaleSpeed * Time.deltaTime);

            //Clamps the value so the shadow does not scale up infinitly if the scaleSpeed is at higher values
            playerShadow.transform.localScale = new Vector3(
                Mathf.Clamp(playerShadow.transform.localScale.x, playerShadow.transform.localScale.x * .25f, playerShadowStartingScale.x),
                Mathf.Clamp(playerShadow.transform.localScale.y, playerShadow.transform.localScale.y * .25f, playerShadowStartingScale.y),
                Mathf.Clamp(playerShadow.transform.localScale.z, playerShadow.transform.localScale.z * .25f, playerShadowStartingScale.z));
        }
    }

    void FlipModel()
    {
        var otherPlayer = PlayerManager.OtherPlayer(this);
        if (otherPlayer == null) return;

        Transform thisModel  = transform.GetChild(0);
        Transform otherModel = otherPlayer.transform.GetChild(0);

        float thisTargetScaleX  = otherPlayer.transform.position.x > transform.position.x ? 1 : -1;
        float otherTargetScaleX = transform.position.x             > otherPlayer.transform.position.x ? 1 : -1;

        float duration = 0.1f; // duration of the flip animation

        // flip this player if it's grounded and the X scale changes
        if (IsGrounded() && Math.Abs(thisModel.localScale.x - thisTargetScaleX) > 0.001f)
        {
            // Flip Animation for this player
            Sequence thisSequence = DOTween.Sequence();
            thisSequence.Append(thisModel.DOScaleX(thisTargetScaleX, duration));
            thisSequence.Join(thisModel.DOScaleZ(0f, duration  / 3)); // scale Z to 0 in the first half of the duration
            thisSequence.Append(thisModel.DOScaleZ(1, duration / 3)); // scale Z back to 1 in the second half of the duration
        }

        // flip other player if it's grounded and the X scale changes
        if (otherPlayer.IsGrounded() && Math.Abs(otherModel.localScale.x - otherTargetScaleX) > 0.001f)
        {
            // Flip Animation for the other player
            Sequence otherSequence = DOTween.Sequence();
            otherSequence.Append(otherModel.DOScaleX(otherTargetScaleX, duration));
            otherSequence.Join(otherModel.DOScaleZ(0f, duration  / 3)); // scale Z to 0 in the first half of the duration
            otherSequence.Append(otherModel.DOScaleZ(1, duration / 3)); // scale Z back to 1 in the second half of the duration
        }
    }

    void CheckIdle()
    {
        if (StateMachine.CurrentState is IdleState)
        {
            IdleTime += Time.deltaTime;

            if (IdleTime >= idleTimeThreshold)
            {
                Animator.SetBool("Idle", true);
            }
        }
        else
        {
            IdleTime = 0;
            Animator.SetBool("Idle", false);
        }
        
        // // Check if the player is idle.
        // if (IsIdle())
        // {
        //     // If the player is idle, we add to the idle time.
        //     IdleTime += Time.deltaTime;
        //
        //     // If the idle time is greater than the threshold, we transition to the idle state.
        //     if (IdleTime >= idleTimeThreshold)
        //     {
        //         StateMachine.TransitionToState(StateType.Idle);
        //         Animator.SetBool("Idle", true);
        //     }
        // }
        // else
        // {
        //     IdleTime = 0; 
        //     Animator.SetBool("Idle", false);
        // }
    }
    
    public void FreezePlayer(bool frozen, float duration = 0.3f, bool resetVelocity = false)
    {
        StartCoroutine(FreezePlayerRoutine());
        
        return; // Local function
        IEnumerator FreezePlayerRoutine()
        {
            DisablePlayer(frozen);
            Rigidbody.velocity = resetVelocity ? Vector3.zero : Rigidbody.velocity;
            yield return new WaitForSecondsRealtime(duration);
            DisablePlayer(false);
        }
    }
    

    public void DisablePlayer(bool disabled)
    {
        InputManager.Enabled = !disabled;
        HitBox.enabled       = !disabled;
    }
    
    public void Death(PlayerController playerThatDied)
    {
        if (!timerDeath)
        {
            Animator.SetTrigger("Died");
            Animator.SetBool("Dead", true);
        }
        
        DisablePlayer(true);
        GamepadExtensions.StopAllRumble();
        GamepadExtensions.RumbleAll(true, 0.5f, 1f, 0.65f);
        AudioManager.PauseAll(3f);
        
        // Flip the death track
        var deathTrack = GetComponentInChildren<CinemachineSmoothPath>();
        deathTrack.transform.localScale = new (FacingDirection, 1, 1);

        // Get the Volume component
        var volume = FindObjectOfType<Volume>();
        if (volume == null) return;

        StartCoroutine(DeathEffect());
        
        Debug.Log($"{ThisPlayer} died!");

        return;
        IEnumerator DeathEffect()
        {
            // Try to get the ChromaticAberration effect
            if (!volume.profile.TryGet(out ChromaticAberration chromaticAberration)) { Debug.LogWarning($"The {nameof(ChromaticAberration)} post processing effect is missing on the volume!", volume); yield break; }
            if (!volume.profile.TryGet(out DepthOfField depthOfField)) { Debug.LogWarning($"The {nameof(DepthOfField)} post processing effect is missing on the volume!", volume); yield break; }
            if (!volume.profile.TryGet(out LensDistortion lensDistortion)) { Debug.LogWarning($"The {nameof(LensDistortion)} post processing effect is missing on the volume!", volume); yield break; }
            
            Sequence sequence = DOTween.Sequence();
            int      mult     = 2;
            sequence.Join(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 1f     * mult, .65f).SetEase(Ease.InOutCubic));
            sequence.Join(DOTween.To(() => depthOfField.focusDistance.value, x => depthOfField.focusDistance.value       = x, 1.85f  * mult, .5f).SetEase(Ease.OutCubic));
            sequence.Join(DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value           = x, -0.35f * mult, 2f).SetEase(Ease.OutCubic));
            //sequence.Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 1f, .65f).SetEase(Ease.OutCubic)); // Holds the effect for a bit
            sequence.AppendInterval(.1f); // Required to prevent the next sequence from starting too early
            sequence.Join(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0f, .5f).SetEase(Ease.OutBounce));
            sequence.Join(DOTween.To(() => depthOfField.focusDistance.value, x => depthOfField.focusDistance.value       = x, 1.85f, .5f).SetEase(Ease.OutBounce));
            sequence.Join(DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value           = x, 0f, .5f).SetEase(Ease.OutCubic));
            sequence.Play();

            var vcam = FindObjectOfType<CinemachineVirtualCamera>();
            if (vcam == null) yield break;

            vcam.Follow = transform;
            vcam.LookAt = transform;

            var body = vcam.AddCinemachineComponent<CinemachineTrackedDolly>();
            if (body == null) yield break;
            
            body.m_Path = GetComponentInChildren<CinemachineSmoothPath>();
            
            // Set the aim to Group Composer
            var aim = vcam.AddCinemachineComponent<CinemachineComposer>();
            if (aim == null) yield break;

            aim.m_TrackedObjectOffset = new (0, 1.35f, 0);

            yield return new WaitForSeconds(0.25f);

            body.m_XDamping = 2f;
            body.m_YDamping = 1.25f;

            // Start the AutoDolly
            body.m_AutoDolly.m_Enabled = true;
        }
    }

#if UNITY_EDITOR
    public void ValidateMovementVariables(Character character)
    {
        if (character == null) return;

        moveSpeed           = character.moveSpeed;
        backwardSpeedFactor = character.backwardSpeedFactor;
        acceleration        = character.acceleration;
        deceleration        = character.deceleration;
        velocityPower       = character.velocityPower;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    PlayerController player;
    bool showCharacterInspector;

    public override void OnInspectorGUI()
    {
        player = (PlayerController) target;

        string label = showCharacterInspector ? "Hide Character Inspector" : "Show Character Inspector";
        showCharacterInspector = GUILayout.Toggle(showCharacterInspector, label, EditorStyles.toolbarButton);

        if (showCharacterInspector)
        {
            Editor editor = CreateEditor(player.Character);
            editor.OnInspectorGUI();
        }
        else { base.OnInspectorGUI(); }
    }
}
#endif