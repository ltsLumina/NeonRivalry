using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        HealthBar.TakingDamage();
    }
}