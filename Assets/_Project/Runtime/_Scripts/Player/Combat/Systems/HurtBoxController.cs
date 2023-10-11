#region
using System.Collections.Generic;
using UnityEngine;
#endregion

public class HurtBoxController : MonoBehaviour
{
    public List<HurtBox> HurtBoxes { get; private set; }

    void Awake() => HurtBoxes = new (GetComponentsInChildren<HurtBox>());
}
