// Uncomment the following lines to enable the use of deprecated features or plugin-related features.
//#define USING_UNITASK
#define USING_DEPRECATED

// !! WARNING !!
// THIS SCRIPT IS DEPRECATED. IT WILL REMAIN IN THE PROJECT FOR LEGACY PURPOSES, BUT WILL NOT BE UPDATED.

#region
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if USING_UNITASK
using Cysharp.Threading.Tasks;
#endif
#endregion

namespace Lumina.Essentials.Modules
{
public static class Deprecated
{
#if USING_UNITASK
        /// <summary>
        ///     Allows you to call a method after a delay through the use of an asynchronous operation.
        /// </summary>
        /// <remarks> To run a method after the task is completed: Task delayTask = delayTask.ContinueWith(_ => action();</remarks>
        /// <example> DelayedTaskAsync(() => action(), delayInSeconds, debugLog, cancellationToken).AsTask(); </example>
        /// <param name="action">The action or method to run. Use delegate lambda " () => " to run. </param>
        /// <param name="delayInSeconds">The delay before running the method.</param>
        /// <param name="cancellationToken"> Token for cancelling the currently running task. </param>
        /// <param name="debugLog">Whether or not to debug the waiting message.</param>
        [Obsolete("This method has been deprecated due to an updated version called 'DelayedAction', please use that instead.")]
        public static async UniTask DelayedTaskAsync(Action action, double delayInSeconds, CancellationToken cancellationToken = default, bool debugLog = false)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(delayInSeconds);

            if (debugLog) Debug.Log($"Waiting for {delayInSeconds} seconds...");

            await UniTask.Delay(timeSpan, cancellationToken: cancellationToken).AsTask();

            action?.Invoke();

            if (debugLog) Debug.Log("Action completed.");
        }
#endif

    // !! WARNING !!
    // OLD SEQUENCING METHODS, STILL WORK BUT ARE DEPRECATED AS THEY ARE REPLACED BY THE NEWER VERSIONS <see cref="Sequencing"/>

    // The sequencing methods are used to run actions before and after a delay.
    // All methods have a async counterpart that can be used instead of the coroutine version if you prefer.
    #region SEQUENCING METHODS
    /// <summary>
    ///     This method provides a convenient way invoke a specified action and then execute a consequent action after the
    ///     first action completes.
    ///     Possible use cases could be to display a UI element for a few seconds and then hide it by running the show method,
    ///     waiting, and then running the hide method.
    /// </summary>
    /// <example>SequenceActions(() => initialAction, 2,5f, consequentAction); </example>
    /// <param name="initialAction">The initial action to run. Completes as soon as this method runs. </param>
    /// <param name="delayInSeconds">The delay before running the consequentAction.</param>
    /// <param name="consequentAction">The action to be run after the initial action is finished.</param>
    /// <param name="useRealtime">Waits the delayInSeconds using unscaled time.</param>
    /// <param name="debugLog">Whether or not to debug the status of the method. Useful for debugging. </param>
    [Obsolete("This method is obsolete. Refer toLumina.Essentials.Sequencing for the updated version.")]
    public static IEnumerator SequenceActions(Action initialAction, float delayInSeconds, Action consequentAction, bool useRealtime = default, bool debugLog = false)
    {
        // Run the action.
        initialAction?.Invoke();

        // Debug the completion and waiting messages.
        if (debugLog)
        {
            Debug.Log($"' {initialAction?.Method.Name} ' completed.");
            Debug.Log($"Waiting for {delayInSeconds} seconds...");
        }

        // Wait for the delay time to pass.
        yield return !useRealtime ? new WaitForSeconds(delayInSeconds) : new WaitForSecondsRealtime(delayInSeconds);

        // Run the consequentAction action.
        consequentAction?.Invoke();

        // Debug the completion message.
        if (debugLog) Debug.Log($"' {consequentAction?.Method.Name} ' completed.");

        yield return null;
    }

    /// <summary>
    ///     This method provides a convenient way to asynchronously invoke a specified action and then execute a consequent
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
    /// <example>SequenceActions(() => initialAction, 2,5f, consequentAction); </example>
    /// <param name="initialAction">The initial action to run. Completes as soon as this method runs. </param>
    /// <param name="delayInSeconds">The delay before running the consequentAction.</param>
    /// <param name="consequentAction">The action to be run after the initial action is finished.</param>
    /// <param name="cancellationToken">A CancellationToken to be passed in if you wish to cancel the Task mid-way. </param>
    /// <param name="debugLog">Whether or not to debug the status of the method. Useful for debugging. </param>
    [Obsolete("This method is obsolete. Refer to Lumina.Essentials.Sequencing for the updated version.")]
    public static async Task SequenceActionsAsync
        (Action initialAction, double delayInSeconds, Action consequentAction, CancellationToken cancellationToken = default, bool debugLog = false)
    {
        // Introduce a variable to store the delay time.
        TimeSpan delayAsTimeSpan = TimeSpan.FromSeconds(delayInSeconds);

        // Run the action.
        initialAction?.Invoke();

        // Debug the completion and waiting messages.
        if (debugLog)
        {
            Debug.Log($"' {initialAction?.Method.Name} ' completed.");
            Debug.Log($"Waiting for {delayInSeconds} seconds...");
        }

        // Wait for the delay time to pass and then run the consequentAction action.
        await Task.Delay(delayAsTimeSpan, cancellationToken).ContinueWith
        (_ =>
        {
            // Cancel the task if the cancellation token has been requested.
            cancellationToken.ThrowIfCancellationRequested();

            // Run the consequentAction action.
            consequentAction?.Invoke();

            // Debug the completion message.
            if (debugLog) Debug.Log($"' {consequentAction?.Method.Name} ' completed.");
        }, cancellationToken);
    }
    #endregion

    #region DELAY METHODS
    /// <summary>
    ///     A simple coroutine that introduces a delay in the execution flow for a specified duration in seconds.
    /// </summary>
    /// <example>StartCoroutine(DelayedAction(() => action, 2,5f);   </example>
    /// <example>IEnumerator delayedAction = DelayedAction(() => action, 2.5f);</example>
    /// // Alternate syntax.
    /// <param name="action">The action or method to run. </param>
    /// <param name="delayInSeconds">The delay before running the method.</param>
    /// <param name="useRealtime">Waits the delayInSeconds using unscaled time. </param>
    /// <param name="debugLog">Whether or not to debug the waiting message and the completion message. </param>
    [Obsolete("This method is obsolete. Refer to Lumina.Essentials.Sequencing for the updated version.")]
    public static IEnumerator DelayedAction(Action action, float delayInSeconds = default, bool useRealtime = false, bool debugLog = false)
    {
        // Debug the waiting message.
        if (debugLog) Debug.Log($"Waiting for {delayInSeconds} seconds...");

        // Wait for the delay time to pass.
        yield return !useRealtime ? new WaitForSeconds(delayInSeconds) : new WaitForSecondsRealtime(delayInSeconds);

        // Run the action.
        action?.Invoke();

        // Debug the completion message.
        if (debugLog) Debug.Log("Action completed.");

        yield return null;
    }

    /// <summary>
    ///     Simple asynchronous method to delay an action by a specified amount of time.
    ///     <remarks>
    ///         !! WARNING !!
    ///         Components can only be modified from the main thread, meaning this method wont be able to affect components
    ///         directly.
    ///         If you need to modify one, use the coroutine counterpart, <see cref="DelayedAction" /> instead.
    ///     </remarks>
    /// </summary>
    /// <param name="action"></param>
    /// <param name="delayInSeconds"></param>
    [Obsolete("This method is obsolete. Refer to Lumina.Essentials.Sequencing for the updated version.")]
    public static async Task DelayedActionAsync(Action action, double delayInSeconds)
    {
        TimeSpan delayAsTimeSpan = TimeSpan.FromSeconds(delayInSeconds);

        await Task.Delay(delayAsTimeSpan).ContinueWith(_ => action.Invoke());
    }
    #endregion
}
}
