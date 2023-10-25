#region
using Lumina.Essentials.Sequencer;
using UnityEngine;
#endregion

public class HurtBox : MonoBehaviour
{
    // DEBUG VALUE:
    [SerializeField] int health = 100;
    
    PlayerController player;
    Rigidbody RB;
    
    public delegate void HurtBoxHitAction(HitBox hitBox);
    public event HurtBoxHitAction OnHurtBoxHit;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        RB = player.GetComponent<Rigidbody>();
    }

    void Start() => OnHurtBoxHit += OnTakeDamage;

    void OnTriggerEnter(Collider other)
    {
        var hitBox = other.GetComponent<HitBox>();
        if (hitBox != null) OnHurtBoxHit?.Invoke(hitBox);
    }
    
    void OnTakeDamage(HitBox hitBox)
    {
        Debug.Log($"HurtBox hit by {hitBox.name} and took {hitBox.DamageAmount} damage!");
        
        // Take damage.
        health -= hitBox.DamageAmount;
        player.Healthbar.Value = health;

        //TODO: Temporary for 25% Assignment
        #region Temporary for 25% Assignment
        // Flash player red.
        var flash = new Sequence(this);
        
        var meshRenderer  = player.GetComponentInChildren<SkinnedMeshRenderer>();
        var originalColor = meshRenderer.material.color;
        
        flash.WaitThenExecute(0.1f, () => meshRenderer.material.color = Color.red).WaitThenExecute(0.1f, () => meshRenderer.material.color = originalColor);
        
        // Temporary: Disable input for a short time.
        Sequence         disableInput   = new(this);
        PlayerController oppositePlayer = player.PlayerID == 1 ? PlayerManager.PlayerTwo : PlayerManager.PlayerOne;
        disableInput.Execute(() => oppositePlayer.PlayerInput.enabled = false).WaitThenExecute(0.5f, () => oppositePlayer.PlayerInput.enabled = true);
        #endregion
        
        Debug.Log($"Health: {health}");

        if (health <= 0)
        {
            Debug.Log("Player is dead!");
            
            var delayLoad = new Sequence(this);
            delayLoad.WaitThenExecute(0.35f, () => SceneManagerExtended.LoadScene(SceneManagerExtended.ActiveScene));
        }
        
        // Knock player back
        Knockback();
    }

    void Knockback()
    {
        // Knockback the player based on the sign of the Y-rotation.
        float knockbackForce     = 450f;
        float knockbackDirection = -Mathf.Sign(transform.rotation.y);
        RB.AddForce(knockbackDirection * knockbackForce * Vector3.right);
    }
}
