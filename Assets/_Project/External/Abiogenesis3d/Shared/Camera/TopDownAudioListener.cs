using System.Linq;
using UnityEngine;

namespace Abiogenesis3d
{
    [RequireComponent(typeof(AudioListener))]
    public class TopDownAudioListener : MonoBehaviour
    {
        public Camera cam;
        public Transform player;

        void EnsureSingleAudioListener()
        {
            var audioListener = GetComponent<AudioListener>();
            audioListener.enabled = true;

            var otherListeners = FindObjectsOfType<AudioListener>()
                .Where(l => l != audioListener)
                .Where(l => l.isActiveAndEnabled);

            if (otherListeners.Count() > 0)
            {
                var disabledNames = otherListeners.Select(l => l.name);
                foreach (var listener in otherListeners)
                {
                    Debug.LogWarning("There should only be one AudioListener, disabling it on " + listener.name, listener.gameObject);
                    listener.enabled = false;
                }
            }
        }
        void Start()
        {
            EnsureSingleAudioListener();
        }

        void Update()
        {
            if (!cam) cam = Camera.main;
            if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (!player) return;

            transform.SetPositionAndRotation(player.position, cam.transform.rotation);
        }
    }
}
