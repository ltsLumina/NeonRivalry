using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Reference")] [Space(5)]
    [SerializeField] [ReadOnly] Slider slider;
    [SerializeField] [ReadOnly] PlayerController player;
    [SerializeField] Slider comboVisualSlider;
    
    [Header("Settings")] [Space(5)]
    [SerializeField] float tweenSpeed;
    [SerializeField, Tooltip("Make sure the easing method is ONLY In*** \nIf you want to use the \"Linear\" method make sure the other is set to \"Unset\"")]
    Ease outEase;
    [SerializeField, Tooltip("Make sure the easing method is ONLY Out***")]
    Ease inEase;
    
    [Header("Debug")] [Space(5)]
    [SerializeField] bool invincible;
    [SerializeField, ReadOnly] float comboTimer;
    [SerializeField, ReadOnly] bool isTweening;

    public Healthbar(bool invincible) { Invincible = invincible; }

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
    public int Health
    {
        get => (int) Slider.value;
        set
        {
            if (Health != value)
            {
                if (Invincible) return;
                
                Slider.value = value;
                comboTimer = 0.75f; // The time a combo lasts. Once it reaches 0, the healthbar will begin to tween.
                OnHealthChanged?.Invoke(value);

                if (value <= 0 && Player != null) OnPlayerDeath?.Invoke(Player);
            }
        }
    }

    /// <summary>
    /// Used to determine whether the player is invincible.
    /// Only meant to be used for debugging purposes.
    /// </summary>
    public bool Invincible
    {
        get => invincible;
        set => invincible = value;
    }

    void Awake()
    {
        Slider = GetComponent<Slider>();

        Initialize();
    }

    public void Initialize()
    {
        if (Player != null) Health = (int)Slider.maxValue;
        else Health = 0;

        if (Player != null) comboTimer = 0.5f;
        HealthbarManager.Initialize(); // Subscribe to the OnPlayerDeath event, allowing the HealthbarManager to check if any player has died.
    }

    void Update()
    {
        if (Invincible) Health = 100;

        if(comboTimer > 0)
        {
            isTweening = false;
            comboTimer -= Time.deltaTime;
        }

        if (comboTimer <= 0 && !isTweening)
        {
            StartCoroutine(UpdateHealthBar(Health, tweenSpeed, inEase, outEase));
        }
    }

    IEnumerator UpdateHealthBar(float currentHealth, float speed, Ease inEase, Ease outEase)
    {
        isTweening = true;
        comboVisualSlider.DOValue(currentHealth, speed, true).SetEase(inEase).SetEase(outEase);

        yield return null;
    }
    
    public int Heal(PlayerController player, int amount)
    {
        if (Health + amount > Slider.maxValue) Health = (int)Slider.maxValue;
        else Health += amount;
        
        return Health;
    }
}