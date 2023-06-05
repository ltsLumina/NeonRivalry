using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerController player;
    NewHealthbar newHealthbar;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        newHealthbar = FindObjectOfType<NewHealthbar>();
        
        // Set the player's health to the max value.
        SetHealth();
    }

    void SetHealth()
    {
        newHealthbar.LHealthbar.value = 100;
        newHealthbar.RHealthbar.value = 100;
    }
}
