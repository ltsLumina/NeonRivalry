using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconAndNameUI : MonoBehaviour
{
    [SerializeField] Image shelbyIcon;
    [SerializeField] TextMeshProUGUI shelbyName;

    [SerializeField] Image morpheIcon;
    [SerializeField] TextMeshProUGUI morpheName;

    [SerializeField] string playerTag; // Add this line

    Healthbar playerHealthbar;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag(playerTag) != null);
        
        // Find the healthbar by tag
        playerHealthbar = GameObject.FindWithTag(playerTag).GetComponent<Healthbar>();
    }

    void Update()
    {
        if (playerHealthbar == null || playerHealthbar.Player == null) return;

        // Initialize the icon and name visibility to false
        shelbyIcon.enabled = false;
        shelbyName.enabled = false;
        morpheIcon.enabled = false;
        morpheName.enabled = false;

        // Enable the icon and name if the corresponding character is present
        if (playerHealthbar.Player.Character.CharacterPrefab.name == "Shelby")
        {
            shelbyIcon.enabled = true;
            shelbyName.enabled = true;
        }

        if (playerHealthbar.Player.Character.CharacterPrefab.name == "Morphe")
        {
            morpheIcon.enabled = true;
            morpheName.enabled = true;
        }
    }
}
