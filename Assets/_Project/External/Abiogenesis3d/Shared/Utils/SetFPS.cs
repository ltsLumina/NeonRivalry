using UnityEngine;

namespace Abiogenesis3d
{
    public class SetFPS : MonoBehaviour
    {
        public int targetFrameRate = 60;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
