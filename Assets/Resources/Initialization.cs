// Uncomment the #define statement to enable the use of the initialization script.
//#define USING_INITIALIZATION
// I highly recommended you use this script, as it will save you a lot of time and effort struggling with singletons.

#region
using UnityEngine;
using static UnityEngine.Object;
#endregion

/// <summary>
/// Simple class to initialize the game.
/// Instantiates the 'Systems' prefab that includes every manager in your game.
/// You must create a folder by name 'Resources' (CaSe SeNsItIvE) and place/create the 'Systems' prefab inside it.
/// Alternatively, you can use the Resources folder and Systems prefab provided in this package.
/// HOWEVER, if you are not using the provided Systems prefab, you should DELETE the 'Systems' folder from the package.
/// 
/// Check GitHub for more information:
/// https://github.com/ltsLumina/Lumina-Essentials
/// or watch this video:
/// https://www.youtube.com/watch?v=zJOxWmVveXU
/// </summary>
public static class Initialization
{
#if USING_INITIALIZATION
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute() => DontDestroyOnLoad(Instantiate(Resources.Load("Systems")));
#endif
}