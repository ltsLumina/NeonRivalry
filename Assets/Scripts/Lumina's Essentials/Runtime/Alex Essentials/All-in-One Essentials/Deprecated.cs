// Uncomment the following line to enable the use of deprecated features or plugin-related features.
//#define USING_DEPRECATED

// !! WARNING !!
// THIS SCRIPT IS DEPRECATED. IT WILL REMAIN IN THE PROJECT FOR LEGACY PURPOSES, BUT WILL NOT BE UPDATED.
// YOU MUST USED "UniTask" FOR THIS TO EVEN WORK.

#region
using System;
using System.Threading;
using UnityEngine;
#if USING_DEPRECATED
using Cysharp.Threading.Tasks;
#endif
#endregion

namespace Lumina.Essentials
{
    public static class Deprecated
    {
#if USING_DEPRECATED
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
    }
}