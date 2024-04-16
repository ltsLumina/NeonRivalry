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
    void PlayerTempEffect() => PlayEffect(TEMP);

    void PlayOverheadSound() => PlaySound(overheadWindupSFX);
    void PlayOverheadWindupSound() => PlaySound(overheadSFX);
    void PlaySlashSound() => PlaySound(slashSFX);
    void PlayUppercutSound() => PlaySound(uppercutSFX);
    void PlayHookSound() => PlaySound(hookSFX);
    void PlayAerialSound() => PlaySound(aerialSFX);
    void PlayBarStepSound() => PlaySound(barStepSFX);
    void PlayBlockSound() => PlaySound(blockSFX);
    void PlayJumpSound() => PlaySound(jumpSFX);
    void PlayHitSound() => PlaySound(hitSFX);

    void Start()
    {
        // TODO: Uncomment this when the sound effects are added
        overheadSFX       = new Sound(SFX.Attack);
        overheadWindupSFX = new Sound(SFX.Attack);
        slashSFX          = new Sound(SFX.Attack);
        uppercutSFX       = new Sound(SFX.Attack);
        hookSFX           = new Sound(SFX.Attack);
        aerialSFX         = new Sound(SFX.Aerial);
        barStepSFX        = new Sound(SFX.BarStep);
        blockSFX          = new Sound(SFX.Block);
        jumpSFX           = new Sound(SFX.Jump);
        hitSFX            = new Sound(SFX.Hit);
        
        // Set the Output Mixer Group and Volume
        ConfigureAudio();
    }

    void ConfigureAudio()
    {
        // TODO: Same here
        overheadWindupSFX.SetVolume(1.0f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(0.75f, 0.75f)).SetFadeOut(0.1f);
        overheadSFX      .SetVolume(1.5f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(0.50f, 0.50f)).SetFadeOut(0.3f);
        slashSFX         .SetVolume(0.6f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(1.30f, 1.50f));
        uppercutSFX      .SetVolume(1.0f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(0.80f, 1.00f)).SetFadeOut(0.15f);
        hookSFX          .SetVolume(1.0f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(0.60f, 0.80f)).SetFadeOut(0.15f);
        aerialSFX        .SetVolume(3.0f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(0.85f, 1.15f)).SetFadeOut(0.15f);
        barStepSFX       .SetVolume(4.0f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(0.75f, 0.85f));
        blockSFX         .SetVolume(0.6f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(1.20f, 1.40f)).SetFadeOut(.15f);
        jumpSFX          .SetVolume(0.3f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch(new Vector2(1.15f, 1.30f));
        hitSFX           .SetVolume(0.6f).SetSpatialSound(false).SetOutput(Output.SFX).SetRandomPitch();
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
