#region
using System;
using System.Collections;
using UnityEngine;
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
    [Header("Punch/Kick Effect")]
    [SerializeField] GameObject punchKickEffect;
    [Space(5)]
    [Header("Block Effect")]
    [SerializeField] GameObject blockEffect;
    [SerializeField] Gradient blockStrainGradient;
    
    PlayerController player;

    public delegate void HurtBoxHit(HitBox hitBox, MoveData moveData);
    public event HurtBoxHit OnHurtBoxHit;

    public delegate void BlockHit();
    public event BlockHit OnBlockHit;

    void Awake() => player = GetComponentInParent<PlayerController>();

    void Update() // TODO: Remove Update and DEBUG method when finished debugging.
    {
        DEBUG_TryHitHurtBox();
    }

    void OnEnable() => OnHurtBoxHit += TakeDamage;
    void OnDisable() => OnHurtBoxHit -= TakeDamage;

    void DEBUG_TryHitHurtBox()
    {
        if (!Input.GetKeyDown(KeyCode.H)) return;

        var hitBox = FindObjectOfType<HitBox>();
        var debugMoveData = Resources.Load<MoveData>("ScriptableObjects/Debug Attack");
        
        if (hitBox != null) OnHurtBoxHit?.Invoke(hitBox, debugMoveData);
    }

    void OnTriggerEnter(Collider other)
    {
        var hitBox = other.GetComponent<HitBox>();
        if (hitBox != null) OnHurtBoxHit?.Invoke(hitBox, hitBox.MoveData);
    }
    
    void TakeDamage(HitBox hitBox, MoveData incomingAttack)
    {
        // Check if the player is invincible or already dead.
        if (player.IsInvincible || player.Healthbar.Health <= 0) return;

        if (incomingAttack.isOverhead && player.IsCrouching)
        {
            Debug.Log("Overhead! (Attack missed)");
            return;
        }
        
        // -- Any logic that needs to happen regardless if the player is blocking or not --
        
        player.FreezePlayer(true, .3f, true);

        // Check for move effects
        foreach (var effect in incomingAttack.moveEffects)
        {
            effect.ApplyEffect(player);
        }
        
        if (incomingAttack.isSweep && !player.IsCrouching)
        {
            Debug.Log("Sweep!");
            HandleHit(incomingAttack);
            return;
        }
        
        if (incomingAttack.isArmor)
        {
            Debug.Log("Armor!");
            HandleHit(incomingAttack);
            return;
        } 
        
        // -- Blocking Logic --
        
        if (player.IsBlocking)
        {
            if (incomingAttack.isGuardBreak)
            {
                Debug.Log("Guard break!");
                HandleHit(incomingAttack, hitBox);
                return;
            }

            HandleBlock(incomingAttack, hitBox);
            return;
        }
        
        // -- Player was hit by another character and was not blocking --
        
        HandleHit(incomingAttack, hitBox);
    }
    
    void HandleHit(MoveData moveData, HitBox hitBox = default)
    {
        if (!player.IsArmored)
        {
            player.Animator.SetTrigger("Hitstun");
            player.StateMachine.TransitionToState(State.StateType.HitStun);

            PlayEffect(punchKickEffect);
            
            // Don't apply knockback if the player is airborne
            Knockback(moveData, hitBox?.Owner);
        }
        else { player.IsArmored = false; }
        
        // Reduce health by the damage amount, and update the health bar.
        // Create variable to represent the player's health
        int health = player.Healthbar.Health;
        health -= moveData.damage;
        player.Healthbar.Health =  health;
        
        // Plays effects over time such as flashing red and invincibility frames.
        TakeDamageRoutine();
    }

    void HandleBlock(MoveData moveData, HitBox hitBox = default)
    {
        OnBlockHit?.Invoke();

        // Update the state of the player.
        player.StateMachine.TransitionToState(State.StateType.Block);

        // If the player is standing, play the standing block animation.
        if (!player.IsCrouching) player.Animator.SetTrigger("Blocked");

        // Calculate the damage taken and the strain percentage.
        CalculateDamageTaken(moveData, out float strainPercentage);
        BlockStrain(strainPercentage);

        // Play block effect.
        PlayEffect(blockEffect);

        StartCoroutine(InvincibilityFrames());

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
            player.Healthbar.Health -= finalDamage;                   // Apply the final damage
            Debug.Log($"Blocked and took {finalDamage} damage!");

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
        Vector3 knockbackForce = moveData.knockbackForce;
        
        var otherPlayer = PlayerManager.OtherPlayer(player);
        if (otherPlayer != null) // Two-player knockback
        {
            // If the player is on the right side of the other player, reverse the knockback direction, and vice versa
            Vector3 playerPos = player.transform.position;
            Vector3 otherPlayerPos = otherPlayer.transform.position;
            if (playerPos.x > otherPlayerPos.x) knockbackDir.x *= -1;
        }
        
        // Calculate the knockback force using the direction and force values
        var force = new Vector2(knockbackDir.x * knockbackForce.x, knockbackDir.y * knockbackForce.y);
    
        // Apply the knockback force to the player who was hit
        player.Rigidbody.AddForce(force, ForceMode.Impulse);

        if (moveData.isAirborne) return;

        // Apply knockback to the player who attacked
        var attackerForce = new Vector2(-force.x, 0);
        attacker.Rigidbody.velocity = new (attackerForce.x, attacker.Rigidbody.velocity.y, 0);
    }

    [Obsolete("I wish this would work, but it doesn't.")]
    IEnumerator ApplySmoothForce(Rigidbody playerRigidbody, Vector2 force, float duration)
    {
        // Slight wait before applying the force
        yield return new WaitForSeconds(0.05f);
        
        float timer = 0;

        while (timer < duration)
        {
            // Calculate how much of the duration has passed
            float completed = timer / duration;

            // Calculate the current force
            Vector2 currentForce = Vector2.Lerp(force, Vector2.zero, completed);

            // Apply the current force
            playerRigidbody.AddForce(currentForce, ForceMode.Impulse);

            // Wait for the next frame
            yield return null;

            // Increase the timer
            timer += Time.deltaTime;
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
        var meshRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
        if (!meshRenderer) yield break;

        int baseColor       = Shader.PropertyToID("_BaseColor");
        int firstShadeColor = Shader.PropertyToID("_1st_ShadeColor");

        var baseOriginalColor       = meshRenderer.material.GetColor(baseColor);
        var firstShadeOriginalColor = meshRenderer.material.GetColor(firstShadeColor);

        // Set colors to red
        var col = new Color(0.91f, 0.34f, 0.47f, 0.45f);
        meshRenderer.material.SetColor(baseColor, col);
        meshRenderer.material.SetColor(firstShadeColor, col);

        // Fade out over 0.15 seconds
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolate color
            Color baseInterpolatedColor       = Color.Lerp(col, baseOriginalColor, t);
            Color firstShadeInterpolatedColor = Color.Lerp(col, firstShadeOriginalColor, t);

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
        player.IsInvincible = true;
        yield return new WaitForSeconds(duration);

        // Set player back to vulnerable
        player.IsInvincible = false;
    }

    void PlayEffect(GameObject effect)
    {
        if (!SettingsManager.ShowEffects) return;
        
        // Enable the effect
        effect.SetActive(true);

        // e.g. freeze game for a short duration for juice
        Sleep(0.075f);

        // Start the coroutine to disable the effect after the animation has finished
        StartCoroutine(DisableEffectAfterAnimation(effect));
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
        // Wait for the duration of the animation
        yield return new WaitForSeconds(0.35f);

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
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
    
}
