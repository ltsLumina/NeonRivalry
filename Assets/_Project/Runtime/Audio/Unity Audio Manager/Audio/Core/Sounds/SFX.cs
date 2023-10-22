using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace FLZ.Audio
{
    public class SFX : ScriptableObject, ISound
    {
        [System.Serializable]
        public struct Sound
        {
            public AudioClip Clip;
            [Range(0, 1f)] public float Volume;
            [Range(0f, 5f)] public float Pitch;
            [Range(0f, 5f)] public float Delay;

            public Sound(AudioClip clip, float volume, float pitch, float delay)
            {
                Clip = clip;
                Volume = volume;
                Pitch = pitch;
                Delay = delay;
            }

            public Sound(AudioClip clip) : this()
            {
                Clip = clip;
                Volume = 1;
                Pitch = 1;
                Delay = 0;
            }
        }
        
        public SFX(Sound[] sounds)
        {
            _sounds = sounds;
        }
            
        public AudioMixerGroup Mixer;
        [SerializeField] private Sound[] _sounds;
        

        [Range(0, 1)] public float GlobalVolume = 1;
        public bool RandomizeVolume;
        [MinMaxRange(-1f, 1f)] public MinMaxRange VolumeRange;

        public bool RandomizePitch;
        [MinMaxRange(-2f, 2f)] public MinMaxRange PitchRange;

        public bool Loop;
        [Range(-1, 1)] public float PanStereo = 0;
        
        public bool RandomizeDelay;
        [MinMaxRange(0f, 2f)] public MinMaxRange DelayRange;
        

        public int GetSoundCount() => _sounds?.Length ?? 0;
        
        private AudioClip GetClip(int index)
        {
            return _sounds[index].Clip;
        }

        private float GetVolume(int index)
        {
            return _sounds[index].Volume * GlobalVolume + (RandomizeVolume ? VolumeRange.Value : 0f);
        }

        private float GetPitch(int index)
        {
            return _sounds[index].Pitch + (RandomizePitch ? PitchRange.Value : 0f);
        }

        private float GetDelay(int index)
        {
            return _sounds[index].Delay + (RandomizeDelay ? DelayRange.Value : 0f);
        }
        
        public bool IsLooping() => Loop;

        public PoolableAudioSource Play()
        {
            return AudioManager.Play(this, Mixer);
        }

        public void Stop()
        {
            AudioManager.Stop(this);
        }

        /// <summary>
        /// Picks a random clip, determines its volume, pitch and delay and returns it in a SoundSettings
        /// </summary>
        public SoundSettings GetSoundSettings()
        {
            if (_sounds == null)
                return default;
                   
            return GetSoundSettings(Random.Range(0, _sounds.Length));
        }

        private SoundSettings GetSoundSettings(int index)
        {
            if (index < 0 || index >= _sounds.Length)
                return default;

            return new SoundSettings(
                clip: GetClip(index), 
                volume: GetVolume(index), 
                pitch: GetPitch(index), 
                panStereo: this.PanStereo, 
                delay: GetDelay(index),
                loop: this.Loop);
        }
        
        public bool HasAnyNullClip()
        {
            return _sounds == null || _sounds.Any(sound => sound.Clip == null);
        }
    }
}