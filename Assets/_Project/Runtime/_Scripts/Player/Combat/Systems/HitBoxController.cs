#region
using System.Collections.Generic;
using UnityEngine;
#endregion

public class HitBoxController : MonoBehaviour
{
    public List<HitBox> HitBoxes { get; private set; }

    void Awake() => HitBoxes = new (GetComponentsInChildren<HitBox>());
}
