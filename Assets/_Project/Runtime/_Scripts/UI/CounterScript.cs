using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterScript : MonoBehaviour
{
    public void ShowCounterText(Animator counterTextAnimator)
    {
         {  counterTextAnimator.Play("CounterAnimation"); }
    }
}
