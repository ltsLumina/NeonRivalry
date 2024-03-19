using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] Scrollbar scrollHelper;
    [SerializeField] RectTransform scrollContent;

    // Settings
    [SerializeField, Range(0, 10)]  float scrollDelay = 1.25f;
    [Range(0, 0.5f)] public float scrollSpeed = 0.05f;
    [Range(1.1f, 15)] public float boostValue = 3f;
    [SerializeField] InputAction boostHotkey;

    public UnityEvent onCreditsEnd = new ();
    
    bool enableScrolling;
    bool invokedEndEvents;
    
    void OnEnable()
    {
        StartScrolling();
    }

    void StartScrolling()
    {
        if (enableScrolling) return;

        StopCoroutine(nameof(StartTimer));

        enableScrolling    = false;
        scrollHelper.value = 1;

        if (scrollDelay != 0) StartCoroutine(nameof(StartTimer));
        else enableScrolling = true;
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(scrollDelay);
        enableScrolling = true;
    }
    
    void Update()
    {
        if (enableScrolling == false) return;

        if (boostHotkey.IsInProgress()) { scrollHelper.value -= (scrollSpeed * boostValue) * Time.deltaTime; }
        else { scrollHelper.value                            -= scrollSpeed                * Time.deltaTime; }

        if (scrollHelper.value <= 0.005f && invokedEndEvents == false)
        {
            onCreditsEnd.Invoke();
            invokedEndEvents = true;
        }

        if (scrollHelper.value <= 0)
        {
            enableScrolling = false;
            onCreditsEnd.Invoke();
        }
    }

    public void ResetCredits()
    {
        // Reset the credits to the top.
        scrollContent.transform.localPosition = new (0, 0, 0);
        
        
        StopCoroutine(nameof(StartTimer));
        enableScrolling    = false;
        invokedEndEvents   = false;
        scrollHelper.value = 1;
        StartScrolling();
    }
}
