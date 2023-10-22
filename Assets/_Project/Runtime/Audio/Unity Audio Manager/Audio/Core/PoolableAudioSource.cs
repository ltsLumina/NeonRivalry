#if UNITY_EDITOR
//#define VERBOSE           // Enable/disable pool instances renaming (useful to keep a clean hierarchy but generates a lot of garbage) 
#endif

using System;
using FLZ.Pooling;
using FLZ.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace FLZ.Audio
{
    public class PoolableAudioSource : MonoBehaviour, IPoolable
    {
        private AudioSourcePool<PoolableAudioSource> _owner;
        
        private AudioSource _audioSource;
        public AudioSource AudioSource => _audioSource;
        
        private ISound _sound;
        public ISound Sound => _sound;

        private bool _looping;
        public bool Looping => _looping;

        private TimeSince _timeSinceLastPlay;
        public float TimeSinceLastPlay => _timeSinceLastPlay;
        
        private Action _endCallback;

        public bool IsPlaying() => _audioSource.isPlaying;

        
        private void Update()
        {
            if (_audioSource.clip == null) return;
            
            if (_timeSinceLastPlay > _audioSource.clip.length / _audioSource.pitch) 
            {
                if (_looping)
                {
                    Play(); 
                }
                else
                {
                    _owner.DeSpawn(this);
                }
            } 
        }

        public void SetupAndPlay(AudioSourcePool<PoolableAudioSource> owner, ISound sound, AudioMixerGroup mixer)
        {
            _owner = owner;
            _sound = sound;
            _audioSource.outputAudioMixerGroup = mixer;
            
            Play();
        }

        private void Play()
        {
            var settings = _sound.GetSoundSettings();
            if (settings.Clip == default)
            {
                throw new NullReferenceException("Trying to play a Sound with no clip!");
            }
            
            _audioSource.ApplySettings(settings);
            _audioSource.loop = false;
            _looping = settings.Loop;
            
            _audioSource.PlayDelayed(settings.Delay);
            _timeSinceLastPlay = 0 - settings.Delay;
        }
        
        
        public PoolableAudioSource SetPitch(float pitch)
        {
            _audioSource.pitch = pitch;
            return this;
        }

        public PoolableAudioSource SetVolume(float volume)
        {
            _audioSource.volume = volume;
            return this;
        }
        
        public PoolableAudioSource SetDelay(float delay)
        {
            //todo: it works, but meh...
            _audioSource.Stop();
            
            _audioSource.PlayDelayed(delay);
            _timeSinceLastPlay = 0 - delay;
            return this;
        }
        
        public PoolableAudioSource SetLoop(bool looping)
        {
            _looping = looping;
            return this;
        }
        
        public PoolableAudioSource SetPanStereo(float panStereo)
        {
            _audioSource.panStereo = panStereo;
            return this;
        }

        
        public PoolableAudioSource OnFinished(Action callback)
        {
            _endCallback = callback;
            return this;
        }

        
        #region IPoolable

        public void OnCreated()
        {
            _audioSource = new ComponentFactory<AudioSource>().Create(transform);
        }

        public void OnSpawn()
        {
#if VERBOSE
            gameObject.name += " (Playing)";
#endif
            _audioSource.loop = false;
            _audioSource.volume = 1f;
            _audioSource.pitch = 1f;
            _audioSource.panStereo = 0f;
            _audioSource.time = 0f;
        }

        public void OnDeSpawn()
        {
#if VERBOSE
            gameObject.name = gameObject.name.Replace(" (Playing)", "");
#endif
            _audioSource.Stop();
            _audioSource.clip = null;
            _sound = null;
            
            _endCallback?.Invoke();
            _endCallback = null;
        }
        #endregion
    }
}