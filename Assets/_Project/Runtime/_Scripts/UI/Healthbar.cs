using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Reference"), Space(5)]
    [SerializeField, ReadOnly] Slider slider;
    [SerializeField, ReadOnly] PlayerController player;
    //[SerializeField, ReadOnly] PlayerStats playerStats;

    /// <summary>
    /// An event that is invoked when the player's health changes.
    /// Can be subscribed to in order to perform actions whenever the player's health changes.
    /// </summary>
    public delegate void HealthChanged(int value);
    public event HealthChanged OnHealthChanged;
    
    // -- Properties --
    
    /// <summary> The slider associated with this <see cref="Healthbar"/>. </summary>
    public Slider Slider
    {
        get => slider;
        private set => slider = value;
    }

    /// <summary> The player associated with this <see cref="Healthbar" />. </summary>
    public PlayerController Player
    {
        get => player;
        set => player = value;
    }
    
    /// <summary> The player's health associated with this <see cref="Healthbar" />. </summary>
    public int Value
    {
        get => (int) Slider.value;
        set
        {
            if (Value != value)
            {
                Slider.value = value;
                OnHealthChanged?.Invoke(value);
            }
        }
    }

    void Awake()
    {
        Slider = GetComponent<Slider>();

        Initialize();
    }

    public void Initialize()
    {
        if (Player != null)
        {
            Value = (int)Slider.maxValue;
        }
        else
        {
            Value = 0;
        }
    }
}