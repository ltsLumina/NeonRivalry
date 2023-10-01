using System;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;

//TODO: Make a partial class for actual Moveset creator window.
public class MovesetCreator : EditorWindow
{
    // -- Menus --

    readonly static Vector2 winSize = new (475, 565);
    
    Action activeMenu;
    bool showAttributes;
    bool showResources;
    bool showProperties;
    bool createdSuccessfully;

    // -- Attributes --
    
    MoveData.Type type;
    MoveData.Direction direction;
    MoveData.Guard guard;
    
    new string name;
    float damage;
    float startup;
    float active;
    float recovery;
    float blockstun;
    
    // -- Resources --
    
    AnimationClip animation;
    AudioClip audioClip;
    Sprite sprite;
    
    // -- Move Properties --
    bool isAirborne;
    bool isSweep;
    bool isOverhead;
    bool isArmor;
    bool isInvincible;
    bool isGuardBreak;

    // -- End --
    
    [MenuItem("Tools/Attacking/Moveset Creator")]
    static void ShowWindow()
    {
        var window = GetWindow<MovesetCreator>();
        window.titleContent = new ("Moveset Creator");
        window.minSize      = new (winSize.x, winSize.y);
        window.maxSize      = window.minSize;
        window.Show();
    }

    void OnEnable()
    {
        createdSuccessfully = false;
        
        activeMenu = DefaultMenu;
    }

    void OnDisable() => activeMenu = null;

    void OnGUI()
    {
        activeMenu();
    }
    
    void DefaultMenu()
    {
        DrawHeaderGUI();
        
        Space(10);
        
        DrawCreationTextGUI();
        
        DrawInstructionsGUI();
    }

    #region Utility
    void DrawBackButton()
    {
        using (new HorizontalScope())
        {
            FlexibleSpace();

            if (Button("Back"))
            {
                createdSuccessfully = false;
                activeMenu = DefaultMenu; 
            }
        }
    }

    void DrawHeaderGUI()
    {
        using (new HorizontalScope("box"))
        {
            Label("Create Moves / Movesets", EditorStyles.boldLabel);

            if (Button("Create Moveset")) activeMenu = CreatingMovesetMenu;
            if (Button("Create Move")) activeMenu    = CreatingMoveMenu;
        }
    }

    void CreatingMovesetMenu()
    {
        Label("Creating Moveset", EditorStyles.boldLabel); 
    }

    void CreatingMoveMenu()
    {
        DrawBackButton();

        Label("Creating Move", EditorStyles.boldLabel);

        DrawTypesGUI();

        Space(10);

        DrawResourcesGUI();

        Space(10);

        DrawAttributesGUI();

        Space(10);

        DrawPropertiesGUI();

        // Button to create the move
        CreateMove();
    }

    void DrawCreationTextGUI()
    {
        using (new HorizontalScope("box"))
        {
            FlexibleSpace();

            if (createdSuccessfully) { Label(createdSuccessfullyContent, EditorStyles.boldLabel); }

            FlexibleSpace();
        }
    }

    static void DrawInstructionsGUI()
    {
        using (new VerticalScope("box"))
        {
            Label("Instructions", EditorStyles.boldLabel);

            Label("1. Click \"Create Moveset\" or \"Create Move\".");
            Label("2. Fill in the fields.");
            Label("3. Click \"Create Moveset\" or \"Create Move\" again.");
            Label("4. Done! The ScriptableObject will be created.");
        }
    }
    
    void DrawTypesGUI()
    {
        using (new HorizontalScope("box"))
        {
            Label(typeContent, EditorStyles.boldLabel);

            // Dropdown (enum)
            type = (MoveData.Type) EditorGUILayout.EnumPopup(type);

            Label(directionContent, EditorStyles.boldLabel);

            // Dropdown (enum)
            direction = (MoveData.Direction) EditorGUILayout.EnumPopup(direction);

            Label(guardContent, EditorStyles.boldLabel);

            // Dropdown (enum)
            guard = (MoveData.Guard) EditorGUILayout.EnumPopup(guard);
        }
    }
    void DrawResourcesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showResources ? "Resources (click to hide)" : "Resources (click to show)";

            showResources = EditorGUILayout.Foldout(showResources, label, true, EditorStyles.boldLabel);

            if (showResources)
            {
                // Object field
                animation = (AnimationClip) EditorGUILayout.ObjectField("Animation", animation, typeof(AnimationClip), false);

                // Object field
                audioClip = (AudioClip) EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false);

                using (new HorizontalScope())
                {
                    // Object field
                    sprite = (Sprite) EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), false);

                    FlexibleSpace();
                    FlexibleSpace();
                }
            }
        }
    }
    void DrawAttributesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showAttributes ? "Attributes (click to hide)" : "Attributes (click to show)";

            showAttributes = EditorGUILayout.Foldout(showAttributes, label, true, EditorStyles.boldLabel);

            if (showAttributes)
            {
                name      = EditorGUILayout.TextField(nameContent, name);
                damage    = EditorGUILayout.FloatField(damageContent, damage);
                startup   = EditorGUILayout.FloatField(startupContent, startup);
                active    = EditorGUILayout.FloatField(activeContent, active);
                recovery  = EditorGUILayout.FloatField(recoveryContent, recovery);
                blockstun = EditorGUILayout.FloatField(blockstunContent, blockstun);
            }
        }
    }
    void DrawPropertiesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showProperties ? "Properties (click to hide)" : "Properties (click to show)";

            showProperties = EditorGUILayout.Foldout(showProperties, label, true, EditorStyles.boldLabel);

            if (showProperties)
            {
                isAirborne   = EditorGUILayout.Toggle(isAirborneContent, isAirborne);
                isSweep      = EditorGUILayout.Toggle(isSweepContent, isSweep);
                isOverhead   = EditorGUILayout.Toggle(isOverheadContent, isOverhead);
                isArmor      = EditorGUILayout.Toggle(isArmorContent, isArmor);
                isInvincible = EditorGUILayout.Toggle(isInvincibleContent, isInvincible);
                isGuardBreak = EditorGUILayout.Toggle(isGuardBreakContent, isGuardBreak);
            }
        }
    }

    void CreateMove()
    {
        if (Button(new GUIContent("Create Move", "Creates the move. \nThe name of the move will be the name of the ScriptableObject.")))
        {
            FlexibleSpace();

            var move = ScriptableObject.CreateInstance<MoveData>();

            const string path        = "Assets/_Project/Runtime/Scripts/Player/Attacking/Scriptable Objects/Moves";
            const string defaultName = "New Unnamed Move";
            string       assetName   = string.IsNullOrEmpty(name) ? defaultName : name;

            AssetDatabase.CreateAsset(move, $"{path}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign the move's values
            move.type      = type;
            move.direction = direction;
            move.guard     = guard;

            move.name      = name;
            move.damage    = damage;
            move.startup   = startup;
            move.active    = active;
            move.recovery  = recovery;
            move.blockstun = blockstun;

            move.animation = animation;
            move.audioClip = audioClip;
            move.sprite    = sprite;

            move.isAirborne   = isAirborne;
            move.isSweep      = isSweep;
            move.isOverhead   = isOverhead;
            move.isArmor      = isArmor;
            move.isInvincible = isInvincible;
            move.isGuardBreak = isGuardBreak;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            activeMenu = DefaultMenu;

            Debug.Log($"Created move {move.name}.");
            Selection.activeObject = move;
            EditorGUIUtility.PingObject(move);

            createdSuccessfully = true;
        }
    }
    #endregion
}
