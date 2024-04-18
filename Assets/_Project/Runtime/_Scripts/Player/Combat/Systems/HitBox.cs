#region
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
}
