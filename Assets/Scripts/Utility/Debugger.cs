#region
using UnityEditor;
using UnityEngine;
#endregion

public class Debugger : EditorWindow
{
    public enum Level
    {
        TRACE,
        DEBUG,
        INFO,
        NONE,
    }

    [MenuItem("Tools/State Debugger")]
    static void Open()
    {
        var window = (Debugger) GetWindow(typeof(Debugger));
        window.titleContent = new ("State Debugger");
        window.Show();
    }

    // These could be set elsewhere in your code, which determines what will be logged.
    static Level LogLevel = Level.NONE;
    static State.StateType ActiveStateType = State.StateType.Idle;

    void OnEnable()
    {
        Initialization();

        return;

        void Initialization()
        {
            // Close the window if there is more than one instance of the window.
            if (Resources.FindObjectsOfTypeAll<Debugger>().Length > 1) Close();
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Log Level", EditorStyles.boldLabel);
        LogLevel = (Level) EditorGUILayout.EnumPopup(LogLevel);

        GUILayout.Label("State to Debug", EditorStyles.boldLabel);
        ActiveStateType = (State.StateType) EditorGUILayout.EnumPopup(ActiveStateType);
    }

    #region
    public static void Info(string message, State.StateType? state = null)
    {
        if ((state == null || state == ActiveStateType) && LogLevel == Level.INFO) UnityEngine.Debug.Log($"INFO: {message}");
    }

    public static void Debug(string message, State.StateType? state = null)
    {
        if ((state == null || state == ActiveStateType) && LogLevel == Level.DEBUG) UnityEngine.Debug.Log($"DEBUG: {message}");

        //TODO: Prints multiple times for each method that calls it.
    }

    public static void Trace(string message, State.StateType? state = null)
    {
        if ((state == null || state == ActiveStateType) && LogLevel == Level.TRACE) UnityEngine.Debug.Log($"TRACE: {message}");
    }
    #endregion
}
