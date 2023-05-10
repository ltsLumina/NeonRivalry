using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] AudioClip music;
    [SerializeField] string triggerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag)) { ChangeMusic(); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag)) { ChangeMusic(); }
    }

    private void ChangeMusic()
    {
        MusicPlayer.Instance.ChangeMusic(music);
    }
}
