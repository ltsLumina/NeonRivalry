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
    public int Health
    {
        get => health;
        set
        {
            // Clamp the incoming new value to the slider's min/max values
            health = Mathf.Clamp(value, (int) Slider.minValue, (int) Slider.maxValue);

            // Set the slider value to the clamped health value
            Slider.value = health;
            
            // If the health is 0, invoke the onPlayerDeath event
            bool isDead = health <= 0;
            if (isDead) onPlayerDeath.Invoke();
        }
    }

    void Start()
    {
        slider = GetComponent<Slider>();
    }
    
    public void OnDeath()
    {
        Debug.Log("Player has died!");
    }
}
