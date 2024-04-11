using UnityEngine;

/// <summary>
/// Contains the character's name, stats and prefab.
/// </summary>
[CreateAssetMenu(fileName = "Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] Moveset moveset;

    public GameObject CharacterPrefab
    {
        get => characterPrefab;
        set => characterPrefab = value;
    }
    
    public Moveset Moveset
    {
        get => moveset;
        set => moveset = value;
    }

    public string characterName => characterPrefab.name;
    public float moveSpeed;
    public float backwardSpeedFactor;
    public float acceleration;
    public float deceleration;
    public float velocityPower;

#if UNITY_EDITOR
    void OnValidate() // Used to update movement variables in runtime.
    {
        var player = FindObjectOfType<PlayerController>();
        if (player == null) return;
        
        player.ValidateMovementVariables(this);
    }
#endif
}
