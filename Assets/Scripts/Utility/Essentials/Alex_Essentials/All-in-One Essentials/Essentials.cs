// Comment out the following line to disable the use of deprecated features or plugin-related features. (If you get errors, comment this line out.)
//#define USE_DEPRECATED_FEATURES

#region namespaces
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
#if USE_DEPRECATED_FEATURES
using Cysharp.Threading.Tasks;
#endif
#endregion

namespace Essentials
{
#if UNITY_EDITOR
    public static class Shortcuts
    {
        /// <summary>
        ///     Alt+ C to clear the console in the Unity Editor.
        /// </summary>
        [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
        public static void ClearConsole()
        {
            var        assembly = Assembly.GetAssembly(typeof(SceneView));
            Type       type     = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method   = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
    }
#endif

    public static class Sequencing
    {
        // The sequencing methods are used to run actions before and after a delay.
        // All methods have a coroutine counterpart that can be used instead of the async version if you prefer.
        #region SEQUENCING METHODS
        /// <summary>
        ///     This method provides a convenient way to asynchronously invoke a specified action and then execute a continuation
        ///     action after the first action completes.
        ///     Possible use cases could be to display a UI element for a few seconds and then hide it by running the show method,
        ///     waiting, and then running the hide method.
        ///     <remarks>
        ///         !! WARNING !!
        ///         Components can only be modified from the main thread, meaning this method wont be able to affect components
        ///         directly.
        ///         If you need to modify one, use the coroutine counterpart, 'SequenceActions' instead.
        ///     </remarks>
        /// </summary>
        /// <example>SequenceActions(() => action, 2,5f, onCompleteAction, false); </example>
        /// <param name="action">The action or method to run.</param>
        /// <param name="delayInSeconds">The delay before running the method.</param>
        /// <param name="onComplete">An action to be completed after the initial action is finished. Not required to be used.</param>
        /// <param name="debugLog">Whether or not to debug the waiting message and the completion message.</param>
        /// <param name="cancellationToken"> A token for cancelling the currently ongoing task.</param>
        public static async Task SequenceActionsAsync(
            Action action, double delayInSeconds, Action onComplete, bool debugLog = false,
            CancellationToken cancellationToken = default)
        {
            // Introduce a variable to store the delay time.
            TimeSpan timeSpan = TimeSpan.FromSeconds(delayInSeconds);

            // Run the action.
            action.Invoke();
            if (debugLog) Debug.Log($"' {action.Method.Name} ' completed.");

            // Debug the waiting message.
            if (debugLog) Debug.Log($"Waiting for {delayInSeconds} seconds...");

            // Wait for the delay time to pass.
            await Task.Delay(timeSpan, cancellationToken).ContinueWith
            (_ =>
            {
                // Cancel the task if the cancellation token has been requested.
                cancellationToken.ThrowIfCancellationRequested();

                // Run the onComplete action, if there is one.
                onComplete?.Invoke();

                // Debug the completion message.
                if (debugLog) Debug.Log($"' {onComplete?.Method.Name} ' completed.");
            }, cancellationToken);
        }

        public static IEnumerator SequenceActions(
            Action action, float delayInSeconds, Action onComplete, bool useRealtime = default, bool debugLog = false)
        {
            // Run the action.
            action.Invoke();
            if (debugLog) Debug.Log($"' {action.Method.Name} ' completed.");

            // Debug the waiting message.
            if (debugLog) Debug.Log($"Waiting for {delayInSeconds} seconds...");

            // Wait for the delay time to pass.
            if (useRealtime) yield return new WaitForSecondsRealtime(delayInSeconds);
            else yield return new WaitForSeconds(delayInSeconds);

            {
                // Run the onComplete action, if there is one.
                onComplete?.Invoke();

                // Debug the completion message.
                if (debugLog) Debug.Log($"' {onComplete?.Method.Name} ' completed.");
            }

            yield return null;
        }
        #endregion

        #region DELAY METHODS
        /// <summary>
        ///     Simple method to delay an action by a specified amount of time.
        ///     <remarks>
        ///         !! WARNING !!
        ///         Components can only be modified from the main thread, meaning this method wont be able to affect components
        ///         directly.
        ///         If you need to modify one, use the coroutine counterpart, 'WaitForSeconds' instead.
        ///     </remarks>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayInSeconds"></param>
        public static async Task WaitForSecondsAsync(Action action, double delayInSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(delayInSeconds);

            await Task.Delay(timeSpan).ContinueWith(_ => action.Invoke());
        }

        /// <summary>
        ///     A coroutine that introduces a delay in the execution flow for a specified duration in seconds.
        ///     It can log a debug message to indicate the duration of the wait if debug logging is enabled.
        /// </summary>
        /// <remarks> Keep in mind that this method calls for an action, which means you can call multiple at once. </remarks>
        /// <example>StartCoroutine(WaitForSeconds(() => action, 2,5f, false); </example>
        /// <param name="action">The action or method to run. </param>
        /// <param name="delayInSeconds">The delay before running the method.</param>
        /// <param name="debugLog">Whether or not to debug the waiting message and the completion message. </param>
        /// <param name="useRealtime">Allows you to run the method using WaitForSecondsRealtime if needed. </param>
        public static IEnumerator WaitForSeconds(
            Action action = default, float delayInSeconds = default, bool debugLog = false, bool useRealtime = false)
        { // Debug the waiting message.
            if (debugLog) Debug.Log($"Waiting for {delayInSeconds} seconds...");

            if (useRealtime) yield return new WaitForSecondsRealtime(delayInSeconds);
            else yield return new WaitForSeconds(delayInSeconds);

            // Run the action, if there is one.
            action?.Invoke();

            // Debug the completion message.
            if (debugLog) Debug.Log("Action completed.");

            yield return null;
        }
        #endregion
    }

    public static class Attributes
    {
        /// <example> [SerializeField, ReadOnly] bool readOnlyBool; </example>
        /// <remarks> Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector. </remarks>
        public class ReadOnlyAttribute : PropertyAttribute { }

        /// <summary>
        ///     Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector.
        ///     Small but useful script, to make your inspectors look pretty and useful :D
        ///     <example> [SerializedField, ReadOnly] int myInt; </example>
        /// </summary>
        [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        public class ReadOnlyPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
        }
    }

    public static class Deprecated
    {
#if USE_DEPRECATED_FEATURES
        /// <summary>
        ///     Allows you to call a method after a delay through the use of an asynchronous operation.
        /// </summary>
        /// <remarks> To run a method after the task is completed: Task delayTask = delayTask.ContinueWith(_ => action();</remarks>
        /// <example> DelayedTaskAsync(() => action(), delayInSeconds, debugLog, cancellationToken).AsTask(); </example>
        /// <param name="action">The action or method to run. Use delegate lambda " () => " to run. </param>
        /// <param name="delayInSeconds">The delay before running the method.</param>
        /// <param name="debugLog">Whether or not to debug the waiting message.</param>
        /// <param name="cancellationToken"> Token for cancelling the currently running task. Not required. </param>
        [Obsolete
            ("This method has been deprecated due to an updated version called 'WaitForSeconds', please use that instead.")]
        public static async UniTask DelayedTaskAsync(
            Action action, double delayInSeconds, bool debugLog = false, CancellationToken cancellationToken = default)
        {
            if (debugLog) Debug.Log($"Waiting for {delayInSeconds} seconds...");
            TimeSpan timeSpan = TimeSpan.FromSeconds(delayInSeconds);
            await UniTask.Delay(timeSpan, cancellationToken).AsTask();
            action();
            if (debugLog) Debug.Log("Action completed.");
        }
#endif
    }

    public static class Helpers
    {
        /// <summary>
        ///     Returns the main camera so that you don't have to use Camera.main every time.
        ///     i.e, instead of writing 'Camera.main' or caching it as a variable, you can just write 'Helpers.Camera'.
        ///     Optionally, you can then import the namespace 'using static Helpers;' to use it as 'Camera' instead.
        /// </summary>
        static Camera camera;
        public static Camera Camera
        {
            get {
                if (camera == null) camera = Camera.main;
                return camera;
            }
        }
    }
}