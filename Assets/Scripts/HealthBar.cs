using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider lHealthBar;
    [SerializeField] Slider backLHealthBar;
    [SerializeField] Slider rHealthBar;
    [SerializeField] float changeRate;

    public static Slider[] healthBars;
    public static Slider[] backHealthBars;

    static float currentValue;
    static readonly float minValue = 0;
    static readonly float maxValue = 100;

    void Start()
    {
        healthBars = new Slider[2];
        healthBars[1] = lHealthBar;
        healthBars[0] = rHealthBar;
        backHealthBars = new Slider[1];
        backHealthBars[0] = backLHealthBar;
    }

    void Update()
    {
        if (backLHealthBar.value > lHealthBar.value)
        {
            InvokeRepeating(nameof(ak), 0f, changeRate);
        }
    }

    public static void TakingDamage(Slider healthBar, Slider backHealthBar)
    {
        if (healthBar.value <= 0)
        {
            SceneManagerExtended.ReloadScene();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeHealthBar(20, healthBar);
            //ChangeBackHealthBar(20, backHealthBar);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeHealthBar(-20, healthBar);
        }
    }

    static void ChangeHealthBar(int damage, Slider healthBar)
    {
        healthBar.value -= damage;
    }

    static void ChangeBackHealthBar(int damage, Slider backHealthBar)
    {
        backHealthBar.value -= damage;
    }

    void ak()
    {
        if (backLHealthBar.value !> lHealthBar.value)
        {
            CancelInvoke();
        }
        
        currentValue = backLHealthBar.value;
        currentValue -= 1;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        backLHealthBar.value = currentValue;
    }
}