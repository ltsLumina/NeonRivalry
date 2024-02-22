using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winner;
    // Get all sliders in the children of this GameObject
    Slider[] sliders;
    public float animationDuration = 2f;

    private void Awake()
    {
        sliders = GetComponentsInChildren<Slider>();
    }

    private void Start()
    {
        if (FindObjectOfType<RoundManager>() == null) { return; }

        FindObjectOfType<RoundManager>().player1Victory = true;
        winner.text = FindObjectOfType<RoundManager>().player1Victory ? "Player 1 Has Won" : "Player 2 Has Won";

    }

    public IEnumerator RandomizeSliderValues(float minValue, float maxValue)
    {
        foreach (Slider slider in sliders)
        {
            // Generate a random value within the specified range
            float randomValue = Random.Range(minValue, maxValue) + Random.Range(1, 10) * Random.Range(0.05f, 2) / Random.Range(0.5f, 3);
            // Lerp the slider to the random value. The bool makes the slider fill in differently, I like true more.
            slider.DOValue(randomValue, animationDuration);
        }

        yield return null;
    }

    public void testmethod()
    {

    }
}