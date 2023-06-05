using UnityEngine;
using static Essentials.Sequencing;

public class HealthbarManager : MonoBehaviour
{
    // Cached References
    NewHealthbar newHealthbar;

    void Start()
    {
        newHealthbar = FindObjectOfType<NewHealthbar>();
        
        // Set the player's health to the max value.
        SetHealth();
    }

    void Update() { DEBUG_ReduceHealth(); }

    void SetHealth()
    {
        newHealthbar.LeftHealthbarValue  = 100;
        newHealthbar.RightHealthbarValue = 100;
    }

    public void ReloadOnDeath()
    {
        // Unsubscribe from the onPlayerDeath event
        NewHealthbar.onPlayerDeath -= ReloadOnDeath;
        
        StartCoroutine(WaitForSeconds(() =>
        {
            Debug.Log("Either Player has died, reloading scene.");
            SceneManagerExtended.ReloadScene();
        }, 1f));
    }

    
    // DEBUGGING
    
    void DEBUG_ReduceHealth()
    { // !! FOR DEBUGGING PURPOSES !!
        // // You must hold the key down to decrement the health smoothly due to the nature of the Input.GetKey() method.
        if (Input.GetKey(KeyCode.L))
        {
            // This method does not kill the player, it only decrements the health to 1.
            newHealthbar.AdjustHealthbar(true, 1, 100);
        }
    }
}
