using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class OpositeEnabled : MonoBehaviour
    {
        public Behaviour target;
        public Behaviour source;

        void Update()
        {
            if (!target) return;

            target.enabled = !source.enabled;
        }
    }
}
