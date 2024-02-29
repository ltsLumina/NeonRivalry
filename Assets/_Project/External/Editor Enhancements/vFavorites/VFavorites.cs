#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static VFavorites.Libs.VUtils;
using static VFavorites.Libs.VGUI;


namespace VFavorites
{
    public static class VFavorites
    {
        static void OnGUI(object dummy)
        {
            void defaultGUI() => origBrowserOnGUIDelegate.GetMethodInfo().Invoke(wrappedBrowser, null);

            if (wrappedBrowser.GetType() != t_BrowserWindow) { defaultGUI(); return; }

            var totalRect = (Rect)t_BrowserWindow.GetField("m_TreeViewRect", maxBindingFlags).GetValue(wrappedBrowser);
            var footerRect = totalRect.SetHeightFromBottom(21);
            var bodyRect = totalRect.SetHeight(totalRect.height - footerRect.height);

            void deltaTime()
            {
                if (eType != EventType.Repaint) return;

                guiDeltaTime = EditorApplication.timeSinceStartup - lastGuiTime;
                lastGuiTime = EditorApplication.timeSinceStartup;
            }
            void dragndrop()
            {
                if (!dragging && eType == EventType.DragUpdated && bodyRect.IsHovered())
                {
                    var guid = DragAndDrop.objectReferences.FirstOrDefault()?.GetGuid() ?? "";
                    if (guid != "")
                        InitDrag(guid);
                }

                if (dragging)
                    DragAndDrop.visualMode = draggingGuidFromList ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Copy;


                if (dragging && eType == EventType.DragPerform)
                    AcceptDrag();

                if (dragging && !bodyRect.IsHovered())
                {
                    CancelDrag();
                    mouseLeftWhileDragging = true;
                }
            }
            void mouseDown()
            {
                if (!bodyRect.IsHovered()) { mousePressed = false; return; }

                if (e.mouseDown())
                {
                    mousePressed = true;
                    mouseDownYScrollLocal = e.mousePosition.y - 20 + scroll.y;
                    e.Use();
                }

                if (e.mouseUp())
                    mousePressed = false;

            }
            void gaps_()
            {
                while (VFavorites.gaps.Count < guids.Count + 1) VFavorites.gaps.Add(0);
                while (VFavorites.gaps.Count > guids.Count + 1) VFavorites.gaps.RemoveLast();

                if (eType == EventType.Layout)
                    for (int i = 0; i < VFavorites.gaps.Count; i++)
                        VFavorites.gaps[i] = Lerp(VFavorites.gaps[i], dragging && i == draggedGuidInsertAt ? rowHeight : 0, lerpSpeed);

            }
            void footer()
            {
                void background()
                {
                    var footerCol = EditorGUIUtility.isProSkin ? Greyscale(.25f) : Greyscale(.81f);
                    var lineCol = EditorGUIUtility.isProSkin ? Greyscale(.155f) : Greyscale(.7f);

                    footerRect.Draw(footerCol);
                    footerRect.SetHeight(1).Draw(lineCol);
                }
                void scaleSlider()
                {
                    var rect = footerRect.SetWidthFromRight(60).MoveX(-15).MoveY(2f).SetHeightFromMid(22);

                    var prev = rowScale;
                    var cur = GUI.HorizontalSlider(rect, rowScale, .6f, 1.4f);

                    cur = Mathf.RoundToInt(cur * 10) / 10f;

                    if (prev != cur)
                        rowScale = cur;
                }
                void mouseDown()
                {
                    if (!footerRect.IsHovered()) return;

                    if (e.mouseDown())
                        e.Use();
                }
                void undoButton()
                {
                    var rect = footerRect.SetWidth(30).MoveX(5).MoveY(.5f).SetHeightFromMid(14);

                    GUI.color = guidsForUndoStack.Any() ? Greyscale(1, .75f) : Greyscale(1, .4f);
                    GUI.Label(rect, EditorGUIUtility.IconContent("Animation.PrevKey@2x"));
                    GUI.color = Color.white;

                    if (rect.IsHovered() && e.mouseUp())
                    {
                        UndoGuidsChange();
                        e.Use();
                    }

                }

                background();
                scaleSlider();
                mouseDown();
                undoButton();
            }
            void background()
            {
                totalRect.Draw(backgroundCol);


                if (guids.Any() || dragging) return;

                var s = "Drag-and-drop\nassets or folders";

                GUI.enabled = false;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.Label(bodyRect, s);
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.enabled = true;
            }
            void rows()
            {
                void row(float y, string guid, float selectedColT = 0, bool isDragged = false)
                {
                    var rowRect = new Rect(0, y, totalRect.width, rowHeight);
                    var rowHovered = rowRect.IsHovered();

                    if (rowHovered && mousePressed && !dragging)
                        selectedColT = 1;



                    var path = guid.ToPath();
                    var name = path.GetName();
                    bool isFolder = AssetDatabase.IsValidFolder(path);
                    var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

                    void background()
                    {
                        var rowColor = Color.Lerp(evenRowCol, oddRowCol, Mathf.PingPong(y, rowHeight) / rowHeight);
                        rowColor = Color.Lerp(rowColor, selectedRowCol, selectedColT);

                        rowRect.Draw(rowColor);
                    }
                    void icon()
                    {
                        var tex = AssetPreview.GetAssetPreview(obj);

                        if (tex == null)
                            tex = AssetPreview.GetMiniThumbnail(obj);

                        if (tex == null && obj != null)
                            tex = AssetPreview.GetMiniTypeThumbnail(obj.GetType());

                        if (tex == null)
                            tex = Texture2D.grayTexture;

                        var rect = rowRect.SetWidth(20).SetWidthFromRight(iconHeight).SetHeightFromMid(iconHeight).MoveX(5).Resize(-2);

                        GUI.DrawTexture(rect, tex);

                    }
                    void name_()
                    {
                        var rect = rowRect.SetHeightFromMid(20).MoveX(29);
                        GUI.Label(rect, name);
                    }
                    void crossButton()
                    {
                        if (!rowRect.IsHovered() || dragging) return;

                        var rect = rowRect.SetWidthFromRight(16).MoveX(-11).SetHeightFromMid(16);

                        var crossHovered = rect.Resize(-3).IsHovered();

                        var cNorm = Greyscale(.45f);
                        var cHovered = EditorGUIUtility.isProSkin ? Greyscale(.8f) : cNorm;

                        GUI.color = crossHovered ? cHovered : cNorm;
                        GUI.Label(rect, EditorGUIUtility.IconContent("CrossIcon"));
                        GUI.color = Color.white;

                        if (crossHovered)
                            mousePressed = false;

                        if (crossHovered && e.mouseUp())
                        {
                            lerpingAcceptedDrag = false;
                            gaps[guids.IndexOf(guid)] = rowHeight;
                            RecordGuidsUndo();
                            guids.Remove(guid);
                            SaveGuids();
                            e.Use();
                        }
                    }
                    void click()
                    {
                        if (!rowHovered) return;

                        if (!e.mouseUp()) return;
                        e.Use();


                        if (dragging) return;
                        if (mouseDragDist > 2) return;


                        if (AssetDatabase.IsValidFolder(path))
                            OpenFolder(path);
                        else if (EditorApplication.timeSinceStartup - lastClickTime < .3f && (path.GetExtension() == ".cs" || path.GetExtension() == ".compute" || path.GetExtension() == ".shader" || path.GetExtension() == ".cginc" || path.GetExtension() == ".json"))
                            AssetDatabase.OpenAsset(guid.LoadGuid());
                        else
                        {
                            OpenFolder((path.ParentPath()));
                            Selection.activeObject = null;
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
                        }

                        lastClickTime = EditorApplication.timeSinceStartup;

                    }
                    void startDragFromList()
                    {
                        if (eType != EventType.MouseDrag) return;
                        if (!rowHovered) return;
                        if (!mousePressed) return;
                        if (dragging) return;
                        if (eType != EventType.MouseDrag || eType == EventType.DragUpdated) return;
                        if (mouseDragDist < 2) return;

                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.objectReferences = new[] { guid.LoadGuid() };
                        DragAndDrop.StartDrag(name);
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        e.Use();

                        InitDrag(guid, y + rowHeight / 2 - e.mousePosition.y);
                    }

                    background();
                    icon();
                    name_();
                    crossButton();
                    click();
                    startDragFromList();

                }


                GUILayout.BeginArea(bodyRect);
                scroll = EditorGUILayout.BeginScrollView(scroll, GUIStyle.none, GUIStyle.none);

                for (int i = 0; i < guids.Count; i++)
                {
                    GUILayout.Space(VFavorites.gaps[i]);
                    GUILayout.Space(rowHeight);

                    if (!lerpingAcceptedDrag || acceptedDragInd != i)
                        row(lastRect.y, guids[i]);
                    else if (eType == EventType.Repaint)
                        accepterDragTargetY = lastRect.y;
                }

                if (eType == EventType.Repaint)
                {
                    draggedGuidLocalY = (e.mousePosition.y - rowHeight / 2 + draggedGuidHoldOffset).Clamp(0, 12321);
                    draggedGuidInsertAt = Mathf.Clamp(Mathf.RoundToInt(draggedGuidLocalY / rowHeight), 0, guids.Count);

                }
                if (dragging)
                    row(draggedGuidLocalY, draggedGuid, 1, true);

                if (lerpingAcceptedDrag)
                {
                    row(accepterDragY, guids[acceptedDragInd], accepterDragSelectedColorT);

                    if (eType == EventType.Repaint)
                    {
                        accepterDragY = Lerp(accepterDragY, accepterDragTargetY, lerpSpeed);
                        accepterDragSelectedColorT = Lerp(accepterDragSelectedColorT, 0, lerpSpeed);
                    }

                    if (accepterDragY.Approx(accepterDragTargetY))
                        lerpingAcceptedDrag = false;
                }

                GUILayout.Space(rowHeight / 2);

                EditorGUILayout.EndScrollView();
                GUILayout.EndArea();


            }


            deltaTime();
            dragndrop();
            mouseDown();
            gaps_();

            if (eType == EventType.Repaint)
            {
                defaultGUI();
                background();
                rows();
                footer();
            }
            else
            {
                footer();
                rows();
                defaultGUI();
            }


            wrappedBrowser.Repaint();

        }

        static Color evenRowCol = EditorGUIUtility.isProSkin ? Greyscale(.24f) : Greyscale(.82f);
        static Color oddRowCol = backgroundCol;
        static Color selectedRowCol = EditorGUIUtility.isProSkin ? Greyscale(.29f) : Greyscale(.9f);

        static bool mousePressed;
        static bool mouseLeftWhileDragging;
        static float mouseDownYScrollLocal;
        static float mouseDragDist => (mouseDownYScrollLocal - e.mousePosition.y).Abs();
        static double lastClickTime;
        static double lastGuiTime;
        static double guiDeltaTime;
        static float lerpSpeed => .1f * (float)guiDeltaTime / .007f;

        static Vector2 scroll { get => Vector2.up * EditorPrefs.GetFloat(prefsScrollKey); set => EditorPrefs.SetFloat(prefsScrollKey, value.y); }
        static float rowScale { get => EditorPrefs.GetFloat(prefsScaleKey, 1); set => EditorPrefs.SetFloat(prefsScaleKey, value); }
        static float rowHeight => 40 * rowScale;
        static float iconHeight => 16 * Mathf.Min(1, rowScale);





        static void InitDrag(string guid, float holdOffset = 0)
        {
            dragging = true;
            lerpingAcceptedDrag = false;

            draggedGuidHoldOffset = holdOffset;
            draggedGuid = guid;

            if (draggingGuidFromList = guids.Contains(draggedGuid))
            {
                RecordGuidsUndo();
                draggingGuidFromListAtIndex = guids.IndexOf(draggedGuid);
                guids.Remove(draggedGuid);
                gaps[draggingGuidFromListAtIndex] = rowHeight;
            }
        }
        static void AcceptDrag()
        {
            DragAndDrop.AcceptDrag();
            e.Use();

            dragging = false;
            mousePressed = false;

            if (!draggingGuidFromList)
                RecordGuidsUndo();

            guids.AddAt(draggedGuidInsertAt, draggedGuid);
            SaveGuids();

            gaps[draggedGuidInsertAt] -= rowHeight;
            gaps.AddAt(draggedGuidInsertAt, 0);
            acceptedDragInd = draggedGuidInsertAt;
            accepterDragY = draggedGuidLocalY;
            accepterDragSelectedColorT = 1;
            lerpingAcceptedDrag = true;
        }
        static void CancelDrag()
        {
            dragging = false;
            mousePressed = false;


            if (!draggingGuidFromList) return;

            guids.AddAt(draggingGuidFromListAtIndex, draggedGuid);

            gaps[draggingGuidFromListAtIndex] -= rowHeight;
            acceptedDragInd = draggingGuidFromListAtIndex;
            accepterDragY = draggedGuidLocalY;
            accepterDragSelectedColorT = 1;
            lerpingAcceptedDrag = true;
            // guids.AddAt(draggingGuidFromListAtIndex, draggedGuid);
            // gaps[draggingGuidFromListAtIndex] = 0;
            // accepterDragSelectedColorT = 1;
            // lerpingAcceptedDrag = true;

        }

        static float draggedGuidHoldOffset;
        static bool draggingGuidFromList;
        static int draggingGuidFromListAtIndex;
        static bool dragging;
        static string draggedGuid;
        static float draggedGuidLocalY;
        static int draggedGuidInsertAt;
        static int acceptedDragInd;
        static float accepterDragSelectedColorT;
        static float accepterDragY;
        static float accepterDragTargetY;
        static bool lerpingAcceptedDrag;





        static void RecordGuidsUndo() => guidsForUndoStack.Push(guids.ToList());
        static void SaveGuids() => EditorPrefs.SetString(prefsGuidsKey, string.Join("-", guids));
        static void UndoGuidsChange()
        {
            dragging = false;
            lerpingAcceptedDrag = false;
            while (guidsForUndoStack.Any() && guids.SequenceEqual(guids = guidsForUndoStack.Pop())) { }
            SaveGuids();
        }

        static List<float> gaps = new List<float>();
        static List<string> guids = new List<string>();
        static Stack<List<string>> guidsForUndoStack = new Stack<List<string>>();

        static string prefsGuidsKey = "vFavorites-guids-" + GetProjectId();
        static string prefsScrollKey = "vFavorites-scroll-" + GetProjectId();
        static string prefsScaleKey = "vFavorites-scale-" + GetProjectId();






        static void UpdateOnGUIWrapping()
        {
            var curEvent = (Event)typeof(Event).GetField("s_Current", maxBindingFlags).GetValue(maxBindingFlags);

            var anyBrowserHovered = EditorWindow.mouseOverWindow?.GetType() == t_BrowserWindow;
            var wrappedBrowserHovered = EditorWindow.mouseOverWindow == wrappedBrowser;

            var shortcutPressed = curEvent.modifiers == EventModifiers.Alt;

            if (!shortcutPressed)
                mouseLeftWhileDragging = false;


            if (wrappedBrowser && (!shortcutPressed || !wrappedBrowserHovered) && !mouseLeftWhileDragging)
            {
                wrappedBrowser.Repaint();
                UnwrapBrowserOnGUI();
            }

            if (!wrappedBrowser && shortcutPressed && anyBrowserHovered)
            {
                WrapBrowserOnGUI(EditorWindow.mouseOverWindow);
                wrappedBrowser.Repaint();
            }

        }

        static void WrapBrowserOnGUI(EditorWindow browser)
        {
            guids.RemoveAll(r => !r.IsValidGuid());

            var hostView = fi_m_Parent.GetValue(browser);
            var newDelegate = mi_OnGUI.CreateDelegate(t_EditorWindowDelegate, hostView);

            origBrowserOnGUIDelegate = fi_m_OnGUI.GetValue(hostView) as System.Delegate;
            fi_m_OnGUI.SetValue(hostView, newDelegate);

            wrappedBrowser = browser;
        }
        static void UnwrapBrowserOnGUI()
        {
            lerpingAcceptedDrag = false;
            dragging = false;
            for (int i = 0; i < gaps.Count; i++)
                gaps[i] = 0;

            var hostView = fi_m_Parent.GetValue(wrappedBrowser);

            fi_m_OnGUI.SetValue(hostView, origBrowserOnGUIDelegate);

            wrappedBrowser = null;
        }

        static EditorWindow wrappedBrowser;
        static System.Delegate origBrowserOnGUIDelegate;

        static System.Type t_BrowserWindow = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        static System.Type t_HostView = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
        static System.Type t_EditorWindowDelegate = t_HostView.GetNestedType("EditorWindowDelegate", maxBindingFlags);
        static FieldInfo fi_m_Parent = typeof(EditorWindow).GetField("m_Parent", maxBindingFlags);
        static FieldInfo fi_m_OnGUI = t_HostView.GetField("m_OnGUI", maxBindingFlags);
        static MethodInfo mi_OnGUI = typeof(VFavorites).GetMethod("OnGUI", maxBindingFlags);




#if !DISABLED
        [InitializeOnLoadMethod]
#endif
        static void Init()
        {
            EditorApplication.update -= UpdateOnGUIWrapping;
            EditorApplication.update += UpdateOnGUIWrapping;

            guids = EditorPrefs.GetString(prefsGuidsKey).Split('-').Where(r => r != "").ToList();

        }


        public const string version = "1.0.8";

    }
}
#endif
