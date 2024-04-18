#region
using System.Collections.Generic;
using UnityEngine;
#endregion

/// <summary>
/// This class literally does nothing lol.
/// Neither does <see cref="HurtBoxController"/>.
/// </summary>
public class HitBoxController : MonoBehaviour
{
    public List<HitBox> HitBoxes { get; private set; }

    void Awake() => HitBoxes = new (GetComponentsInChildren<HitBox>());
}