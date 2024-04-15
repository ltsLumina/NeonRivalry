using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconAndNameUI : MonoBehaviour
{
    [SerializeField] Image shelbyIcon;
    [SerializeField] TextMeshProUGUI shelbyName;
    
    [SerializeField] Image morpheIcon;
    [SerializeField] TextMeshProUGUI morpheName;

    Healthbar leftHealthbar;
    Healthbar rightHealthbar;

    void Start()
    {
        // Find each healthbar by tag
        leftHealthbar  = GameObject.FindWithTag("[Healthbar] Left").GetComponent<Healthbar>();
        rightHealthbar = GameObject.FindWithTag("[Healthbar] Right").GetComponent<Healthbar>();
    }

    void Update()
    {
        if (leftHealthbar        == null || rightHealthbar        == null) return;
        if (leftHealthbar.Player == null || rightHealthbar.Player == null) return;

        // Initialize the icon and name visibility to false
        shelbyIcon.enabled = false;
        shelbyName.enabled = false;
        morpheIcon.enabled = false;
        morpheName.enabled = false;

        // Enable the icon and name if the corresponding character is present
        if (leftHealthbar.Player.Character.CharacterPrefab.name == "Shelby" || rightHealthbar.Player.Character.CharacterPrefab.name == "Shelby")
        {
            shelbyIcon.enabled = true;
            shelbyName.enabled = true;
        }

        if (leftHealthbar.Player.Character.CharacterPrefab.name == "Morphe" || rightHealthbar.Player.Character.CharacterPrefab.name == "Morphe")
        {
            morpheIcon.enabled = true;
            morpheName.enabled = true;
        }
    }
}
