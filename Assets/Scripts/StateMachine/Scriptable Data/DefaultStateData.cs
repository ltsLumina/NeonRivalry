using UnityEngine;
using static Lumina.Essentials.Attributes;

/// <summary>
/// This scriptable object is intended to be used a template for creating new state data.
/// </summary>
[CreateAssetMenu(fileName = "DefaultStateData", menuName = "State Data/DefaultState Data", order = 0)]
public class DefaultStateData : ScriptableObject
{
    [SerializeField, ReadOnly] float exampleVariable1;
    [SerializeField] float exampleVariable2;

    public float ExampleVariable1 => exampleVariable1;
    public float ExampleVariable2 => exampleVariable2;
}