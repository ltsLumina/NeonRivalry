using UnityEngine;

public class CounterScript : MonoBehaviour
{
    Animator counterAnimator;

    void Start() => counterAnimator = GetComponent<Animator>();

    public void ShowCounterText()
    {
        counterAnimator.Play("CounterAnimation");
    }
}
