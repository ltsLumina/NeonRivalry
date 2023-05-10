// COMMENT OUT THIS IF YOU ARE NOT USING IT
//#define IN_USE

#if IN_USE
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Essentials
{

    /// <summary>
    ///     A collection of debugging shortcuts.
    ///     Includes keyboard shortcuts tied to the F-keys, as well as context menus.
    /// </summary>
    public static class UsefulShortcuts
    {
#if UNITY_EDITOR //!WARNING! This class is only accessible in the Unity Editor, and may cause errors when building the game.
        /// <summary>
        ///     Alt+ C to clear the console in the Unity Editor.
        /// </summary>
        [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
        public static void ClearConsole()
        {
            var        assembly = Assembly.GetAssembly(typeof(SceneView));
            Type       type     = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method   = type.GetMethod("Clear");
            method?.Invoke(new(), null);
        }

        [Shortcut("Damage Player", KeyCode.F1), ContextMenu("Damage Player")]
        static void DamagePlayer()
        {
            // Damage the player by 10.
            GameManager.Instance.Player.Health -= 10;
            Debug.Log("Player damaged.");
        }

        [Shortcut("Heal Player", KeyCode.F2), ContextMenu("Heal Player")]
        static void HealPlayer()
        {
            // Heal the player by 10.
            if (GameManager.Instance.Player.IsDead) return;
            GameManager.Instance.Player.Health += 10;
            Debug.Log("Player healed.");
        }

        [Shortcut("Kill Player", KeyCode.F3), ContextMenu("Kill Player")]
        static void KillPlayer()
        {
            // Kill the player.
            GameManager.Instance.Player.Health = 0;
            Debug.Log("Player killed.");
        }

        [Shortcut("Reload Scene", KeyCode.F5), ContextMenu("Reload Scene")]
        static void ReloadScene()
        {
            // Reload Scene
            SceneManagerExtended.ReloadScene();
            Debug.Log("Scene reloaded.");
        }
#endif
    }
}
#endif