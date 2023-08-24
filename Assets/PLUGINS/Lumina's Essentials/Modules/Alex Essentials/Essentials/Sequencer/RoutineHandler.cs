#region
using System;
using System.Collections;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Sequencer
{
/// <summary>
///     Represents a handler for managing routines and steps within a sequence.
///     It executes them while taking care of exceptions that can occur during execution.
/// </summary>
public sealed class RoutineHandler
{
    readonly MonoBehaviour host;

    /// <summary>Gets or sets the Exception if occurred while executing routine.</summary>
    public Exception Exception { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RoutineHandler" />.
    /// </summary>
    /// <param name="host">The host of this routine.</param>
    public RoutineHandler(MonoBehaviour host) { this.host = host; }

    /// <summary>
    ///     Executes the specified action and handles exceptions if occur.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>An enumerator that executes the action.</returns>
    public IEnumerator ExecuteRoutine(Action action)
    {
        SequenceErrorHandler.TryExecute(action, out Exception exception);

        if (exception != null)
        {
            Exception = exception;
            Debug.LogError(exception);
        }

        yield return null;
    }

    /// <summary>
    ///     Executes the specified coroutine and handles exceptions if occur.
    /// </summary>
    /// <param name="coroutine">The action to execute.</param>
    /// <returns>An enumerator that executes the action.</returns>
    public IEnumerator ExecuteCoroutineRoutine(IEnumerator coroutine)
    {
        SequenceErrorHandler.TryExecuteCoroutine(host, coroutine, out Exception exception);

        if (exception != null)
        {
            Exception = exception;
            Debug.LogError(exception);
        }

        yield return null;
    }

    /// <summary>
    ///     Waits for the specified duration, then executes the specified action.
    /// </summary>
    /// <param name="seconds">The duration to wait before executing the action (in seconds).</param>
    /// <param name="action">The action to execute after the duration has passed.</param>
    /// <returns>An enumerator that waits then executes the action.</returns>
    public IEnumerator WaitThenExecuteRoutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        yield return host.StartCoroutine(ExecuteRoutine(action));
    }

    /// <summary>
    ///     Waits for the specified duration.
    /// </summary>
    /// <param name="seconds">The duration to wait (in seconds).</param>
    /// <returns>An enumerator that waits for the specified duration.</returns>
    public IEnumerator WaitForSecondsRoutine(float seconds) { yield return new WaitForSeconds(seconds); }

    /// <summary>
    ///     Executes a follow-up action after the previous action has completed.
    /// </summary>
    /// <param name="action">The action to execute after the previous action has completed.</param>
    /// <returns>An enumerator that waits then executes the given action.</returns>
    public IEnumerator ContinueWithRoutine(Action action)
    {
        yield return null;
        yield return host.StartCoroutine(ExecuteRoutine(action));
    }

    /// <summary>
    ///     Executes a follow-up action after the previous action has completed if the specified condition is true.
    /// </summary>
    /// <param name="action">The action to execute if the condition is met.</param>
    /// <param name="condition">The condition to check before executing the action.</param>
    /// <returns>An enumerator that checks the condition and then if condition is met, waits then executes the given action.</returns>
    public IEnumerator ContinueWithIfRoutine(Action action, Func<bool> condition)
    {
        while (!condition()) yield return null;
        yield return host.StartCoroutine(ExecuteRoutine(action));
    }

    /// <summary>
    ///     Logs a message when the sequence completes successfully.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>An enumerator that logs the message.</returns>
    public IEnumerator OnCompleteRoutine(string message)
    {
        yield return null;
        Debug.Log(message ?? "Sequence completed successfully.");
    }

    /// <summary>
    ///     Executes an action when an exception occurs.
    /// </summary>
    /// <param name="action">The action to execute if an exception occurs.</param>
    /// <returns>An enumerator that waits then if an exception occurs, executes the given action.</returns>
    public IEnumerator OnFailRoutine(Action<Exception> action)
    {
        yield return null;
        if (Exception != null) action?.Invoke(Exception);
    }

    /// <summary>
    ///     Repeats the specified action a number of times.
    /// </summary>
    /// <param name="times">The number of times to repeat the action.</param>
    /// <param name="action">The action to repeat.</param>
    /// <returns>An enumerator that repeats execution of the given action specified number of times.</returns>
    public IEnumerator RepeatExecuteRoutine(int times, Action action)
    {
        for (int i = 0; i < times; i++) { yield return host.StartCoroutine(ExecuteRoutine(action)); }
    }
}
}
