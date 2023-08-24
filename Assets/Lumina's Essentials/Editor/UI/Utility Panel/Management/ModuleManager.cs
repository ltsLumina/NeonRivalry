#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
public class ModuleManager : MonoBehaviour
{
    static string relativePath;

    /// <summary> Whether or not the import of the module package was successful. </summary>
    internal static bool ImportStatus { get; private set; }
    /// <summary> Whether or not a module is currently being imported. </summary>
    internal static bool Importing { get; private set; }
    /// <summary> Whether or not the import of the module package has finished. </summary>
    internal static bool FinishedImporting { get; set; }

    #region Checks for Modules
    internal static void CheckForInstalledModules()
    {
        bool isDebugVersion = VersionManager.DebugVersion;
        
        if (FullPackageIsInstalled())
        {
            SetAllModulesTo(true, isDebugVersion);
            return;
        }
        
        CheckIndividualModules(isDebugVersion);
    }

    static void SetAllModulesTo(bool state, bool debugVersion)
    {
        foreach (string module in UtilityPanel.InstalledModules.Keys.ToList())
        {
            UtilityPanel.InstalledModules[module] = state;
            DebugLogState(debugVersion, state, module);
        }
    }

    static void CheckIndividualModules(bool debugVersion)
    {
        foreach (string item in new List<string>(UtilityPanel.InstalledModules.Keys)) { SetModuleInstalledState(item, debugVersion); }
    }

    static void SetModuleInstalledState(string item, bool debugVersion)
    {
        string projectDirectory = Application.dataPath;
        string itemFile         = item + ".cs";

        bool exists = DirectoryUtilities.IsFolderInEssentials(projectDirectory, item) || DirectoryUtilities.IsFileInEssentials(projectDirectory, itemFile);

        DebugLogState(debugVersion, exists, item);
        UtilityPanel.InstalledModules[item] = exists;
    }

    static void DebugLogState(bool debugVersion, bool state, string module)
    {
        if (!debugVersion) return;

        string stateMessage = state ? "exists in the project." : "does NOT exist in the project.";
        Debug.Log($"Item '{module}' {stateMessage}");
    }

    static bool FullPackageIsInstalled()
    {
        string       mainDirectory   = Path.Combine("Lumina's Essentials", "Modules");
        const string targetDirectory = "Examples";

        string[] allDirectories = Directory.GetDirectories(Application.dataPath, "*.*", SearchOption.AllDirectories);

        foreach (string directory in allDirectories)
        {
            // Get the relative path from Assets
            relativePath = directory[(Application.dataPath.Length - "Assets".Length)..];

            // If directory is within "Lumina's Essentials/Modules" (Why does this work?)
            if (Path.GetFullPath(relativePath).EndsWith(mainDirectory))
            {
                // Get all subdirectories within the main directory.
                string[] subDirectories = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories);

                // Check if "Examples" directory exists within any of the subdirectories
                if (subDirectories.Any(sub => Path.GetFileName(sub).Equals(targetDirectory, StringComparison.OrdinalIgnoreCase)))
                {
                    if (VersionManager.DebugVersion) Debug.Log("Full package is installed.");
                    return true;
                }
            }
        }

        if (VersionManager.DebugVersion) Debug.Log("Full package is NOT installed.");
        return false;
    }

    internal static void InstallSelectedModules()
    {
        // Before doing anything, check which modules are installed.
        CheckForInstalledModules();

        // Find the path to the "Lumina's Essentials/Editor/Packages" folder
        const string targetDirectory = "Lumina's Essentials/Editor/Packages";
        relativePath = GetRelativePath(targetDirectory);

        if (string.IsNullOrEmpty(relativePath))
        {
            Debug.LogError(targetDirectory + " not found.");
            return;
        }

        AssetDatabase.StartAssetEditing();

        try
        {
            InstallModules();
        } 
        catch (Exception e)
        {
            EssentialsDebugger.LogError("Failed to install module(s)" + "\n" + e.Message + "\n" + e.StackTrace);
            throw;
        }
        finally
        {
            // Stop the AssetEditing at the end, even if some error was thrown.
            AssetDatabase.StopAssetEditing();

            // Check for installed modules again.
            CheckForInstalledModules();
        }
    }

    static void InstallModules()
    {
        // Compare selected and installed modules
        foreach (KeyValuePair<string, bool> module in UtilityPanel.SelectedModules.Where(module => module.Value))
        {
            if (HandleFullPackage(module)) return;

            HandleOtherModules(module);
        }
    }

    static bool HandleFullPackage(KeyValuePair<string, bool> module)
    { // Handle "Full Package" separately
        if (module.Key != "Full Package") return false;

        if (UtilityPanel.InstalledModules.ContainsKey(module.Key) && UtilityPanel.InstalledModules[module.Key])
        {
            bool reInstallConfirmation = ModuleInstallConfirmation(module.Key);
            if (reInstallConfirmation) ImportModulePackage(relativePath, module.Key);
        }
        else // Full Package is selected but not installed, install directly.
        {
            ImportModulePackage(relativePath, module.Key);
        }

        return true;
    }
    static void HandleOtherModules(KeyValuePair<string, bool> module)
    { // For other modules
        if (UtilityPanel.InstalledModules.ContainsKey(module.Key) && UtilityPanel.InstalledModules[module.Key])
        {
            bool reInstallConfirmation = ModuleInstallConfirmation(module.Key);
            if (reInstallConfirmation) ImportModulePackage(relativePath, module.Key);
        }
        else // If not installed, install directly.
        {
            ImportModulePackage(relativePath, module.Key);
        }
    }

    static bool ModuleInstallConfirmation(string moduleName) => EditorUtility.DisplayDialog
        ("Module Installation Warning", $"The module \"{moduleName}\" is already installed. Would you like to reinstall it?", "Yes", "No");

    static string GetRelativePath(string targetDirectory)
    {
        string[] allDirectories = Directory.GetDirectories(Application.dataPath, "*.*", SearchOption.AllDirectories);

        foreach (string directory in allDirectories)
        {
            string pathFromAssets = directory.Replace(Application.dataPath, "Assets");

            if (pathFromAssets.Replace("\\", "/").EndsWith(targetDirectory)) return pathFromAssets + "/";
        }

        return string.Empty;
    }

    static void ImportModulePackage(string relativePath, string moduleName)
    {
        // Begin importing the packages.
        Importing = true;
        
        string modulePath = Path.Combine(relativePath, moduleName) + ".unitypackage";

        // Subscribe to events before importing the package.
        AssetDatabase.importPackageCompleted += packageName =>
        {
            if (VersionManager.DebugVersion) EssentialsDebugger.Log("Imported: " + packageName);
            ImportStatus = true;
        };

        AssetDatabase.importPackageFailed += (packageName, errorMessage) =>
        {
            if (VersionManager.DebugVersion) EssentialsDebugger.LogError("Failed to import: " + packageName + "\n" + errorMessage);
            ImportStatus = false;
        };

        AssetDatabase.ImportPackage(modulePath, false);
        Importing                = false;
        FinishedImporting        = true;

        AssetDatabase.importPackageCompleted -= AssetDatabaseOnImportPackageCompleted;
        AssetDatabase.importPackageFailed -= AssetDatabaseOnImportPackageFailed;

        // Delete the Autorun.cs file from the project
        DirectoryUtilities.DeleteAutorunFiles();
        return;

        void AssetDatabaseOnImportPackageCompleted(string packageName)
        {
            ImportStatus = true;
            Debug.Log("Imported: " + packageName);
        }
        
        void AssetDatabaseOnImportPackageFailed(string packageName, string errorMessage)
        {
            ImportStatus = false;
            Debug.LogError("Failed to import: " + packageName + "\n" + errorMessage);
        }
    }

    internal static void ClearSelectedModules()
    {
        foreach (KeyValuePair<string, bool> module in UtilityPanel.SelectedModules.ToList()) { UtilityPanel.SelectedModules[module.Key] = false; }
    }

    // -- End of Module Checks --
    #endregion
}
}
