#if UNITY_EDITOR
#region
using UnityEditor;
using UnityEngine;
using static State;
#endregion

// Lumina.Debugging namespace contains classes for debugging Lumina game engine
namespace Lumina.Debugging
{
/// <summary>
///     FGDebuggerWindow provides a custom editor window for the Unity Editor that is used to debug states.
/// </summary>
public class FGDebuggerWindow : EditorWindow
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
    public static StateType ActiveStateType { get; private set; } = StateType.None;

    // MenuItem attribute that adds a new menu item under "Tools/State Debugger" in the Editor's menu bar.
    [MenuItem("Tools/Debugger")]
    static void Open()
    {
        // Creates a new FGDebuggerWindow or focus an existing one
        var window = (FGDebuggerWindow) GetWindow(typeof(FGDebuggerWindow));

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
            if (Resources.FindObjectsOfTypeAll<FGDebuggerWindow>().Length > 1) Close();
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

#endif