using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Refresh inspector while in play mode
        if (Application.isPlaying) Repaint();

        // Draw the default inspector
        DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Draw read-only GameState
        EditorGUILayout.LabelField("Current State:  ", GameManager.State.ToString(), EditorStyles.boldLabel);
    }
}
