#region
using UnityEngine;
using UnityEngine.UI;
#endregion

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class ImageFillGradient : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] Image image;

    void Awake() => image = GetComponent<Image>();

    void Update() => image.color = gradient.Evaluate(image.fillAmount);
}
