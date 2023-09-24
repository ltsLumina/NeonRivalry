#region
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
#endregion

public class TimerManager : MonoBehaviour
{
    enum TimerFormats
    {
        Whole,
        TenthDecimal,
        HundredthsDecimal,
    }

    [Header("Reference"), Space(5)]
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Timer Settings"), Space(5)]
    [SerializeField] float currentTime;
    [SerializeField] bool countdown;

    [Header("Limit Settings"), Space(5)]
    [SerializeField] bool hasLimit;
    [SerializeField] float timerLimit;

    [Header("Format Settings"), Space(5)]
    [SerializeField] bool hasFormat;
    [SerializeField] TimerFormats format;

    
    // Cached References
    readonly Dictionary<TimerFormats, string> timeFormats = new ();

    void Awake() => InitializeTimeFormats();

    void Update() => ProcessTimeUpdate();

    void InitializeTimeFormats()
    {
        timeFormats.Add(TimerFormats.Whole, "0");
        timeFormats.Add(TimerFormats.TenthDecimal, "0.0");
        timeFormats.Add(TimerFormats.HundredthsDecimal, "0.00");
    }

    void ProcessTimeUpdate()
    {
        UpdateCurrentTime();
        SetTimerText();

        if (IsTimeLimitReached())
        {
            LimitCurrentTime();
            SetTimerText();
        }
    }

    void UpdateCurrentTime() => currentTime = countdown ? currentTime - Time.deltaTime : currentTime + Time.deltaTime;

    bool IsTimeLimitReached() => hasLimit && (countdown && currentTime <= timerLimit || !countdown && currentTime >= timerLimit);

    void LimitCurrentTime() => currentTime = timerLimit;

    void SetTimerText()
    {
        timerText.text = hasFormat ? currentTime.ToString(timeFormats[format]) : currentTime.ToString(CultureInfo.CurrentCulture);
        SetTimerTextColor();
    }

    void SetTimerTextColor()
    {
        if (currentTime <= 20.5) timerText.color = Color.red;
    }
}
