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

    GameObject pooledObject;
    
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
            var newEffect = GetObject(effect);
            newEffect.transform.position         = effect.transform.position;
            newEffect.transform.rotation         = effect.transform.rotation;
            newEffect.transform.localScale       = effect.transform.localScale;
            newEffect.transform.localEulerAngles = effect.transform.localEulerAngles;
            newEffect.transform.SetParent(effect.transform.parent);

            newEffect.SetActive(true);
            StartCoroutine(DisableEffectAfterAnimation(newEffect));
            return;
        }
        
        // Enable the effect
        effect.SetActive(true);

        // Start the coroutine to disable the effect after the animation has finished
        StartCoroutine(DisableEffectAfterAnimation(effect));
    }

    GameObject GetObject(GameObject effect)
    {
        if (pooledObject == null) pooledObject = Instantiate(effect);
        else if (pooledObject.activeInHierarchy) pooledObject = Instantiate(effect);

        return pooledObject;
    }

    IEnumerator DisableEffectAfterAnimation(GameObject effect)
    {
        var length = effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        
        // Wait for the duration of the animation
        yield return new WaitForSecondsRealtime(length);

        effect.SetActive(false);
    }

    void PlayHitstun()
    {
        // var player = GetComponentInParent<PlayerController>();
        // player.FreezePlayer(true, player.HitBox.MoveData.hitstunDuration, true);
    }
}
