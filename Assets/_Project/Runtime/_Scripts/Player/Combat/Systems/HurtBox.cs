﻿#region
using System;
using System.Collections;
using Lumina.Essentials.Sequencer;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
#endregion

public class HurtBox : MonoBehaviour
{
    [Tab("Stats")]
    [SerializeField] float blockDamageReductionPercentage = 0.5f;
    
    [Tab("Effects")]
    [SerializeField] float totalBlockedDamage;
    [SerializeField] float lastBlockTime;
    [SerializeField] float blockFadeDuration = 3f; // Duration in seconds to fade the block
    
    [Header("Punch/Kick Effect")]
    [SerializeField] GameObject punchKickEffect;
    [Space(5)]
    [Header("Block Effect")]
    [SerializeField] GameObject blockEffect;
    [SerializeField] Gradient blockStrainGradient;
    
    string ThisPlayer => $"Player {player.PlayerID}";

     PlayerController player;

     public delegate void HurtBoxHit(HitBox hitBox);
     public event HurtBoxHit OnHurtBoxHit;
    
    delegate void BlockHit();
    event BlockHit OnBlockHit;

    void Awake()
    {
        player    = GetComponentInParent<PlayerController>();
        player.GetComponent<Rigidbody>();
    }

    void Update() // TODO: Remove Update and DEBUG method when finished debugging.
    {
        DEBUG_TryHitHurtBox();
    }

    void OnEnable()
    {
        OnHurtBoxHit += OnTakeDamage; 
        OnBlockHit   += () => PlayEffect(blockEffect);
    }

    void OnDisable() => OnHurtBoxHit -= OnTakeDamage;

    void DEBUG_TryHitHurtBox()
    {
        if (!Input.GetKeyDown(KeyCode.H)) return;

        var hitBox = FindObjectOfType<HitBox>();
        if (hitBox != null) OnHurtBoxHit?.Invoke(hitBox);
    }

    void OnTriggerEnter(Collider other)
    {
        var hitBox = other.GetComponent<HitBox>();
        if (hitBox != null) OnHurtBoxHit?.Invoke(hitBox);
    }

    IEnumerator InvincibilityFrames(float duration = 0.35f)
    {
        player.IsInvincible = true;
        yield return new WaitForSeconds(duration);

        // Set player back to vulnerable
        player.IsInvincible = false;
    }
    
    void OnTakeDamage(HitBox hitBox)
    {
        // Check if the player is invincible
        if (player.IsInvincible) return;

        // Create variable to represent the player's health
        int health = player.Healthbar.Value;

        if (player.IsBlocking)
        {
            OnBlockHit?.Invoke();

            int reducedDamage = Mathf.RoundToInt(hitBox.DamageAmount * blockDamageReductionPercentage);
            int finalDamage   = hitBox.DamageAmount - reducedDamage; // Subtract the reduced damage from the original damage
            player.Healthbar.Value -= finalDamage;                   // Apply the final damage
            Debug.Log($"Blocked and took {finalDamage} damage!");

            // Add the blocked damage to the total
            totalBlockedDamage += finalDamage;
            lastBlockTime      =  Time.time;

            // Start the coroutine to reset the total blocked damage
            StartCoroutine(ResetTotalBlockedDamage());

            // Calculate the strain percentage
            float strainPercentage = totalBlockedDamage / (totalBlockedDamage + hitBox.DamageAmount);

            // Get the color from the gradient based on the strain percentage
            Color strainColor = blockStrainGradient.Evaluate(strainPercentage);

            // Apply the color to the block
            blockEffect.GetComponent<SpriteRenderer>().material.color = strainColor;

            // Play block effect.
            PlayEffect(blockEffect);

            StartCoroutine(InvincibilityFrames());

            return;
        }
        
        Debug.Log($"HurtBox hit by {hitBox.name} and took {hitBox.DamageAmount} damage!");
        PlayEffect(punchKickEffect);
        
        // Reduce health by the damage amount, and update the health bar.
        health -= hitBox.DamageAmount;
        player.Healthbar.Value = health;
        
        // Plays effects over time such as flashing red and invincibility frames.
        TakeDamageRoutine();

        // Player has died.
        if (health <= 0)
        {
            player.SetPlayerState(true);
            
            Gamepad.current.Rumble(this);
            
            //may god save us
            // RoundManager.player1WonRoundsText.text = $"Rounds won: \n{FindObjectOfType<RoundManager>().player1WonRounds}/2";
            // FindObjectOfType<RoundManager>().player1WonRounds++;
            // FindObjectOfType<RoundManager>().currentRounds++;
            Debug.Log($"{ThisPlayer} is dead!");
        }
    }
    
    IEnumerator ResetTotalBlockedDamage()
    {
        // Wait for the block fade duration
        yield return new WaitForSeconds(blockFadeDuration);

        // If no new blocks have occurred during the wait time, reset the total blocked damage
        if (Time.time >= lastBlockTime + blockFadeDuration) { totalBlockedDamage = 0f; }
    }
    
    
    void TakeDamageRoutine()
    {
        // Flash player red on take damage.
        StartCoroutine(FlashRedOnDamage());

        // Set player to invincible and start the invincibility frames coroutine
        StartCoroutine(InvincibilityFrames());
    }

    IEnumerator FlashRedOnDamage()
    {
        var meshRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();

        int baseColor       = Shader.PropertyToID("_BaseColor");
        int firstShadeColor = Shader.PropertyToID("_1st_ShadeColor");

        var baseOriginalColor       = meshRenderer.material.GetColor(baseColor);
        var firstShadeOriginalColor = meshRenderer.material.GetColor(firstShadeColor);

        // Set colors to red
        var col = new Color(0.91f, 0.34f, 0.47f, 0.45f);
        meshRenderer.material.SetColor(baseColor, col);
        meshRenderer.material.SetColor(firstShadeColor, col);

        // Fade out over 0.15 seconds
        float duration    = 0.15f;
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

    void PlayEffect(GameObject effect)
    {
        // Enable the effect
        effect.SetActive(true);

        // Start the coroutine to disable the effect after the animation has finished
        StartCoroutine(DisableEffectAfterAnimation(effect));
    }

    IEnumerator DisableEffectAfterAnimation(GameObject effect)
    {
        // Wait for the duration of the animation
        yield return new WaitForSeconds(0.35f);

        // Disable the effect
        effect.SetActive(false);
    }
}
