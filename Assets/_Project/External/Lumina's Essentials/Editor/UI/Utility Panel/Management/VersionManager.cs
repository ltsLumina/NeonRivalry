#region
using System;
using System.Text.RegularExpressions;
using UnityEditor;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
/// <summary>
///    Manages the version of Lumina's Essentials.
/// </summary>
internal static class VersionManager
{
    internal readonly static string[] EssentialsPrefs =
    { "CurrentVersion",
      "LatestVersion",
      "LastOpenVersion",
      "UpToDate",
      "DebugVersion",
      "SuppressDebugBuildAlert",
      "SetupRequired",
      "SafeMode",
      "AutoSaveEnabled",
      "AutoSaveInterval",
      "AutoSaveLogging" };
    
    /// <summary> The current version of Lumina's Essentials. </summary>
    internal static string CurrentVersion => "3.0.4";
    
    /// <summary> The latest version of Lumina's Essentials available on GitHub. </summary>
    internal static string LatestVersion
    {
        get => EditorPrefs.GetString("LatestVersion", null);
        set => EditorPrefs.SetString("LatestVersion", string.IsNullOrEmpty(value) 
                                         ? "Error fetching version... \n  └ <i>(Are you connected to the internet?)</i>" 
                                         : value);
    }
    
    /// <summary> The version of the package that was last opened. </summary>
    internal static string LastOpenVersion
    {
        get => EditorPrefs.GetString("LastOpenVersion", "Unknown");
        set => EditorPrefs.SetString("LastOpenVersion", value);
    }
    
    /// <summary> Whether or not the current version is a debug version. </summary>
    internal static bool DebugVersion => CurrentVersion.Contains("Debug", StringComparison.OrdinalIgnoreCase);
    internal static bool SuppressDebugBuildAlert
    {
        get => EditorPrefs.GetBool("SuppressDebugBuildAlert");
        set => EditorPrefs.SetBool("SuppressDebugBuildAlert", value);
    }

    internal static void UpdatePrefs()
    {
        EditorPrefs.SetString("CurrentVersion", CurrentVersion);
        EditorPrefs.SetString("LatestVersion", LatestVersion ?? "Error fetching version..");
        EditorPrefs.SetBool("UpToDate", CompareVersions(CurrentVersion, LatestVersion));
        EditorPrefs.SetBool("DebugVersion", DebugVersion);
    }

    /// <summary>
    ///     Compares a version string to different version string.
    ///     If v1 is newer than v2, the action is performed.
    /// </summary>
    /// <param name="v1"> The first version to compare. </param>
    /// <param name="v2"> The second version to compare. </param>
    /// <returns> Whether or not the current version is newer than the last opened version. </returns>
    internal static bool CompareVersions(string v1, string v2)
    {
        // extract the numeric parts of the versions
        var   regex   = new Regex(@"(\d+(\.\d+){0,3})");
        Match matchV1 = regex.Match(v1);
        Match matchV2 = regex.Match(v2);

        // if either version string doesn't contain a valid version number, return false
        if (!matchV1.Success || !matchV2.Success) return false;

        // convert the numeric parts of the versions to Version objects
        var version1 = new Version(matchV1.Value);
        var version2 = new Version(matchV2.Value);

        // compare the versions
        int comparisonResult = version1.CompareTo(version2);

        // return true if v1 is newer or equal to v2
        return comparisonResult >= 0;
    }
}
}
