using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static Slider healthBar;

    public void Start()
    {
        healthBar = GetComponent<Slider>();
    }

    public static void TakingDamage()
    {
        if (healthBar.value == 0)
        {
            SceneManagerExtended.ReloadScene();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeHealthBar(20);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeHealthBar(-20);
        }
    }

    public static void ChangeHealthBar(int damage)
    {
        healthBar.value -= damage;
    }
}