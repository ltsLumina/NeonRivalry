using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Abiogenesis3d
{
    [Serializable]
    public class PackageInfo
    {
        public bool export = true;
        public string name;
        public string version;
        public string[] paths;
    }

    [ExecuteInEditMode]
    public class AutoExportUnityPackages : MonoBehaviour
    {
        [Header("Click")]
        public bool export;

        public string[] commonPaths;
        public PackageInfo[] packages;

        #if UNITY_EDITOR

        void OnValidate()
        {
            if (export)
            {
                export = false;
                var destination = EditorUtility.OpenFolderPanel("Select destination", "", "");
                if (destination != "") Export(destination);
            }
        }

        void Export(string destination)
        {
            foreach (var package in packages)
            {
                if (!package.export) continue;

                var path = destination + "/"  + package.name + "_v" + package.version + ".unitypackage";
                Debug.Log("Exporting: " + path);
                AssetDatabase.ExportPackage(package.paths.Concat(commonPaths).ToArray(), path, ExportPackageOptions.Recurse);
            }
            AssetDatabase.Refresh();
        }
        #endif
    }
}
