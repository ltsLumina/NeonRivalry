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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) { counterAnimator.Play("CounterAnimation"); }
    }
}
