#region
using System.Collections;
using UnityEngine;
#endregion

public class MusicPlayer : SingletonPersistent<MusicPlayer>
{
    [SerializeField] AudioClip defaultMusic;
    [SerializeField] float musicFadeDuration = 1f;
    [SerializeField, Range(0f, 1f)] float musicVolume = 1f;

    AudioSource activeAudioSource;
    AudioSource inactiveAudioSource;

    Coroutine currentFadeRoutine;

    void Start() => ChangeMusic(defaultMusic);

    /// <summary>
    ///     Changes the music currently playing.
    /// </summary>
    /// <param name="newMusic"></param>
    public void ChangeMusic(AudioClip newMusic = null)
    {
        ChangeAudioSource(newMusic);

        currentFadeRoutine = StartCoroutine(MusicFadeRoutine());
    }

    /// <summary>
    ///     Changes the currently used audio source to phase in the new music.
    /// </summary>
    /// <param name="newMusic"></param>
    void ChangeAudioSource(AudioClip newMusic)
    {
        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
            Destroy(inactiveAudioSource);
            currentFadeRoutine = null;
        }

        if (activeAudioSource != null) inactiveAudioSource = activeAudioSource;

        activeAudioSource      = gameObject.AddComponent<AudioSource>();
        activeAudioSource.clip = newMusic;
        activeAudioSource.loop = true;
    }

    /// <summary>
    ///     Fades out the old music and fades in the new music using the two audiosources.
    /// </summary>
    /// <returns></returns>
    IEnumerator MusicFadeRoutine()
    {
        activeAudioSource.Play();

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / musicFadeDuration;

            activeAudioSource.volume = Mathf.Lerp(activeAudioSource.volume, musicVolume, t);
            if (inactiveAudioSource != null) inactiveAudioSource.volume = Mathf.Lerp(inactiveAudioSource.volume, 0f, t);

            yield return new WaitForEndOfFrame();
        }

        if (inactiveAudioSource != null) Destroy(inactiveAudioSource);

        currentFadeRoutine = null;
    }
}
