using UnityEngine;
using UnityEngine.Audio;

namespace FLZ.Audio
{
    public class AudioManagerSettings : ScriptableObjectSingleton<AudioManagerSettings>
    {
        public AudioMixer MasterMixer;
        public AudioMixer MusicMixer;
        public AudioMixer SfxMixer;

        public int AudioSourcePoolSize = 10;
        public AnimationCurve VolumeCurve;

        public float MinVolume = -80;
        public float MaxVolume = 0;
    }
}