using DG.Tweening;
using UnityEngine;

public class DOTweenTestScript : MonoBehaviour
{
    void Start()
    {
        transform.DOScaleX(2, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
