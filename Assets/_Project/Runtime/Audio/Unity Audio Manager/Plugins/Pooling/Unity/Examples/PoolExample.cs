using UnityEngine;

namespace FLZ.Pooling.Examples
{
    public class PoolExample : MonoBehaviour
    {
        public GameObject PooledPrefab;

        private MonoPool<PooledBehaviour> _gameObjectPool;

        private void Awake()
        {
            _gameObjectPool = new MonoPool<PooledBehaviour>(10, PooledPrefab, transform);
        }

        public void SpawnVFX()
        {
            var position = Random.insideUnitCircle * 5.0f;
            _gameObjectPool.Spawn(position, Quaternion.identity);
        }
        
        public void Dispose()
        {
            _gameObjectPool.Dispose();
            _gameObjectPool = null;
        }
    }
}