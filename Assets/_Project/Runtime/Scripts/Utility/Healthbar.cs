using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Reference"), Space(5)]
    [SerializeField, ReadOnly] Slider slider;
    
    [Header("Healthbar Options"), Space(5)]
    [SerializeField] int health;

    // Unity event for onPlayerDeath
    public UnityEvent onPlayerDeath;
    
    // Properties
    /// <summary> The slider associated with this <see cref="Healthbar"/>. </summary>
    public Slider Slider => slider;
    
    void Awake()
    {
        slider = GetComponent<Slider>();
    }
    
    public void OnDeath()
    {
        Debug.Log("Player has died!");
    }
}
