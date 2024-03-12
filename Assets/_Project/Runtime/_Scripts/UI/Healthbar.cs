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

    //[SerializeField, ReadOnly] PlayerStats playerStats;

    /// <summary>
    /// An event that is invoked when the player's health changes.
    /// Can be subscribed to in order to perform actions whenever the player's health changes.
    /// </summary>
    public delegate void HealthChanged(int value);
    public event HealthChanged OnHealthChanged;
    
    public delegate void PlayerDeath(PlayerController player = default);
    public static event PlayerDeath OnPlayerDeath;
    
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
                comboTimer = 0.75f; // The time a combo lasts. Once it reaches 0, the healthbar will begin to tween.
                OnHealthChanged?.Invoke(value);

                if (value <= 0) OnPlayerDeath?.Invoke(Player);
            }
        }
    }

    /// <summary>
    /// Used to determine whether or not the player is invincible.
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
        if (Player != null) Value = (int)Slider.maxValue;
        else Value = 0;

        if (Player != null) comboTimer = 0.5f;
    }

    void Update()
    {
        if (Invincible) Value = 100;

        if(comboTimer > 0)
        {
            isTweening = false;
            comboTimer -= Time.deltaTime;
        }

        if (comboTimer <= 0 && !isTweening)
        {
            StartCoroutine(UpdateHealthBar(Value, tweenSpeed, inEase, outEase));
        }
    }

    IEnumerator UpdateHealthBar(float currentHealth, float speed, Ease inEase, Ease outEase)
    {
        isTweening = true;
        comboVisualSlider.DOValue(currentHealth, speed, true).SetEase(inEase).SetEase(outEase);

        yield return new WaitForEndOfFrame();
    }
}