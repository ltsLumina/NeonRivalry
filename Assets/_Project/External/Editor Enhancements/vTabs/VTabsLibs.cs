
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace VTabs.Libs
{

    public static class VUtils
    {
        #region Text

        public static string FormatDistance(float meters)
        {
            int m = (int)meters;

            if (m < 1000) return m + " m";
            else return (m / 1000) + "." + (m / 100) % 10 + " km";
        }
        public static string FormatLong(long l) => System.String.Format("{0:n0}", l);
        public static string FormatInt(int l) => FormatLong((long)l);
        public static string FormatSize(long bytes, bool sizeUnknownIfNotMoreThanZero = false)
        {
            if (sizeUnknownIfNotMoreThanZero && bytes == 0) return "Size unknown";

            var ss = new[] { "B", "KB", "MB", "GB", "TB" };
            var bprev = bytes;
            int i = 0;
            while (bytes >= 1024 && i++ < ss.Length - 1) bytes = (bprev = bytes) / 1024;

            if (bytes < 0) return "? B";
            if (i < 3) return string.Format("{0:0.#} ", bytes) + ss[i];
            return string.Format("{0:0.##} ", bytes) + ss[i];
        }
        public static string FormatTime(long ms, bool includeMs = false)
        {
            System.TimeSpan t = System.TimeSpan.FromMilliseconds(ms);
            var s = "";
            if (t.Hours != 0) s += " " + t.Hours + " hour" + CountSuffix(t.Hours);
            if (t.Minutes != 0) s += " " + t.Minutes + " minute" + CountSuffix(t.Minutes);
            if (t.Seconds != 0) s += " " + t.Seconds + " second" + CountSuffix(t.Seconds);
            if (t.Milliseconds != 0 && includeMs) s += " " + t.Milliseconds + " millisecond" + CountSuffix(t.Milliseconds);

            if (s == "")
                if (includeMs) s = "0 milliseconds";
                else s = "0 seconds";

            return s.Trim();
        }
        static string CountSuffix(long c) => c % 10 != 1 ? "s" : "";
        public static string Remove(this string s, string toRemove)
        {
            if (toRemove == "") return s;
            return s.Replace(toRemove, "");
        }

        public static string Decamelcase(this string str) => Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        // public static string PrettifyVarName(this string str) => string.Join(' ', str.Decamelcase().Split(' ').Select(r => new[] { "", "and", "or", "with", "without", "by", "from" }.Contains(r.ToLower()) ? r.ToLower() : r.Substring(0, 1).ToUpper() + r.Substring(1))).Trim(' ');
        public static string PrettifyVarName(this string str, bool lowercaseFollowingWords = true) => string.Join(" ", str.Decamelcase().Split(' ').Select(r => new[] { "", "and", "or", "with", "without", "by", "from" }.Contains(r.ToLower()) || (lowercaseFollowingWords && !str.Trim().StartsWith(r)) ? r.ToLower() : r.Substring(0, 1).ToUpper() + r.Substring(1))).Trim(' ');
        // public static string RemoveParenthesized(this string s) => Regex.Replace(s, @"/\().*?\) /g", "").Trim();



        #endregion

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

        #region Linq

        public static T NextTo<T>(this IEnumerable<T> e, T to) => e.SkipWhile(r => !r.Equals(to)).Skip(1).FirstOrDefault();
        public static T PreviousTo<T>(this IEnumerable<T> e, T to) => e.Reverse().SkipWhile(r => !r.Equals(to)).Skip(1).FirstOrDefault();

        public static Dictionary<T1, T2> MergeDictionaries<T1, T2>(IEnumerable<Dictionary<T1, T2>> dicts)
        {
            if (dicts.Count() == 0) return null;
            if (dicts.Count() == 1) return dicts.First();

            var mergedDict = new Dictionary<T1, T2>(dicts.First());

            foreach (var dict in dicts.Skip(1))
                foreach (var r in dict)
                    if (!mergedDict.ContainsKey(r.Key))
                        mergedDict.Add(r.Key, r.Value);

            return mergedDict;
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
        public static string ToLocalPath(this string path) => "Assets" + path.Remove(Application.dataPath);



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

            var m_ListAreaState = t.GetField("m_ListAreaState", (BindingFlags)62).GetValue(w);

            m_ListAreaState.GetType().GetField("m_SelectedInstanceIDs").SetValue(m_ListAreaState, new List<int> { folder.GetInstanceID() });

            t.GetMethod("OpenSelectedFolders", (BindingFlags)62).Invoke(null, null);
        }

        public static void Dirty(this Object o) => UnityEditor.EditorUtility.SetDirty(o);
        public static void RecordUndo(this Object so) => Undo.RecordObject(so, "");

#endif


        #endregion

    }

}
#endif