// TODO:
// Fade in/fade out musics
// Intro played once and Clip looping
// Composite music, parameters
// Fancy custom editor with clip timeline

using UnityEngine;

namespace FLZ.Audio
{
    public class Music : ScriptableObject, ISound
    {
        // public AudioClip Intro;
        // public AudioClip Clip;
        // public AudioMixerGroup AudioMixerGroup;
        //
        // [Space(6)] 
        //
        // public bool Loop;
        //
        // [Range(0f, 1f)] public float Volume = 1f;
        // [Range(0.05f, 10f)] public float Pitch = 1f;
        // [Range(-1f, 1f)] public float PanStereo = 0f;
        //
        // public Music(AudioClip intro, AudioClip clip, bool loop = true)
        // {
        //     Intro = intro;
        //     Clip = clip;
        //     Loop = true;
        // }
        //
        // public bool IsLooping() => Loop;

        public SoundSettings GetSoundSettings()
        {
            throw new System.NotImplementedException();
        }

        public bool IsLooping()
        {
            throw new System.NotImplementedException();
        }

        public PoolableAudioSource Play()
        {
            throw new System.NotImplementedException();
        }
    }
}