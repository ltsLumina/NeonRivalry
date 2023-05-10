
// Comment out the #define statement to disable the use of the initialization script.
#define INITIALIZATION_ENABLED

#region
using UnityEngine;
using static UnityEngine.Object;
#endregion

/// Simple class to initialize the game.
/// Instantiates the Systems prefab that includes every manager in your game.
/// Make sure to create a Resources folder in your Assets folder and place the Systems prefab in there.
/// The Systems prefab should be the parent of all the managers in your game.
/// Each manager script should be its own child of the Systems prefab.
///
/// If you wish to disable this script, be it temporary or not, simply comment out the "#define INITIALIZATION_ENABLED" line at the top of this script.
public static class Initialization
{
#if INITIALIZATION_ENABLED
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute() => DontDestroyOnLoad(Instantiate(Resources.Load("Systems")));
#endif
}