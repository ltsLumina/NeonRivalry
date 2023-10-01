using System;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;

//TODO: Make a partial class for actual Moveset creator window.
public partial class MoveCreator : EditorWindow
{
    // -- Menus --

    readonly static Vector2 winSize = new (475, 615);
    
    Action activeMenu;
    MoveData currentMove;
    string moveName;
    
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
        var window = GetWindow<MoveCreator>();
        window.titleContent = new ("Moveset Creator");
        window.minSize      = new (winSize.x, winSize.y);
        window.maxSize      = window.minSize;
        window.Show();
    }

    void OnEnable()
    {
        createdSuccessfully = false;

        // Initialize the dictionary with types
        InitializeExistingMoves();

        LoadExistingMoves();
        
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

    #region GUI
    
    void DrawHeaderGUI()
    {
        using (new HorizontalScope("box"))
        {
            Label("Create Moves / Movesets", EditorStyles.boldLabel);

            if (Button("Manage Movesets"))
            {
                createdSuccessfully = false;
                activeMenu = CreatingMovesetMenu;
            }

            if (Button("Manage Moves"))
            {
                createdSuccessfully = false;
                
                showAttributes = true;
                showResources  = true;
                showProperties = true;
                
                activeMenu = CreatingMoveMenu;
            }
        }
    }

    void CreatingMoveMenu()
    {
        DrawBackButton();

        EditorGUILayout.LabelField("Creating Move", EditorStyles.boldLabel);

        currentMove = (MoveData) EditorGUILayout.ObjectField("Move to Edit", currentMove, typeof(MoveData), false);

        if (currentMove == null)
        {
            EditorGUILayout.HelpBox("Select a move or create a new one.", MessageType.Warning);

            // Set the moveset name
            moveName = EditorGUILayout.TextField("Move Name", moveName);

            // If the asset name is empty, a new move called "New Move" will be created.
            const string defaultName = "New Move";
            string       assetName   = string.IsNullOrEmpty(moveName) ? defaultName : moveName;

            if (Button($"Create {assetName}"))
            {
                // If the asset name is empty, a new move called "New Move" will be created.
                if (string.IsNullOrEmpty(assetName) || assetName == "New Move")
                {
                    // If there already exists a move called "New Move", then the old one will be overwritten.
                    const string warning = "Warning";
                    string       message = WarningMessage(assetName, false);

                    if (EditorUtility.DisplayDialog(warning, message, "Proceed", "Cancel")) { activeMenu = DrawCreatingMoveMenu; }
                }
                else { activeMenu = DrawCreatingMoveMenu; }
            }

            return;
        }
        
        Space(10);
        
        // Show the inspector for the moveset
        Editor inspector = Editor.CreateEditor(currentMove);
        inspector.OnInspectorGUI();
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

    void DrawCreatingMoveMenu()
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
                // Initialize the name of the move with the name of the ScriptableObject
                name = moveName;
                if (string.IsNullOrEmpty(moveName)) name = "New Move";
                
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
    
    #endregion

    #region Utility
    void InitializeExistingMoves()
    {
        existingMoves["Punch"]  = new ();
        existingMoves["Kick"]   = new ();
        existingMoves["Slash"]  = new ();
        existingMoves["Unique"] = new ();
    }
    
    void DrawBackButton()
    {
        using (new HorizontalScope())
        {
            FlexibleSpace();

            if (Button("Back"))
            {
                // -- Move Creator --
                ResetMoveCreator();

                // -- Moveset Creator --
                ResetMovesetCreator();

                // -- End --
                activeMenu = DefaultMenu; 
            }
        }
    }
    
    void ResetMoveCreator()
    { 
        currentMove = null;
        moveName    = string.Empty;
        
        // Reset all fields
        type      = MoveData.Type.Punch;
        direction = MoveData.Direction.Neutral;
        guard     = MoveData.Guard.High;

        name      = string.Empty;
        damage    = 0;
        startup   = 0;
        active    = 0;
        recovery  = 0;
        blockstun = 0;

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

    void CreateMove()
    {
        if (Button(new GUIContent("Create Move", "Creates the move. \nThe name of the move will be the name of the ScriptableObject.")))
        {
            FlexibleSpace();

            currentMove = ScriptableObject.CreateInstance<MoveData>();

            const string path        = "Assets/_Project/Runtime/Scripts/Player/Attacking/Scriptable Objects/Moves";
            const string defaultName = "New Move";
            string       assetName   = string.IsNullOrEmpty(name) ? defaultName : name;

            AssetDatabase.CreateAsset(currentMove, $"{path}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign the currentMove's values
            currentMove.type      = type;
            currentMove.direction = direction;
            currentMove.guard     = guard;
            
            currentMove.name      = name;
            currentMove.damage    = damage;
            currentMove.startup   = startup;
            currentMove.active    = active;
            currentMove.recovery  = recovery;
            currentMove.blockstun = blockstun;

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

            activeMenu = DefaultMenu;

            Debug.Log($"Created currentMove \"{currentMove.name}\".");
            Selection.activeObject = currentMove;
            EditorGUIUtility.PingObject(currentMove);

            createdSuccessfully = true;
        }
    }
    #endregion
}
