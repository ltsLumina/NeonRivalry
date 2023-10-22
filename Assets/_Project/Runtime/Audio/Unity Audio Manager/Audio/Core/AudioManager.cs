using FLZ.Pooling;
using FLZ.Services;
using UnityEngine;
using UnityEngine.Audio;

namespace FLZ.Audio
{
    public class AudioManager : IService
    {
        private static AudioManagerSettings _settings => AudioManagerSettings.Instance;
        public static AudioManagerSettings Settings => _settings;

        public static AudioMixer MasterMixer => _settings.MasterMixer;
        public static AudioMixer MusicMixer => _settings.MusicMixer;
        public static AudioMixer SFXMixer => _settings.SfxMixer;
        
        private AnimationCurve _volumeCurve => _settings.VolumeCurve;
        private float _minVolume => _settings.MinVolume;
        private float _maxVolume => _settings.MaxVolume;
        
        private const string GLOBAL_VOLUME = "GlobalMasterVolume";
        private const string MUSIC_VOLUME = "GlobalMusicVolume";
        private const string SFX_VOLUME = "GlobalSFXVolume";
        
        private static AudioSourcePool<PoolableAudioSource> _audioSourcePool;
        private bool _initialized;

        #region IService
        public void OnPreAwake()
        {
            var parent = new GameObject("AudioManager").transform;
            parent.gameObject.AddComponent<AudioListener>();
            _audioSourcePool = new AudioSourcePool<PoolableAudioSource>(_settings.AudioSourcePoolSize, new ComponentFactory<PoolableAudioSource>(), parent);
        }

        public void OnAfterAwake()
        {
            InitializeVolume(MasterMixer, GLOBAL_VOLUME);
            InitializeVolume(MusicMixer, MUSIC_VOLUME);
            InitializeVolume(SFXMixer, SFX_VOLUME);
            
            _initialized = true;
        }
        
        public bool IsReady() => _initialized;
        #endregion

        /// <summary>
        /// Initialize mixer volume parameters, pull the data from PlayerPrefs and feed the mixers 
        /// </summary>
        private void InitializeVolume(AudioMixer mixer, string paramName)
        {
            float value = PlayerPrefs.GetFloat(paramName, 1);
            SetVolumeParameter(mixer, paramName, value);
        }
        
        public void SetParameter(AudioMixer mixer, string paramName, float value)
        {
            if (!mixer.SetFloat(paramName, value))
            {
                Debug.LogError($"AudioManager - SetParameter - Can't find any parameter {paramName} on mixer {mixer}");
            }
        }
        
        private float GetParameter(string paramName, AudioMixer mixer = null)
        {
            if (mixer == null)
                mixer = MasterMixer;

            mixer.GetFloat(paramName, out var value);
            return value;
        }

        #region VOLUME
        private void SetVolumeParameter(AudioMixer mixer, string paramName, float value)
        {
            PlayerPrefs.SetFloat(paramName, value);
            
            float volume = GetVolume(value);
            SetParameter(mixer, paramName, volume);
        }
        
        private float GetVolumeParameter(AudioMixer mixer, string paramName)
        {
            var volume = GetParameter(paramName, mixer);
            return GetVolumeRatio(volume);
        }
        
        /// <summary>
        /// Takes a value in between 0 and 1 and returns the calculated volume based on AudioManager min, max and curve volume settings.
        /// </summary>
        /// <param name="value">0-1</param>
        private float GetVolume(float value)
        {
            return Mathf.Lerp(_minVolume, _maxVolume, _volumeCurve?.Evaluate(value) ?? value);
        }
        
        /// <summary>
        /// Takes a raw volume values and returns it's relative value in between 0 and 1 based on AudioManager min, max and curve volume settings.
        /// </summary>
        /// <param name="volume"></param>
        private float GetVolumeRatio(float volume)
        {
            var lerp = Mathf.InverseLerp(_minVolume, _maxVolume, volume);
            return _volumeCurve.EvaluateInverse(lerp);
        }
        
        
        public void SetGlobalVolume(float value)
        {
            SetVolumeParameter(MasterMixer, GLOBAL_VOLUME, value);
        }

        public void SetMusicVolume(float value)
        {
            SetVolumeParameter(MusicMixer, MUSIC_VOLUME, value);
        }

        public void SetSFXVolume(float value)
        {
            SetVolumeParameter(SFXMixer, SFX_VOLUME, value);
        }


        public float GetGlobalVolume() => GetVolumeParameter(MasterMixer, GLOBAL_VOLUME);

        public float GetMusicVolume() => GetVolumeParameter(MusicMixer, MUSIC_VOLUME);
        
        public float GetSFXVolume() => GetVolumeParameter(SFXMixer, SFX_VOLUME);
        #endregion

        #region PLAY/STOP
        public static PoolableAudioSource Play(ISound sound, AudioMixerGroup mixer = null)
        {
            if (mixer == null)
            {
                mixer = MasterMixer.outputAudioMixerGroup;
            }
            
            var pooledAudioSource = _audioSourcePool.Spawn();
            pooledAudioSource.SetupAndPlay(_audioSourcePool, sound, mixer);
            
            return pooledAudioSource;
        }

        public static void Stop(ISound sound)
        {
            PoolableAudioSource playingSource = null;
            foreach (var audioSource in _audioSourcePool.ActiveObjects)
            {
                if (audioSource.Sound == sound)
                {
                    playingSource = audioSource;
                    break;
                }
            }

            if (playingSource != null)
            {
                _audioSourcePool.DeSpawn(playingSource);
            }
        }
        #endregion

        public static int GetCurrentPoolSize() => _audioSourcePool.Capacity;
    }
    
    public struct SoundSettings
    {
        public readonly AudioClip Clip;
        public readonly float Volume;
        public readonly float Pitch;
        public readonly float PanStereo;
        public readonly float Delay;
        public readonly bool Loop;

        public SoundSettings(AudioClip clip, float volume, float pitch, float panStereo, float delay, bool loop)
        {
            Clip = clip;
            Volume = volume;
            Pitch = pitch;
            PanStereo = panStereo;
            Delay = delay;
            Loop = loop;
        }
    }
    
    public static class AudioSourceExtensions
    {
        public static void ApplySettings(this AudioSource source, SoundSettings sourceSettings)
        {
            source.clip = sourceSettings.Clip;
            source.volume = sourceSettings.Volume;
            source.pitch = sourceSettings.Pitch;
            source.panStereo = sourceSettings.PanStereo;
            source.loop = sourceSettings.Loop;
        }
    }
}