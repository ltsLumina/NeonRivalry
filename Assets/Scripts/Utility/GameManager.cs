using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    HealthBar healthBar;

    void Start()
    {
        healthBar = FindObjectOfType<HealthBar>();
    }

    void Update()
    {
        healthBar.TakingDamage();
    }
}