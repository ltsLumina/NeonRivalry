#region
using UnityEditor;
using UnityEngine;
#endregion

public class EditorGUIUtils : Editor
{
    public readonly static GUIContent createdSuccessfullyContent = new ("Created Successfully.");

    public readonly static GUIContent typeContent = new ("Type", "The type of the move. It can be a punch, kick, slash, or unique move.");
    public readonly static GUIContent directionContent = new ("Direction", "The direction of the move. It can be neutral, up, down, or forward.");
    public readonly static GUIContent guardContent = new ("Guard", "The guarding condition of the move. It can be guarded while standing, crouching, or both.");

    public readonly static GUIContent nameContent = new ("Name", "The name of the move. This will be the name of the ScriptableObject as well.");
    public readonly static GUIContent damageContent = new ("Damage", "The damage value of the move.");
    public readonly static GUIContent startupContent = new ("Startup", "The startup frames of the move.");
    public readonly static GUIContent activeContent = new ("Active", "The active frames of the move.");
    public readonly static GUIContent recoveryContent = new ("Recovery", "The recovery frames of the move.");
    public readonly static GUIContent blockstunContent = new ("Blockstun", "The blockstun frames of the move.");

    public readonly static GUIContent animationContent = new ("Animation", "The animation to play during the move.");
    public readonly static GUIContent audioClipContent = new ("Audio Clip", "The audio clip to play during the move.");
    public readonly static GUIContent spriteContent = new ("Sprite", "The sprite to display during the move.");

    public readonly static GUIContent isAirborneContent = new ("Airborne", "If checked, this move can be performed while airborne.");
    public readonly static GUIContent isSweepContent = new ("Sweep", "If checked, this move sweeps the opponent off their feet.");
    public readonly static GUIContent isOverheadContent = new ("Overhead", "If checked, this move will hit crouching opponents.");
    public readonly static GUIContent isArmorContent = new ("Armor", "If checked, this move causes the player to have super armor during the move.");
    public readonly static GUIContent isInvincibleContent = new ("Invincible", "If checked, this move can interrupt an opponent's move.");
    public readonly static GUIContent isGuardBreakContent = new ("Guard Break", "If checked, this move breaks the opponent's guard.");
    
    public readonly static GUIContent existingMovesContent = new ("Existing Moves", "Due to a limitation in Unity, you must manually add moves to the moveset through the use of these buttons.");
}
