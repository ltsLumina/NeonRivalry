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

        // Check if the effect is already playing
        if (effect.activeSelf)
        {
            ObjectPool pool = ObjectPoolManager.FindObjectPool(effect);
            pool.transform.parent = effect.transform.parent;
            GameObject newEffect = pool.GetPooledObject(); 
            
            newEffect.SetActive(true);
            StartCoroutine(DisableEffectAfterAnimation(newEffect));
            
            return;
        }
        
        // Enable the effect
        effect.SetActive(true);

        // Start the coroutine to disable the effect after the animation has finished
        StartCoroutine(DisableEffectAfterAnimation(effect));
    }

    IEnumerator DisableEffectAfterAnimation(GameObject effect)
    {
        var length = effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        
        // Wait for the duration of the animation
        yield return new WaitForSecondsRealtime(length);

        // Disable the effect
        effect.SetActive(false);
    }

    void PlayHitstun()
    {
        // var player = GetComponentInParent<PlayerController>();
        // player.FreezePlayer(true, player.HitBox.MoveData.hitstunDuration, true);
    }
}
