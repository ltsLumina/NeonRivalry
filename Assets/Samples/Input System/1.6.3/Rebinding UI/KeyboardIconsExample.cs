using System;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class KeyboardIconsExample : MonoBehaviour
    {
        public KeyboardIcons keyboard;

        protected void OnEnable()
        {
            var rebindUIComponents = transform.GetComponentsInChildren<RebindActionUI>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString, string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard"))
                icon = keyboard.GetSprite(controlPath);

            var textComponent = component.bindingText;
            var imageGO = textComponent.transform.parent.Find("ActionBindingIcon");
            var imageComponent = imageGO.GetComponent<Image>();

            if (icon != null)
            {
                textComponent.gameObject.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.gameObject.SetActive(true);
            }
            else
            {
                textComponent.gameObject.SetActive(true);
                imageComponent.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public struct KeyboardIcons
        {
            public Sprite keyA;
            public Sprite keyB;
            public Sprite keyC;
            public Sprite keyD;
            public Sprite keyE;
            public Sprite keyF;
            public Sprite keyG;
            public Sprite keyH;
            public Sprite keyI;
            public Sprite keyJ;
            public Sprite keyK;
            public Sprite keyL;
            public Sprite keyM;
            public Sprite keyN;
            public Sprite keyO;
            public Sprite keyP;
            public Sprite keyQ;
            public Sprite keyR;
            public Sprite keyS;
            public Sprite keyT;
            public Sprite keyU;
            public Sprite keyV;
            public Sprite keyW;
            public Sprite keyX;
            public Sprite keyY;
            public Sprite keyZ;
            public Sprite keySpace;
            public Sprite keyLeftShift;
            public Sprite keyRightShift;
            public Sprite keyUpArrow;
            public Sprite keyDownArrow;
            public Sprite keyLeftArrow;
            public Sprite keyRightArrow;

            public Sprite GetSprite(string controlPath) => controlPath switch
            { "a"          => keyA,
              "b"          => keyB,
              "c"          => keyC,
              "d"          => keyD,
              "e"          => keyE,
              "f"          => keyF,
              "g"          => keyG,
              "h"          => keyH,
              "i"          => keyI,
              "j"          => keyJ,
              "k"          => keyK,
              "l"          => keyL,
              "m"          => keyM,
              "n"          => keyN,
              "o"          => keyO,
              "p"          => keyP,
              "q"          => keyQ,
              "r"          => keyR,
              "s"          => keyS,
              "t"          => keyT,
              "u"          => keyU,
              "v"          => keyV,
              "w"          => keyW,
              "x"          => keyX,
              "y"          => keyY,
              "z"          => keyZ,
              "space"      => keySpace,
              "leftShift"  => keyLeftShift,
              "rightShift" => keyRightShift,
              "upArrow"    => keyUpArrow,
              "downArrow"  => keyDownArrow,
              "leftArrow"  => keyLeftArrow,
              "rightArrow" => keyRightArrow,
              _            => null };
        }
    }
}