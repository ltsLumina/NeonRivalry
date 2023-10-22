namespace FLZ.Audio
{
    public interface ISound
    {
        SoundSettings GetSoundSettings();

        PoolableAudioSource Play();
    }
}