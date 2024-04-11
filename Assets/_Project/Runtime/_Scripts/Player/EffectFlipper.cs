#region
using UnityEngine;
#endregion

public class EffectFlipper : MonoBehaviour
{
    PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    void Update() => FlipEffects();
    
    void FlipEffects()
    {
        var otherPlayer = PlayerManager.OtherPlayer(player);
        if (otherPlayer == null) return;
        
        float thisTargetScaleX  = otherPlayer.transform.position.x > transform.position.x ? 1 : -1;
        
        transform.localScale  = new (thisTargetScaleX, 1, 1);
    }
}
