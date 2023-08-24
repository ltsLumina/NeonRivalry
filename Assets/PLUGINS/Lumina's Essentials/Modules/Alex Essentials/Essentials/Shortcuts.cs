#region
using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

//TODO: Rework this script entirely. It's a mess.
//TODO: I'll probably have to move this to the Editor Modules folder, but that would remove the ability to call the methods in other scripts.

namespace Lumina.Essentials.Modules
{
    public static class Shortcuts
    {
        // Clear Console Keys
        const KeyCode clearConsoleKey = KeyCode.C;
#if UNITY_EDITOR
        const ShortcutModifiers clearConsoleModifiers = ShortcutModifiers.Shift | ShortcutModifiers.Control;
#endif
        
        // Reload Scene Keys
        const KeyCode reloadSceneKey = KeyCode.Backspace;
#if UNITY_EDITOR
        const ShortcutModifiers reloadSceneModifiers = ShortcutModifiers.Shift | ShortcutModifiers.Control;
#endif

        /// <summary>
        ///     Alt+ C to clear the console in the Unity Editor.
        /// </summary>
#if UNITY_EDITOR
        [Shortcut("Clear Console", clearConsoleKey, clearConsoleModifiers)]
#endif
        public static async void ClearConsole()
        {
#if UNITY_EDITOR
                ShortcutPressedWarning("Clearing Console...", clearConsoleKey, clearConsoleModifiers);
#endif
                // Wait for a little to give the reader time to read the warning message.
                await Task.Delay(1500);

#if UNITY_EDITOR
                var assembly = Assembly.GetAssembly(typeof(SceneView));
                if (assembly == null) return;
                Type       type   = assembly.GetType("UnityEditor.LogEntries");
                MethodInfo method = type.GetMethod("Clear");
                method?.Invoke(new (), null);
#endif
        }

        /// <summary>
        ///     Alt + Backspace to reload the scene in the Unity Editor.
        /// </summary>
#if UNITY_EDITOR
        [Shortcut("Reload Scene", reloadSceneKey, reloadSceneModifiers)]
#endif
        public static async void ReloadScene()
        {
#if UNITY_EDITOR
            ShortcutPressedWarning("Reloading Scene...", reloadSceneKey, reloadSceneModifiers);
#endif

            // Wait for a little to give the reader time to read the warning message.
            await Task.Delay(1500);
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
        }

        // Warning method for when a shortcut is pressed.
#if UNITY_EDITOR
            static void ShortcutPressedWarning(string warningMessage, KeyCode shortcutKey, ShortcutModifiers shortcutModifier1, ShortcutModifiers shortcutModifiers2 = default)
            {
                    var shortcutStr = $"{shortcutKey} pressed.";

                    if (!Equals(shortcutModifier1, default(ShortcutModifiers))) shortcutStr  = $"{shortcutModifier1} + "  + shortcutStr;
                    if (!Equals(shortcutModifiers2, default(ShortcutModifiers))) shortcutStr = $"{shortcutModifiers2} + " + shortcutStr;

                    Debug.LogWarning(shortcutStr + "\n" + warningMessage);
            }
#endif
    }
}
