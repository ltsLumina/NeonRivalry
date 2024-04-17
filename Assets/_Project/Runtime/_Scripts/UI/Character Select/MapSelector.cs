using MelenitasDev.SoundsGood;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [SerializeField] Image level1;
    [SerializeField] Image level2;
    
    public int SelectedMap => level1.gameObject.GetComponent<Image>().enabled ? 1 : 2;
    
    void Start()
    {
        level1.gameObject.GetComponent<Image>().enabled = false;
        level2.gameObject.GetComponent<Image>().enabled = false;
    }

    void OnEnable()
    {
        // Make all players select the first level by default.
        SelectLevel(1);
    }

    void OnDisable()
    {
        level1.gameObject.GetComponent<Image>().enabled = false;
        level2.gameObject.GetComponent<Image>().enabled = false;
    }

    public bool IsSelecting()
    {
        return level1.gameObject.GetComponent<Image>().enabled || level2.gameObject.GetComponent<Image>().enabled;
    }

    
    // Player 1 chooses the map.
    void Update()
    {
        MapSelector mapSelector = FindObjectOfType<MapSelector>();
        if (mapSelector == null) return;
        
        foreach (var player in PlayerManager.MenuNavigators)
        {
            player.PlayNavigationSounds = false;
            
            var currentSelected = player.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject;

            if (currentSelected == mapSelector.transform.GetChild(0).gameObject)
            {
                mapSelector.ShowLevel1();
                
                // make the other player also select the first level
                foreach (var otherPlayer in PlayerManager.MenuNavigators)
                {
                    if (otherPlayer != player)
                    {
                        var eventSystem = otherPlayer.GetComponent<MultiplayerEventSystem>();
                        eventSystem.SetSelectedGameObject(mapSelector.transform.GetChild(0).gameObject);
                    }
                }
            }
            else if (currentSelected == mapSelector.transform.GetChild(1).gameObject)
            {
                mapSelector.ShowLevel2();
                
                // make the other player also select the second level
                foreach (var otherPlayer in PlayerManager.MenuNavigators)
                {
                    if (otherPlayer != player)
                    {
                        var eventSystem = otherPlayer.GetComponent<MultiplayerEventSystem>();
                        eventSystem.SetSelectedGameObject(mapSelector.transform.GetChild(1).gameObject);
                    }
                }
            }
        }
    }

    public void ShowLevel1()
    {
        level1.gameObject.GetComponent<Image>().enabled = true;
        level2.gameObject.GetComponent<Image>().enabled = false;
    }

    public void ShowLevel2()
    {
        level1.gameObject.GetComponent<Image>().enabled = false;
        level2.gameObject.GetComponent<Image>().enabled = true;
    }

    public void SelectLevel(int level)
    {
        level1.gameObject.GetComponent<Image>().enabled = level == 1;
        level2.gameObject.GetComponent<Image>().enabled = level == 2;
        
        foreach (var player in FindObjectsOfType<MenuNavigator>())
        {
            var eventSystem = player.GetComponent<MultiplayerEventSystem>();
            eventSystem.SetSelectedGameObject(level == 1 ? level1.gameObject : level2.gameObject);
        }
    }

    public void LoadMap()
    {
        Debug.Log("Selected map: " + SelectedMap);
        var transitionAnimator = FindObjectOfType<TransitionAnimator>();
        transitionAnimator.sceneNameToLoad = SelectedMap == 1 ? "Bar" : "Street";
        transitionAnimator.gameObject.SetActive(true);
        
        PlayConfirmSound();
    }

    void PlayConfirmSound()
    {
        Sound CSConfirm = new (SFX.CSConfirm);
        CSConfirm.SetOutput(Output.SFX).SetVolume(0.75f);
        CSConfirm.Play();
    }
}
