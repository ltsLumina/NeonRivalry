using UnityEngine;

/// <summary>
/// Contains the character's name, stats and prefab.
/// </summary>
[CreateAssetMenu(fileName = "Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [SerializeField] GameObject characterPrefab;

    public GameObject CharacterPrefab
    {
        get => characterPrefab;
        set => characterPrefab = value;
    }

    public string characterName;
    public float moveSpeed;
    public float backwardSpeedFactor;
    public float acceleration;
    public float deceleration;
    public float velocityPower;
}
