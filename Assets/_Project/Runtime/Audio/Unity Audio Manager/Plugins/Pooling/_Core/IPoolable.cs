namespace FLZ.Pooling
{
    public interface IPoolable
    {
        void OnCreated();
        void OnSpawn();
        void OnDeSpawn();
    }
}