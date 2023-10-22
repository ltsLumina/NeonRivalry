using System;
using System.Collections.Generic;

namespace FLZ.Pooling
{
    public abstract class AbstractPool<T> where T : class, IPoolable
    {
        /// <summary>
        /// The total # of objects that this pool can currently accommodate without growing.
        /// </summary>
        public int Capacity => _poolSize;

        /// <summary>
        /// The total # of objects that are available to use w/o having to re-allocate.
        /// </summary>
        public int AvailableObjectCount => _stack.Count;

        /// <summary>
        /// The total # of objects that are currently being used.
        /// </summary>
        public int ActiveObjectCount => _activeObjects.Count;

        /// <summary>
        /// Is the pool allowed to grow if it needs to spawn new objects?
        /// </summary>
        public bool CanGrow = true;

        protected readonly Stack<T> _stack;
        protected readonly HashSet<T> _activeObjects;
        private readonly IFactory<T> _factory;
        protected int _poolSize = 0;
        
        /// <summary>
        /// Initialize and fill the pool by instantiating objects using the given factory.
        /// </summary>
        /// <param name="size">The amount of objects instantiated in the pool.</param>
        /// <param name="factory">The factory in charge of creating pool's objects</param>
        protected AbstractPool(int size, IFactory<T> factory)
        {
            this._factory = factory;

            _activeObjects = new HashSet<T>(size);
            _stack = new Stack<T>();

            Fill(size);
        }

        private void Fill(int size)
        {
            for (int i = 0; i < size; i++)
                _stack.Push(CreateObject());

            _poolSize += size;
        }

        protected virtual T CreateObject()
        {
            T theObject = _factory.Create();
            theObject.OnCreated();
            
            return theObject;
        }
        
        protected virtual void DeleteObject(T theObject) { }

        /// <summary>
        /// Spawns a new object from the pool.
        /// Can return an used object in the pool if available.
        ///
        /// If there are no more objects ready to be spawned and that CanGrow is true, the pool will automatically
        /// increase its size and spawn a new object.
        /// </summary>
        public virtual T Spawn()
        {
            if (AvailableObjectCount == 0)
            {
                if (CanGrow)
                {
                    Grow();
                }
                else
                {
                    OnPoolDepleted();
                    return null;
                }
            }

            T thePoolable = _stack.Pop();
            _activeObjects.Add(thePoolable);

            thePoolable.OnSpawn();
            return thePoolable;
        }

        /// <summary>
        /// UnSpawns a given object and adds it back to the pool.
        /// </summary>
        public virtual void DeSpawn(T thePooled)
        {
            thePooled.OnDeSpawn();
            _activeObjects.Remove(thePooled as T);
            _stack.Push(thePooled as T);
        }

        /// <summary>
        /// UnSpawns all spawned objects and adds them all back to the pool.
        /// </summary>
        public void DeSpawnAll()
        {
            T[] actives = new T[_activeObjects.Count];
            _activeObjects.CopyTo(actives);
            
            foreach (var poolable in actives)
            {
                DeSpawn(poolable);
            }

            _activeObjects.Clear();
        }


        protected virtual void Grow()
        {
            Fill((Math.Max(1, _poolSize) + 1) / 2);
        }

        /// <summary>
        /// Called when the pool failed to spawn an objects because there's no instance available anymore
        /// </summary>
        protected virtual void OnPoolDepleted()
        {
        }
        
        
        public virtual void Dispose()
        {
            DeSpawnAll();

            foreach (var obj in _stack)    
                DeleteObject(obj);
            
            _stack.Clear();
            _activeObjects.Clear();
        }
    }
}