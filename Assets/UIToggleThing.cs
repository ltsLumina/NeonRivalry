using UnityEngine;
using UnityEngine.UI;

public class UIToggleThing : MonoBehaviour
{
    [SerializeField] Sprite checkmark;
    [SerializeField] Sprite x;
    
    public void Toggle()
    { 
        var child = transform.GetChild(0);
        var toggle = transform.parent.GetComponent<Toggle>();
        child.GetComponent<Image>().sprite = toggle.isOn ? checkmark : x;
    }
}
