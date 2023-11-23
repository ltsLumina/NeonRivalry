using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winner;  
    // Get all sliders in the children of this GameObject
    Slider[] sliders;
    public float animationDuration = 2f;
    public float targetValue = 1f;

    private void Awake()
    {
        sliders = GetComponentsInChildren<Slider>();

    }

    private void Start()
    {
        StartCoroutine(RandomizeSliderValues(1, 100));
        if (FindObjectOfType<RoundManager>() == null) { return; }

        FindObjectOfType<RoundManager>().player1Victory = true;
        winner.text = FindObjectOfType<RoundManager>().player1Victory ? "Player 1 Has Won" : "Player 2 Has Won"; 
    }

    IEnumerator RandomizeSliderValues(float minValue, float maxValue)
    {
        foreach (Slider slider in sliders)
        {
            // Generate a random value within the specified range
            float randomValue = Random.Range(minValue, maxValue) + Random.Range(1, 10) * Random.Range(0.05f, 2) / Random.Range(0.5f, 3);

            // Start the animation for each slider individually
            StartCoroutine(AnimateSlider(slider, randomValue));
        }

        yield return null;
    }

    IEnumerator AnimateSlider(Slider slider, float targetValue)
    {
        float startTime = Time.time;
        float startValue = slider.value;

        while (Time.time - startTime < animationDuration)
        {
            float progress = (Time.time - startTime  / animationDuration);
            slider.value = Mathf.Lerp(startValue, targetValue, progress );
            yield return null;
        }

        // Ensure the final value is exactly the target value
        slider.value = targetValue;
    }
}