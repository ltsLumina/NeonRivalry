#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static Lumina.Essentials.Editor.UI.Management.VersionManager;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
/// <summary>
/// Updates the version of Lumina's Essentials.
/// </summary>
    internal static class VersionUpdater
    {
        /// <summary> Whether or not the current version is the latest version. </summary>
        internal static string LastUpdateCheck => TimeManager.TimeSinceLastUpdate();
        
        /// <summary> The queue of coroutines to run. </summary>
        readonly static Queue<IEnumerator> coroutineQueue = new ();

        internal static void CheckForUpdates()
        {
            EditorApplication.update += Update;
            coroutineQueue.Enqueue(RequestUpdateCheck());
        }

        static void Update()
        {
            if (coroutineQueue.Count > 0)
            {
                IEnumerator coroutine = coroutineQueue.Peek();
                if (!coroutine.MoveNext()) coroutineQueue.Dequeue();
            }
            else { EditorApplication.update -= Update; }
        }

        static IEnumerator RequestUpdateCheck()
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get("https://api.github.com/repos/ltsLumina/Lumina-Essentials/releases/latest");

            yield return webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                yield return null; // Wait for the request to complete
            }

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success: {
                    string jsonResult = Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                    string tag        = JsonUtility.FromJson<Release>(jsonResult).tag_name;

                    // Update LatestVersion, UpToDate, LastUpdateCheck accordingly.
                    EditorPrefs.SetString("LastUpdateCheck", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                
                    // Set the Editor Prefs to their updated values.
                    LatestVersion = tag;
                    UpdatePrefs();
                    break;
                }

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("UnityWebRequest failed with result: ProtocolError" + "\nYou have probably been rate limited by GitHub.");

                    LatestVersion = null;
                    break;

                // Web Request failed, log errors and set LatestVersion to null.
                default:
                    Debug.LogError($"UnityWebRequest failed with result: {webRequest.result} \nAre you connected to the internet?");
                    Debug.LogError($"Error message: {webRequest.error}");

                    LatestVersion = null;
                    break;
            }
        }

    }

    [Serializable]
    internal class Release
    {
        public string tag_name;
    }
}
