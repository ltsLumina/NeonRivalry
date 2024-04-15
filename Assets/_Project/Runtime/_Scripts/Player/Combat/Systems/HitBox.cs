#region
using System.Collections;
using Lumina.Essentials.Attributes;
using UnityEngine;
#endregion

public class HitBox : MonoBehaviour
{
    [SerializeField, ReadOnly] MoveData performedMove;

    public MoveData MoveData => performedMove;
    public PlayerController Owner { get; private set; }

    bool wasColliderEnabled;
    

    void Start() => Owner = GetComponentInParent<PlayerController>();

    public void SetAttack(MoveData moveData) => performedMove = moveData;

    void Update()
    {
        if (Owner.IsArmored) Debug.LogError("Armor doesn't work! (has been deprecated)");
    }
    
    IEnumerator ResetArmor()
    {
        yield return new WaitForSeconds(0.2f);
        Owner.IsArmored = false;
    }
}
