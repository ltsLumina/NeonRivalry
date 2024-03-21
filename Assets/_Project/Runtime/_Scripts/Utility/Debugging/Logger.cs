#region
using System.Collections.Generic;
using System.Linq;
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
public static class Logger
{
    [Tooltip("Allows the developer to skip the loading screen, join a player into the game scene directly, among with some other minor QoL changes.")]
    public static bool DebugMode = true;

    [Tooltip("Allows the developer to reset the persistent players when the game is restarted.")]
    public static bool ResetPersistentPlayers = true;

    static Logger()
    {
        // Disable the debug mode if we are not in the editor.
        // It might seem as though this is redundant, but it does in fact execute during a build.
#if !UNITY_EDITOR
        DebugMode = false;
        ResetPersistentPlayers = false;
#endif
    }
    
    public enum Level
    {
        TRACE, // Track the flow of the program and trace its execution
        DEBUG, // Debug information such as variable values, etc.
        INFO, // Informational messages, typically used for reporting arbitrary changes
        NONE, // No log reporting
    }
    
    // The debugger will only log states that match the ActiveStateType
    public static State.StateType ActiveStateType { get; set; } = State.StateType.None;

    // LogLevel Property used to get or set current log level
    public static Level LogLevel { get; set; } = Level.NONE;
    
    // Displays a cyan colored prefix on every debug logs
    public static string ErrorMessagePrefix { get; private set; } = "<color=cyan>[Logger] ►</color>";

    // Default error message if no particular message is provided
    const string defaultMessage = "An Error Has Occurred:";
    
    /// <summary>
    /// Generic method for logging an error message.
    /// Prints the same way of as the Debug.Log() method, but with a prefix.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logType"></param>
    public static void Log(string message = "", LogType logType = LogType.Log)
    {
        ErrorMessagePrefix = "<color=lightblue>[Log] ►</color>";
        string logMsg = string.IsNullOrEmpty(message) ? $"{ErrorMessagePrefix} {defaultMessage}" : $"{ErrorMessagePrefix} {message}";

        switch (logType)
        {
            case LogType.Log:
                UnityEngine.Debug.Log(logMsg + "\n");
                break;
            
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
    
    /// <summary>
    /// Overload of the Log method that takes a condition.
    /// </summary>
    public static void Log(string message = "", bool condition = false, LogType logType = LogType.Log)
    {
        if (condition) Log(message, logType);
    }
    
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
            ErrorMessagePrefix = "<color=cyan>[INFO] ►</color>";

            string logMsg = string.IsNullOrEmpty(message) ? $"{ErrorMessagePrefix} {defaultMessage}" : $"{ErrorMessagePrefix} {message}";
            UnityEngine.Debug.Log(logMsg + "\n");
        }
    }

    /// <summary>
    /// Overload of the Info method that takes an array of state types.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="states"></param>
    public static void Info(string message, IEnumerable<State.StateType> states)
    {
        if (states.Contains(ActiveStateType) && LogLevel == Level.INFO)
        {
            ErrorMessagePrefix = "<color=cyan>[INFO] ►</color>";
            
            string logMsg = string.IsNullOrEmpty(message) ? $"{ErrorMessagePrefix} {defaultMessage}" : $"{ErrorMessagePrefix} {message}";
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
            ErrorMessagePrefix = "<color=green>[DEBUG] ►</color>";

            string logMsg = string.IsNullOrEmpty(message) ? $"{ErrorMessagePrefix} {defaultMessage}" : $"{ErrorMessagePrefix} {message}";

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
    /// Overload of the Debug method that takes an array of state types.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logType"></param>
    /// <param name="states"></param>
    public static void Debug(string message, LogType logType, IEnumerable<State.StateType> states)
    {
        if (states.Contains(ActiveStateType) && LogLevel == Level.DEBUG)
        {
            ErrorMessagePrefix = "<color=green>[DEBUG] ►</color>";
            string logMsg = string.IsNullOrEmpty(message) ? $"{ErrorMessagePrefix} {defaultMessage}" : $"{ErrorMessagePrefix} {message}";

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
            ErrorMessagePrefix = "<color=orange>[TRACE] ►</color>";

            string logMsg = string.IsNullOrEmpty(message) ? $"{ErrorMessagePrefix} {defaultMessage}" : $"{ErrorMessagePrefix} {message}";
            UnityEngine.Debug.Log(logMsg + "\n");
        }
    }

    /// <summary>
    /// Overload of the Trace method that takes an array of state types.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="states"></param>
    public static void Trace(string message, IEnumerable<State.StateType> states)
    {
        if (states.Contains(ActiveStateType) && LogLevel == Level.TRACE)
        {
            // Change the prefix color to orange for TRACE logs
            ErrorMessagePrefix = "<color=orange>[TRACE] ►</color>";
            string logMsg = string.IsNullOrEmpty(message) ? $"{ErrorMessagePrefix} {defaultMessage}" : $"{ErrorMessagePrefix} {message}";
            UnityEngine.Debug.Log(logMsg + "\n");
        }
    }
}
}