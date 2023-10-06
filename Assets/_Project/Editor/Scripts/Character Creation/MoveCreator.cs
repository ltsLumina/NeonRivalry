#region
using Lumina.Debugging;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;
#endregion

public static class MoveCreator
{
    // -- Fields --
    
    static MoveData currentMove;
    static string moveName;

    public static bool showAttributes;
    public static bool showResources;
    public static bool showProperties;

    // -- Attributes --

    static MoveData.Type type;
    static MoveData.Direction direction;
    static MoveData.Guard guard;

    static string name;
    static string description;
    static float damage;
    static float startup;
    static float active;
    static float recovery;
    static float blockstun;

    // -- Resources --

    static AnimationClip animation;
    static AudioClip audioClip;
    static Sprite sprite;

    // -- Move Properties --
    static bool isAirborne;
    static bool isSweep;
    static bool isOverhead;
    static bool isArmor;
    static bool isInvincible;
    static bool isGuardBreak;

    // -- End --

    public static void CreatingMoveMenu()
    {
        DrawMenuHeader();
        currentMove = GetMoveToEdit();

        if (currentMove == null)
        {
            PromptCreateNewMove();
            return;
        }

        DisplayMoveEditor();
    }
    
    #region GUI
    static void DrawMenuHeader()
    {
        CharacterMovesetCreator.DrawBackButton();
        EditorGUILayout.LabelField("Creating Move", EditorStyles.boldLabel);
    }

    static MoveData GetMoveToEdit() => (MoveData) EditorGUILayout.ObjectField("Move to Edit", currentMove, typeof(MoveData), false);

    static void PromptCreateNewMove()
    {
        EditorGUILayout.HelpBox("Select a move or create a new one.", MessageType.Warning);
        Space(10);
        
        moveName = GetMoveName();
        string assetName = GenerateAssetName(moveName);

        if (Button($"Create {moveName}")) SwitchToMoveCreatorMenu(assetName);
    }

    static string GetMoveName() => EditorGUILayout.TextField("Move Name", moveName);

    static string GenerateAssetName(string moveName)
    {
        const string defaultName = "New Move";
        return string.IsNullOrEmpty(moveName) ? defaultName : moveName;
    }

    static void SwitchToMoveCreatorMenu(string assetName)
    {
        if (string.IsNullOrEmpty(assetName) || assetName == "New Move")
        {
            const string warning = "Warning";
            string       message = CharacterMovesetCreator.WarningMessage(assetName, false);

            if (EditorUtility.DisplayDialog(warning, message, "Proceed", "Cancel")) CharacterMovesetCreator.activeMenu = DrawCreatingMoveMenu;
        }
        else { CharacterMovesetCreator.activeMenu = DrawCreatingMoveMenu; }
    }

    static void DisplayMoveEditor()
    {
        Space(10);
        var inspector = Editor.CreateEditor(currentMove);
        inspector.OnInspectorGUI();
    }

    static void DrawCreatingMoveMenu()
    {
        CharacterMovesetCreator.DrawBackButton();

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

    static void DrawTypesGUI()
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
    static void DrawResourcesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showResources ? "Resources (click to hide)" : "Resources (click to show)";

            showResources = EditorGUILayout.Foldout(showResources, label, true, EditorStyles.boldLabel);

            if (showResources)
            {
                // Object field
                animation = (AnimationClip) EditorGUILayout.ObjectField(animationContent, animation, typeof(AnimationClip), false);

                // Object field
                audioClip = (AudioClip) EditorGUILayout.ObjectField(audioClipContent, audioClip, typeof(AudioClip), false);

                using (new HorizontalScope())
                {
                    // Object field
                    sprite = (Sprite) EditorGUILayout.ObjectField(spriteContent, sprite, typeof(Sprite), false);

                    FlexibleSpace();
                    FlexibleSpace();
                }
            }
        }
    }
    
    static void DrawAttributesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showAttributes ? "Attributes (click to hide)" : "Attributes (click to show)";

            showAttributes = EditorGUILayout.Foldout(showAttributes, label, true, EditorStyles.boldLabel);

            if (showAttributes)
            {
                // Initialize the name of the move with the name of the ScriptableObject
                name = moveName;
                if (string.IsNullOrEmpty(moveName)) name = "New Move";

                name        = EditorGUILayout.TextField(nameContent, name);
                description = EditorGUILayout.TextField(descriptionContent, description);
                damage      = EditorGUILayout.FloatField(damageContent, damage);
                startup     = EditorGUILayout.FloatField(startupContent, startup);
                active      = EditorGUILayout.FloatField(activeContent, active);
                recovery    = EditorGUILayout.FloatField(recoveryContent, recovery);
                blockstun   = EditorGUILayout.FloatField(blockstunContent, blockstun);
            }
        }
    }
    
    static void DrawPropertiesGUI()
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
    #endregion

    #region Utility
    public static void ResetMoveCreator()
    {
        currentMove = null;
        moveName    = string.Empty;

        // Reset all fields
        type      = MoveData.Type.Punch;
        direction = MoveData.Direction.Neutral;
        guard     = MoveData.Guard.High;

        name        = string.Empty;
        description = string.Empty;
        damage      = 0;
        startup     = 0;
        active      = 0;
        recovery    = 0;
        blockstun   = 0;

        animation = null;
        audioClip = null;
        sprite    = null;

        isAirborne   = false;
        isSweep      = false;
        isOverhead   = false;
        isArmor      = false;
        isInvincible = false;
        isGuardBreak = false;
    }

    static void CreateMove()
    {
        if (Button(new GUIContent("Create Move", "Creates the move. \nThe name of the move will be the name of the ScriptableObject.")))
        {
            FlexibleSpace();

            currentMove = ScriptableObject.CreateInstance<MoveData>();

            const string path        = "Assets/_Project/Runtime/_Scripts/Player/Combat/Scriptable Objects/Moves";
            const string defaultName = "New Move";
            string       assetName   = string.IsNullOrEmpty(name) ? defaultName : name;

            try { AssetDatabase.CreateAsset(currentMove, $"{path}/{assetName}.asset"); } catch (UnityException e)
            {
                Debug.LogError($"{FGDebugger.errorMessagePrefix} Failed to create asset. The path in the script is probably invalid.\n{e.Message}");
                throw;
            }
            finally
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // Assign the currentMove's values
            currentMove.type      = type;
            currentMove.direction = direction;
            currentMove.guard     = guard;

            currentMove.name        = name;
            currentMove.description = description;
            currentMove.damage      = damage;
            currentMove.startup     = startup;
            currentMove.active      = active;
            currentMove.recovery    = recovery;
            currentMove.blockstun   = blockstun;

            currentMove.animation = animation;
            currentMove.audioClip = audioClip;
            currentMove.sprite    = sprite;

            currentMove.isAirborne   = isAirborne;
            currentMove.isSweep      = isSweep;
            currentMove.isOverhead   = isOverhead;
            currentMove.isArmor      = isArmor;
            currentMove.isInvincible = isInvincible;
            currentMove.isGuardBreak = isGuardBreak;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            CharacterMovesetCreator.activeMenu = CharacterMovesetCreator.DefaultMenu;

            Debug.Log($"Created currentMove \"{currentMove.name}\".");
            Selection.activeObject = currentMove;
            EditorGUIUtility.PingObject(currentMove);

            currentMove = null;
            CharacterMovesetCreator.createdSuccessfully = true;
        }
    }
    #endregion
}
