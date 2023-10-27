using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterScript : MonoBehaviour
{
    Animator counterAnimator;

    private void Start()
    {
        counterAnimator = GetComponent<Animator>();
    }

    public void ShowCounterText()
    {
         {counterAnimator.Play("CounterAnimation"); }
    }
}
