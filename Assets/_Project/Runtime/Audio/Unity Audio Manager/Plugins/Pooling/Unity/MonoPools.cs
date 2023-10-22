#if UNITY_EDITOR
//#define VERBOSE           // Enable/disable pool instances renaming (useful to keep a clean hierarchy but generates a lot of garbage) 
#define CLEAN_HIERARCHY     // Keeps the hierarchy clean and readable, but it slows down the pool and we dont need that on build don't we? :)
#endif

using System.Collections.Generic;
using UnityEngine;

namespace FLZ.Pooling
{
    public abstract class UnityObjectPool<T> : AbstractPool<T> where T : UnityEngine.Component, IPoolable
    {
        public HashSet<T> ActiveObjects => _activeObjects;
        protected readonly Transform _objectsParentTransform;
        
        protected UnityObjectPool(int size, IFactory<T> factory, Transform parent) : base(size, factory)
        {
#if CLEAN_HIERARCHY
            _objectsParentTransform = new GameObjectFactory().Create().transform;
            _objectsParentTransform.SetParent(parent, true);
            
            SetInstancesInParent();
#endif
        }

        protected override void Grow()
        {
            base.Grow();
            
            UpdatePoolName();
            Debug.Log($"Refilled stack. New size = {_poolSize}", _objectsParentTransform);
        }

        protected override void OnPoolDepleted()
        {
            Debug.LogError($"Pool {_objectsParentTransform.name} capacity [{_poolSize}] reached");
        }
        
        protected virtual void UpdatePoolName() { }

        private void SetInstancesInParent()
        {
            UpdatePoolName();
            foreach (var poolObject in _stack)
            {
                poolObject.transform.SetParent(_objectsParentTransform);
            }
        }

        protected override void DeleteObject(T theObject)
        {
            Object.Destroy(theObject);
        }

        public override void Dispose()
        {
            base.Dispose();
            Object.Destroy(_objectsParentTransform.gameObject);
        }
    }

    /// <summary>
    /// MonoBehaviour pool implementation, can spawn any object inheriting MonoBehaviour
    /// ie: Prefabs, components...
    /// </summary>
    public class MonoPool<T> : UnityObjectPool<T> where T : MonoBehaviour, IPoolable
    {
#if VERBOSE
        private string _prefabName = null;
#endif
        /// <summary>
        /// Initialize and fill the pool by instantiating objects using the given factory.
        /// </summary>
        /// <param name="size">The amount of objects instantiated in the pool.</param>
        /// <param name="factory">The factory in charge of creating pool's objects</param>
        /// <param name="parent">The parent transform of the objects pool.</param>
        public MonoPool(int size, IFactory<T> factory, Transform parent) : base(size, factory, parent) { }
        
        /// <summary>
        /// Initialize and fill the pool by instantiating objects using a PrefabFactory filled with the given prefab.
        /// </summary>
        /// <param name="size">The amount of objects instantiated in the pool.</param>
        /// <param name="prefab">The prefab gave to the PrefabFactory</param>
        /// <param name="parent">The parent transform of the objects pool.</param>
        public MonoPool(int size, GameObject prefab, Transform parent) : base(size, new PrefabFactory<T>(prefab), parent) { }

        protected override T CreateObject()
        {
            T theObject = base.CreateObject();
            theObject.OnCreated();
            theObject.transform.SetParent(_objectsParentTransform, false);

#if VERBOSE
            if (string.IsNullOrEmpty(_prefabName))
            {
                _prefabName = theObject.name.Replace("(Clone)", "");
            }

            theObject.name = _prefabName;
#endif
            theObject.gameObject.SetActive(false);
            theObject.transform.position = Vector3.zero;

            return theObject;
        }

        protected override void DeleteObject(T theObject)
        {
            Object.Destroy(theObject.gameObject);
        }

        public override T Spawn()
        {
            return Spawn(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Spawn a new GameObject with the given position/rotation/parent and set it active.
        /// </summary>
        public T Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            T thePooled = base.Spawn();
            if (thePooled != null)
            {
#if VERBOSE
                thePooled.name += " (In use)";
#endif
                
#if CLEAN_HIERARCHY
                if (parent)
                {
                    thePooled.transform.SetParent(parent);
                }
#endif

                thePooled.transform.SetPositionAndRotation(position, rotation);
                thePooled.gameObject.SetActive(true);
            }

            return thePooled;
        }
        
        /// <summary>
        /// UnSpawns a given GameObject, de-activate it and adds it back to the pool.
        /// </summary>
        public override void DeSpawn(T thePooled)
        {
            var pooled = thePooled as T;
#if VERBOSE
            pooled.name = pooled.name.Replace(" (In use)", "");
#endif
#if CLEAN_HIERARCHY
            pooled.transform.SetParent(_objectsParentTransform);
#endif
            pooled.gameObject.SetActive(false);

            base.DeSpawn(thePooled);
        }
        
        protected override void UpdatePoolName()
        {
#if VERBOSE
             _objectsParentTransform.name = $"{_prefabName} pool ({_poolSize})";
#endif
        }
    }
}