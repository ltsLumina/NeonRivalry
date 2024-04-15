using DG.Tweening;
using UnityEngine;

public class MainMenuButtonEffect : MonoBehaviour
{
    [SerializeField] MainMenuButtonEffectStats stats;
    
    Transform button;

    void InitialEffect()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(5f);

        // Add three quick scale up and down animations to the sequence
        for (int i = 0; i < 3; i++)
        {
            sequence.Append(button.DOScale(stats.scaleIncrease, 0.15f));
            sequence.Append(button.DOScale(1, 0.15f));
        }
        
        //sequence.OnComplete(Effect);
    }

    // void Effect()
    // {
    //     // Create a new sequence
    //     Sequence sequence = DOTween.Sequence();
    //     DOTween.SetTweensCapacity(500, 50);
    //     
    //     for (int i = 0; i < stats.downbeatEvents.Count; i++)
    //     {
    //         sequence.AppendInterval(stats.downbeatEvents[i].delay);
    //
    //         for (int b = 0; b < 3; b++)
    //         {
    //             sequence.Append(button.DOScale(stats.scaleIncrease, 0.15f));
    //             sequence.Append(button.DOScale(1, 0.15f));
    //         }
    //         
    //         // Once you've reached the final downbeat event, loop indefinitely using the last delay value
    //         if (i == stats.downbeatEvents.Count - 1)
    //         {
    //             sequence.AppendInterval(stats.downbeatEvents[i].delay);
    //             sequence.SetLoops(-1, LoopType.Restart);
    //         }
    //     }
    // }

    void Start()
    {
        button = GetComponent<Transform>();
        
        InitialEffect();
    }
}