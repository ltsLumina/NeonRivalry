#region
using UnityEngine;
using static Lumina.Debugging.DebuggerWindow;
#endregion

// Lumina.Debugging namespace contains classes and methods related to debugging.
namespace Lumina.Debugging
{
    /// <summary>
    /// Helper class for handling common debugging tasks such as logging.
    /// This class is specifically designed to be used within the Unity engine hence derives from MonoBehaviour.
    /// </summary>
    public class Debugger : MonoBehaviour
    {
        // Displays a cyan colored prefix on every debug logs
        static string errorMessagePrefix;
        
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
        /// <param name="state">The optional state type used for filtering logs.</param>
        public static void Debug(string message = "", State.StateType? state = null)
        {
            if ((state == null || state == ActiveStateType) && LogLevel == Level.DEBUG)
            {
                // Change the prefix color to cyan for DEBUG logs
                errorMessagePrefix = "<color=cyan>[DEBUG] ►</color>";

                string logMsg = string.IsNullOrEmpty(message) ? $"{errorMessagePrefix} {defaultMessage}" : $"{errorMessagePrefix} {message}";
                UnityEngine.Debug.Log(logMsg + "\n");
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