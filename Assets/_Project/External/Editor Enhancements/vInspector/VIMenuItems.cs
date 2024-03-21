#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static VInspector.Libs.VUtils;

namespace VInspector
{
    class VIMenuItems
    {
        public static bool scriptInspectorEnabled { get => !ScriptHasDefineDisabled(typeof(VIScriptComponentEditor)); set => SetDefineDisabledInScript(typeof(VIScriptComponentEditor), !value); }
        public static bool soInspectorEnabled { get => !ScriptHasDefineDisabled(typeof(VIScriptableObjectEditor)); set => SetDefineDisabledInScript(typeof(VIScriptableObjectEditor), !value); }
        public static bool staticInspectorEnabled { get => !ScriptHasDefineDisabled(typeof(VIScriptAssetEditor)); set => SetDefineDisabledInScript(typeof(VIScriptAssetEditor), !value); }
        public static bool resettableVariablesEnabled { get => !ScriptHasDefineDisabled(typeof(VIResettablePropDrawer)); set => SetDefineDisabledInScript(typeof(VIResettablePropDrawer), !value); }
        public static bool cleanerHeaderEnabled { get => EditorPrefs.GetBool("vInspector-hideScriptField", true); set { SetDefineDisabledInScript(typeof(VICleanerHeader), !value); EditorPrefs.SetBool("vInspector-hideScriptField", value); SetDefineDisabledInScript(typeof(VIResettablePropDrawer), !value); } }

        public static bool vInspectorDisabled { get => !scriptInspectorEnabled && !soInspectorEnabled && !staticInspectorEnabled && !resettableVariablesEnabled && !cleanerHeaderEnabled; set => scriptInspectorEnabled = soInspectorEnabled = staticInspectorEnabled = resettableVariablesEnabled = cleanerHeaderEnabled = !value; }


        const string menuDir = "Tools/vInspector/";

        const string cleanerHeader = menuDir + "Cleaner header";
        const string resettableVariables = menuDir + "Resettable variables";
        const string staticInspector = menuDir + "Static inspector";

        const string disable = menuDir + "Disable vInspector";



        [MenuItem(resettableVariables, false, 1)] static void dadsaadsdadsas() => resettableVariablesEnabled = !resettableVariablesEnabled;
        [MenuItem(resettableVariables, true, 1)] static bool dadsadadsdasadsas() { UnityEditor.Menu.SetChecked(resettableVariables, resettableVariablesEnabled); return true; }

        [MenuItem(cleanerHeader, false, 2)] static void dadsadadsas() { cleanerHeaderEnabled = !cleanerHeaderEnabled; UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); }
        [MenuItem(cleanerHeader, true, 2)] static bool dadsaddasadsas() { UnityEditor.Menu.SetChecked(cleanerHeader, cleanerHeaderEnabled); return true; }

        [MenuItem(staticInspector, false, 3)] static void dadsaadsdadsdasas() => staticInspectorEnabled = !staticInspectorEnabled;
        [MenuItem(staticInspector, true, 3)] static bool dadsadadsddsaasadsas() { UnityEditor.Menu.SetChecked(staticInspector, staticInspectorEnabled); return true; }


        [MenuItem(menuDir + "Join our Discord", false, 101)]
        static void dadsas() => Application.OpenURL("https://discord.gg/4dG9KsbspG");

        [MenuItem(menuDir + "Get the rest of our Editor Enhancers with a discount", false, 102)]
        static void dadsadsas() => Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/editor-enhancers-bundle-251318?aid=1100lGLBn&pubref=menu");


        [MenuItem(disable, false, 1001)] static void dadsaaadsdsdadsdasas() => vInspectorDisabled = !vInspectorDisabled;
        [MenuItem(disable, true, 1001)] static bool dadsadaadsdsddsaasadsas() { UnityEditor.Menu.SetChecked(disable, vInspectorDisabled); return true; }



    }
}
#endif