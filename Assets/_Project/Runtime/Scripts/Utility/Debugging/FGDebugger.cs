#region
using UnityEngine;
#endregion

// Lumina.Debugging namespace contains classes and methods related to debugging.
namespace Lumina.Debugging
{
/// <summary>
/// Helper class for handling common debugging tasks such as logging.
/// This class is specifically designed to be used within the Unity engine hence derives from MonoBehaviour.
/// <remarks> FG stands for FightingGame. </remarks>
/// </summary>
public static class FGDebugger
{
    // Level enum is used to specify the log level
    public enum Level
    {
        TRACE,
        DEBUG,
        INFO,
        NONE, // No log reporting
    }
    
    // The debugger will only log states that match the ActiveStateType
    public static State.StateType ActiveStateType { get; set; } = State.StateType.None;

    // LogLevel Property used to get or set current log level
    public static Level LogLevel { get; set; } = Level.NONE;
    
    // Displays a cyan colored prefix on every debug logs
    public static string errorMessagePrefix = "<color=cyan>[FGDebugger] ►</color>";
        
    // Default error message if no particular message is provided
    const string defaultMessage = "An Error Has Occurred:";
    
    /// <summary>
    /// Logs an informational message with an optional state type.
    /// The message will be logged only when logging level is INFO.
    /// </summary>
    /// <param name="message">The content of the log message.</param>
    /// <param name="state">The optional state type used for filtering logs.</param>
    public static void Info(string message = "", State.StateType? state = null)
    {
        if ((state == null || state == ActiveStateType) && LogLevel == Level.INFO)
        {
            // Change the prefix color to green for INFO logs
            errorMessagePrefix = "<color=green>[INFO] ►</color>";

            string logMsg = string.IsNullOrEmpty(message) ? $"{errorMessagePrefix} {defaultMessage}" : $"{errorMessagePrefix} {message}";
            UnityEngine.Debug.Log(logMsg + "\n");
        }
    }

    /// <summary>
    /// Logs a debug message with an optional state type.
    /// The message will be logged only when logging level is DEBUG.
    /// </summary>
    /// <param name="message">The content of the log message.</param>
    /// <param name="logType"></param>
    /// <param name="state">The optional state type used for filtering logs. If it is left null, the log will be displayed regardless of the state.</param>
    public static void Debug(string message = "", LogType logType = LogType.Log, State.StateType? state = null)
    {
        if ((state == null || state == ActiveStateType) && LogLevel == Level.DEBUG)
        {
            // Change the prefix color to cyan for DEBUG logs
            errorMessagePrefix = "<color=cyan>[DEBUG] ►</color>";

            string logMsg = string.IsNullOrEmpty(message) ? $"{errorMessagePrefix} {defaultMessage}" : $"{errorMessagePrefix} {message}";

            switch (logType)
            {
                case LogType.Error:
                    UnityEngine.Debug.LogError(logMsg + "\n");
                    break;

                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(logMsg + "\n");
                    break;

                default:
                    UnityEngine.Debug.Log(logMsg + "\n");
                    break;
            }
        }
    }

    /// <summary>
    /// Logs a trace message with an optional state type.
    /// The message will be logged only when logging level is TRACE.
    /// </summary>
    /// <param name="message">The content of the log message.</param>
    /// <param name="state">The optional state type used for filtering logs.</param>
    public static void Trace(string message = "", State.StateType? state = null)
    {
        if ((state == null || state == ActiveStateType) && LogLevel == Level.TRACE)
        {
            // Change the prefix color to orange for TRACE logs
            errorMessagePrefix = "<color=orange>[TRACE] ►</color>";

            string logMsg = string.IsNullOrEmpty(message) ? $"{errorMessagePrefix} {defaultMessage}" : $"{errorMessagePrefix} {message}";
            UnityEngine.Debug.Log(logMsg + "\n");
        }
    }
}
}