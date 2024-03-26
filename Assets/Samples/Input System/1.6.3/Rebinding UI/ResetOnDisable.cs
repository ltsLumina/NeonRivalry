using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public sealed class ResetOnDisable : MonoBehaviour
{
    RectTransform rect;
    Button button;
    Slider slider;
    
    // Slider:
    RectTransform handleRect;
    Color? originalHandleRectColour;

    void OnEnable()
    {
        rect   = GetComponent<RectTransform>();
        button = GetComponentInParent<Button>();
        slider = TryGetComponent(out slider) ? slider : null;
        
        handleRect = slider ? slider.handleRect : null;
        originalHandleRectColour = handleRect?.GetComponent<Image>().color;
    }

    void OnDisable()
    {
        if (button)
        {
            // Set the left value of the rect to 250.
            rect.offsetMin = new (250, rect.offsetMin.y);
        
            // Reset the button's scale to 1.
            button.transform.localScale = Vector3.one;
        }
        else if (slider)
        {
            // Reset the color of the handle.
            if (originalHandleRectColour != null)   
                slider.handleRect.GetComponent<Image>().color = (Color) originalHandleRectColour;
        }
    }
}
