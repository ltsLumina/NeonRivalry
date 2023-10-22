using UnityEngine;

namespace FLZ.Pooling.Examples
{
    public abstract class PooledBehaviour : MonoBehaviour, IPoolable
    {
        #region IPoolable
        public void OnCreated() { }

        public void OnSpawn() { }

        public virtual void OnDeSpawn() { }
        #endregion
    }
}