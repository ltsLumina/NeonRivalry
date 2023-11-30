using Lumina.Essentials.Attributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Reference"), Space(5)]
    [SerializeField, ReadOnly] Slider slider;
    [SerializeField, ReadOnly] PlayerController player;
    [SerializeField] Slider comboVisualSlider;

    [SerializeField, ReadOnly] float comboTimer;
    public Healthbar(bool invincible) { Invincible = invincible; }

    //[SerializeField, ReadOnly] PlayerStats playerStats;

    /// <summary>
    /// An event that is invoked when the player's health changes.
    /// Can be subscribed to in order to perform actions whenever the player's health changes.
    /// </summary>
    public delegate void HealthChanged(int value);
    public event HealthChanged OnHealthChanged;
    
    public delegate void PlayerDeath(PlayerController player);
    public event PlayerDeath OnPlayerDeath;
    
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
                if (Invincible) return;
                
                Slider.value = value;
                comboTimer = 0.5f;
                OnHealthChanged?.Invoke(value);

                if (value <= 0) OnPlayerDeath?.Invoke(Player);
            }
        }
    }

    /// <summary>
    /// Used to determine whether or not the player is invincible.
    /// Only meant to be used for debugging purposes.
    /// </summary>
    public bool Invincible { get; set; }

    void Awake()
    {
        Slider = GetComponent<Slider>();

        Initialize();
    }

    public void Initialize()
    {
        if (Player != null) Value = (int)Slider.maxValue;
        else Value = 0;

        if (Player != null) comboTimer = 0.5f;
    }

    private void Update()
    {
        if(comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }

        if (comboTimer <= 0)
        {
            comboVisualSlider.value = Value;
        }
    }
}