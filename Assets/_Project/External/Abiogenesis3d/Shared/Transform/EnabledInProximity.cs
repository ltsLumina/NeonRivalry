using UnityEngine;

namespace Abiogenesis3d
{
    public class EnabledInProximity : MonoBehaviour
    {
        public Transform distanceTarget;
        public GameObject target;
        public float radius;

        void Update()
        {
            target.SetActive(Vector3.Distance(transform.position, distanceTarget.position) < radius);
        }
    }
}
