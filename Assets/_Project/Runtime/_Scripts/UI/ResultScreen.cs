#region
using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#endregion

public class ResultScreen : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI winner;
    [SerializeField] Button p1Rematch;
    [SerializeField] Button p2Rematch;
    
    [Header("Values")]
    [SerializeField] float animationDuration = 2f;

    Slider[] sliders;
    void Awake() => sliders = GetComponentsInChildren<Slider>();

    void Start()
    {
        Reset();
        RandomizeSliderValues(1, 100);
    }

    void Reset()
    {
        foreach (Slider slider in sliders)
        {
            slider.value = 0;
        }
    }

    public IEnumerator EnableResultScreen(PlayerController winningPlayer)
    {
        // Disable the timer.
        RoundTimer timer = FindObjectOfType<RoundTimer>();
        timer.gameObject.SetActive(false);
        
        yield return new WaitUntil(() => FindObjectsOfType<PlayerController>().Length == 2);
        
        // Disable the players
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            player.DisablePlayer(true);
        }
        
        transform.GetChild(0).position = new (0, 1000, 0);
        gameObject.SetActive(true);

        transform.GetChild(0).DOLocalMoveY(0, 3f).SetEase(Ease.OutBack);
        StartCoroutine(Navigate());

        RandomizeSliderValues(1, 100);

        winner.text = winningPlayer ? "Player 1 Has Won" : "Player 2 Has Won";
    }

    IEnumerator Navigate()
    {
        yield return new WaitUntil(() => FindObjectsOfType<MenuNavigator>().Length == 2);
        
        var p1 = FindObjectsOfType<MenuNavigator>().FirstOrDefault(p => p.PlayerID == 1);
        var p2 = FindObjectsOfType<MenuNavigator>().FirstOrDefault(p => p.PlayerID == 2);
        
        p1.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(p1Rematch.gameObject);
        p2.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(p2Rematch.gameObject);
        
        Debug.Log("Navigated to rematch buttons.", p1.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject);
    }

    /// <summary>
    ///     This is the method in charge of randomizing the showcased slider values in the result screen.
    /// </summary>
    /// <param name="minValue">Minimum range value which serves as a basis for the sliders</param>
    /// <param name="maxValue">Maximum range value which serves as a basis for the sliders</param>
    /// <returns></returns>
    void RandomizeSliderValues(float minValue, float maxValue)
    {
        foreach (Slider slider in sliders)
        {
            // Generate a random value within the specified range
            float randomValue = Random.Range(minValue, maxValue) + Random.Range(1, 10) * Random.Range(0.05f, 2) / Random.Range(0.5f, 3);

            // Lerp the slider to the random value. The bool makes the slider fill in differently, I like true more.
            slider.DOValue(randomValue, animationDuration, true);
        }
    }
}
