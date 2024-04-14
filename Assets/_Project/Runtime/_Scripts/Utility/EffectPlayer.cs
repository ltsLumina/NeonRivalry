#region
using System.Collections;
using MelenitasDev.SoundsGood;
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
    
    // -- Sounds -- \\
    
    Sound overheadSFX;
    Sound slashSFX;
    Sound uppercutSFX;
    Sound hookSFX;
    Sound aerialSFX;

    void PlayOverheadEffect() => PlayEffect(overhead);
    void PlaySlashEffect() => PlayEffect(slash);
    void PlayUppercutEffect() => PlayEffect(uppercut);
    void PlayHookEffect() => PlayEffect(hook);
    void PlayAerialEffect() => PlayEffect(aerial);
    
    void PlayOverheadSound() => PlaySound(overheadSFX);
    void PlaySlashSound() => PlaySound(slashSFX);
    void PlayUppercutSound() => PlaySound(uppercutSFX);
    void PlayHookSound() => PlaySound(hookSFX);
    void PlayAerialSound() => PlaySound(aerialSFX);

    void Start()
    {
        // TODO: Uncomment this when the sound effects are added
        // overheadSFX = new Sound(SFX.Overhead);
        // slashSFX = new Sound(SFX.Slash);
        // uppercutSFX = new Sound(SFX.Uppercut);
        // hookSFX = new Sound(SFX.Hook);
        // aerialSFX = new Sound(SFX.Aerial);
        
        // Set the Output Mixer Group and Volume
        SetMixerAndVolume();
    }

    void SetMixerAndVolume()
    {
        // TODO: Same here
        // overheadSFX.SetOutput(Output.SFX).SetVolume(1);
        // slashSFX.SetOutput(Output.SFX).SetVolume(1);
        // uppercutSFX.SetOutput(Output.SFX).SetVolume(1);
        // hookSFX.SetOutput(Output.SFX).SetVolume(1);
        // aerialSFX.SetOutput(Output.SFX).SetVolume(1);
    }

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
    
    void PlaySound(Sound sound)
    {
        sound.Play();
    }

    void PlayHitstun()
    {
        // var player = GetComponentInParent<PlayerController>();
        // player.FreezePlayer(true, player.HitBox.MoveData.hitstunDuration, true);
    }
}
