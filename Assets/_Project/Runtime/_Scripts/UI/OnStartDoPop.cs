using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnStartDoPop : MonoBehaviour
{
    public TextMeshProUGUI textComponentOnObject = null;
    public float Drag = 0.7f;
    public float Spring = 0.4f;
     public float TargetScaleFactor = 1.5f;
    [SerializeField] private float StartScale = 0.0f;
    [SerializeField] private float TargetScale = 0.0f;
    private bool invert = false;
    // Start is called before the first frame update

    private void Awake()
    {
        StartScale = textComponentOnObject.fontSize;
        TargetScale = textComponentOnObject.fontSize * TargetScaleFactor;
    }
    public void FontUp()
    {
        textComponentOnObject.fontSize = TargetScale;
        FontDown();

    }
    private void FontDown()
    {
        textComponentOnObject.fontSize = StartScale;
        TargetScale = textComponentOnObject.fontSize * TargetScaleFactor;
    }
    // Update is called once per frame
    void Update()
    {
        float scaleVelocity = TargetScale - textComponentOnObject.fontSize * Spring;
        scaleVelocity *= Drag;
        textComponentOnObject.fontSize += scaleVelocity;
        //if(textComponentOnObject.fontSize > TargetScale)
        //{
        //    TargetScale = StartScale;
        //}
    }
}
