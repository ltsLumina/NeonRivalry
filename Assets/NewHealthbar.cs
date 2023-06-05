using UnityEditor;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class NewHealthbar : MonoBehaviour
{
    [Header("Serialized References")]
    [SerializeField] Slider L_Healthbar;
    [SerializeField] Slider R_Healthbar;
    
    [Header("Optional Parameters")]
    [SerializeField] float changeRate;
    
    // Properties
    public Slider LHealthbar
    {
        get => L_Healthbar;
        set
        {
            // The health before deducting after a hit.
            float L_PreviousHealth = L_Healthbar.value;
            
            L_Healthbar = value;
            L_Healthbar.value = Mathf.Clamp(LHealthbar.value, 0, 100); 
            //TODO: Currently, health is clamped between 0 and 100.
            //TODO: If we want more or less, don't forget to edit this.
        }
    }
    public Slider RHealthbar
    {
        get => R_Healthbar;
        set
        {
            // The health before deducting after a hit.
            float R_PreviousHealth = R_Healthbar.value;
            
            R_Healthbar = value;
            R_Healthbar.value = Mathf.Clamp(RHealthbar.value, 0, 100);
        }
    }

    void Update()
    {
        AdjustHealthbar();
    }

    void AdjustHealthbar()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LHealthbar.value = Mathf.MoveTowards(LHealthbar.value, 0, changeRate * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RHealthbar.value -= 10;
        }
    }
}

[CustomEditor(typeof(NewHealthbar))]
public class NewHealthbarEditor : Editor
{
    //TODO:
}
