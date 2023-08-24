#region
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
public class DirectoryUtilities : MonoBehaviour
{
    #region Check for Folder/File
    internal static bool IsFolderInEssentials(string baseDirectory, string targetFolderName)
    {
        try
        {
            // Search for Lumina's Essentials in the whole directory
            string[] luminaDirectories = Directory.GetDirectories(baseDirectory, "Lumina's Essentials", SearchOption.AllDirectories);

            if (!luminaDirectories.Any())
            {
                EssentialsDebugger.LogWarning("Lumina's Essentials folder not found." + "\nPlease contact Lumina about this error or create an issue on the GitHub page.");
                return false;
            }

            foreach (string luminaDirectory in luminaDirectories)
            {
                string[] directories = Directory.GetDirectories(luminaDirectory, targetFolderName, SearchOption.AllDirectories);

                if (directories.Length > 0) return true; // If target folder is found within any Lumina's Essentials folder, return true immediately
            }

            return false; // If the process finishes without finding the target folder, return false
        } catch (Exception ex)
        {
            // Handle exception, mostly due to lack of access to some directories
            EssentialsDebugger.LogError($"An error occurred: {ex.Message}");
            return false;
        }
    }

    internal static bool IsFileInEssentials(string baseDirectory, string targetFileName)
    {
        try
        {
            // Search for Lumina's Essentials in the whole directory
            string[] luminaDirectories = Directory.GetDirectories(baseDirectory, "Lumina's Essentials", SearchOption.AllDirectories);

            if (!luminaDirectories.Any())
            {
                Console.WriteLine("Lumina's Essentials folder not found.");
                return false;
            }

            foreach (string luminaDirectory in luminaDirectories)
            {
                string[] files = Directory.GetFiles(luminaDirectory, targetFileName, SearchOption.AllDirectories);

                if (files.Length > 0) return true; // If target file is found within any Lumina's Essentials folder, return true immediately
            }

            return false; // If the process finishes without finding the target file, return false
        } catch (Exception ex)
        {
            // Handle exception, mostly due to lack of access to some directories
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
    #endregion
    
    internal static void CreateDirectories(string root, params string[] directories)
    {
        string fullpath = Path.Combine(Application.dataPath, root);

        if (!Directory.Exists(fullpath))
        {
            Directory.CreateDirectory(fullpath);

            if (VersionManager.DebugVersion) EssentialsDebugger.Log("Successfully created directory: " + fullpath);
        }

        foreach (string newDirectory in directories)
        {
            string newFullPath = Path.Combine(fullpath, newDirectory);

            if (!Directory.Exists(newFullPath))
            {
                Directory.CreateDirectory(newFullPath);

                if (VersionManager.DebugVersion) EssentialsDebugger.Log("Successfully created directory: " + newFullPath);
            }
        }
    }

    internal static string GetFolderNameFromString(string str)
    {
        // Assign a default string in case directoryInfo.Name returns an empty string.
        if (string.IsNullOrEmpty(str)) str = "New Folder";

        var directoryInfo = new DirectoryInfo(str);
        return directoryInfo.Name;
    }

    internal static void DeleteAutorunFiles()
    {
        string       baseDirectory    = Application.dataPath;
        const string targetFolderName = "Lumina's Essentials";
        const string targetFileName   = "Autorun.cs";

        // Search for Lumina's Essentials in the whole directory
        string[] luminaDirectories = Directory.GetDirectories(baseDirectory, targetFolderName, SearchOption.AllDirectories);

        foreach (string luminaDirectory in luminaDirectories)
        {
            // Get all files in the Lumina's Essentials directory and its subdirectories
            string[] files = Directory.GetFiles(luminaDirectory, targetFileName, SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Get the relative path from Assets
                string relativePath = $"Assets{file[baseDirectory.Length..]}";

                // If Autorun.cs file exists
                if (File.Exists(file))
                {
                    // Delete the file
                    if (!VersionManager.DebugVersion) AssetDatabase.DeleteAsset(relativePath);
                    else EssentialsDebugger.LogWarning("Can't delete Autorun.cs in debug mode.");

                    AssetDatabase.Refresh();
                }
                else if (VersionManager.DebugVersion)
                {
                    EssentialsDebugger.LogError($"Autorun.cs not found in {file}\n └ <i>(This is not an error. It just means that the file does not exist.)</i>");
                }
            }
        }
    }
}
}
