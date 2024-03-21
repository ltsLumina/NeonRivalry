#region
using System.Collections;
using UnityEngine;
#endregion

public class CoroutineHelper : MonoBehaviour
{
    static CoroutineHelper instance;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        if (instance == null)
        {
            var coroutineHelperObject = new GameObject("[Coroutine Helper]");
            instance = coroutineHelperObject.AddComponent<CoroutineHelper>();
            DontDestroyOnLoad(coroutineHelperObject);
        }
    }

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
    }

    static CoroutineHelper Instance
    {
        get
        {
            if (instance == null) Initialize();
            return instance;
        }
    }

    new public static void StartCoroutine(IEnumerator coroutine)
    {
        if (Instance != null) ((MonoBehaviour) Instance).StartCoroutine(coroutine);
        else Debug.LogError("[Coroutine Helper] is null. Coroutine cannot be started.");
    }
}
