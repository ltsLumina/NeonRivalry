using UnityEditor;

namespace Lumina.Essentials.Editor.UI.Management
{
    internal static class AutoSaveConfig
    {
        /// <summary>
        /// Enable auto save functionality
        /// </summary>
        internal static bool Enabled
        {
            get => EditorPrefs.GetBool("AutoSaveEnabled", false);
            set => EditorPrefs.SetBool("AutoSaveEnabled", value);
        }

        /// <summary>
        /// The frequency in minutes auto save will activate
        /// </summary>
        internal static int Interval
        {
            get => EditorPrefs.GetInt("AutoSaveInterval", 15);
            set => EditorPrefs.SetInt("AutoSaveInterval", value);
        }

        /// <summary>
        /// Log a message every time the scene is auto saved
        /// </summary>
        internal static bool Logging
        {
            get => EditorPrefs.GetBool("AutoSaveLogging", false);
            set => EditorPrefs.SetBool("AutoSaveLogging", value);
        }
    }
}
