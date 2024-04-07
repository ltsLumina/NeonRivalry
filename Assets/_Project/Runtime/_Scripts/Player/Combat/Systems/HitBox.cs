#region
using Lumina.Essentials.Attributes;
using UnityEngine;
#endregion

public class HitBox : MonoBehaviour
{
    [SerializeField, ReadOnly] MoveData performedMove;

    public MoveData MoveData => performedMove;
    
    public void SetAttack(MoveData moveData)
    {
        Debug.Log($"Setting attack to {moveData}!");
        performedMove = moveData;
    }
    
    void Update()
    {
        if (performedMove != null && performedMove.isArmor)
        {
            var player = GetComponentInParent<PlayerController>();
            player.IsArmored = true;
            Debug.Log("Player is armored!");
        }
    }
}
