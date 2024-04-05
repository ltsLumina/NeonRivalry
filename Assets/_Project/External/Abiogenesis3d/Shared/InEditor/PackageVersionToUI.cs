using UnityEngine;
using UnityEngine.UI;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class PackageVersionToUI : MonoBehaviour
    {
        public Text text;
        public string package;
        public AutoExportUnityPackages exporter;

        void Update()
        {
            if (!text) return;
            if (!exporter) return;

            foreach (var p in exporter.packages)
            {
                if (p.name == package)
                {
                    text.text = "v" + p.version;
                    break;
                }
            }
        }
    }
}