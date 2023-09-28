#region
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Sequencer
{
/// <summary>
///     Represents a sequence of actions to be executed.
///     Actions are processed in the order the are added.
/// </summary>
public sealed class Sequence : ISequence
{
    /// <summary>The <see cref="MonoBehaviour" /> on which the sequence is hosted.</summary>
    readonly MonoBehaviour host;

    /// <summary>The <see cref="RoutineHandler" /> used to execute the actions.</summary>
    readonly RoutineHandler routineHandler;

    /// <summary>The actions to be executed.</summary>
    readonly Queue<IEnumerator> actions = new ();

    /// <summary>
    ///     Initializes a new instance of the <see cref="Sequence" /> class.
    /// </summary>
    /// <param name="host">The <see cref="MonoBehaviour" /> on which the sequence is hosted.</param>
    public Sequence(MonoBehaviour host)
    {
        this.host      = host;
        routineHandler = new (host);
        host.StartCoroutine(ProcessActions());
    }

    /// <summary>
    ///     Starts processing the actions in the sequence.
    /// </summary>
    /// <returns>An enumerator that iterates through the sequence.</returns>
    IEnumerator ProcessActions()
    {
        while (true)
        {
            if (host == null) yield break;

            if (actions.Count > 0) yield return host.StartCoroutine(actions.Dequeue());
            else yield return new WaitUntil(() => actions.Count > 0);
        }
    }

    /// <summary>
    ///     Executes the specified action.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence Execute(Action action)
    {
        actions.Enqueue(routineHandler.ExecuteRoutine(action));
        return this;
    }

    /// <summary>
    ///     Executes the specified coroutine.
    /// </summary>
    /// <param name="routine">The coroutine to execute.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence ExecuteCoroutine(IEnumerator routine)
    {
        actions.Enqueue(routineHandler.ExecuteCoroutineRoutine(routine));
        return this;
    }

    /// <summary>
    ///     Waits for the specified duration, then executes the specified action.
    /// </summary>
    /// <param name="duration">The duration to wait before executing the action (in seconds).</param>
    /// <param name="action">The action to execute after the duration has passed.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence WaitThenExecute(float duration, Action action)
    {
        actions.Enqueue(routineHandler.WaitThenExecuteRoutine(duration, action));
        return this;
    }

    /// <summary>
    ///     Waits for the specified duration.
    /// </summary>
    /// <param name="duration">The duration to wait (in seconds).</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence WaitForSeconds(float duration)
    {
        actions.Enqueue(routineHandler.WaitForSecondsRoutine(duration));
        return this;
    }

    /// <summary>
    ///     Executes a follow up action after the previous action has completed.
    /// </summary>
    /// <param name="action">The action to execute after the previous action has completed.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence ContinueWith(Action action)
    {
        actions.Enqueue(routineHandler.ContinueWithRoutine(action));
        return this;
    }

    /// <summary>
    ///     Executes a follow up action after the previous action has completed, if the specified condition is true.
    /// </summary>
    /// <param name="action">The action to execute if the condition is true.</param>
    /// <param name="condition">The condition to verify.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence ContinueWithIf(Action action, bool condition)
    {
        actions.Enqueue(routineHandler.ContinueWithIfRoutine(action, () => condition));
        return this;
    }

    /// <summary>
    ///     Executes a follow up action after the previous action has completed, if the specified condition is true.
    /// </summary>
    /// <param name="action">The action to execute if the condition is true.</param>
    /// <param name="condition">A delegate that returns the condition to verify.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence ContinueWithIf(Action action, Func<bool> condition)
    {
        actions.Enqueue(routineHandler.ContinueWithIfRoutine(action, condition));
        return this;
    }

    /// <summary>
    ///     Executes an action when the sequence completes successfully.
    /// </summary>
    /// <param name="message">
    ///     The message to print to the console when the sequence completes successfully. If the message is
    ///     null, the default message will be used.
    /// </param>
    /// <returns>The current sequence instance.</returns>
    public ISequence OnComplete(string message)
    {
        actions.Enqueue(routineHandler.OnCompleteRoutine(message));
        return this;
    }

    /// <summary>
    ///     Executes an action when an exception occurs in the sequence.
    /// </summary>
    /// <param name="action">The action to execute when an exception occurs.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence OnFail(Action<Exception> action)
    {
        actions.Enqueue(routineHandler.OnFailRoutine(action));
        return this;
    }

    /// <summary>
    ///     Repeats specified action specified number of times.
    /// </summary>
    /// <param name="times">The number of repetitions.</param>
    /// <param name="action">The action to repeat.</param>
    /// <returns>The current sequence instance.</returns>
    public ISequence RepeatExecute(int times, Action action)
    {
        actions.Enqueue(routineHandler.RepeatExecuteRoutine(times, action));
        return this;
    }
}
}
