#region
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
        Debug.Log($"Health: {health}");

        if (health <= 0)
        {
            Debug.Log("Player is dead!");
            SceneManagerExtended.ReloadScene();
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
