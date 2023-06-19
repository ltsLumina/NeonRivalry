#region
using UnityEngine;
#endregion

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] AudioClip music;
    [SerializeField] string triggerTag = "";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag)) ChangeMusic();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag)) ChangeMusic();
    }

    void ChangeMusic() => MusicPlayer.Instance.ChangeMusic(music);
}
