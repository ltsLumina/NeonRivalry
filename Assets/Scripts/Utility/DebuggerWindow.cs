#region
using UnityEditor;
using UnityEngine;
using static State;
#endregion

// Lumina.Debugging namespace contains classes for debugging Lumina game engine
namespace Lumina.Debugging
{
/// <summary>
///     DebuggerWindow provides a custom editor window for the Unity Editor that is used to debug states.
/// </summary>
public class DebuggerWindow : EditorWindow
{
    // Level enum is used to specify the log level
    public enum Level
    {
        TRACE,
        DEBUG,
        INFO,
        NONE, // No log reporting
    }

    // LogLevel Property used to get or set current log level
    public static Level LogLevel { get; private set; } = Level.NONE;

    // The debugger will only log states that match the ActiveStateType
    public static StateType ActiveStateType { get; private set; } = StateType.Idle;

    // MenuItem attribute that adds a new menu item under "Tools/State Debugger" in the Editor's menu bar.
    [MenuItem("Tools/State Debugger")]
    static void Open()
    {
        // Creates a new DebuggerWindow or focus an existing one
        var window = (DebuggerWindow) GetWindow(typeof(DebuggerWindow));

        // Sets the window title
        window.titleContent = new ("State Debugger");

        // Shows the window
        window.Show();
    }

    void OnEnable()
    {
        Initialization();

        return;

        // The Initialization method initializes the debugger window
        void Initialization()
        {
            // Close the window if there is more than one instance of the window.
            if (Resources.FindObjectsOfTypeAll<DebuggerWindow>().Length > 1) Close();
        }
    }

    // OnGUI is called for rendering and handling GUI events.
    void OnGUI()
    {
        // GUI layout for Log Level
        GUILayout.Label("Log Level", EditorStyles.boldLabel);
        LogLevel = (Level) EditorGUILayout.EnumPopup(LogLevel);

        // GUI Layout for State to Debug
        GUILayout.Label("State to Debug", EditorStyles.boldLabel);
        ActiveStateType = (StateType) EditorGUILayout.EnumPopup(ActiveStateType);
    }
}
}
