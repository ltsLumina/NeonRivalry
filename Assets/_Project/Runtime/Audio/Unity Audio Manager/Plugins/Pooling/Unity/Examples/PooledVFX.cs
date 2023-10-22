using UnityEngine;

namespace FLZ.Pooling.Examples
{
    public class PooledVFX : PooledBehaviour
    {
        [SerializeField] private float timeSpan = 2.0f;
        private float timer;

        private void Update()
        {
            if (!gameObject.activeInHierarchy)
                return;

            timer += Time.deltaTime;
            if (timer > timeSpan)
            {
                //DeSpawn();
            }
        }

        public override void OnDeSpawn()
        {
            timer = 0;
            base.OnDeSpawn();
        }
    }
}