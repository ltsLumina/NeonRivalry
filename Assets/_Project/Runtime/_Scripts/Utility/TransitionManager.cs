using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] RawImage transitionImage;
    
    void Start()
    {
        transitionImage.enabled = true;
    }
}
