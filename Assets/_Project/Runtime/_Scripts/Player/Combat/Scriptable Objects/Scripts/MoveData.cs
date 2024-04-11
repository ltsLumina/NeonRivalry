using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    
    [Tooltip("Damage caused by the move.")]
    public int damage;

    [Tooltip("The amount of force applied to the enemy when hit by the move.")]
    public Vector2 knockbackForce;
    
    [Tooltip("The direction the enemy is knocked back when hit by the move.")]
    public Vector2 knockbackDir;

    public bool screenShake;
    public bool screenShakeAmplitude;

    // [Tooltip("Block advantage/disadvantage.")]
    // public float blockstun;

    [Space(15)]
    [Header("Move Properties")]

    [Tooltip("Whether the move can be performed while airborne.")]
    public bool isAirborne;

    [Tooltip("Whether the move hits low and causes knockdown.")]
    public bool isSweep;

    [Tooltip("Whether the move hits crouching opponents.")]
    public bool isOverhead;

    [Tooltip("Whether the move can be blocked.")]
    public bool isArmor;

    [Tooltip("Whether the player becomes invincible to attacks while performing the move.")]
    public bool isInvincible;

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
        damage = Mathf.Clamp(damage, 1, 100);
        
        #region Knockback
        knockbackDir.x = Mathf.Clamp(knockbackDir.x, -1, 1);
        knockbackDir.y = Mathf.Clamp(knockbackDir.y, -1, 1);
        
        knockbackForce.x = Mathf.Clamp(knockbackForce.x, -15, 15);
        knockbackForce.y = Mathf.Clamp(knockbackForce.y, -15, 15);

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
        
        if (moveData.showWarning)
        {
            var wrongDir   = moveData.wrongDir;
            var wrongForce = moveData.wrongForce;

            string warning = "The knockback direction and force are not aligned. " + "\n" + $"Direction: ({wrongDir.x}, {wrongDir.y})" + "\n" + $"Force: ({wrongForce.x}, {wrongForce.y})" + "\n" +
                             "You are either trying to apply a force to a direction that is zero, or the force is zero when the direction is not.";
            EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }

        GUILayout.Space(15);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        GUILayout.Space(15);

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

    
}
#endif
