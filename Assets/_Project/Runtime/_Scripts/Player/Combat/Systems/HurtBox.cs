#region
using System;
using Lumina.Essentials.Sequencer;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
#endregion

public class HurtBox : MonoBehaviour
{
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
    
    void OnTakeDamage(HitBox hitBox)
    {
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

        //TODO: Temporary for 25% Assignment (doesnt even work anymore lol)
        #region Temporary for 25% Assignment
        
        // Flash player red.
        var flash = new Sequence(this);
        
        var meshRenderer  = player.GetComponentInChildren<SkinnedMeshRenderer>();

        int    emissionColorId  = Shader.PropertyToID("_EmissionColor");
        var    originalColor    = meshRenderer.material.GetColor(emissionColorId);
        Action setRedColor      = () => SetColor(emissionColorId, Color.red);
        Action setOriginalColor = () => SetColor(emissionColorId, originalColor);

        flash.Execute(setRedColor).WaitThenExecute(0.15f, setOriginalColor);

        void SetColor(int colorId, Color color) => meshRenderer.material.SetColor(colorId, color);

        // Temporary: Disable input for a short time.
        Sequence         disableInput   = new(this);
        PlayerController oppositePlayer = player.PlayerID == 1 ? PlayerManager.PlayerTwo : PlayerManager.PlayerOne;
        disableInput.Execute(() => oppositePlayer.PlayerInput.enabled = false).WaitThenExecute(0.5f, () => oppositePlayer.PlayerInput.enabled = true);
        #endregion

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
