using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Abiogenesis3d
{

    [ExecuteInEditMode]
    public class AutoImportUnityPackage : MonoBehaviour
    {
        [Header("This gameObject will be destroyed after import, unless you're in the prefab stage.")]
        public Object unitypackage;
        public bool builtin;
        public bool urp;

        void Awake()
        {
        #if UNITY_EDITOR
            bool shouldImport = false;

            if (builtin) shouldImport = true;
        #if UNITY_PIPELINE_URP
            if (urp) shouldImport = true;
        #endif

            if (shouldImport)
            {
                string path = AssetDatabase.GetAssetPath(unitypackage);
                if (PrefabStageUtility.GetCurrentPrefabStage())
                {
                    Debug.Log("Emulating import in prefab stage: " + path);
                }
                else
                {
                    AssetDatabase.ImportPackage(path, false);
                    Debug.Log("Imported: " + path);
                    DestroyImmediate(gameObject, true);
                }
            }
            #endif
        }
    }
}
