#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VFavorites.Libs
{
    public static class VUtils
    {

        #region IEnumerables

        public static T AddAt<T>(this List<T> l, int i, T r)
        {
            if (i < 0) i = 0;
            if (i >= l.Count)
                l.Add(r);
            else
                l.Insert(i, r);
            return r;
        }
        public static void RemoveLast<T>(this List<T> l)
        {
            if (!l.Any()) return;

            l.RemoveAt(l.Count - 1);
        }



        #endregion

        #region Reflection

        public const BindingFlags maxBindingFlags = (BindingFlags)62;

        public static List<System.Type> GetSubclasses(this System.Type t) => t.Assembly.GetTypes().Where(type => type.IsSubclassOf(t)).ToList();
        public static object GetDefaultValue(this FieldInfo f, params object[] constructorVars) => f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType, constructorVars));
        public static object GetDefaultValue(this FieldInfo f) => f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType));

        public static IEnumerable<FieldInfo> GetFieldsWithoutBase(this System.Type t) => t.GetFields().Where(r => !t.BaseType.GetFields().Any(rr => rr.Name == r.Name));
        public static IEnumerable<PropertyInfo> GetPropertiesWithoutBase(this System.Type t) => t.GetProperties().Where(r => !t.BaseType.GetProperties().Any(rr => rr.Name == r.Name));



        #endregion

        #region Math

        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
        public static bool CloseTo(this float f1, float f2, float distance) => f1.DistTo(f2) <= distance;
        public static float DistTo(this float f1, float f2) => Mathf.Abs(f1 - f2);
        public static float Dist(float f1, float f2) => Mathf.Abs(f1 - f2);
        public static float Avg(float f1, float f2) => (f1 + f2) / 2;
        public static float Abs(this float f) => Mathf.Abs(f);
        public static int Abs(this int f) => Mathf.Abs(f);
        public static float Sign(this float f) => Mathf.Sign(f);
        public static float Clamp(this float f, float f0, float f1) => Mathf.Clamp(f, f0, f1);

        public static float Lerp(float f1, float f2, float t) => Mathf.LerpUnclamped(f1, f2, t);
        public static Vector3 Lerp(Vector3 f1, Vector3 f2, float t) => Vector3.LerpUnclamped(f1, f2, t);

        public static bool IsOdd(this int i) => i % 2 == 1;
        public static bool IsEven(this int i) => i % 2 == 0;



        #endregion

        #region Rects

        public static Rect Resize(this Rect rect, float px) { rect.x += px; rect.y += px; rect.width -= px * 2; rect.height -= px * 2; return rect; }

        public static Rect SetPos(this Rect rect, Vector2 v) => rect.SetPos(v.x, v.y);
        public static Rect SetPos(this Rect rect, float x, float y) { rect.x = x; rect.y = y; return rect; }

        public static Rect SetX(this Rect rect, float x) => rect.SetPos(x, rect.y);
        public static Rect SetY(this Rect rect, float y) => rect.SetPos(rect.x, y);

        public static Rect SetMidPos(this Rect r, Vector2 v) => r.SetPos(v).MoveX(-r.width / 2).MoveY(-r.height / 2);

        public static Rect Move(this Rect rect, Vector2 v) { rect.position += v; return rect; }
        public static Rect Move(this Rect rect, float x, float y) { rect.x += x; rect.y += y; return rect; }
        public static Rect MoveX(this Rect rect, float px) { rect.x += px; return rect; }
        public static Rect MoveY(this Rect rect, float px) { rect.y += px; return rect; }

        public static Rect SetWidth(this Rect rect, float f) { rect.width = f; return rect; }
        public static Rect SetWidthFromMid(this Rect rect, float px) { rect.x += rect.width / 2; rect.width = px; rect.x -= rect.width / 2; return rect; }
        public static Rect SetWidthFromRight(this Rect rect, float px) { rect.x += rect.width; rect.width = px; rect.x -= rect.width; return rect; }

        public static Rect SetHeight(this Rect rect, float f) { rect.height = f; return rect; }
        public static Rect SetHeightFromMid(this Rect rect, float px) { rect.y += rect.height / 2; rect.height = px; rect.y -= rect.height / 2; return rect; }
        public static Rect SetHeightFromBottom(this Rect rect, float px) { rect.y += rect.height; rect.height = px; rect.y -= rect.height; return rect; }

        public static Rect AddWidth(this Rect rect, float f) => rect.SetWidth(rect.width + f);
        public static Rect AddWidthFromMid(this Rect rect, float f) => rect.SetWidthFromMid(rect.width + f);
        public static Rect AddWidthFromRight(this Rect rect, float f) => rect.SetWidthFromRight(rect.width + f);

        public static Rect AddHeight(this Rect rect, float f) => rect.SetHeight(rect.height + f);
        public static Rect AddHeightFromMid(this Rect rect, float f) => rect.SetHeightFromMid(rect.height + f);
        public static Rect AddHeightFromBottom(this Rect rect, float f) => rect.SetHeightFromBottom(rect.height + f);

        public static Rect SetSize(this Rect rect, Vector2 v) => rect.SetWidth(v.x).SetHeight(v.y);
        public static Rect SetSize(this Rect rect, float w, float h) => rect.SetWidth(w).SetHeight(h);
        public static Rect SetSize(this Rect rect, float f) { rect.height = rect.width = f; return rect; }

        public static Rect SetSizeFromMid(this Rect r, Vector2 v) => r.Move(r.size / 2).SetSize(v).Move(-v / 2);
        public static Rect SetSizeFromMid(this Rect r, float x, float y) => r.SetSizeFromMid(new Vector2(x, y));
        public static Rect SetSizeFromMid(this Rect r, float f) => r.SetSizeFromMid(new Vector2(f, f));




        #endregion


        #region Paths

        public static string ParentPath(this string path) => path.Substring(0, path.LastIndexOf('/'));
        public static bool HasParentPath(this string path) => path.Contains('/') && path.ParentPath() != "";

        public static string ToGlobalPath(this string path) => Application.dataPath + "/" + path.Substring(0, path.Length - 1);
        public static string ToLocalPath(this string path) => "Assets" + path.Replace(Application.dataPath, "");



        public static string CombinePath(this string p, string p2) => Path.Combine(p, p2);

        public static bool IsSubpathOf(this string path, string of) => path.StartsWith(of + "/") || of == "";

        public static string EnsureDirExists(this string dirOrPath)
        {
            var dir = dirOrPath.Contains('.') ? dirOrPath.Substring(0, dirOrPath.LastIndexOf('/')) : dirOrPath;

            if (dir.Contains('.')) dir = dir.Substring(0, dir.LastIndexOf('/'));
            if (dir.HasParentPath() && !Directory.Exists(dir.ParentPath())) EnsureDirExists(dir.ParentPath());
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dirOrPath;
        }
#if UNITY_EDITOR
        public static string EnsurePathIsUnique(this string path) => AssetDatabase.GenerateUniqueAssetPath(path);
#endif
        public static string ClearDir(this string dir)
        {
            if (!Directory.Exists(dir)) return dir;

            var diri = new DirectoryInfo(dir);
            foreach (var r in diri.EnumerateFiles()) r.Delete();
            foreach (var r in diri.EnumerateDirectories()) r.Delete(true);

            return dir;
        }
#if UNITY_EDITOR
        public static void EnsureDirExistsAndRevealInFinder(string dir)
        {
            EnsureDirExists(dir);
            UnityEditor.EditorUtility.OpenWithDefaultApp(dir);
        }
#endif



        #endregion

        #region AssetDatabase
#if UNITY_EDITOR

        public static TextureImporter GetImporter(this Texture2D t) => (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));
        public static AssetImporter GetImporter(this Object t) => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));


        public static string ToPath(this string guid) => AssetDatabase.GUIDToAssetPath(guid);
        public static List<string> ToPaths(this IEnumerable<string> guids) => guids.Select(r => r.ToPath()).ToList();

        public static string GetName(this string path, bool withExtension = false) => withExtension ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
        public static string GetExtension(this string path) => Path.GetExtension(path);


        public static string ToGuid(this string pathInProject) => AssetDatabase.AssetPathToGUID(pathInProject);
        public static List<string> ToGuids(this IEnumerable<string> pathsInProject) => pathsInProject.Select(r => r.ToGuid()).ToList();

        public static string GetPath(this Object o) => AssetDatabase.GetAssetPath(o);
        public static string GetGuid(this Object o) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));

        public static string GetScriptPath(string scriptName) => AssetDatabase.FindAssets("t: script " + scriptName, null).FirstOrDefault()?.ToPath() ?? "scirpt not found";


        public static bool IsValidGuid(this string guid) => AssetDatabase.AssetPathToGUID(AssetDatabase.GUIDToAssetPath(guid)) != "";

        public static Object LoadGuid(this string guid) => AssetDatabase.LoadAssetAtPath(guid.ToPath(), typeof(Object));

        public static List<string> FindAllAssetsOfType_guids(System.Type type) => AssetDatabase.FindAssets("t:" + type.Name).ToList();
        public static List<string> FindAllAssetsOfType_guids(System.Type type, string path) => AssetDatabase.FindAssets("t:" + type.Name, new[] { path }).ToList();
        public static List<T> FindAllAssetsOfType<T>() where T : Object => FindAllAssetsOfType_guids(typeof(T)).Select(r => (T)r.LoadGuid()).ToList();
        public static List<T> FindAllAssetsOfType<T>(string path) where T : Object => FindAllAssetsOfType_guids(typeof(T), path).Select(r => (T)r.LoadGuid()).ToList();

        public static T Reimport<T>(this T t) where T : Object { AssetDatabase.ImportAsset(t.GetPath(), ImportAssetOptions.ForceUpdate); return t; }

#endif


        #endregion

        #region Editor
#if UNITY_EDITOR

        public static void ToggleDefineDisabledInScript(System.Type scriptType)
        {
            var path = GetScriptPath(scriptType.Name);

            var lines = File.ReadAllLines(path);
            if (lines.First().StartsWith("#define DISABLED"))
                File.WriteAllLines(path, lines.Skip(1));
            else
                File.WriteAllLines(path, lines.Prepend("#define DISABLED    // this line was added by VUtils.ToggleDefineDisabledInScript"));

            AssetDatabase.ImportAsset(path);
        }
        public static bool ScriptHasDefineDisabled(System.Type scriptType) => File.ReadLines(GetScriptPath(scriptType.Name)).First().StartsWith("#define DISABLED");

        public static int GetProjectId() => Application.dataPath.GetHashCode();

        public static void PingObject(Object o, bool select = false, bool focusProjectWindow = true)
        {
            if (select)
            {
                Selection.activeObject = null;
                Selection.activeObject = o;
            }
            if (focusProjectWindow) EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(o);
        }
        public static void PingObject(string guid, bool select = false, bool focusProjectWindow = true) => PingObject(AssetDatabase.LoadAssetAtPath<Object>(guid.ToPath()));


        public static void OpenFolder(string path)
        {
            var folder = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

            var t = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            var w = (EditorWindow)t.GetField("s_LastInteractedProjectBrowser").GetValue(null);

            var m_ListAreaState = t.GetField("m_ListAreaState", maxBindingFlags).GetValue(w);

            m_ListAreaState.GetType().GetField("m_SelectedInstanceIDs").SetValue(m_ListAreaState, new List<int> { folder.GetInstanceID() });

            t.GetMethod("OpenSelectedFolders", maxBindingFlags).Invoke(null, null);
        }

        public static void Dirty(this Object o) => UnityEditor.EditorUtility.SetDirty(o);
        public static void RecordUndo(this Object so) => Undo.RecordObject(so, "");

#endif


        #endregion

    }

    public static class VGUI
    {

        #region Colors

        public static Color Greyscale(float s, float a = 1) { var c = Color.white; c *= s; c.a = a; return c; }

        public static Color accentColor => Color.white * .81f;
        public static Color dividerColor => EditorGUIUtility.isProSkin ? Color.white * .42f : Color.white * .72f;
        public static Color treeViewBG => EditorGUIUtility.isProSkin ? new Color(.22f, .22f, .22f, .56f) : new Color(.7f, .7f, .7f, .1f);
        public static Color greyedOutColor = Color.white * .72f;

        public static Color objFieldCol = Color.black + Color.white * .16f;
        public static Color objFieldClearCrossCol = Color.white * .5f;
        public static Color objPieckerCol = Color.black + Color.white * .21f;

        public static Color dragndropTintHovered = Greyscale(.8f, .07f);

        public static Color footerCol = Greyscale(.26f);

        // public static Color pressedCol = new Color(.4f, .7f, 1f, .4f) * 1.65f;
        public static Color pressedCol = new Color(.35f, .57f, 1f, 1f) * 1.25f;// * 1.65f;
        public static Color pressedButtonCol => EditorGUIUtility.isProSkin ? new Color(.48f, .76f, 1f, 1f) * 1.4f : new Color(.48f, .7f, 1f, 1f) * 1.2f;

        public static Color hoveredCol = Greyscale(.5f, .15f);

        public static Color rowCol = Greyscale(.28f, .28f);

        public static Color backgroundCol => EditorGUIUtility.isProSkin ? Greyscale(.22f) : Greyscale(.78f);
        public static Color backgroundLightCol = Greyscale(.255f);

        public static Color selectedCol = new Color(.15f, .4f, 1f, .7f);// * 1.65f;

        public static Color buttonCol = Greyscale(.33f);




        #endregion

        #region Shortcuts

        public static Rect lastRect => GUILayoutUtility.GetLastRect();

        public static float LabelWidth(this string s) => GUI.skin.label.CalcSize(new GUIContent(s)).x;
        public static float GetCurrentInspectorWidth() => (float)typeof(EditorGUIUtility).GetProperty("contextWidth", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);


        public static Event e => Event.current;
        public static bool ePresent => Event.current != null;
        public static UnityEngine.EventType eType => ePresent ? e.type : UnityEngine.EventType.Ignore;
        public static bool mouseDown(this Event e) => eType == EventType.MouseDown && e.button == 0;
        public static bool mouseUp(this Event e) => eType == EventType.MouseUp && e.button == 0;
        public static bool keyDown(this Event e) => eType == EventType.KeyDown;
        public static bool keyUp(this Event e) => eType == EventType.KeyUp;


        public static bool holdingAlt => ePresent && (e.alt);
        public static bool holdingCmd => ePresent && (e.command || e.control);
        public static bool holdingShift => ePresent && (e.shift);




        #endregion

        #region Rect Drawing

        public static void DrawRect() => EditorGUI.DrawRect(lastRect, Color.black);
        public static void DrawRect(Rect r) => EditorGUI.DrawRect(r, Color.black);
        public static Rect Draw(this Rect r) { EditorGUI.DrawRect(r, Color.black); return r; }
        public static Rect Draw(this Rect r, Color c) { EditorGUI.DrawRect(r, c); return r; }



        public static bool IsHovered(this Rect r) => ePresent && r.Contains(e.mousePosition);

        #endregion

    }
}
#endif