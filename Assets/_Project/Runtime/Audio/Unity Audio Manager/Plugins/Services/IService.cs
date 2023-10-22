namespace FLZ.Services
{
    public interface IService
    {
        void OnPreAwake();
        void OnAfterAwake();
        bool IsReady();
    }
}