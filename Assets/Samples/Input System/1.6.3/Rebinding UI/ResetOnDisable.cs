using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public sealed class ResetOnDisable : MonoBehaviour
{
    RectTransform rect;
    Button button;

    void OnEnable()
    {
        rect   = GetComponent<RectTransform>();
        button = GetComponentInParent<Button>();
    }

    void OnDisable()
    {
        // Set the left value of the rect to 250.
        rect.offsetMin = new (250, rect.offsetMin.y);
        
        // Reset the button's scale to 1.
        button.transform.localScale = Vector3.one;
    }
}
