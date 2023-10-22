using System.Collections.Generic;
using FLZ.Audio;
using FLZ.Services;
using UnityEngine;

public static class ServiceBuilder
{
    private static List<IService> _services;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AppAwake()
    {
        Application.quitting += OnApplicationQuitting;
        AddDynamicServices();
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        foreach (IService service in _services)
        {
            Debug.Log($"Service ready: {service.GetType().Name}");
            service?.OnAfterAwake();
        }
    }
    
    private static void OnApplicationQuitting()
    {
        ServiceProvider.Dispose();
    }

    private static void AddDynamicServices()
    {
        _services = new List<IService>
        {
            new AudioManager()
        };

        foreach (IService service in _services)
        {
            ServiceProvider.AddService(service.GetType(), service);
            service?.OnPreAwake();
        }
    }
}
