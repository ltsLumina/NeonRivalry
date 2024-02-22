using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class AnimationEndEvent : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(DestroySelf), 1);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
