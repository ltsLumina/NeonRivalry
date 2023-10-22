using FLZ.Audio;
using FLZ.Services;
using NUnit.Framework;

namespace FLZ.Tests
{
    public class AudioManagerTests
    {
        [SetUp]
        public void Init()
        {
            ServiceProvider.AddService(typeof(AudioManager), new AudioManager());
        }

        [TearDown]
        public void TearDown() 
        { 
            ServiceProvider.Dispose();
        }
        
        [Test]
        public void AudioManager_Settings_NotNull()
        {
            Assert.IsNotNull(AudioManager.Settings);
        }
        
        [Test]
        public void AudioManager_MasterMixer_NotNull()
        {
            Assert.IsNotNull(AudioManager.MasterMixer);
        }
        
        [Test]
        public void AudioManager_MusicMixer_NotNull()
        {
            Assert.IsNotNull(AudioManager.MusicMixer);
        }
        
        [Test]
        public void AudioManager_SFXMixer_NotNull()
        {
            Assert.IsNotNull(AudioManager.SFXMixer);
        }
    }
}