#region
using System;
using FLZ.Pooling;
using NUnit.Framework;
#endregion

namespace FLZ.Tests
{
[TestFixture(typeof(MockPooledObject))]
public class PoolingTests<T>
    where T : class, IPoolable, new()
{
    MockPool<T> mockPool;
    Factory<T> factory;
    int poolSize;

    [SetUp]
    public void Init() => factory = new ();

    [TearDown]
    public void TearDown()
    {
        mockPool = null;
        factory  = null;
    }

    [TestCase(10)]
    public void GIVEN_Size_WHEN_Initialized_Capacity_Equals_Size(int size)
    {
        mockPool = new (size, factory);
        Assert.AreEqual(size, mockPool.Capacity);
    }

    [TestCase(10)]
    public void GIVEN_AvailableObjectCount_WHEN_Spawn_THEN_AvailableObjectCount_Decreased(int size)
    {
        mockPool = new (size, factory);
        mockPool.Spawn();

        Assert.AreEqual(size - 1, mockPool.AvailableObjectCount);
    }

    [TestCase(10)]
    public void GIVEN_AvailableObjectCount_WHEN_DeSpawn_THEN_AvailableObjectCount_Increased(int size)
    {
        mockPool = new (size, factory);

        T pooled = mockPool.Spawn();

        Assert.AreEqual(size - 1, mockPool.AvailableObjectCount);

        mockPool.DeSpawn(pooled);

        Assert.AreEqual(size, mockPool.AvailableObjectCount);
    }

    [TestCase(10)]
    public void GIVEN_Pool_CanGrow_WHEN_Capacity_Overflowed_THEN_Capacity_Increased(int size)
    {
        mockPool = new (size, factory);
        int initialCapacity = mockPool.Capacity;

        for (int i = 0; i < size + 1; i++) { mockPool.Spawn(); }

        Assert.Greater(mockPool.Capacity, initialCapacity);
    }

    [TestCase(10)]
    public void GIVEN_Pool_CantGrow_WHEN_Capacity_Overflowed_THEN_PoolDepleted(int size)
    {
        mockPool         = new (size, factory);
        mockPool.CanGrow = false;

        for (int i = 0; i < 11; i++) { mockPool.Spawn(); }

        Assert.IsTrue(mockPool.Depleted);
    }

    [TestCase(10)]
    public void WHEN_DeSpawnAll_THEN_Capacity_Equals_Size(int size)
    {
        mockPool = new (size, factory);

        for (int i = 0; i < size; i++) { mockPool.Spawn(); }

        mockPool.DeSpawnAll();

        Assert.AreEqual(mockPool.Capacity, size);
    }

    [TestCase(10)]
    public void WHEN_Spawn_THEN_PooledObject_OnSpawn_Called(int size)
    {
        mockPool = new (size, factory);
        var spawnedObject = mockPool.Spawn() as MockPooledObject;

        Assert.IsTrue(spawnedObject.Spawned);
    }

    [TestCase(10)]
    public void WHEN_DeSpawn_THEN_PooledObject_DeSpawn_Called(int size)
    {
        mockPool = new (size, factory);
        var spawnedObject = mockPool.Spawn() as MockPooledObject;

        mockPool.DeSpawn(spawnedObject as T);

        Assert.IsFalse(spawnedObject is { Spawned: true });
    }
}

public class MockPool<T> : AbstractPool<T>
    where T : class, IPoolable
{
    public bool Depleted;

    public MockPool(int size, IFactory<T> factory) : base(size, factory) { }

    protected override void OnPoolDepleted()
    {
        base.OnPoolDepleted();
        Depleted = true;
    }
}

public class MockPooledObject : object, IPoolable
{
    public bool Spawned;
    public event Action<IPoolable> OnDeSpawned;

    public void OnCreated() { }

    public void OnSpawn() => Spawned = true;

    public void OnDeSpawn() => Spawned = false;

    protected virtual void OnOnDeSpawned(IPoolable obj) => OnDeSpawned?.Invoke(obj);
}
}
