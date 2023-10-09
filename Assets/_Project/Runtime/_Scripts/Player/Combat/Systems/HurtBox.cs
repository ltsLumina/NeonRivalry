#region
using UnityEngine;
#endregion

public class HurtBox : MonoBehaviour
{
    public delegate void HurtBoxHitAction(HitBox hitBox);
    public event HurtBoxHitAction OnHurtBoxHit;

    void Start()
    {
        OnHurtBoxHit += hitBox => Debug.Log($"HurtBox hit by {hitBox.name} and took {hitBox.DamageAmount} damage!");
    }

    void OnTriggerEnter(Collider other)
    {
        var hitBox = other.GetComponent<HitBox>();
        if (hitBox != null) OnHurtBoxHit?.Invoke(hitBox);
    }
}
