#region
using System.IO;
using System.Linq;
using System.Threading;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.EditorGUIUtils;
#endregion

namespace Lumina.Essentials.Editor.UI
{
// One of the three tabs in the Utility Window.
// Includes all the functions for the Utilities tab.
internal sealed partial class UtilityPanel
{
    #region Utilities variables

    /// <summary> The position of the scroll view. </summary>
    Vector2 utilities_scrollPos;
    
    enum DragAndDropType // Only the ConvertImagesUtility enum is being used. The rest are deprecated.
    {
        CreateProjectUtility,
        ConvertImagesUtility,
    }

    // Deprecated. Kept here for reference.
    #region Deprecated
#pragma warning disable CS0414 // Field is assigned but its value is never used
    readonly DragAndDropType createProjectEnum = DragAndDropType.CreateProjectUtility;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    #endregion

    const DragAndDropType convertImagesEnum = DragAndDropType.ConvertImagesUtility;

    internal const string ProjectPath = "";

    float dropAreaHeight;
    Rect dropArea;

    string folderSelectedMsg = $"The folder: \"{DirectoryUtilities.GetFolderNameFromString(ProjectPath)}\" will be used as root project.";
    static string noFolderSelectedMsg = "No folder selected. \nPlease drag and drop a folder to use.";

    /// <summary> Opens the configure images options. </summary>
    bool configuringImages;
    
    #region Image Settings
    TextureImporter importer;
    static SpriteImportMode spriteImportMode = SpriteImportMode.Single;
    static float pixelsPerUnit;
    static FilterMode filterMode = FilterMode.Point;
    static TextureImporterCompression compression = TextureImporterCompression.Uncompressed;
    #endregion
    
    /// <summary> Quick toggle to set the sprite import mode to the recommended settings for importing sprites. </summary>
    bool isSpriteSheet;
    bool dragAndDropFolder;
    bool isCorrectDirectory;
    /// <summary> Shows the path of the selected object. </summary>
    string imageConverterPath = "";

    // End of utilities variables //
    #endregion

    /// <summary>
    ///     Draws the header of the utilities panel.
    /// </summary>
    static void DrawUtilitiesHeader()
    {
        const float spacer = 6.5f; // Has to be 6.5 to ensure that the text is at the same height as the Settings panel.
        GUILayout.Space(spacer);
        GUILayout.Label("Utilities", centerLabelStyle);
        GUILayout.Label("Provides useful features to improve your workflow.", subLabelStyle);

        // Draw a horizontal line (separator)
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    void DrawUtilitiesButtonsGUI()
    {
        DrawUtilityOptions();

        // Horizontal line (separator)
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Space(10);

        DrawCreateDefaultProjectButton();

        GUILayout.Space(5);

        GUI.backgroundColor = configuringImages ? new (0.76f, 0.76f, 0.76f) : Color.white;

        DrawConfigureImagesButton();

        GUI.backgroundColor = Color.white;

        GUILayout.Space(5);

        if (advancedSettings) DrawPlaceholderButton();
    }

    static void DrawPlaceholderButton()
    {
        if (GUILayout.Button("Placeholder Button", GUILayout.Height(35)))
        {
            // Meme button :)
            float progress = 0;

            while (progress < 1)
            {
                progress += Random.Range(-0.1f, 0.3f); // randomly increment progress

                if (progress > 1) // clamp the progress to maximum of 1
                    progress = 1;

                // if the user presses cancel while the progress bar is updating, stop the process
                if (EditorUtility.DisplayCancelableProgressBar("Processing...", $"Loading... {progress * 100:F0} / 100", progress))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }

                Thread.Sleep(Random.Range(250, 4000)); // randomly wait for some time before updating
            }

            Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            EditorUtility.ClearProgressBar();
        }
    }

    static void DrawUtilityOptions()
    { // Checkbox to enable or disable the auto save feature
        AutoSaveConfig.Enabled = EditorGUILayout.Toggle(autoSaveEnabledContent, AutoSaveConfig.Enabled);

        if (AutoSaveConfig.Enabled)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("└  Interval (Min)", GUILayout.Width(200));
                AutoSaveConfig.Interval = EditorGUILayout.IntSlider(AutoSaveConfig.Interval, 1, 60);
            }

            GUILayout.Space(1.5f);

            AutoSaveConfig.Logging = EditorGUILayout.Toggle(autoSaveLoggingContent, AutoSaveConfig.Logging);

            GUILayout.Space(5.5f);
        }

        // Checkbox to enable or disable the enter playmode options
        EditorSettings.enterPlayModeOptionsEnabled = EditorGUILayout.Toggle(enterPlaymodeOptionsContent, EditorSettings.enterPlayModeOptionsEnabled);
    }

    void DrawCreateDefaultProjectButton()
    { // Button that creates a default project directory structure
        if (GUILayout.Button(createDefaultProjectContent, GUILayout.Height(35)))
        {
            if (!SafeMode)
            {
                configuringImages = false;

                if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to create the default project structure?", "Yes", "No"))
                {
                    CreateProjectStructure();
                }
                else
                {
                    SafeMode = true;
                    EssentialsDebugger.LogAbort();
                }
            }
            else
            {
                EditorApplication.Beep();

                if (EditorUtility.DisplayDialog
                ("Warning!",
                 "Safe Mode has not been disabled and is required to create the default project structure. \nThis is done in order to prevent you from creating folders unintentionally.",
                 "Disable Safe Mode", "Cancel"))
                {
                    SafeMode = false;
                    EssentialsDebugger.LogWarning("Safe Mode has been disabled. Please try again.");
                }
                else { EssentialsDebugger.LogAbort(); }
            }
        }
    }

    void DrawConfigureImagesButton()
    {
        if (GUILayout.Button(configureImagesContent, GUILayout.Height(35)))
        {
            configuringImages = !configuringImages;

            // ResetUtilityPanel the image converter path if the user stops configuring the configure images settings.
            if (!configuringImages)
            {
                imageConverterPath = "";
                isCorrectDirectory = false;
            }
            else { DrawConfigureImagesGUI(); }
        }
    }

    static void CreateProjectStructure()
    {
        const string rootName = "_Project";

        // Create the default folders in the root of the project 
            DirectoryUtilities.CreateDirectories(rootName, "Scripts", "Art", "Audio", "Scenes", "PREFABS", "Materials", "Plugins"); // "DEL" to put it at the bottom.
            DirectoryUtilities.CreateDirectories(rootName + "/Art", "Animations");
            DirectoryUtilities.CreateDirectories(rootName + "/Audio", "SFX", "Music");
            AssetDatabase.Refresh();

            #region Looks like this in the project window:
            // _Project
            // └ Scripts
            // └ Art
            // |  └  Animations
            // └ Audio
            // |  └  SFX
            // |  └  Music
            // └ Scenes
            // └ PREFABS
            // └ Materials
            // └ Plugins
            #endregion
    }

    void DrawConfigureImagesGUI()
    {
        if (configuringImages)
        {
            DrawImageSettingsConfig();

            GUILayout.Space(5);

            DrawDragAndDropConfig(convertImagesEnum); // The convertImagesEnum is used to determine what the drag and drop GUI will display.
        }
        else
        {
            // ResetUtilityPanel the enums to the default values
            spriteImportMode = SpriteImportMode.Single;
            pixelsPerUnit    = 100;
            filterMode       = FilterMode.Bilinear;
            compression      = TextureImporterCompression.Compressed;
        }

        GUILayout.Space(10);

        // Draw a horizontal line (separator)
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    void DrawImageSettingsConfig()
    { // Display the image configuration options
        GUILayout.Label("Image Configuration", centerLabelStyle);
        GUILayout.Label("Configure the default settings for images.", subLabelStyle);
        GUILayout.Space(10);

        // Enum popups for the image configuration
        spriteImportMode = (SpriteImportMode) EditorGUILayout.EnumPopup("Sprite Mode", spriteImportMode);
        pixelsPerUnit    = EditorGUILayout.FloatField("Pixels Per Unit", pixelsPerUnit);
        filterMode       = (FilterMode) EditorGUILayout.EnumPopup("Filter Mode", filterMode);
        compression      = (TextureImporterCompression) EditorGUILayout.EnumPopup("Compression", compression);

        // Quick toggle to set the recommended settings for sprites (multiple)
        var spriteModeContent = new GUIContent
        ("Sprite-Sheet",
         "Sets the recommended settings for sprite-sheets \n" + "This will set the sprite mode to multiple, filter mode to point, and compression to uncompressed.");

        isSpriteSheet = EditorGUILayout.Toggle(spriteModeContent, isSpriteSheet);

        if (isSpriteSheet)
        {
            spriteImportMode = SpriteImportMode.Multiple;
            filterMode       = FilterMode.Point;
            compression      = TextureImporterCompression.Uncompressed;
        }
    }

    void DrawDragAndDropConfig(DragAndDropType type)
    {
        switch (type)
        { // This case has since been deprecated as I figured the way I was designing the project structure was not the best way to do it.
            // It remains here in case I decide to use it again, or if I want to repurpose it for something else.
            // I don't like leaving large amounts of code commented, but I want it for reference.
            case DragAndDropType.CreateProjectUtility:
                EssentialsDebugger.LogError("You are using the deprecated Create Project Structure enum!");
                #region Deprecated Create Project Structure Code
                // GUILayout.Label("Drag a folder here:", middleStyle);
                // GUILayout.Label("The selected folder will be used as the root folder.", subLabelStyle);
                // GUILayout.Label("\"/Assets/\" is the default folder if nothing is selected.", subLabelStyle);
                // GUILayout.Space(10);
                //
                // dropAreaHeight = ProjectPath.Length > 60 ? 45 : 30;
                //
                // dropArea = GUILayoutUtility.GetRect(0, dropAreaHeight, GUILayout.ExpandWidth(true));
                // GUI.Box(dropArea, ProjectPath, dropAreaStyle);
                //
                // if (Event.current.type == EventType.DragUpdated && dropArea.Contains(Event.current.mousePosition))
                // {
                //     DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                //     Event.current.Use();
                // }
                //
                // if (Event.current.type == EventType.DragPerform && dropArea.Contains(Event.current.mousePosition))
                // {
                //     DragAndDrop.AcceptDrag();
                //
                //     foreach (string path in DragAndDrop.paths)
                //     {
                //         // check if the path is directory (folder)
                //         if (Directory.Exists(path))
                //         {
                //             ProjectPath = path;
                //             Event.current.Use();
                //             break;
                //         }
                //     }
                // }
                //
                // GUILayout.Space(8);
                //
                // folderSelectedMsg   = $"The folder: \"{GetFolderNameFromString(ProjectPath)}\" will be used as root project.";
                // noFolderSelectedMsg = "No folder selected. \nPlease drag and drop a folder to use.";
                //
                // if (!string.IsNullOrEmpty(ProjectPath))
                // {
                //     GUILayout.Label(folderSelectedMsg, middleStyle);
                //     GUILayout.Space(4);
                //
                //     var correctDirectoryContent = new GUIContent("Is this directory correct?", "This will disable safe mode. \nPlease proceed with caution.");
                //     isCorrectDirectory = EditorGUILayout.Toggle(correctDirectoryContent, isCorrectDirectory);
                //
                //     if (isCorrectDirectory)
                //     {
                //         ProjectStructureWindow.ShowWindow();
                //     }
                //
                //     GUILayout.Space(5);
                //
                //     if (GUILayout.Button("Apply"))
                //     {
                //         if (isCorrectDirectory) ProjectStructureGUI.ApplyChanges();
                //         else EssentialsDebugger.LogWarning("The action was aborted. \nYou haven't checked the confirmation box!");
                //     }
                // }
                // else
                // {
                //     GUILayout.Label(noFolderSelectedMsg, middleStyle);
                // }
                #endregion
                break;

            case DragAndDropType.ConvertImagesUtility: {
                GUILayout.Label("Drag a folder here:", middleStyle);
                GUILayout.Label("The selected folder will be used to convert the images.", subLabelStyle);
                GUILayout.Space(10);

                dropAreaHeight = imageConverterPath.Length > 60 ? 45 : 30;

                dropArea = GUILayoutUtility.GetRect(0, dropAreaHeight, GUILayout.ExpandWidth(true));
                GUI.Box(dropArea, imageConverterPath, dropAreaStyle);

                if (Event.current.type == EventType.DragUpdated && dropArea.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }

                if (Event.current.type == EventType.DragPerform && dropArea.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.AcceptDrag();

                    foreach (string path in DragAndDrop.paths)
                    {
                        // check if the path is directory (folder)
                        if (Directory.Exists(path))
                        {
                            imageConverterPath = path;
                            Event.current.Use();
                            break;
                        }
                    }
                }

                GUILayout.Space(8);

                folderSelectedMsg   = $"The folder: \"{DirectoryUtilities.GetFolderNameFromString(imageConverterPath)}\" will be used to convert all images.";
                noFolderSelectedMsg = "No folder selected.\nPlease drag and drop a folder to use.";

                if (!string.IsNullOrEmpty(imageConverterPath))
                {
                    GUILayout.Label(folderSelectedMsg, middleStyle);
                    GUILayout.Label("Please put any images you wish to convert in said folder.", subLabelStyle);
                    GUILayout.Space(4);

                    var correctDirectoryContent = new GUIContent("Is this directory correct?", "This will disable safe mode. \nPlease proceed with caution.");
                    isCorrectDirectory = EditorGUILayout.Toggle(correctDirectoryContent, isCorrectDirectory);

                    // Unnecessary, but I like it.
                    if (isCorrectDirectory) SafeMode = false;

                    GUILayout.Space(5);

                    if (isCorrectDirectory && !SafeMode)
                        if (GUILayout.Button("Apply Settings", GUILayout.Height(25)))
                        {
                            if (isCorrectDirectory)
                            {
                                ConfigureImages();
                            }
                            else EssentialsDebugger.LogWarning("You haven't checked the confirmation box!");
                        }
                }
                else { GUILayout.Label(noFolderSelectedMsg, middleStyle); }

                break;
            }
        }
    }

    void ConfigureImages()
    {
        const string imageMsg  = "Are you sure you want to configure the images?";
        const string spriteMsg = "Are you sure you want to configure the sprites?";

        string confirmationMessage = isSpriteSheet
            ? $"{imageMsg}\nThis will set the sprite mode to multiple, filter mode to point, and compression to uncompressed."
            : $"{spriteMsg}\nThis will set the filter mode to point and compression to uncompressed." +
              " Keep in mind that the 'multiple' sprite mode is used for sprite-sheets and is not recommended for single sprites.";

        if (!SafeMode)
        {
            if (EditorUtility.DisplayDialog("Confirmation", confirmationMessage, "Yes", "No"))
            {
                // Get all files in /Lumina Essentials/Editor/Utilities/Image Converter/
                string folderPath = imageConverterPath;

                // Get the search patterns for the files (png, jpg, jpeg)
                string[] searchPatterns = { "*.png", "*.jpg", "*.jpeg" };

                string[] images = searchPatterns.SelectMany(pattern => Directory.GetFiles(folderPath, pattern)).ToArray();

                // Loop through each file
                foreach (string image in images)
                {
                    // Get the asset path and texture importer
                    string assetPath       = Path.Combine(folderPath, Path.GetFileName(image)).Replace('\\', '/');
                    var    textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                    if (textureImporter == null) continue;

                    // Set the filter mode and compression to point and no compression at the path
                    textureImporter.spriteImportMode    = spriteImportMode;
                    textureImporter.spritePixelsPerUnit = pixelsPerUnit;
                    textureImporter.filterMode          = filterMode;
                    textureImporter.textureCompression  = compression;
                    textureImporter.SaveAndReimport();

                    foreach (string configuredImage in images) { EssentialsDebugger.Log($"Configured {Path.GetFileName(configuredImage)}"); }
                }

                SafeMode = true;
                AssetDatabase.Refresh();
            }
            else { EssentialsDebugger.LogAbort(SafeMode); }
        }
        else { EssentialsDebugger.LogAbort(SafeMode); }
    }
}
}
