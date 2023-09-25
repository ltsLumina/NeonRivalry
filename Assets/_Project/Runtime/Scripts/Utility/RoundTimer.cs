#region
using System.Collections.Generic;
using System.Globalization;
using Lumina.Essentials.Attributes;
using TMPro;
using UnityEngine;
#endregion

public class RoundTimer : MonoBehaviour
{
    enum TimerFormat
    {
        Whole,
        TenthDecimal,
        HundredthsDecimal,
    }

    [Header("Reference"), Space(10)]
    [SerializeField, ReadOnly] TextMeshProUGUI timerText;
    
    [Header("Timer Settings"), ReadOnly]
    [SerializeField] float currentTime;
    [SerializeField] bool countdownMode;
    
    [Header("Limit Settings"), Tooltip("If true, the timer will have a time limit.")]
    [SerializeField] bool hasTimeLimit;
    [SerializeField] float timeLimit;
    
    [Header("Format Settings")]
    [SerializeField, Tooltip("If true, the timer will use a custom format.")] 
    bool customFormat;
    [SerializeField, Tooltip("The value at which the timer will switch to a red color.")]
    float colorSwitchValue;
    [SerializeField, Tooltip("The value at which the timer will switch to a whole number format.")] 
    float tenthSwitchValue;
    [SerializeField, Tooltip("The value at which the timer will switch to a tenth decimal format.")]
    float hundredthsSwitchValue;

    // Cached Values
    readonly Dictionary<TimerFormat, string> timeFormats = new ();
    
    // -- Properties --
    public float CurrentTime
    {
        get => currentTime;
        private set => currentTime = value;
    }

    void Awake()
    {
        InitializeTimeFormats();
        FindTimerTextComponent();

        return;
        void FindTimerTextComponent()
        {
            var timerTextObject = GameObject.FindWithTag("[Header] User Interface").GetComponentInChildren<TextMeshProUGUI>();
            if (timerTextObject != null) timerText = timerTextObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (timerText == null) return;

        if (countdownMode) DecreaseTime(Time.deltaTime);
        else IncreaseTime(Time.deltaTime);

        UpdateTimerText();
    }

    void InitializeTimeFormats()
    {
        timeFormats.Add(TimerFormat.Whole, "0");
        timeFormats.Add(TimerFormat.TenthDecimal, "0.0");
        timeFormats.Add(TimerFormat.HundredthsDecimal, "0.00");
    }

    #region Count Up/Down Methods
    void IncreaseTime(float delta)
    {
        CurrentTime += delta;
        if (hasTimeLimit && CurrentTime > timeLimit) CurrentTime = timeLimit;
    }

    void DecreaseTime(float delta)
    {
        CurrentTime -= delta;
        if (CurrentTime < 0.0f) CurrentTime = 0.0f;
        if (hasTimeLimit && CurrentTime < timeLimit) CurrentTime = timeLimit;
    }

    public void SetTimer(float newTime) => CurrentTime = newTime;
    
    public void ResetTimer() => CurrentTime = timeLimit;
    #endregion

    void UpdateTimerText()
    {
        TimerFormat currentTimerFormat = customFormat ? GetTimerFormat() : TimerFormat.Whole;
        timerText.text  = CurrentTime.ToString(timeFormats[currentTimerFormat], CultureInfo.CurrentCulture);
        
        timerText.color = CurrentTime <= colorSwitchValue ? Color.red : new (0.86f, 0.86f, 0.86f);
    }

    TimerFormat GetTimerFormat() => CurrentTime < hundredthsSwitchValue ? TimerFormat.HundredthsDecimal : CurrentTime < tenthSwitchValue ? TimerFormat.TenthDecimal : TimerFormat.Whole;
}