using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MaskMasker : MonoBehaviour
{
    [SerializeField] RectTransform mask;
    [SerializeField] Slider slider;
    [SerializeField] bool leftHealthbar;
    [SerializeField] bool rightHealthbar;

    void Start() => mask = GetComponent<RectTransform>();

    void Update()
    {
        Vector2 offset      = mask.offsetMax;
        var     otherOffset = mask.offsetMin;
    
        if (leftHealthbar) offset.x       = Map(slider.value, 0, 100, -710, 445);
        else if (rightHealthbar)
        {
            // // Set left to -545
            offset.x = Map(slider.value, 0, 100, -700, 55f);
        }

        mask.offsetMax = offset;
        mask.offsetMin = otherOffset;
    }

    static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}

// -545, 720
// 60
