using System;
using System.Reflection;
using UnityEngine;

namespace Abiogenesis3d
{
    public partial class Utils
    {
        public static void AddCallbackToStart<T>(Type type, string methodName, T callback)
        {
            FieldInfo fieldInfo = type.GetField(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            var currentCallbacks = (T)fieldInfo.GetValue(null);

            Delegate currentDelegate = currentCallbacks as Delegate;
            Delegate newDelegate = callback as Delegate;

            if (currentDelegate == null)
            {
                fieldInfo.SetValue(null, newDelegate);
                return;
            }

            if (newDelegate != null)
            {
                fieldInfo.SetValue(null, Delegate.Combine(newDelegate, currentDelegate));
                return;
            }

            Debug.LogError("Unable to combine delegates: Invalid callback types.");
        }
    }
}
