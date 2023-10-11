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
        player    = GetComponentInParent<PlayerController>();
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
        // Add a force that knocks back the player.
        RB.AddForce(Vector3.right * 10, ForceMode.Impulse);
    }
}
