using UnityEngine;
//using static Lumina.Essentials.Sequencing;

public class HealthbarManager : MonoBehaviour
{
    // Cached References
    Healthbar healthbar;
    RoundScript roundScript;

    void Start()
    {
        healthbar = FindObjectOfType<Healthbar>();
        roundScript = FindObjectOfType<RoundScript>();
        
        // Set the player's health to the max value.
        SetHealth();
    }

    void Update() { 
        if (Input.GetKeyDown(KeyCode.K)) { healthbar.LeftHealthbarValue = 0; roundScript.p2HasWon = true; }
        
        DEBUG_ReduceHealth(); }

    void SetHealth()
    {
        healthbar.LeftHealthbarValue  = 100;
        healthbar.RightHealthbarValue = 100;
    }

    // public void ReloadOnDeath()
    // {
    //     // Unsubscribe from the onPlayerDeath event
    //     Healthbar.onPlayerDeath -= ReloadOnDeath;
    //     
    //     StartCoroutine(DelayedAction(() =>
    //     {
    //         Debug.Log("Either Player has died, reloading scene.");
    //         SceneManagerExtended.ReloadScene();
    //     }, 1f));
    // }

    
    // DEBUGGING
    
    void DEBUG_ReduceHealth()
    { // !! FOR DEBUGGING PURPOSES !!
        // // You must hold the key down to decrement the health smoothly due to the nature of the Input.GetKey() method.
        if (Input.GetKey(KeyCode.L))
        {
            // This method does not kill the player, it only decrements the health to 1.
            healthbar.AdjustHealthbar(true, 0, 100);
        }
    }
}
