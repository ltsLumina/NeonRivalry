using Microsoft.Win32;
using UnityEditor;
using UnityEngine;

namespace Lumina.Essentials.Editor.UI
{
internal class EditorPrefsWindow : EditorWindow
{
#if DEBUG_BUILD
    [MenuItem("Lumina's Essentials/Editor Preferences")]
#endif
    internal static void ShowWindow() => GetWindow<EditorPrefsWindow>("Editor Preferences");

    Vector2 scrollPos;

    void OnGUI()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Unity Technologies\Unity Editor 5.x\");

        if (key == null)
        {
            EditorGUILayout.LabelField("No EditorPrefs found.");
            return;
        }

        EditorGUILayout.LabelField("All EditorPrefs:");

        // Begin a scroll view. Note the use of the field to store the scroll position.
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (string valueName in key.GetValueNames())
        {
            var value = key.GetValue(valueName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(valueName);
            EditorGUILayout.LabelField(value.ToString(), GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
        }

        // Always do a `EndScrollView` when you do a `BeginScrollView`
        EditorGUILayout.EndScrollView();
    }
}
}
