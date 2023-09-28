using System.Collections.Generic;
using Lumina.Essentials.Attributes;
using UnityEngine;

public class HealthbarManager : SingletonPersistent<HealthbarManager>
{
    [Header("Serialized References")]
    [SerializeField, ReadOnly] List<Healthbar> healthbars = new ();
    
    [Header("Optional Parameters")]
    [SerializeField] float placeholder;

    // -- Properties --

    public List<Healthbar> Healthbars => healthbars;
}