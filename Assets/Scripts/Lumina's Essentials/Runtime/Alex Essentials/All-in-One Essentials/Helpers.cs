using UnityEngine;
using static UnityEngine.Object;

namespace Lumina.Essentials
{
    /// <summary>
    ///     General helper methods that don't fit into any other category.
    /// </summary>
    public static class Helpers
    {
        // -- Camera --
        
        static Camera cameraMain;

        /// <summary>
        ///     Allows you to call camera.main without it being expensive, as we cache it here after the first call.
        ///     <example>Helpers.Camera.transform.position.etc </example>
        /// </summary>
        public static Camera Camera
        {
            get
            {
                if (cameraMain == null) cameraMain = Camera.main;
                return cameraMain;
            }
        }
        
        
        // -- Audio --
        
        
        /// <summary>
        /// Plays the given audio clip on the given audio source with a random pitch between the given min and max pitch.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="audioSource"></param>
        /// <param name="minPitch"></param>
        /// <param name="maxPitch"></param>
        public static void PlayRandomPitch(AudioClip audioClip, AudioSource audioSource, float minPitch, float maxPitch)
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;
            audioSource.PlayOneShot(audioClip);
        }
        
        
        // -- Miscellaneous --

        /// <summary>
        ///     Destroys all children of the given transform.
        ///     Can be used as extension method.
        /// </summary>
        /// <param name="parent"></param>
        public static void DeleteAllChildren(this Transform parent)
        {
            foreach (Transform child in parent) Destroy(child.gameObject);
        }
    }
}