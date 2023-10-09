using UnityEngine;

/// <summary>
/// This scriptable object is intended to be used a base for creating new state data.
/// </summary>
public abstract class DefaultStateData : ScriptableObject
{
    // Doesn't include anything in particular as it's intended to be used as a base.

    protected virtual void OnEnable()
    {
        // Check if there are more than once instance of this type of state data.
        // If there is, log an error.

#if UNITY_EDITOR
        var stateDataInstances = Resources.FindObjectsOfTypeAll(GetType());
        if (stateDataInstances.Length <= 1) return;

        Debug.Assert(stateDataInstances.Length == 1,
            $"There are {stateDataInstances.Length} instances of {GetType()} in the project. \nPlease delete the duplicates.");
#endif
    }
}