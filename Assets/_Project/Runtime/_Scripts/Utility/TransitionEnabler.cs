using UnityEngine;
using UnityEngine.UI;

public class TransitionEnabler : MonoBehaviour
{
    void Start() => GetComponent<RawImage>().enabled = true;
}
