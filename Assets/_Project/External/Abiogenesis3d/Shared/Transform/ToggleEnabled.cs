using UnityEngine;

namespace Abiogenesis3d
{
    public class ToggleEnabled : MonoBehaviour
    {
        public Behaviour behaviour;
        public KeyCode toggleKey;

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                behaviour.enabled = !behaviour.enabled;
        }
    }
}
