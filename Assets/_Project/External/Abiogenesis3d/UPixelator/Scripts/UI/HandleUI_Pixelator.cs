using UnityEngine;
using UnityEngine.UI;

namespace Abiogenesis3d.UPixelator_Demo
{
    public class HandleUI_Pixelator : MonoBehaviour
    {
        [HideInInspector]
        public UPixelator uPixelator;
        public Toggle uPixelatorEnabled;
        public Slider uPixelatorPixelMultiplier;
        public Text uPixelatorPixelMultiplierNumber;
        public Toggle uPixelatorSnap;
        public Toggle uPixelatorStabilize;

        public GameObject uPixelatorNoteSnapDisabled;
        public GameObject uPixelatorNoteStabilizeDisabled;

        public KeyCode toggleUPixelatorKey = KeyCode.Z;
        void Start()
        {
            uPixelator = FindObjectOfType<UPixelator>(true);
            if (!uPixelator) return;

            uPixelatorEnabled.isOn = uPixelator.gameObject.activeInHierarchy;
            uPixelatorPixelMultiplier.value = uPixelator.pixelMultiplier;

            uPixelatorEnabled.onValueChanged.AddListener((value) => DoUpdate());
            uPixelatorPixelMultiplier.onValueChanged.AddListener((value) => DoUpdate());
            uPixelatorSnap.onValueChanged.AddListener((value) => DoUpdate());
            uPixelatorStabilize.onValueChanged.AddListener((value) => DoUpdate());
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleUPixelatorKey)) uPixelatorEnabled.isOn = !uPixelatorEnabled.isOn;
        }

        void DoUpdate()
        {
            if (!uPixelator) return;

            uPixelator.gameObject.SetActive(uPixelatorEnabled.isOn);
            if (!uPixelatorSnap.isOn) uPixelatorStabilize.isOn = true;

            foreach (var c in uPixelator.cameraInfos)
            {
                if (c.cam == uPixelator.uPixelatorCam) continue;
                c.snap = uPixelatorSnap.isOn;
                c.stabilize = uPixelatorStabilize.isOn;
            }
            int value = (int)uPixelatorPixelMultiplier.value;
            uPixelator.pixelMultiplier = value;
            uPixelatorPixelMultiplierNumber.text = value + "";

            // disable state
            uPixelatorPixelMultiplier.transform.parent.SetSiblingIndex(uPixelatorEnabled.isOn ? 100: 0);
            uPixelatorSnap.transform.parent.SetSiblingIndex(uPixelatorEnabled.isOn ? 100: 0);
            var snap = uPixelator.cameraInfos[0]?.snap ?? false;
            var stabilize = uPixelator.cameraInfos[0]?.stabilize ?? false;
            uPixelatorStabilize.transform.parent.SetSiblingIndex(uPixelatorEnabled.isOn && snap ? 100: 0);

            uPixelatorNoteSnapDisabled.SetActive(uPixelatorEnabled.isOn && !snap);
            uPixelatorNoteStabilizeDisabled.SetActive(uPixelatorEnabled.isOn && !stabilize);

            uPixelator.Refresh();
        }
    }
}
