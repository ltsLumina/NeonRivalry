using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abiogenesis3d
{
    public partial class Utils
    {
        private static List<(Action action, int order)> actions = new List<(Action action, int order)>();
        private static Coroutine coroutine = null;

        public static void RunAtEndOfFrameOrdered(Action action, int order, MonoBehaviour caller)
        {
            actions.Add((action, order));
            if (coroutine == null)
            {
                if (caller.isActiveAndEnabled)
                    coroutine = caller.StartCoroutine(WaitForEndOfFrameAndExecute());
            }
        }

        public static IEnumerator WaitForEndOfFrameAndExecute()
        {
            // NOTE: unreliable, WaitForEndOfFrame will not be called when game window is not opened...
            yield return new WaitForEndOfFrame();

            foreach (var action in actions.OrderBy(a => a.order))
                action.action.Invoke();

            actions.Clear();
            coroutine = null;
        }
    }
}

    // IEnumerator RunAtEndOfFrame(Action action)
    // {
    //     yield return new WaitForEndOfFrame();
    //     action?.Invoke();
    // }
