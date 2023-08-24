#region
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Lumina.Essentials.Editor.UI.Management.EditorGUIUtils;
using static Lumina.Essentials.Editor.UI.UtilityPanel;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
internal sealed class ProjectStructureGUI : EditorWindow
{
    static Vector2 _scrollPosition;
    static Dictionary<string, bool> folderToggleStates = new ();
    static Dictionary<string, string> folderChanges = new ();

    static string selectedFolder;
    static string renamedFolder;

    internal static void Create()
    {
        GUILayout.Space(8);

        if (GUILayout.Button("Load Directory")) { LoadDirectory(); }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
        foreach (string folder in folderToggleStates.Keys.ToArray())
        {
            folderToggleStates[folder] = EditorGUILayout.Toggle(Path.GetFileName(folder), folderToggleStates[folder]);
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Create Subfolder"))
        {
            selectedFolder = GetSelectedFolder();

            if (selectedFolder != null)
            {
                string newFolderPath = Path.Combine(selectedFolder, "New Folder");

                // if the name "New Folder" already exists, add a number to the end of the name.
                if (Directory.Exists(newFolderPath))
                {
                    int i = 1;
                    while (Directory.Exists(newFolderPath + $" ({i})")) i++;
                    newFolderPath += $" ({i})";
                }
                
                folderChanges.Add(newFolderPath, selectedFolder);
                
                ApplyChanges();
            }
            else { EssentialsDebugger.LogWarning("Selected folder does not exist."); }
        }
        else { EssentialsDebugger.LogWarning("No folder is selected."); }

        if (GUILayout.Button("Delete Selected"))
        {
            selectedFolder = GetSelectedFolder();

            if (selectedFolder != null)
            {
                // This is a test for Rider's VCS integration.
                string relativePath = selectedFolder.Replace(Application.dataPath, "Assets"); // Convert absolute path to a relative path

                if (AssetDatabase.IsValidFolder(relativePath))
                {
                    // Delete the folder in Unity's Project panel.
                    AssetDatabase.DeleteAsset(relativePath);
                    AssetDatabase.Refresh();

                    // Delete the folder from your Dictionary if it exists.
                    if (folderToggleStates.ContainsKey(selectedFolder)) { folderToggleStates.Remove(selectedFolder); }
                }
                else { EssentialsDebugger.LogWarning("Selected folder does not exist."); }
            }
            else { EssentialsDebugger.LogWarning("No folder is selected."); }
        }
        
        // Text field to rename the folder.
        renamedFolder = GUILayout.TextField(renamedFolder); 

        if (GUILayout.Button("Rename Selected"))
        {
            selectedFolder = GetSelectedFolder();
            
            if (selectedFolder != null)
            {
                if (Directory.Exists(selectedFolder))
                {
                    // Delete the folder physically.
                    AssetDatabase.RenameAsset(selectedFolder, renamedFolder);
                    AssetDatabase.Refresh();
                    
                    

                    // Delete the folder from your Dictionary if exists.
                    if (folderToggleStates.ContainsKey(selectedFolder)) { folderToggleStates.Remove(selectedFolder); }
                    
                    ApplyChanges();
                }
                else { EssentialsDebugger.LogWarning("Selected folder does not exist."); }
            }
            else { EssentialsDebugger.LogWarning("No folder is selected."); }
        }

        // GUILayout.Label
        // ($"{ProjectPath} \n"           +
        //  $"L {subFolderArt}"           +
        //  $"   L {subFolderAnimations}" +
        //  $"L {subFolderScripts}"       +
        //  $"   L {subFolderEditor}"     +
        //  $"   L {subFolderRuntime}"    +
        //  $"L {subfolderScenes}", wordWrapRichTextLabelStyle);

        // if (GUILayout.Button("Rename Selected"))
        // {
        //     selectedFolder = GetSelectedFolder();
        //     
        //     if (selectedFolder != null)
        //     {
        //         string newFolderPath = Path.Combine(Directory.GetParent(selectedFolder).FullName, "New Folder");
        //         folderChanges.Add(newFolderPath, selectedFolder);
        //         AddFolderToStates(newFolderPath);
        //     }
        // }

        if (GUILayout.Button("Apply Changes")) ApplyChanges();
    }
    
    static void LoadDirectory()
    {
        folderToggleStates.Clear();

        // If the project path is valid, use it. Otherwise, use the Assets folder.
        AddFolderToStates(Directory.Exists(ProjectPath) ? ProjectPath : Application.dataPath);
    }

    static void AddFolderToStates(string folderPath)
    {
        folderToggleStates.Add(folderPath, false);

        foreach (string subfolder in Directory.GetDirectories(folderPath))
        {
            folderToggleStates.Add(subfolder, false);
        }
    }

    static string GetSelectedFolder()
    {
        foreach (KeyValuePair<string, bool> kvp in folderToggleStates)
        {
            if (kvp.Value) return kvp.Key;
        }

        return null;
    }

    internal static void ApplyChanges()
    {
        foreach (KeyValuePair<string, string> change in folderChanges)
        {
            // Check if the directory already exists before creating it.
            if (!Directory.Exists(change.Key))
            {
                Directory.CreateDirectory(change.Key);
                AssetDatabase.Refresh();
            }
            
            // Reflect these changes to folderToggleStates
            folderToggleStates.TryAdd(change.Key, false);
        }

        folderChanges.Clear();

        LoadDirectory();
    }

    internal static void ClearAllChanges()
    {
        folderChanges.Clear();
        folderToggleStates.Clear();
    }
}
}
