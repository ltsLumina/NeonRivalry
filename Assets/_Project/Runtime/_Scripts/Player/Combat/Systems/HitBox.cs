#region
using System.Collections;
using Lumina.Essentials.Attributes;
using UnityEngine;
#endregion

public class HitBox : MonoBehaviour
{
    [SerializeField, ReadOnly] MoveData performedMove;

    public MoveData MoveData => performedMove;
    public BoxCollider Collider => GetComponent<BoxCollider>();
    
    PlayerController player;
    
    void Start() => player = GetComponentInParent<PlayerController>();

    public void SetAttack(MoveData moveData) => performedMove = moveData;

    void Update()
    {
        if (performedMove != null && performedMove.isArmor)
        {
            player.IsArmored = true;
            Debug.Log("Player is armored!");
            StartCoroutine(ResetArmor());
        }
    }
    
    IEnumerator ResetArmor()
    {
        yield return new WaitForSeconds(0.2f);
        player.IsArmored = false;
    }
}
