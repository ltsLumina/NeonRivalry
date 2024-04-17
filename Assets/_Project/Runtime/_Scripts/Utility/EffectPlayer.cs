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
    [SerializeField] GameObject TEMP;

    GameObject pooledObject;
    
    // -- Sounds -- \\
    
    Sound overheadSFX;
    Sound overheadWindupSFX;
    Sound slashSFX;
    Sound uppercutSFX;
    Sound hookSFX;
    Sound aerialSFX; 
    Sound barStepSFX;
    Sound blockSFX;
    Sound jumpSFX;
    Sound hitSFX;

    void PlayOverheadEffect() => PlayEffect(overhead);
    void PlaySlashEffect() => PlayEffect(slash);
    void PlayUppercutEffect() => PlayEffect(uppercut);
    void PlayHookEffect() => PlayEffect(hook);
    void PlayAerialEffect() => PlayEffect(aerial);

    void PlayOverheadSound() => PlaySound(overheadWindupSFX, 1.0f, Output.SFX, new (0.9f, 1f), 0.1f);
    void PlayOverheadWindupSound() => PlaySound(overheadSFX, 1.5f, Output.SFX, new (0.50f, 0.50f), 0.3f);
    void PlaySlashSound() => PlaySound(slashSFX, 0.6f, Output.SFX, new (1.30f, 1.50f));
    void PlayUppercutSound() => PlaySound(uppercutSFX, 1.0f, Output.SFX, new (0.80f, 1.00f), 0.15f);
    void PlayHookSound() => PlaySound(hookSFX, 1.0f, Output.SFX, new (0.60f, 0.80f), 0.15f);
    void PlayAerialSound() => PlaySound(aerialSFX, 3.0f, Output.SFX, new (0.85f, 1.15f), 0.15f);
    void PlayBarStepSound() => PlaySound(barStepSFX, 4.0f, Output.SFX, new (0.75f, 0.85f));
    void PlayBlockSound() => PlaySound(blockSFX, 0.6f, Output.SFX, new (1.20f, 1.40f), 0.15f);
    void PlayJumpSound() => PlaySound(jumpSFX, 0.3f, Output.SFX, new (1.15f, 1.30f));
    void PlayHitSound() => PlaySound(hitSFX, 1f, Output.SFX, new (0.95f, 1.05f));

    void Start()
    {
        overheadSFX       = new (SFX.Attack);
        overheadWindupSFX = new (SFX.Attack);
        slashSFX          = new (SFX.Attack);
        uppercutSFX       = new (SFX.Attack);
        hookSFX           = new (SFX.Attack);
        aerialSFX         = new (SFX.Aerial);
        barStepSFX        = new (SFX.BarStep);
        blockSFX          = new (SFX.Block);
        jumpSFX           = new (SFX.Jump);
        hitSFX            = new (SFX.Hit);
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

    public static void PlaySound(Sound sound, float volume = default, Output output = default, Vector2 randomPitch = default, float fadeOut = default)
    {
        sound.SetSpatialSound(false);
        sound.SetVolume(volume).SetOutput(output).SetRandomPitch(randomPitch).SetFadeOut(fadeOut).Play();
    }
}
