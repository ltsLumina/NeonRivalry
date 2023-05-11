using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider rHealthBar;
    [SerializeField] Slider lHealthBar;

    public void TakingDamage()
    {
        if (Input.GetKeyDown(KeyCode.R))
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

    void ChangeHealthBar(int damage)
    {
        lHealthBar.value -= damage;
        rHealthBar.value -= damage;
    }
}