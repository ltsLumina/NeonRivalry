using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Lumina.Essentials
{
    public static class Sequencing
    {
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
        /// <example>StartCoroutine(DelayedAction(() => action, 2,5f);</example>
        /// <example>IEnumerator delayedAction = DelayedAction(() => action, 2.5f);</example> // Alternate syntax.
        /// <param name="action">The action or method to run. </param>
        /// <param name="delayInSeconds">The delay before running the method.</param>
        /// <param name="useRealtime">Waits the delayInSeconds using unscaled time. </param>
        /// <param name="debugLog">Whether or not to debug the waiting message and the completion message. </param>
        public static IEnumerator DelayedAction(Action action = default, float delayInSeconds = default, bool useRealtime = false, bool debugLog = false)
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
        ///         If you need to modify one, use the coroutine counterpart, 'DelayedAction' instead.
        ///     </remarks>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayInSeconds"></param>
        public static async Task DelayedActionAsync(Action action, double delayInSeconds)
        {
            TimeSpan delayAsTimeSpan = TimeSpan.FromSeconds(delayInSeconds);

            await Task.Delay(delayAsTimeSpan).ContinueWith(_ => action.Invoke());
        }
        #endregion
    }
}
