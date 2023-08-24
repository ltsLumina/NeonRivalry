#region
using System;
using System.Linq;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.EditorGUIUtils;
using static Lumina.Essentials.Editor.UI.Management.VersionManager;
#endregion

namespace Lumina.Essentials.Editor.UI
{
/// <summary>
///     The Utility Window that provides various features to enhance the user's workflow.
///     Includes the Setup Window, Settings Window, and Utilities Window.
///     This class is split into three partials, one for each panel.
/// </summary>
internal sealed partial class UtilityPanel : EditorWindow
{
    readonly static Vector2 winSize = new (370, 630);
    readonly static float buttonSize = winSize.x * 0.5f - 6;

    #region Panels
    /// <summary> The panel that will be displayed. </summary>
    int selectedTab;

    /// <summary> The labels of the tabs. </summary>
    readonly string[] tabLabels = { "Setup", "Settings", "Utilities" };

    /// <summary> Used to invoke the setup panel when necessary. (Not the main panel) </summary>
    Action currentPanel;

    /// <summary> Used to determine the tooltip message. </summary>
    static bool showTooltip;
    /// <summary> Used to determine if the user has checked for updates. Used for the tooltip. </summary>
    static bool checkedForUpdates;
    
    /// <summary> Shows the various tooltip messages. </summary>
    static string Tooltip
    {
        get
        {
            string message;

            switch (showTooltip)
            {
                case true when ModuleManager.FinishedImporting:
                    message = ModuleManager.ImportStatus 
                        ? "Import was successful!" 
                        : "Import was unsuccessful. Please try again.";
                    break;

                default: {
                    if (checkedForUpdates)
                        message = EditorPrefs.GetBool("UpToDate") 
                            ? "You are up to date!" 
                            : "There is a new version available!";
                    else message 
                            = "Welcome to Lumina's Essentials!";

                    break;
                }
            }

            return message;
        }
    }
    #endregion

    [MenuItem("Tools/Lumina/Open Utility Panel")]
    internal static void OpenUtilityPanel()
    {
        // Get existing open window or if none, make a new one:
        var window = (UtilityPanel) GetWindow(typeof(UtilityPanel), true, "Lumina's Essentials Utility Panel");
        window.minSize = winSize;
        window.maxSize = window.minSize;
        window.Show();
    }

    void OnEnable()
    {
        // Initialize all the variables
        Initialization();

        // If the window is open, repaint it every frame.
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
        
        return;

        void Initialization()
        {
            // -- Initialize the Utility Panel --
            
            // Enable safe mode by default.
            SafeMode = true;

            // Initialize the installed modules
            ModuleManager.CheckForInstalledModules();
            SetupRequired = !InstalledModules.Values.Any(module => module);
            
            showTooltip = true;
            
            // -- Panel related variables --
            
            // Display the Toolbar.
            currentPanel = DisplayToolbar;

            // Displays the header and footer images.
            const string headerGUID = "7a1204763dac9b142b9cd974c88fdc8d"; // grabbed from .meta file in project folder.
            const string footerGUID = "22cbfe0e1e5aa9a46a9bd08709fdcac6"; // grabbed from .meta file in project folder.
            string       headerPath = AssetDatabase.GUIDToAssetPath(headerGUID);
            string       footerPath = AssetDatabase.GUIDToAssetPath(footerGUID);

            if (headerPath == null || footerPath == null) return;
            headerImg = AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);
            footerImg = AssetDatabase.LoadAssetAtPath<Texture2D>(footerPath);

            // If the user is not up-to-date, display a warning.
            if (!EditorPrefs.GetBool("UpToDate")) 
                EssentialsDebugger.LogWarning("There is a new version available!" + "\nPlease update to the latest version for the latest features.");

            // ResetUtilityPanel the window closed count.
            UpgradeWindow.WindowClosedCount = 0;
        }
    }

    // Clear the selected modules when the window is closed.
    void OnDisable()
    {
        Terminate();
        return;
        
        void Terminate() // Resets all the necessary variables.
        {
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            
            showTooltip                     = false;
            ModuleManager.FinishedImporting = false;
            checkedForUpdates               = false;
        }
    }

    /// <summary> Subscribes to the play mode state changed event to repaint the window when the user enters play mode. </summary>
    void PlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            // Start repainting when entering play mode
            case PlayModeStateChange.EnteredPlayMode:
                EditorApplication.update += Repaint;
                break;

            // Stop repainting when exiting play mode
            case PlayModeStateChange.ExitingPlayMode:
                EditorApplication.update -= Repaint;
                break;
        }
    }

    /// <summary>
    ///     Displays the toolbar at the top of the window that toggles between the two panels.
    /// </summary>
    void OnGUI()
    {
        // Initialize GUIStyles
        SetGUIStyles();

        // If the user is in play mode, displays a warning and deactivates the Utility Window.
        if (IsPlaymodeActive()) return;

        // Don't show the toolbar if the user in the setup panel or if the window doesn't exist.
        var utilityWindow = GetWindow<UtilityPanel>();
        if (utilityWindow != null) currentPanel();
    }

    /// <summary>
    ///     The toolbar at the top of the window with the tabs.
    /// </summary>
    void DisplayToolbar()
    {
        var areaRect = new Rect(0, 0, 370, 30);
        selectedTab = GUI.Toolbar(areaRect, selectedTab, tabLabels);

        GUILayout.Space(30);

        switch (selectedTab)
        {
            case 1:
                DrawSettingsGUI();
                break;

            case 2:
                DrawUtilitiesGUI();
                break;

            default:
                DrawSetupGUI();
                break;
        }
    }

    /// <summary>
    ///     Displays the main panel that shows the current version, latest version, etc.
    /// </summary>
    void DrawSetupGUI()
    {
        var areaRect = new Rect(0, 30, 370, 118);
        GUI.DrawTexture(areaRect, headerImg, ScaleMode.StretchToFill, false);
        GUILayout.Space(areaRect.y + 90);

        #region Main Labels (Version, Update Check, etc.)
        GUILayout.Label($"  Essentials Version:    {CurrentVersion}", mainLabelStyle);
        GUILayout.Label($"  Latest Version:           {LatestVersion}", mainLabelStyle);
        GUILayout.Label($"  Last Update Check:  {VersionUpdater.LastUpdateCheck}", mainLabelStyle);

        // End of Main Labels
        #endregion
        GUILayout.Space(3);

        #region Setup Lumina Essentials Button
        GUILayout.Space(3);

        if (SetupRequired)
        {
            GUI.backgroundColor = Color.red;
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                GUILayout.Label("SETUP REQUIRED", setupLabelStyle);
            }
            GUI.backgroundColor = Color.white;
        }
        else { GUILayout.Space(8); }

        GUI.color = Color.green;

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("<b>Setup Essentials...</b>\n(add/remove Modules)", buttonSetup, GUILayout.Width(200)))
            {
                // Select Setup Panel (not main panel)
                currentPanel = DrawModulesGUI;
            }

            GUILayout.FlexibleSpace();
        }

        GUI.color = new (0.89f, 0.87f, 0.87f);

        GUI.backgroundColor = Color.white;
        GUILayout.Space(4);

        // End of Setup Lumina Essentials Button
        #endregion
        GUILayout.Space(3);

        #region Text Box (Description)
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUI.color = new (1f, 0.75f, 0.55f);

                GUILayout.Label(Tooltip, buttonStyle, GUILayout.Width(200));

                GUI.color = Color.white;
                GUILayout.FlexibleSpace();
            }

            #region Description Label
            const string labelContent = @"Thank you for using Lumina's Essentials!
This window will help you get started. 
Please press the ""Setup Essentials"" button to begin.

Check out the ""Utilities"" tab to access the various workflow-enhancing features that this package provides.";

            GUILayout.Label(labelContent, wrapCenterLabelStyle);
            #endregion
        }
        #endregion
        GUILayout.Space(3);

        #region Grid of Buttons (Open Documentation, Open Changelog, etc.)
        GUILayout.Space(4);
        
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button(openDocumentationContent, GUILayout.Width(buttonSize), GUILayout.Height(40))) 
                Application.OpenURL("https://github.com/ltsLumina/Lumina-Essentials");

            if (GUILayout.Button(openChangeLogContent, GUILayout.Width(buttonSize), GUILayout.Height(40))) 
                Application.OpenURL("https://github.com/ltsLumina/Lumina-Essentials/releases/latest");
        }

        using (new GUILayout.HorizontalScope())
        {
            // Display the button to check for updates
            if (GUILayout.Button(checkForUpdatesContent, GUILayout.Width(buttonSize), GUILayout.Height(40)))
            {
                checkedForUpdates = true;
                VersionUpdater.CheckForUpdates();
                
                if (LatestVersion.Contains("Error"))
                {
                    EssentialsDebugger.LogError("Failed to fetch version! \nAre you connected to the internet?");
                    return;
                }

                // if there is a new version available, open the GitHub repository's releases page
                if (!EditorPrefs.GetBool("UpToDate"))
                {
                    EssentialsDebugger.LogWarning("There is a new version available!" + "\nPlease update to the latest version to ensure functionality.");
                    Application.OpenURL("https://github.com/ltsLumina/Lumina-Essentials/releases/latest");
                }
                else
                {
                    EssentialsDebugger.Log("You are up to date!");
                }
            }

            if (GUILayout.Button(openKnownIssuesContent, GUILayout.Width(buttonSize), GUILayout.Height(40))) 
                Application.OpenURL("https://github.com/ltsLumina/Lumina-Essentials/issues");
        }
        #endregion
        GUILayout.Space(3);

        // Footer/Developed by Lumina
        if (GUILayout.Button(footerImg, btImgStyle)) Application.OpenURL("https://github.com/ltsLumina/");
    }

    /// <summary>
    ///     Displays the setup panel that allows the user to select which modules to install.
    /// </summary>
    void DrawModulesGUI()
    {
        DrawModulesHeader();
        DrawModulesInstallGUI();
        DrawModulesHelpBox();
        DrawModulesButtons();
    }

    /// <summary>
    ///     Displays the settings panel that allows the user to change various settings.
    /// </summary>
    void DrawSettingsGUI()
    {
        // Allow for scrolling in the Settings tab.
        settings_scrollPos = EditorGUILayout.BeginScrollView(settings_scrollPos, false, false, GUILayout.Width(winSize.x), GUILayout.Height(winSize.y));

        DrawSettingsHeader();
        DrawResetSettingsButton();
        DrawSettingsLabels();
        DrawAdvancedSettingsGUI();

        // End scroll view
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    ///     Displays the utilities panel that provides various features to enhance the user's workflow.
    /// </summary>
    void DrawUtilitiesGUI()
    {
        // Allow for scrolling in the Utilities tab.
        utilities_scrollPos = EditorGUILayout.BeginScrollView(utilities_scrollPos, false, false, GUILayout.Width(winSize.x), GUILayout.Height(winSize.y));
        
        DrawUtilitiesHeader();
        DrawUtilitiesButtonsGUI();
        DrawConfigureImagesGUI();
        
        // End scroll view
        EditorGUILayout.EndScrollView();
    }
}
}
