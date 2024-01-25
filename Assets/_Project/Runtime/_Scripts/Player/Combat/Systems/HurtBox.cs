#region
using System;
using System.Diagnostics.CodeAnalysis;
using Lumina.Essentials.Sequencer;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Lumina.Debugging.Logger;
#endregion

public class HurtBox : MonoBehaviour
{
    [SerializeField] GameObject punchKickEffect;
    
    
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
    
    [SuppressMessage("ReSharper", "ConvertToLocalFunction")]
    void OnTakeDamage(HitBox hitBox)
    {
        int health = player.Healthbar.Value;
        
        Debug.Log($"HurtBox hit by {hitBox.name} and took {hitBox.DamageAmount} damage!");
        
        PlayEffect(out GameObject effect);
        
        // psuedo code:
        // if (player.StateMachine.CurrentState is BlockState)
        // {
        //     // perform block related stuff
        //     // make blockstate subscribe to OnHurtBoxHit and do the following:
        //     // - on hit, execute the animation
        // }
        
        
        // Take damage.
        if (!player.Healthbar.Invincible) health -= hitBox.DamageAmount;
        player.Healthbar.Value = health;

        //TODO: Temporary for 25% Assignment
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

            if (!Logger.DebugPlayers)
            {
                var delayLoad = new Sequence(this);
                delayLoad.WaitThenExecute(0.35f, () => SceneManagerExtended.LoadScene(SceneManagerExtended.ActiveScene));
            }
        }
        
        // Knock player back
        Knockback();
    }
    void PlayEffect(out GameObject effect) => effect = Instantiate(punchKickEffect, transform.position + new Vector3(0.2f, transform.position.y - 1, transform.position.z - 1), transform.rotation);

    void Knockback()
    {
        // Don't knockback players while debugging.
        if (Logger.DebugPlayers) return;
        
        // Knockback the player based on the sign of the Y-rotation.
        float knockbackForce     = 450f;
        float knockbackDirection = -Mathf.Sign(transform.rotation.y);
        rigidbody.AddForce(knockbackDirection * knockbackForce * Vector3.right);
    }
}
