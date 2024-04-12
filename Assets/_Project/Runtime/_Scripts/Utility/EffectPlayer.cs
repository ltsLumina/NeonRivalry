#region
using System.Collections;
using UnityEngine;
#endregion

public class EffectPlayer : MonoBehaviour
{
    [SerializeField] GameObject overhead;
    [SerializeField] GameObject slash;
    [SerializeField] GameObject uppercut;
    [SerializeField] GameObject hook;
    [SerializeField] GameObject aerial;
    
    void PlayOverheadEffect() => PlayEffect(overhead);
    void PlaySlashEffect()    => PlayEffect(slash);
    void PlayUppercutEffect() => PlayEffect(uppercut);
    void PlayHookEffect()     => PlayEffect(hook);
    void PlayAerialEffect()   => PlayEffect(aerial);
    
    void PlayEffect(GameObject effect)
    {
        if (!SettingsManager.ShowEffects) return;

        // Enable the effect
        effect.SetActive(true);

        // Start the coroutine to disable the effect after the animation has finished
        StartCoroutine(DisableEffectAfterAnimation(effect));
    }

    IEnumerator DisableEffectAfterAnimation(GameObject effect)
    {
        // Wait for the duration of the animation
        yield return new WaitForSeconds(0.3f);

        // Disable the effect
        effect.SetActive(false);
    }

    void PlayHitstun()
    {
        // var player = GetComponentInParent<PlayerController>();
        // player.FreezePlayer(true, player.HitBox.MoveData.hitstunDuration, true);
    }
}
