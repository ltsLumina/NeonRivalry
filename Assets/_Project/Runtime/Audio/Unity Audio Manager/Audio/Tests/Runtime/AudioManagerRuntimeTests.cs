using System.Collections;
using FLZ.Services;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FLZ.Audio.Tests
{
    public class AudioManagerRuntimeTests
    {
        private static readonly string SCENE_ROOT_PATH = "Assets/FLZ/Audio/Tests/Scenes/" + "Test_AudioManager.unity";

        private static float[] _volumeValues = {10.0f, -10.0f, 0.5f};
        private static float[] _volumeExpectedResults = {1, 0, 0.5f};

        private AudioTests _testsRunner;
        
        private readonly AudioManager _audioManager = new ServiceRef<AudioManager>().Value;


        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(SCENE_ROOT_PATH, new LoadSceneParameters(LoadSceneMode.Additive));

            _testsRunner = GameObject.Find("AudioTests").GetComponent<AudioTests>();
            
            yield return new WaitUntil(() => _audioManager.IsReady());
        }
                
        
        [UnityTest]
        public IEnumerator AudioManager_PlayEmptySound_ThrowException()
        {
            yield return null;
            var SFX = ScriptableObject.CreateInstance<SFX>();
            
            Assert.That( () =>
            {
                SFX.Play();
            }, Throws.Exception);
        }
            
        [UnityTest]
        public IEnumerator AudioManager_PlaySound_SourcePooled()
        {
            var source = _testsRunner.SFX.Play();
            
            yield return null;
            
            Assert.IsNotNull(source);
        }
        
        [UnityTest]
        public IEnumerator AudioManager_PlaySound_SoundPlayed()
        {
            var source = _testsRunner.SFX.Play();
            
            yield return null;
            
            Assert.IsTrue(source.IsPlaying());
        }
        
        [UnityTest]
        public IEnumerator AudioManager_PlayMultipleSounds_PoolIncreased()
        {
            int poolSize = AudioManager.Settings.AudioSourcePoolSize;
            for (int i = 0; i < poolSize+1; i++)
            {
                var source = _testsRunner.SFX.Play();
            
                yield return null;
            
                Assert.IsTrue(source.IsPlaying()); 
            }
            
            Assert.IsTrue(AudioManager.GetCurrentPoolSize() != poolSize); 
        }
        
        [UnityTest]
        public IEnumerator AudioManager_PlaySoundSetPitch_PitchChanged()
        {
            var source = _testsRunner.SFX.Play().SetPitch(3);

            yield return null;
            
            Assert.IsTrue(source.AudioSource.pitch == 3);
        }
        
        [UnityTest]
        public IEnumerator AudioManager_PlaySoundSetVolume_VolumeChanged()
        {
            yield return null;
            var source = _testsRunner.SFX.Play().SetVolume(0);
            
            Assert.IsTrue(source.AudioSource.volume == 0);
        }
        
        [UnityTest]
        public IEnumerator AudioManager_PlaySoundSetDelay_DelayChanged()
        {
            var source = _testsRunner.SFX.Play().SetDelay(1);
            
            Assert.IsTrue(source.TimeSinceLastPlay < 0);
            yield return null;
        }
        
        
        [UnityTest]
        public IEnumerator AudioManager_PlaySoundSetLoopSound_SoundLooping()
        {
            var source = _testsRunner.SFX.Play().SetLoop(true);
            
            yield return null;
            
            Assert.IsTrue(source.Looping);
        }
        
        [UnityTest, Order(1)]
        public IEnumerator AudioManager_StopSound_SoundStopped()
        {
            var source = _testsRunner.SFX.Play().SetLoop(true);
            
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsTrue(source.IsPlaying());

            _testsRunner.SFX.Stop();
            
            yield return new WaitForSeconds(0.1f);

            Assert.IsFalse(source.IsPlaying());
        }
        
        /// These could have been editor tests but the audio mixer only work in runtime...
        
        [UnityTest]
        public IEnumerator AudioManager_SetGlobalVolume_GlobalVolumeChanged([ValueSource(nameof(_volumeValues))] float value)
        {
            yield return null;

            var audioManager = new ServiceRef<AudioManager>().Value;
            audioManager.SetGlobalVolume(value);

            for (int i = 0; i < _volumeValues.Length; i++)
            {
                if (value == _volumeValues[i])
                {
                    var volume = audioManager.GetGlobalVolume();
                    Assert.IsTrue(_volumeExpectedResults[i] == volume);
                }
            }
        }
        
        [UnityTest]
        public IEnumerator AudioManager_SetMusicVolume_MusicVolumeChanged([ValueSource(nameof(_volumeValues))] float value)
        {
            yield return null;

            var audioManager = new ServiceRef<AudioManager>().Value;
            audioManager.SetMusicVolume(value);

            for (int i = 0; i < _volumeValues.Length; i++)
            {
                if (value == _volumeValues[i])
                {
                    var volume = audioManager.GetMusicVolume();
                    Assert.IsTrue(_volumeExpectedResults[i] == volume);
                }
            }
        }
        
        [UnityTest]
        public IEnumerator AudioManager_SetSFXVolume_SFXVolumeChanged([ValueSource(nameof(_volumeValues))] float value)
        {
            yield return null;

            var audioManager = new ServiceRef<AudioManager>().Value;
            audioManager.SetSFXVolume(value);

            for (int i = 0; i < _volumeValues.Length; i++)
            {
                if (value == _volumeValues[i])
                {
                    var volume = audioManager.GetSFXVolume();
                    Assert.IsTrue(_volumeExpectedResults[i] == volume);
                }
            }
        }
    }
}
