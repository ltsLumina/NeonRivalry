#region
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
#endregion

public class HurtBox : MonoBehaviour
{
    [Tab("Stats")]
    [SerializeField] float blockDamageReductionPercentage = 0.5f;
    [SerializeField] float totalBlockedDamage;
    [SerializeField] float lastBlockTime;
    [SerializeField] float blockFadeDuration = 3f; // Duration in seconds to fade the block
    
    [Tab("Effects")]
    [Header("Flash Red On Damage")]
    [SerializeField] Color flashColor = new (0.91f, 0.34f, 0.47f, 0.45f);
    
    [Header("Punch/Kick Effect")]
    [SerializeField] GameObject punchKickEffect;
    [Space(5)]
    [Header("Block Effect")]
    [SerializeField] GameObject blockEffect;
    [SerializeField] Gradient blockStrainGradient;
    
    int ID => victim.PlayerID;
    int TotalBlocks { get; set; }
    
    PlayerController victim;
    Gamepad gamepad;

    public delegate void HurtBoxHit(HitBox hitBox, MoveData moveData);
    public event HurtBoxHit OnHurtBoxHit;

    public delegate void BlockHit();
    public event BlockHit OnBlockHit;

    void Awake()
    {
        victim  = GetComponentInParent<PlayerController>();
        gamepad = victim.Device as Gamepad;
    }

    void OnEnable() => OnHurtBoxHit += TakeDamage;
    void OnDisable() => OnHurtBoxHit -= TakeDamage;

    void OnTriggerEnter(Collider other)
    {
        var hitBox = other.GetComponent<HitBox>();
        if (hitBox != null) OnHurtBoxHit?.Invoke(hitBox, hitBox.MoveData);
    }
    
    void TakeDamage(HitBox hitBox, MoveData incomingAttack)
    {
        // Check if the player is invincible or already dead.
        if (victim.IsInvincible || victim.Healthbar.Health <= 0) return;
        
        // -- Any logic that needs to happen regardless if the player is blocking or not --
        
        victim.FreezePlayer(true, incomingAttack.hitstunDuration, true);

        // Check for move effects
        foreach (var effect in incomingAttack.moveEffects)
        {
            effect.ApplyEffect(victim);
        }
        
        // -- Blocking Logic --
        
        if (victim.IsBlocking)
        {
            // Hits if player is blocking an overhead attack while crouching
            if (incomingAttack.isOverhead && victim.IsCrouching)
            {
                Debug.Log("Overhead attack!");
                HandleHit(incomingAttack, hitBox);
                return;
            }
            
            // Hits: Always
            if (incomingAttack.isGuardBreak)
            {
                Debug.Log("Guard break!");
                HandleHit(incomingAttack, hitBox);
                return;
            }
            
            // Hits: Player is not blocking low. | Blocks: Player is crouching & Blocking
            if (incomingAttack.guard == MoveData.Guard.Low && victim.IsCrouching)
            {
                HandleBlock(incomingAttack, hitBox);
                return;
            }

            // Hits: Player is not blocking high. | Blocks: Player is not crouching
            if (incomingAttack.guard == MoveData.Guard.High && !victim.IsCrouching)
            {
                HandleBlock(incomingAttack, hitBox);
                return;
            }

            // Hits: Player is not blocking low. | Blocks: Player is not crouching
            if (incomingAttack.guard == MoveData.Guard.All)
            {
                HandleBlock(incomingAttack, hitBox);
                return;
            }
        }
        
        // -- Player was hit by another character and was not blocking --
        
        HandleHit(incomingAttack, hitBox);
    }
    
    void HandleHit(MoveData moveData, HitBox hitBox = default)
    {
        victim.Animator.SetFloat("HitstunDuration", moveData.hitstunDuration * 3f);
        victim.Animator.SetTrigger("Hitstun");
        victim.StateMachine.TransitionToState(State.StateType.HitStun);

        gamepad.Rumble(ID, .5f, 0f, 0.1f);
        // Apply screenshake
        if (moveData.screenShake) CameraController.Shake(moveData.screenShakeAmplitude, moveData.screenShakeFrequency, moveData.screenShakeDuration);

        var attackerGamepad = hitBox?.Owner.Device as Gamepad;
        attackerGamepad?.Rumble(hitBox.Owner.PlayerID, 0.3f, .5f, 0.1f);

        PlayEffect(punchKickEffect);

        // Don't apply knockback if the player is airborne
        Knockback(moveData, hitBox?.Owner);
        
        // Reduce health by the damage amount, and update the health bar.
        // Create variable to represent the player's health
        int health = victim.Healthbar.Health;
        health -= moveData.damage;
        victim.Healthbar.Health =  health;
        
        // Plays effects over time such as flashing red and invincibility frames.
        TakeDamageRoutine();
    }

    void HandleBlock(MoveData moveData, HitBox hitBox = default)
    {
        OnBlockHit?.Invoke();

        // Update the state of the player.
        victim.StateMachine.TransitionToState(State.StateType.Block);

        // If the player is standing, play the standing block animation.
        if (!victim.IsCrouching) victim.Animator.SetTrigger("Blocked");

        // Calculate the damage taken and the strain percentage.
        CalculateDamageTaken(moveData, out float strainPercentage);
        BlockStrain(strainPercentage);

        // Play block effect.
        PlayEffect(blockEffect);

        StartCoroutine(InvincibilityFrames(0.2f));

        #region Local Functions
        return;

        void CalculateDamageTaken(MoveData moveData, out float _strainPercentage)
        {
            // Calculate the strain percentage
            _strainPercentage = totalBlockedDamage / (totalBlockedDamage + moveData.damage);

            // Modify the blockDamageReductionPercentage based on the strainPercentage
            float modifiedBlockDamageReductionPercentage = blockDamageReductionPercentage * (1 - _strainPercentage);
            modifiedBlockDamageReductionPercentage = Mathf.Clamp(modifiedBlockDamageReductionPercentage, 0.35f, 1f);

            int reducedDamage = Mathf.RoundToInt(moveData.damage * modifiedBlockDamageReductionPercentage);
            int finalDamage   = moveData.damage - reducedDamage; // Subtract the reduced damage from the original damage
            victim.Healthbar.Health -= finalDamage;                   // Apply the final damage
            //Debug.Log($"Blocked and took {finalDamage} damage!");

            // Add the blocked damage to the total
            totalBlockedDamage += finalDamage;
            lastBlockTime      =  Time.time;

            // Start the coroutine to reset the total blocked damage
            StartCoroutine(ResetTotalBlockedDamage());
        }

        IEnumerator ResetTotalBlockedDamage()
        {
            // Wait for the block fade duration
            yield return new WaitForSeconds(blockFadeDuration);

            // If no new blocks have occurred during the wait time, reset the total blocked damage
            if (Time.time >= lastBlockTime + blockFadeDuration) { totalBlockedDamage = 0f; }
        }

        // ReSharper disable once VariableHidesOuterVariable
        void BlockStrain(float strainPercentage)
        {
            // Get the color from the gradient based on the strain percentage
            Color strainColor = blockStrainGradient.Evaluate(strainPercentage);

            // Apply the color to the block
            blockEffect.GetComponent<SpriteRenderer>().material.color = strainColor;
        }
        #endregion
    }

    void Knockback(MoveData moveData, PlayerController attacker)
    {
        Vector2 knockbackDir   = moveData.knockbackDir;
        Vector2 knockbackForce = moveData.knockbackForce;
        
        Vector2 aerialKnockbackDir = moveData.aerialKnockbackDir;
        Vector2 aerialKnockbackForce = moveData.aerialKnockbackForce;
        
        var otherPlayer = PlayerManager.OtherPlayer(victim);
        if (otherPlayer != null) // Two-player knockback
        {
            // If the player is on the right side of the other player, reverse the knockback direction, and vice versa
            Vector3 playerPos = victim.transform.position;
            Vector3 otherPlayerPos = otherPlayer.transform.position;

            if (playerPos.x > otherPlayerPos.x)
            {
                knockbackDir.x *= -1;
                aerialKnockbackDir.x *= -1;
            }
        }

        Vector2 regularForce = Vector2.one;
        Vector2 aerialForce = Vector2.one;
        
        // if the moveData has an aerial override knockback, use the aerial knockback values
        if (moveData.aerialOverrideKnockback)
        {
            // Calculate the knockback force using the direction and force values
            aerialForce = new (aerialKnockbackDir.x * aerialKnockbackForce.x, aerialKnockbackDir.y * aerialKnockbackForce.y);

            // Apply the knockback force to the player who was hit
            victim.Rigidbody.AddForce(aerialForce, ForceMode.Impulse);
        }
        else
        {
            // Calculate the knockback force using the direction and force values
            regularForce = new (knockbackDir.x * knockbackForce.x, knockbackDir.y * knockbackForce.y);

            // Apply the knockback force to the player who was hit
            victim.Rigidbody.AddForce(regularForce, ForceMode.Impulse);
        }

        
        // Knockback attacker logic
        if (!moveData.knockBackAttacker) return;

        if (attacker.IsGrounded())
        {
            // Apply knockback to the player who attacked
            var attackerForce = new Vector2(-regularForce.x * moveData.attackerKnockbackMultiplier, 0);
            attacker.Rigidbody.velocity = new (attackerForce.x, attacker.Rigidbody.velocity.y);
        }
        else // Attacker is airborne
        {
            // Apply knockback to the player who attacked
            var attackerForce = new Vector2(-aerialForce.x * moveData.attackerKnockbackMultiplier, 0);
            attacker.Rigidbody.velocity = new (attackerForce.x, attacker.Rigidbody.velocity.y);
        }
    }
    
    void TakeDamageRoutine()
    {
        // Flash player red on take damage.
        StartCoroutine(FlashRedOnDamage());

        // Set player to invincible and start the iframes coroutine
        StartCoroutine(InvincibilityFrames(0.2f));
    }

    IEnumerator FlashRedOnDamage(float duration = 0.15f)
    {
        var meshRenderer = victim.GetComponentInChildren<SkinnedMeshRenderer>();
        if (!meshRenderer) yield break;

        int baseColor       = Shader.PropertyToID("_BaseColor");
        int firstShadeColor = Shader.PropertyToID("_1st_ShadeColor");

        Color baseOriginalColor       = meshRenderer.material.GetColor(baseColor);
        Color firstShadeOriginalColor = meshRenderer.material.GetColor(firstShadeColor);

        if (baseOriginalColor != Color.white)
        {
            Debug.LogWarning("The weird colour bug occured. womp womp");
            baseOriginalColor       = Color.white;
            firstShadeOriginalColor = new (0.48f, 0.42f, 0.91f);
            // NOTE: If we change the shade colors in the prefab, this will be off.
        }
        
        meshRenderer.material.SetColor(baseColor, flashColor);
        meshRenderer.material.SetColor(firstShadeColor, flashColor);

        // Fade out over *duration* seconds
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolate color
            Color baseInterpolatedColor       = Color.Lerp(flashColor, baseOriginalColor, t);
            Color firstShadeInterpolatedColor = Color.Lerp(flashColor, firstShadeOriginalColor, t);

            // Apply interpolated color
            meshRenderer.material.SetColor(baseColor, baseInterpolatedColor);
            meshRenderer.material.SetColor(firstShadeColor, firstShadeInterpolatedColor);

            yield return null;
        }

        // Wait for 0.05 seconds
        yield return new WaitForSeconds(0.05f);
    }

    IEnumerator InvincibilityFrames(float duration = 0.35f)
    {
        victim.IsInvincible = true;
        yield return new WaitForSeconds(duration);

        // Set player back to vulnerable
        victim.IsInvincible = false;
    }

    void PlayEffect(GameObject effect)
    {
        if (!SettingsManager.ShowEffects) return;
        
        // Enable the effect
        effect.SetActive(true);

        // Start the coroutine to disable the effect after the animation has finished
        StartCoroutine(DisableEffectAfterAnimation(effect));
        
        // e.g. freeze game for a short duration for juice
        Sleep(0.125f);
        // Sleep(0.095f);
    }
    
    // void PlayParticles(GameObject particles)
    // {
    //     if (!SettingsManager.ShowParticles) return;
    //     
    //     // Enable the particles
    //     particles.SetActive(true);
    //
    //     // Start the coroutine to disable the particles after the animation has finished
    //     StartCoroutine(DisableEffectAfterAnimation(particles));
    // }

    IEnumerator DisableEffectAfterAnimation(GameObject effect)
    {
        var length = effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        
        // Wait for the duration of the animation
        yield return new WaitForSecondsRealtime(length);

        // Disable the effect
        effect.SetActive(false);
    }
    
    public static void Sleep(float duration = 0.1f)
    {
        CoroutineHelper.StartCoroutine(PerformSleep(duration));
        //StartCoroutine(PerformSleep(duration));
    }
    
    /// <summary>
    /// Freezes the game for a very short duration to give a sense of impact on each hit.
    /// </summary>
    /// <returns></returns>
    static IEnumerator PerformSleep(float duration = 0.1f)
    {
        // Freeze the game for a short duration
        Time.timeScale = 0.25f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
    
}
