#region
using System;
using UnityEditor;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
/// <summary>
/// Handles time-related operations.
/// Formats time messages for the Utility window.
/// </summary>
internal static class TimeManager
{
    internal static string TimeSinceLastUpdate()
    {
        string lastUpdateCheckString = EditorPrefs.GetString("LastUpdateCheck");
        bool   isParsedSuccessfully  = DateTime.TryParse(lastUpdateCheckString, out DateTime lastUpdateCheck);

        if (!isParsedSuccessfully) return "Failed to parse.";

        TimeSpan timeSpanSinceLastUpdate = DateTime.Now - lastUpdateCheck;

        (TimeSpan timeSpan, string singularMessage, string pluralMessage)[] timeFrames ={ 
        (TimeSpan.FromHours(1), "{0} minute ago", "{0} minutes ago"),
        (TimeSpan.FromHours(2), "{0} hour ago", "{0} hour ago"),
        (TimeSpan.FromDays(1), "{0} hours ago", "{0} hours ago"),
        (TimeSpan.FromDays(2), "{0} day ago", "{0} day ago"),
        (TimeSpan.FromDays(7), "{0} days ago", "{0} days ago"),
        (TimeSpan.FromDays(30), "{0} weeks ago", "{0} weeks ago"),
        (TimeSpan.MaxValue, "<color=red>More than a week ago</color>", 
         "<color=red>More than a week ago</color>") };

        foreach ((TimeSpan timeSpan, string singularMessage, string pluralMessage) in timeFrames)
        {
            if (timeSpanSinceLastUpdate < timeSpan) return FormatTimeMessage(timeSpanSinceLastUpdate, singularMessage, pluralMessage);
        }

        return string.Empty; // unreachable code but added to satisfy C# rules
    }

    static string FormatTimeMessage(TimeSpan deltaTime, string singularMessage, string pluralMessage)
    {
        if (deltaTime.TotalMinutes < 1) return "Less than a minute ago";
        if (deltaTime.TotalMinutes < 2) return string.Format(singularMessage, 1);
        if (deltaTime.TotalMinutes < 60) return string.Format(pluralMessage, (int) deltaTime.TotalMinutes);

        // Convert minutes to hours in delta time_totalMinutes is more than 60 but less than 120
        if (deltaTime.TotalMinutes is >= 60 and < 120) return string.Format(singularMessage, 1);

        if (deltaTime.TotalHours < 2) return string.Format(singularMessage, (int) deltaTime.TotalHours);
        if (deltaTime.TotalHours < 24) return string.Format(pluralMessage, (int) deltaTime.TotalHours);

        if (deltaTime.TotalDays < 2) return string.Format(singularMessage, (int) deltaTime.TotalDays);
        if (deltaTime.TotalDays >= 2) return string.Format(pluralMessage, (int) deltaTime.TotalDays);
        return "";
    }

    internal static int TimeSinceLastUpdateInDays()
    {
        if (DateTime.TryParse(VersionUpdater.LastUpdateCheck, out DateTime lastUpdateCheck))
        {
            TimeSpan timeSpanSinceLastUpdate = DateTime.Now - lastUpdateCheck;
            return (int) timeSpanSinceLastUpdate.TotalDays;
        }

        // Handle parse failure - depends on your requirements
        return -1; // Example: return -1 if the date could not be parsed
    }
}
}
