#region
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
#endif
using UnityEngine;
#endregion

namespace Lumina.Essentials
{
    public static class Shortcuts
    {
#if UNITY_EDITOR
        /// <summary>
        ///     Alt+ C to clear the console in the Unity Editor.
        /// </summary>
        [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
        public static void ClearConsole()
        {
            var        assembly = Assembly.GetAssembly(typeof(SceneView));
            Type       type     = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method   = type.GetMethod("Clear");
            method?.Invoke(new (), null);
        }

        /// <summary>
        ///     Alt + Backspace to reload the scene in the Unity Editor.
        /// </summary>
        [Shortcut("Reload Scene", KeyCode.Backspace, ShortcutModifiers.Alt)]
        public static void ReloadScene() => SceneManagerExtended.ReloadScene();
#endif
    }
}
