#region
using System;
using System.Collections;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Sequencer
{
public static class SequenceErrorHandler
{
    /// <summary>
    ///     Tries to execute the specified action.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="exception">When this method returns, contains the Exception that was thrown if an error occurred.</param>
    /// <returns>True if the action was executed successfully, False otherwise.</returns>
    public static bool TryExecute(Action action, out Exception exception)
    {
        try
        {
            action?.Invoke();
            exception = null;
            return true;
        } catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }

    public static bool TryExecuteCoroutine(MonoBehaviour host, IEnumerator coroutine, out Exception exception)
    {
        try
        {
            host.StartCoroutine(coroutine);
            exception = null;
            return true;
        } catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }

    /// <summary>
    ///     Handles the error of a sequence.
    /// </summary>
    /// <param name="routineHandler">The RoutineHandler from which to handle error.</param>
    public static void HandleError(RoutineHandler routineHandler)
    {
        if (routineHandler.Exception == null) return;
        Debug.LogError(routineHandler.Exception);
        routineHandler.Exception = null;
    }
}
}
