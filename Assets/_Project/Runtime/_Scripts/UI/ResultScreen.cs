using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winner;

    private void Start()
    {
        RandomizeSliderValues(1, 100);
        

        FindObjectOfType<RoundManager>().player1Victory = true;
        if (FindObjectOfType<RoundManager>() == null) { return; }
        winner.text = FindObjectOfType<RoundManager>().player1Victory ? "Player 1 Has Won" : "Player 2 Has Won"; 
    }

    // Function to randomize values for all sliders
    public void RandomizeSliderValues(float minValue, float maxValue)
    {
        // Get all sliders in the children of this GameObject
        Slider[] sliders = GetComponentsInChildren<Slider>();

        foreach (Slider slider in sliders)
        {
            // Generate a random value within the specified range
            float randomValue = Random.Range(minValue, maxValue) + Random.Range(1, 10) * Random.Range(0.05f, 2) / Random.Range(0.5f, 3);
            slider.value = randomValue;
        }
    }
}
