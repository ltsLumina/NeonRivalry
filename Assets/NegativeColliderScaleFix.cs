#region
using UnityEngine;
#endregion

[RequireComponent(typeof(Collider))]
public class NegativeColliderScaleFix : MonoBehaviour
{
    PlayerController player;

    void Start() => player = GetComponentInParent<PlayerController>();

    void Update() => transform.localScale = new (-player.FacingDirection, 1, 1);
}
