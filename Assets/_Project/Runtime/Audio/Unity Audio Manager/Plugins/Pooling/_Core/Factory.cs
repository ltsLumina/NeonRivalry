public abstract class IFactory<T>
{
    public abstract T Create();
}

public class Factory<T> : IFactory<T> where T : new()
{
    public Factory()
    {
    }

    public override T Create()
    {
        return new T();
    }
}