using UnityEngine;

namespace FLZ.Pooling
{
    public class AudioSourcePool<T> : UnityObjectPool<T> where T : MonoBehaviour, IPoolable
    {
        public AudioSourcePool(int size, IFactory<T> factory, Transform parent) : base(size, factory, parent) { }
        
        protected override T CreateObject()
        {
            T theObject = base.CreateObject();
            theObject.transform.SetParent(_objectsParentTransform, false);

#if VERBOSE
            if (string.IsNullOrEmpty(_prefabName))
            {
                _prefabName = theObject.name.Replace("(Clone)", "");
            }

            theObject.name = _prefabName;
#endif
            theObject.enabled = false;
            return theObject;
        }

        public override T Spawn()
        {
            var theObject = base.Spawn();
            theObject.enabled = true;

            return theObject;
        }

        public override void DeSpawn(T thePooled)
        {
#if VERBOSE
            pooled.name = pooled.name.Replace(" (In use)", "");
#endif
            base.DeSpawn(thePooled);
        }
    }
}