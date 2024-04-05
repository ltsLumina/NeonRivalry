using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class AutoUnpackPrefab : MonoBehaviour
    {
        void Awake()
        {
        #if UNITY_EDITOR
            // prefabs are unpacked in play mode by default
            if (Application.isPlaying) return;

            if (PrefabStageUtility.GetCurrentPrefabStage())
            {
                Debug.Log("Emulating auto unpack in prefab stage.");
                return;
            }

            StartCoroutine(DelayedUnpack());
        #endif
        }

        IEnumerator DelayedUnpack()
        {
            // Wait for one frame to prevent a crash when creating a prefab variant
            yield return null;

        #if UNITY_EDITOR
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            DestroyImmediate(this);
        #endif
        }
    }
}
