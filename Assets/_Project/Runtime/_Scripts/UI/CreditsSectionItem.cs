

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsSectionItem : MonoBehaviour
{
    [Header("Resources")]
    public HorizontalLayoutGroup headerLayout;
    public VerticalLayoutGroup listLayout;
    [SerializeField] TextMeshProUGUI headerText;
    public GameObject namePreset;

    public void AddNameToList(string name)
    {
        GameObject go = Instantiate(namePreset, new (0, 0, 0), Quaternion.identity);
        go.transform.SetParent(listLayout.transform, false);
        go.name = name;

        var goText = go.GetComponent<TextMeshProUGUI>();
        goText.text = name;
    }

    public void SetHeader(string text) => headerText.text = text;
}