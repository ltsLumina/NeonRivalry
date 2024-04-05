using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class GlobalSetFont : MonoBehaviour
    {
        public bool enableThisToApply;

        public bool applyToChildrenOnly = true;

        public bool applyFont = true;
        public Font font;

        public bool applySize = true;
        public int size = 16;

        void Update()
        {
            if (enableThisToApply)
            {
                enableThisToApply = false;

                List<Text> texts = new List<Text>();

                if (applyToChildrenOnly) texts.AddRange(GetComponentsInChildren<Text>());
                else texts.AddRange(FindObjectsOfType<Text>());

                foreach (var text in texts)
                {
                    if (applyFont) text.font = font;
                    if (applySize) text.fontSize = size;
                }
            }
        }
    }
}
