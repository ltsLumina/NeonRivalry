#region
using UnityEditor;
using UnityEngine;
using static State;
using static Lumina.Debugging.Logger;
using static UnityEngine.GUILayout;
#endregion

// Lumina.Debugging namespace contains classes for debugging
namespace Lumina.Debugging
{
/// <summary>
///     StateDebuggerWindow provides a custom editor window for the Unity Editor that is used to debug states.
/// </summary>
public class StateDebuggerWindow : EditorWindow
{
    static StateDebuggerWindow window;
    
    // MenuItem attribute that adds a new menu item under "Tools/State Debugger" in the Editor's menu bar.
    [MenuItem("Tools/Debugging/State Debugger")]
    public static void Open()
    {
        if (window != null)
        {
            window.Focus();
        }
        else
        {
            var hierarchy = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            window              = GetWindow<StateDebuggerWindow>(hierarchy);
            window.titleContent = new ("State Debugger");
        
            window.Show();
        }
    }
    
    void OnGUI()
    {
        using (new HorizontalScope("box"))
        {
            Label("Log Level", EditorStyles.boldLabel);
            LogLevel = (Level) EditorGUILayout.EnumPopup(LogLevel);
            
            Label("State to Debug", EditorStyles.boldLabel);
            ActiveStateType = (StateType) EditorGUILayout.EnumPopup(ActiveStateType);
        }

        Space(10);
        
        // Horizontal line (separator)
        Label("", GUI.skin.horizontalSlider);
        
        Space(25);
        
        Label("Debug Modes", EditorStyles.boldLabel);
        DebugMode = EditorGUILayout.Toggle("Debug Mode", DebugMode);
    }
}
}