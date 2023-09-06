// Creator: Tarodev
// Modified by Lumina for Lumina's Essentials.

#region
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Editor.UI.Management
{
[CustomEditor(typeof(AutoSave))]
internal class AutoSave : UnityEditor.Editor
{
    static CancellationTokenSource tokenSource;
    static Task task;

    [InitializeOnLoadMethod]
    static void OnInitialize()
    {
        CancelTask();

        tokenSource = new ();
        task        = SaveInterval(tokenSource.Token);
    }

    static void CancelTask()
    {
        if (task == null) return;
        tokenSource.Cancel();
        task.Wait();
    }

    static async Task SaveInterval(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(AutoSaveConfig.Interval * 1000 * 60, token);

            if (!AutoSaveConfig.Enabled || Application.isPlaying || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) continue;
            if (!InternalEditorUtility.isApplicationActive) continue;

            EditorSceneManager.SaveOpenScenes();
            if (AutoSaveConfig.Logging) EssentialsDebugger.Log($"Auto-saved at {DateTime.Now:h:mm:ss tt}");
        }
    }
}
}
