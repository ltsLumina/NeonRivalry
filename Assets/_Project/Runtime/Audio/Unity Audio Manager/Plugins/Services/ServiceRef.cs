namespace FLZ.Services
{
    public class ServiceRef<I> where I : class
    {
        private I _value;
        public I Value => _value ??= ServiceProvider.GetService<I>();
    }

    public class ServiceRef<I, T> where T : class, I where I : class
    {
        private T _value;
        public T Value => _value ??= ServiceProvider.GetService<I>() as T;
    } 
}