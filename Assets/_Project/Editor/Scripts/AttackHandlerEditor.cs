// using System;
// using UnityEditor;
// using UnityEngine;
//
// // Used to display the moveset in the inspector.
// [CustomEditor(typeof(AttackHandler)), Obsolete("Deprecated. This script will be repurposed in the future.")]
// public class AttackHandlerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         var     attackSystem = (AttackHandler) target;
//         Moveset moveset      = attackSystem.ActiveMoveset;
//         
//         // Draw the default inspector of the AttackHandler class.
//         base.OnInspectorGUI();
//         
//         GUILayout.Space(35);
//
//         GUILayout.Label("Active Moveset", EditorStyles.boldLabel);
//
//         // Displays the moveset in the inspector as read-only.
//         using (new EditorGUI.DisabledScope(true))
//         {
//             Editor inspector = CreateEditor(moveset);
//             inspector.OnInspectorGUI();
//         }
//     }
// }
