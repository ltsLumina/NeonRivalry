using UnityEngine;

public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                var path = typeof(T).Name;
                 _instance = Resources.Load<T>(path);

                if (!_instance)
                    Debug.LogErrorFormat("Can't load singleton at {0}. Make sure you've created it with the right name, in a Resources folder", path);
            }

            return _instance;
        }
    }
}