using UnityEngine;
using UnityEngine.UI;

namespace FLZ.Audio.Examples
{
    public class AudioExample : MonoBehaviour
    {
        public SFX SFX;
        [SerializeField] private Text _text;

        public void PlaySFX()
        {
            var source = SFX.Play().OnFinished(() =>
            {
                _text.text = "";
            });
            _text.text = $"{SFX.name} ({source.AudioSource.clip.name})";
            
            // SFX.Play().SetPitch(5f).SetVolume(0.2f);
            // SFX.Play().SetDelay(1f);
            // SFX.Play().SetLoop(true);
        }

        public void StopSFX()
        {
            SFX.Stop();
        }
    }
}