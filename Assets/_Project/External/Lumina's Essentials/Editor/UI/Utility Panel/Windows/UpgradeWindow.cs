// ONLY UNCOMMENT THIS IF DEBUG BUILD IS TRUE IN VERSION-MANAGER.
//#define DEBUG_BUILD

#region
using System.Linq;
using Lumina.Essentials.Editor.UI.Management;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.EditorGUIUtils;
#endregion

namespace Lumina.Essentials.Editor.UI
{
/// <summary>
///     A window that launches upon project launch to help the user get started with Lumina's Essentials.
///     Provides a list of all the features and a button to open the documentation.
///     It also provides a button to install the latest version of Lumina's Essentials as well as DOTween (in a future
///     update).
/// </summary>
internal sealed class UpgradeWindow : EditorWindow
{
#if DEBUG_BUILD
        [MenuItem("Lumina's Essentials/Setup", false, 0)]
        static void Debug_OpenUpgradeWindow() => Open();
#endif

    readonly static Vector2 winSize = new (400,300);
    
    internal static int WindowClosedCount;
    const string dialogMessage = "The Setup Window will not open again unless you open the Utility Panel. "                 +
                                 "If you wish to access the Utility Panel to setup your installation of Lumina's Essentials" +
                                 ", it can be found under \"Tools\" > \"Lumina\" > \"Open Utility Panel\" in the toolbar.";
    
    /// <summary>
    ///     Opens the upgrade window that instructs the user on how to get started with Lumina's Essentials.
    /// </summary>
    /// <param name="updateOccured">Whether or not an update has occured since the last time the window was opened.</param>
    internal static void Open(bool updateOccured = false)
    {
        var window = GetWindow<UpgradeWindow>(true, "New Version of Lumina's Essentials Imported", true);
        if (window == null) return;
        
        window.minSize = winSize;
        window.maxSize = window.minSize;
        window.Show();

        if (updateOccured) HandleUpdateOccured();
    }

    void OnDisable()
    {
        VersionManager.LastOpenVersion = VersionManager.CurrentVersion;
        WindowClosedCount++;

        if (WindowClosedCount > 4)
        {
            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Lumina's Essentials", dialogMessage, "I understand.");
        }
    }

    void OnGUI()
    {
        SetGUIStyles();
        
        DisplayGUIElements();
    }

    static void HandleUpdateOccured()
    {
        EditorUtility.DisplayDialog("Update Occured", "An update has occured. Please setup the new version of Lumina's Essentials.", "OK");
        UtilityPanel.SetupRequired = !UtilityPanel.InstalledModules.Values.Any(module => module);
    }

    void DisplayGUIElements()
    {
        DrawBackground();
        DrawTitleAndInstructions();
        DrawUtilityPanelButton();
    }

    void DrawBackground() => EditorGUI.DrawRect(new (0, 0, maxSize.x, maxSize.y), new (0.18f, 0.18f, 0.18f));

    void DrawTitleAndInstructions()
    {
        // Top label with the title of the window in large rose gold text
        GUILayout.Label("Lumina's Essentials", UpgradeWindowHeaderStyle);

        GUILayout.Label("Select <color=orange>\"Setup Lumina Essentials\"</color> in the Utility Panel to set it up \n and ensure functionality", middleStyle);

        GUILayout.Space(10);

        // Large header in case of upgrade
        GUILayout.Label("IMPORTANT IN CASE OF UPGRADE", UpgradeWindowHeaderStyle);

        // Text that asks the user to press the button below in regular text with the "Setup Lumina Essentials" text in orange
        GUILayout.Label
        ("If you are upgrading from a Lumina Essentials version older than <b>3.0.0</b> \n (<color=orange>before the rework of the Essentials package</color>) " +
         "\n You will see a lot of errors. This is due to several reworks since then. " + "\n     \n <b>Follow these instructions</b> to fix them: ", middleStyle);

        GUILayout.Label("<color=orange>1) Delete the old version </color>of the Lumina Essentials package.", middleStyle);
        GUILayout.Label("<color=orange>2)</color> Open the Utility Panel and press <color=orange>\"Setup Essentials\"</color> to set it up.", middleStyle);
    }

    void DrawUtilityPanelButton()
    {
        // Button to open the Utility Panel
        const int buttonWidth  = 200;
        const int buttonHeight = 35;

        using (new GUILayout.VerticalScope())
        {
            GUILayout.FlexibleSpace();

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Open Utility Panel", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                {
                    // Open the Utility Panel and close the Setup Window
                    UtilityPanel.OpenUtilityPanel();
                    Close();
                }
                
                GUILayout.FlexibleSpace();
            }
            
            GUILayout.FlexibleSpace();
        }
    }
}
}
