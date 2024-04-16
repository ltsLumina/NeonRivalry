using System.Collections.Generic;
using Lumina.Essentials.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// This is a ScriptableObject that represents a move (e.g. punch, kick, etc.) that can be used by the player.
/// It is used by the Moveset ScriptableObject where each move is stored in an array.
/// This ScriptableObject is created by the MovesetCreator.cs EditorWindow.
/// </summary>
public class MoveData : ScriptableObject
{
    public enum Type
    {
        Punch,
        Kick,
        Slash,
        Unique
    }

    /// <summary>
    ///     The direction that the player needs to move in order to perform this move.
    ///     <para>
    ///         For instance, if a move is set to "Horizontal", then the player needs to press the forward key in order to
    ///         perform the move.
    ///     </para>
    ///     <para>Or if a move is set to "Neutral", then the player needs to press the down key in order to perform the move.</para>
    ///     An example of a move that is set to "Crouch" is a crouching attack or sweep. (2P, 2K, 2S, 2U)
    ///     <para></para>
    ///     <remarks>P = Punch, K = Kick, S = Slash, U = Unique</remarks>
    /// </summary>
    public enum Direction // Note: I don't entirely love the fact that 'Airborne' is included here, but it's the best solution I could come up with.
    {
        Neutral,    // Example: 5P, 5K, 5S, 5U
        Forward, // Example: 6P, 6K, 6S, 6U or 4P, 4K, 4S, 4U
        Down,     // Example: 2P, 2K, 2S, 2U
        Airborne,   // Example: 8P, 8K, 8S, 8U
    }
    
    /// <summary>
    /// The guard height that an enemy needs to be in order to be block this move.
    /// <para>For instance, if a move is set to "Low", then the enemy must be blocking low (crouch-blocking) in order to block the move,
    /// and if they are blocking high (standing-blocking), then they will get hit by the move.</para>
    /// If a move is set to "All", then the enemy can block the move regardless of their guard height.
    /// </summary>
    public enum Guard
    {
        High,
        Low,
        All
    }
    
    public Type type;
    public Direction direction;
    public Guard guard;

    [Space(10)]
    [Header("Resources")] // Note: These variables are unused, and due to Unity limitations, we cannot make them usable. They are intended as reference only.
    public AnimationClip animation;
    public AudioClip audioClip;
    public Sprite sprite;
    
    [Space(15)]
    [Header("Move Attributes")]
    
    [Tooltip("Name of the move.")]
    new public string name;

    [Tooltip("Description of the move.")]
    public string description;

    [Tooltip("The name of the move's owner." + "\nReally only used to easier know which move belongs to who when looking at the MoveData")]
    [SerializeField, ReadOnly] PlayerController owner;
    
    [Tooltip("Damage caused by the move.")]
    public int damage;

    [Tooltip("The amount of force applied to the enemy when hit by the move.")]
    public Vector2 knockbackForce;
    
    [Tooltip("The direction the enemy is knocked back when hit by the move.")]
    public Vector2 knockbackDir;

    [Space(10)]
    
    [Tooltip("Whether this move should knock-back the attacker.")]
    public bool knockBackAttacker;

    [Tooltip("The amount of force applied to the attacker when hit by the move.")]
    public float attackerKnockbackMultiplier = 1;

    [Space(10)]
    
    [Tooltip("Whether this move performs a different knockback when the player being hit is in the air.")]
    public bool aerialOverrideKnockback;
    
    public Vector2 aerialKnockbackForce;
    public Vector2 aerialKnockbackDir;
    
    [Space(10)]
    
    public bool screenShake;
    public float screenShakeAmplitude = 1;
    public float screenShakeFrequency = 1;
    public float screenShakeDuration = 0.25f;

    [Tooltip("The duration of the hitstun. (Multiplier)")]
    public float hitstunDuration = 1;

    [Space(15)]
    [Header("Move Properties")]

    [Tooltip("If this is true and the target is crouching, this attack will miss.")]
    public bool isAirborne;

    [Tooltip("A move that must be blocked while standing.")]
    public bool isOverhead;

    [Tooltip("Whether the move ignores the enemy's guard.")]
    public bool isGuardBreak;
    
    [Space(15)]
    [Header("Effects"), Tooltip("Effects that are applied to a player when the move is performed. For instance, a move could apply a stun effect to the enemy.")]
    public List<MoveEffect> moveEffects;

#if UNITY_EDITOR
    #region Editor-Only
    [HideInInspector]
    public bool showWarning;
    [HideInInspector]
    public Vector2 wrongDir;
    [HideInInspector]
    public Vector2 wrongForce;
    
    void OnValidate()
    {
        #region Knockback
        knockbackDir.x = Mathf.Clamp(knockbackDir.x, -1, 1);
        knockbackDir.y = Mathf.Clamp(knockbackDir.y, -1, 1);
        knockbackForce.x = Mathf.Clamp(knockbackForce.x, -15, 15);
        knockbackForce.y = Mathf.Clamp(knockbackForce.y, -15, 15);
        
        aerialKnockbackDir.x = Mathf.Clamp(aerialKnockbackDir.x, -1, 1);
        aerialKnockbackDir.y = Mathf.Clamp(aerialKnockbackDir.y, -1, 1);
        aerialKnockbackForce.x = Mathf.Clamp(aerialKnockbackForce.x, -15, 15);
        aerialKnockbackForce.y = Mathf.Clamp(aerialKnockbackForce.y, -15, 15);

        // If the dir is non-zero on an axis and a force is zero on that axis, or vice versa, warn the user
        if ((knockbackDir.x != 0 && knockbackForce.x == 0) || (knockbackDir.y != 0 && knockbackForce.y == 0) || (knockbackDir.x == 0 && knockbackForce.x != 0) || (knockbackDir.y == 0 && knockbackForce.y != 0))
        {
            showWarning = true;
            wrongDir    = knockbackDir;
            wrongForce  = knockbackForce;
        }
        else { showWarning = false; }
        #endregion
    }
    #endregion
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MoveData))]
public class MoveDataEditor : Editor
{
    MoveData moveData;
    bool leftPlayerGraph = true;

    void OnEnable() => moveData = (MoveData) target;

    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        GUILayout.Space(15);
        
        if (GUILayout.Button("Show Graphs in Separate Window", GUILayout.Height(35))) MoveDataGraphWindow.ShowWindow();

        GUILayout.Space(15);
        
        if (moveData.showWarning)
        {
            var wrongDir   = moveData.wrongDir;
            var wrongForce = moveData.wrongForce;

            string warning = "The knockback direction and force are not aligned. " + "\n" + $"Direction: ({wrongDir.x}, {wrongDir.y})" + "\n" + $"Force: ({wrongForce.x}, {wrongForce.y})" + "\n" +
                             "You are either trying to apply a force to a direction that is zero, or the force is zero when the direction is not.";
            
            if (moveData.knockbackDir != Vector2.zero && moveData.knockbackForce != Vector2.zero) EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }

        GUILayout.Space(15);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        GUILayout.Space(15);

        GroundedKnockbackGraph();

        GUILayout.Space(5);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        GUILayout.Space(5);
        
        AerialKnockbackGraph();
    }
    
    void GroundedKnockbackGraph()
    {
        // Draw graph
        string label   = leftPlayerGraph ? "Right Player" : "Left Player";
        var    content = new GUIContent(label, $"The direction the {label} is knocked back when hit by the move.");
        leftPlayerGraph = GUILayout.Toggle(leftPlayerGraph, content, EditorStyles.toolbarButton);
        EditorGUILayout.LabelField("Knockback Direction and Force Graph");

        GUILayout.Space(15);

        Rect rect = GUILayoutUtility.GetRect(150, 150);
        Handles.DrawSolidRectangleWithOutline(rect, new (0.22f, 0.22f, 0.22f), new (0.22f, 0.22f, 0.22f));
        Vector2 center = rect.center;

        // Scale the knockback direction by the magnitude of the knockback force
        // Flip the X direction if leftPlayerGraph is true
        Vector2 knockbackDir = moveData.knockbackDir;
        knockbackDir.x = leftPlayerGraph ? -knockbackDir.x : knockbackDir.x;
        Vector2 end = center + new Vector2(knockbackDir.x * moveData.knockbackForce.x, -knockbackDir.y * moveData.knockbackForce.y) * 4.5f; // 4.5f is the scale factor to fit the graph in the rect
        Handles.DrawLine(center, end);
        Handles.CircleHandleCap(0, end, Quaternion.identity, 2, EventType.Repaint);
    }

    void AerialKnockbackGraph()
    {
        // Draw graph
        string label   = leftPlayerGraph ? "Right Player" : "Left Player";
        var    content = new GUIContent(label, $"The direction the {label} is knocked back when hit by the move in the air.");
        leftPlayerGraph = GUILayout.Toggle(leftPlayerGraph, content, EditorStyles.toolbarButton);
        EditorGUILayout.LabelField("Aerial Knockback Direction and Force Graph");

        GUILayout.Space(15);

        Rect rect = GUILayoutUtility.GetRect(150, 150);
        Handles.DrawSolidRectangleWithOutline(rect, new Color(0.22f, 0.22f, 0.22f), new Color(0.22f, 0.22f, 0.22f));
        Vector2 center = rect.center;

        // Scale the aerial knockback direction by the magnitude of the aerial knockback force
        // Flip the X direction if leftPlayerGraph is true
        Vector2 aerialKnockbackDir = moveData.aerialKnockbackDir;
        aerialKnockbackDir.x = leftPlayerGraph ? -aerialKnockbackDir.x : aerialKnockbackDir.x;
        Vector2 end = center + new Vector2(aerialKnockbackDir.x * moveData.aerialKnockbackForce.x, -aerialKnockbackDir.y * moveData.aerialKnockbackForce.y) * 4.5f; // 4.5f is the scale factor to fit the graph in the rect
        Handles.DrawLine(center, end);
        Handles.CircleHandleCap(0, end, Quaternion.identity, 2, EventType.Repaint);
    }
}

public class MoveDataGraphWindow : EditorWindow
{
    MoveData moveData;
    static bool leftPlayerGraph = true;

    [MenuItem("Window/Move Data Graphs")]
    public static void ShowWindow() => GetWindow<MoveDataGraphWindow>("Move Data Graphs");

    void OnEnable() => EditorApplication.update += Repaint;
    void OnDisable() => EditorApplication.update -= Repaint;

    void OnGUI()
    {
        string label = leftPlayerGraph ? "Right Player" : "Left Player";
        GUIContent content = new (label, $"The direction the {label} is knocked back when hit by the move.");
        
        leftPlayerGraph = GUILayout.Toggle(leftPlayerGraph, content, EditorStyles.toolbarButton);
        
        EditorGUI.BeginChangeCheck();
        moveData = (MoveData) EditorGUILayout.ObjectField("Move Data", moveData, typeof(MoveData), false);

        if (EditorGUI.EndChangeCheck())
        {
            EditorApplication.QueuePlayerLoopUpdate(); 
        }

        // Get the currently selected object in the inspector
        Object selectedObject = Selection.activeObject;

        // If the selected object is of type MoveData, set it as the moveData
        if (selectedObject is MoveData data) moveData = data;

        if (moveData != null)
        {
            DrawKnockbackGraph("Knockback Direction and Force Graph", moveData.knockbackDir, moveData.knockbackForce);
            DrawKnockbackGraph("Aerial Knockback Direction and Force Graph", moveData.aerialKnockbackDir, moveData.aerialKnockbackForce);
        }
    }

    void DrawKnockbackGraph(string title, Vector2 knockbackDir, Vector2 knockbackForce)
    {
        EditorGUILayout.LabelField(title);

        Rect rect = GUILayoutUtility.GetRect(150, 150);
        Handles.DrawSolidRectangleWithOutline(rect, new (0.22f, 0.22f, 0.22f), new (0.22f, 0.22f, 0.22f));
        Vector2 center = rect.center;

        knockbackDir.x = leftPlayerGraph ? -knockbackDir.x : knockbackDir.x;
        Vector2 end = center + new Vector2(knockbackDir.x * knockbackForce.x, -knockbackDir.y * knockbackForce.y) * 4.5f;
        Handles.DrawLine(center, end);
        Handles.CircleHandleCap(0, end, Quaternion.identity, 2, EventType.Repaint);
    }
}
#endif
