#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static VFavorites.Libs.VUtils;

namespace VFavorites
{
    class VFavoritesMenuItems
    {
        const string menuDir = "Tools/vFavorites/";


        [MenuItem(menuDir + "Join our Discord", false, 101)]
        static void dadsas() => Application.OpenURL("https://discord.gg/4dG9KsbspG");

        [MenuItem(menuDir + "Get the rest of our Editor Ehnancers with a discount", false, 102)]
        static void dadsadsas() => Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/editor-enhancers-bundle-251318?aid=1100lGLBn&pubref=menu");

        [MenuItem(menuDir + "Disable vFavorites", false, 1001)]
        static void das() => ToggleDefineDisabledInScript(typeof(VFavorites));
        [MenuItem(menuDir + "Disable vFavorites", true, 1001)]
        static bool dassadc() { UnityEditor.Menu.SetChecked(menuDir + "Disable vFavorites", ScriptHasDefineDisabled(typeof(VFavorites))); return true; }

    }
}
#endif