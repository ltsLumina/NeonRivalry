using UnityEditor;

namespace Lumina.Essentials.Editor.UI.Management
{
/// <summary>
/// Runs checks upon startup to ensure the user is up-to-date and to display warnings if necessary.
/// </summary>
internal static class StartupChecks
{
    /// <summary>
    ///     Checks if a new version is available by comparing the current version with the latest version.
    /// </summary>
    /// <param name="currentVersion"> The current version of Lumina's Essentials. </param>
    /// <param name="comparisonVersion"> The version to compare the current version with. </param>
    /// <returns> True if the current version is older than the comparison version. </returns>
    internal static bool IsNewVersionAvailable(string currentVersion, string comparisonVersion) => !VersionManager.CompareVersions(currentVersion, comparisonVersion);
    
    static void CheckForUpdatesAfterOneWeek()
    {
        if (TimeManager.TimeSinceLastUpdateInDays() > 7)
        {
            VersionUpdater.CheckForUpdates();

            // If there is an update available, display a warning to the user.
            if (IsNewVersionAvailable(VersionManager.CurrentVersion, VersionManager.LatestVersion))
                EditorUtility.DisplayDialog("Update Available", "A new version of Lumina's Essentials is available. \n" + "Please check the changelog for more information.", "OK");
        }
    }

    internal static void DebugBuildWarning()
    {
        if (!VersionManager.SuppressDebugBuildAlert && VersionManager.DebugVersion)
            if (EditorUtility.DisplayDialog
                ("Debug Version", "You are currently using a Debug Version of Lumina's Essentials. " + "\nThis means the application might not work as intended.", "OK"))
            {
                VersionManager.SuppressDebugBuildAlert = true;
                EditorPrefs.SetBool("DontShowAgain", VersionManager.SuppressDebugBuildAlert);
            }
    }
}
}
