using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI winner;
    Slider[] sliders;
    [Header("Values")]
    public float animationDuration = 2f;

    private void Awake()
    {
        sliders = GetComponentsInChildren<Slider>();
    }

    private void Start()
    {
        StartCoroutine(RandomizeSliderValues(1, 100));
        if (FindObjectOfType<RoundManager>() == null) { return; }

        //FindObjectOfType<RoundManager>().player1Victory = true;
        //winner.text = FindObjectOfType<RoundManager>().player1Victory ? "Player 1 Has Won" : "Player 2 Has Won";

    }

    /// <summary>
    /// This is the method in charge of randomizing the showcased slider values in the result screen.
    /// </summary>
    /// <param name="minValue">Minimum range value which serves as a basis for the sliders</param>
    /// 
    /// <param name="maxValue">Maximum range value which serves as a basis for the sliders</param>
    /// <returns></returns>
    IEnumerator RandomizeSliderValues(float minValue, float maxValue)
    {
        foreach (Slider slider in sliders)
        {
            // Generate a random value within the specified range
            float randomValue = Random.Range(minValue, maxValue) + Random.Range(1, 10) * Random.Range(0.05f, 2) / Random.Range(0.5f, 3);

            // Lerp the slider to the random value. The bool makes the slider fill in differently, I like true more.
            slider.DOValue(randomValue, animationDuration, true);
        }

        yield return null;
    }
}