#region
using System.Collections.Generic;
using UnityEngine;
#endregion

/// <summary>
/// This class literally does nothing lol.
/// Same goes for <see cref="HitBoxController"/>.
/// </summary>
public class HurtBoxController : MonoBehaviour
{
    public List<HurtBox> HurtBoxes { get; private set; }

    void Awake() => HurtBoxes = new (GetComponentsInChildren<HurtBox>());
}