#region
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
    [SerializeField] int blockHealth = 100;
    [SerializeField] float blockDamageReductionPercentage = 0.5f;
    [SerializeField] int blockStunDuration;
    
    [Tab("Locations")]
    [Tooltip("Where to spawn the effect.")]
    [SerializeField] Transform punchKickLocation;
    [SerializeField] Transform blockLocation;
    
    [Tab("Effects")]
    [SerializeField] GameObject punchKickEffect;
    [SerializeField] GameObject blockEffect;
    
    PlayerController player;
    new Rigidbody rigidbody;

    public delegate void HurtBoxHit(HitBox hitBox);
    public event HurtBoxHit OnHurtBoxHit;

    void Awake()
    {
        player    = GetComponentInParent<PlayerController>();
        rigidbody = player.GetComponent<Rigidbody>();
    }

    void Update() // TODO: Remove Update and DEBUG method when finished debugging.
    {
        DEBUG_TryHitHurtBox();
    }

    void OnEnable() => OnHurtBoxHit += OnTakeDamage;

    //player.Healthbar.OnHealthChanged += healthbarValue => Debug.Log($"Healthbar value changed to {healthbarValue}!");
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

    IEnumerator InvincibilityFrames()
    {
        yield return new WaitForSeconds(0.5f);

        // Set player back to vulnerable
        player.IsInvincible = false;
    }
    
    void OnTakeDamage(HitBox hitBox)
    {
        // Check if the player is invincible
        if (player.IsInvincible) return;

        if (player.IsBlocking)
        {
            Debug.Log("Player blocked the incoming attack!");
            // Play block effect.
            PlayEffect(blockEffect, blockLocation);

            // Note: psuedo code for dash gauge
            int dashGauge = 0;
            dashGauge += hitBox.DamageAmount;
            Debug.Log($"Dash gauge: {dashGauge}");
            
            return;
        }

        int health = player.Healthbar.Value;
        Debug.Log($"HurtBox hit by {hitBox.name} and took {hitBox.DamageAmount} damage!");

        PlayEffect(punchKickEffect, punchKickLocation);
        
        // Take damage.
        if (!player.Healthbar.Invincible) health -= hitBox.DamageAmount;
        player.Healthbar.Value = health;
        
        // Flash player red on take damage.
        StartCoroutine(FlashRedOnDamage());

        // Set player to invincible and start the invincibility frames coroutine
        player.IsInvincible = true;
        StartCoroutine(InvincibilityFrames());

        if (health <= 0)
        {
            Gamepad.current.Rumble(this);
            
            //may god save us
            // RoundManager.player1WonRoundsText.text = $"Rounds won: \n{FindObjectOfType<RoundManager>().player1WonRounds}/2";
            // FindObjectOfType<RoundManager>().player1WonRounds++;
            // FindObjectOfType<RoundManager>().currentRounds++;
            Debug.Log("Player is dead!");
        }
        
        // Knock player back
        Knockback();
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

    void PlayEffect(GameObject effect, Transform location)
    {
        effect = Instantiate(effect, location.position, effect.transform.rotation);
        
        var destroyDelay = new Sequence(this);
        GameObject o = effect;
        destroyDelay.WaitThenExecute(0.5f, () => Destroy(o));
    }

    void Knockback()
    {
        if (PlayerManager.PlayerTwo == null) return;
        
        // Knockback the player based on the sign of the Y-rotation.
        float knockbackForce     = 65f;
        
        // Knockback player away from the other player
        Vector3 knockbackDirection;
        var playerOne = PlayerManager.PlayerOne;
        var playerTwo = PlayerManager.PlayerTwo;
        
        if (player.PlayerID == 1) knockbackDirection = playerTwo.transform.position - playerOne.transform.position;
        else                      knockbackDirection = playerOne.transform.position - playerTwo.transform.position;
        
        rigidbody.AddForce(-knockbackDirection * knockbackForce);
    }
}
