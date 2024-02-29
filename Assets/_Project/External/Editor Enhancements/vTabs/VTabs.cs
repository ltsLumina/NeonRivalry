#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static VTabs.Libs.VUtils;



namespace VTabs
{
    public static class VTabs
    {
        static void Update()
        {
            if (!EditorWindow.mouseOverWindow) return;

            var curEvent = (Event)typeof(Event).GetField("s_Current", maxBindingFlags).GetValue(maxBindingFlags);

            void dragndrop_()
            {
                if (!VTabsMenuItems.dragndropEnabled) return;
                if (curEvent.type != EventType.DragUpdated && curEvent.type != EventType.DragPerform) return;


                var dropAreaHeight = 40;

                if (EditorWindow.mouseOverWindow.GetType() == hierarchyType && DragAndDrop.objectReferences.Any(r => r is GameObject))
                    dropAreaHeight = 20;

                if (!EditorWindow.mouseOverWindow.rootVisualElement.contentRect.SetHeight(dropAreaHeight).Contains(curEvent.mousePosition)) return;


                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (curEvent.type != EventType.DragPerform) return;

                DragAndDrop.AcceptDrag();

                new TabDescription(DragAndDrop.objectReferences.First()).CreateWindow(GetDockArea(EditorWindow.mouseOverWindow), false);

            }
            void scrolling()
            {
                if (keyPressed) return;
                if (curEvent.delta == Vector2.zero) return;
                if (curEvent.type == EventType.MouseMove) return;
                if (curEvent.type == EventType.MouseDrag) return;
                if (curEvent.type != EventType.ScrollWheel && delayedMousePosition_screenSpace != EditorGUIUtility.GUIToScreenPoint(curEvent.mousePosition)) return; // uncaptured mouse move/drag check
                if (curEvent.type != EventType.ScrollWheel && VTabsMenuItems.fixPhantomScrollingEnabled && curEvent.delta.x == (int)curEvent.delta.x) return; // osx uncaptured mouse move/drag in sceneview ang gameview workaround


                void switchTab(bool right)
                {
                    if (!VTabsMenuItems.shiftscrollSwitchTabEnabled) return;
                    if (!EditorWindow.mouseOverWindow.docked) return;
                    if (EditorWindow.mouseOverWindow.maximized) return;

                    var tabs = GetTabList();

                    var i0 = tabs.IndexOf(EditorWindow.mouseOverWindow);
                    var i1 = Mathf.Clamp(i0 + (right ? 1 : -1), 0, tabs.Count - 1);

                    tabs[i1].Focus();

                    UpdateTabHeader(tabs[i1]);

                    EnsureFocusedTabVisibleOnScroller();
                }
                void moveTab(bool right)
                {
                    if (!VTabsMenuItems.shiftscrollMoveTabEnabled) return;

                    var tabs = GetTabList();

                    var i0 = tabs.IndexOf(EditorWindow.mouseOverWindow);
                    var i1 = Mathf.Clamp(i0 + (right ? 1 : -1), 0, tabs.Count - 1);

                    var r = tabs[i0];
                    tabs[i0] = tabs[i1];
                    tabs[i1] = r;
                    tabs[i1].Focus();

                    EnsureFocusedTabVisibleOnScroller();
                }

                void shiftscroll()
                {
                    if (!curEvent.shift) return;


                    var dScroll = Application.platform == RuntimePlatform.OSXEditor ? curEvent.delta.x // osx sends delta.y as delta.x when shift is pressed
                                                                                    : curEvent.delta.x - curEvent.delta.y; // some software on windows may do that too

                    if (dScroll != 0)
                        if (curEvent.control || curEvent.command)
                        {
                            if (VTabsMenuItems.shiftscrollMoveTabEnabled)
                                moveTab(dScroll > 0);
                        }
                        else
                        {
                            if (VTabsMenuItems.shiftscrollSwitchTabEnabled)
                                switchTab(dScroll > 0);
                        }

                }
                void sidescroll()
                {
                    if (curEvent.shift) return;
                    if (curEvent.delta.x == 0) return;
                    if (curEvent.delta.x.Abs() < curEvent.delta.y.Abs()) return;


                    if ((int)(sidesscrollPosition * VTabsMenuItems.sidescrollSensitivity / 2) != (int)((sidesscrollPosition += curEvent.delta.x) * VTabsMenuItems.sidescrollSensitivity / 2))
                        if (curEvent.control || curEvent.command)
                        {
                            if (VTabsMenuItems.sidescrollMoveTabEnabled)
                                moveTab(curEvent.delta.x < 0);
                        }
                        else
                        {
                            if (VTabsMenuItems.sidescrollSwitchTabEnabled)
                                switchTab(curEvent.delta.x < 0);
                        }

                }

                shiftscroll();
                sidescroll();

            }

            dragndrop_();
            scrolling();

            UpdateTabHeader(EditorWindow.mouseOverWindow);

        }
        static float sidesscrollPosition;



        static void UpdateDelayedMousePosition()
        {
            var curEvent = (Event)typeof(Event).GetField("s_Current", maxBindingFlags).GetValue(maxBindingFlags);

            delayedMousePosition_screenSpace = EditorGUIUtility.GUIToScreenPoint(curEvent.mousePosition);

            EditorApplication.delayCall += UpdateDelayedMousePosition;

        }
        static Vector2 delayedMousePosition_screenSpace;

        static void OnKeyEvent()
        {
            void keyPressed_()
            {
                if (Event.current.keyCode == KeyCode.LeftShift) return;
                if (Event.current.keyCode == KeyCode.LeftControl) return;
                if (Event.current.keyCode == KeyCode.LeftCommand) return;
                if (Event.current.keyCode == KeyCode.RightShift) return;
                if (Event.current.keyCode == KeyCode.RightControl) return;
                if (Event.current.keyCode == KeyCode.RightCommand) return;

                if (Event.current.type == EventType.KeyDown)
                    keyPressed = true;

                if (Event.current.type == EventType.KeyUp)
                    keyPressed = false;

            }
            void closeTab()
            {
                if (Event.current.type != EventType.KeyDown) return;
                if (!Event.current.control && !Event.current.command) return;
                if (Event.current.keyCode != KeyCode.W) return;
                if (!VTabsMenuItems.closeTabEnabled) return;
                if (!EditorWindow.mouseOverWindow) return;
                if (!EditorWindow.mouseOverWindow.docked) return;
                if (EditorWindow.mouseOverWindow.maximized) return;
                if (GetTabList().Count <= 1) return;


                Event.current.Use();

                closedTabsForReopening.Push(new TabDescription(EditorWindow.mouseOverWindow));

                EditorWindow.mouseOverWindow.Close();

                EnsureFocusedTabVisibleOnScroller();

            }
            void addTab()
            {
                if (Event.current.type != EventType.KeyDown) return;
                if (!Event.current.control && !Event.current.command) return;
                if (Event.current.keyCode != KeyCode.T) return;
                if (Event.current.shift) return;
                if (!EditorWindow.mouseOverWindow) return;
                if (!VTabsMenuItems.addTabEnabled) return;


                Event.current.Use();

                var tabListKey = "vTabs-AddTabMenu-" + GetProjectId();

                var customList = JsonUtility.FromJson<TabDescription.ListHolderForJson>(EditorPrefs.GetString(tabListKey))?.list ?? new List<TabDescription>();

                var defaultList = new List<TabDescription>();
                defaultList.Add(new TabDescription("SceneView", "Scene"));
                defaultList.Add(new TabDescription("GameView", "Game"));
                defaultList.Add(new TabDescription("ProjectBrowser", "Project"));
                defaultList.Add(new TabDescription("InspectorWindow", "Inspector"));
                defaultList.Add(new TabDescription("ConsoleWindow", "Console"));
                defaultList.Add(new TabDescription("ProfilerWindow", "Profiler"));
                defaultList.Add(new TabDescription("LightingWindow", "Lighting"));
                defaultList.Add(new TabDescription("ProjectSettingsWindow", "Project Settings"));


                var curTab = EditorWindow.mouseOverWindow;

                var curTabAlreadyAdded = defaultList.Any(r => r.menuItemName == curTab.titleContent.text.Replace("/", " \u2215 ").Trim(' ')) || customList.Any(r => r.menuItemName == curTab.titleContent.text.Replace("/", " \u2215 ").Trim(' '));


                void addCurrentTabToList()
                {
                    customList.Add(new TabDescription(curTab));
                    EditorPrefs.SetString(tabListKey, JsonUtility.ToJson(new TabDescription.ListHolderForJson(customList)));
                }
                void removeFromList(TabDescription t)
                {
                    customList.Remove(t);
                    EditorPrefs.SetString(tabListKey, JsonUtility.ToJson(new TabDescription.ListHolderForJson(customList)));
                }




                var menu = new GenericMenu();

                foreach (var tab in defaultList)
                    menu.AddItem(new GUIContent(tab.menuItemName), false, () => tab.CreateWindow(GetDockArea(curTab), false));



                menu.AddSeparator("");

                foreach (var tab in customList)
                    menu.AddItem(new GUIContent(tab.menuItemName), false, () => tab.CreateWindow(GetDockArea(curTab), false));



                menu.AddSeparator("");

                if (!curTabAlreadyAdded)
                    menu.AddItem(new GUIContent("Add current tab to list"), false, addCurrentTabToList);

                foreach (var tab in customList)
                    menu.AddItem(new GUIContent("Remove from list/" + tab.menuItemName), false, () => removeFromList(tab));


#if UNITY_2020_1_OR_NEWER
                menu.ShowAsContext();
#else
                var pos = Event.current.mousePosition + EditorWindow.mouseOverWindow.position.position;
                menu.DropDown(new Rect(pos.x ,pos.y ,0,0) );
#endif

            }
            void reopenTab()
            {
                if (Event.current.type != EventType.KeyDown) return;
                if (!Event.current.control && !Event.current.command) return;
                if (Event.current.keyCode != KeyCode.T) return;
                if (!Event.current.shift) return;
                if (!EditorWindow.mouseOverWindow) return;
                if (!VTabsMenuItems.reopenTabEnabled) return;
                if (!closedTabsForReopening.Any()) return;


                Event.current.Use();

                closedTabsForReopening.Pop().CreateWindow();

            }

            keyPressed_();
            closeTab();
            addTab();
            reopenTab();

        }
        static bool keyPressed;
        static Stack<TabDescription> closedTabsForReopening = new Stack<TabDescription>();


        static void UpdateTabHeader(EditorWindow window)
        {
            if (window == null) return;

            var isInspector = window.GetType() == inspectorType;
            var isFolder = window.GetType() == browserType;

            if (!isInspector && !isFolder) return;

            if (!(bool)window.GetType().GetProperty("isLocked", maxBindingFlags).GetValue(window)) return;


            string name = "";
            Texture icon = null;
            if (isInspector)
            {
                var obj = ((Object)inspectorType.GetMethod("GetInspectedObject", maxBindingFlags).Invoke(window, null));

                if (obj == null) return;

                name = obj.name;
                icon = EditorGUIUtility.FindTexture("UnityEditor.InspectorWindow");
            }
            if (isFolder)
            {
                name = ((string)window.GetType().GetMethod("GetActiveFolderPath", maxBindingFlags).Invoke(window, null)).GetName();
                icon = EditorGUIUtility.FindTexture("Project");
            }


            GUIContent titleContent;
            if (icon)
                titleContent = new GUIContent(name, icon);
            else
                titleContent = new GUIContent("  " + name + "");


            window.titleContent = titleContent;
        }
        static void UpdateBrowserTabHeaders()
        {
            foreach (var browser in lockedBrowsers)
                UpdateTabHeader(browser);
        }
        static void UpdateInspectorTabHeaders()
        {
            foreach (var insepctor in lockedInspectors)
                UpdateTabHeader(insepctor);
        }
        static void UpdateInspectorTabHeadersPreventingReset()
        {
            var prevFocused = EditorWindow.focusedWindow;
            foreach (var dockAreaGroup in lockedInspectors.GroupBy(r => GetDockArea(r)))
            {
                var tabsInDockArea = dockAreaType.GetField("m_Panes", maxBindingFlags).GetValue(dockAreaGroup.Key) as List<EditorWindow>;
                var prevSelected = tabsInDockArea[(int)dockAreaGroup.Key.GetType().GetField("m_Selected", maxBindingFlags).GetValue(dockAreaGroup.Key)];

                foreach (var inspector in dockAreaGroup)
                {
                    inspector.Focus();
                    inspector.SendEvent(EditorGUIUtility.CommandEvent(""));

                    UpdateTabHeader(inspector);
                }

                prevSelected.Focus();
                if (prevSelected.GetType() == inspectorType)
                {
                    prevSelected.SendEvent(EditorGUIUtility.CommandEvent(""));

                    UpdateTabHeader(prevSelected);
                }
            }
            prevFocused?.Focus();
        }

        static void EnsureFocusedTabVisibleOnScroller()
        {
            if (!EditorWindow.focusedWindow) return;
            if (!EditorWindow.focusedWindow.docked) return;
            if (EditorWindow.focusedWindow.maximized) return;

            var dockArea = GetDockArea(EditorWindow.focusedWindow);

            var scrollOffsetField = dockArea.GetType().GetField("m_ScrollOffset", maxBindingFlags);

            var scrollOffset = (float)scrollOffsetField.GetValue(dockArea);

#if UNITY_2020_1_OR_NEWER
            var getWidth = dockArea.GetType().GetMethods(maxBindingFlags).First(r => r.Name == "GetTabWidth" && r.GetParameters().Any(rr => rr.ParameterType == typeof(EditorWindow)));
#else
            var getWidth = dockArea.GetType().GetMethods(maxBindingFlags).First(r => r.Name == "GetTabWidth");
#endif

            var tabStyle = dockArea.GetType().GetField("tabStyle", maxBindingFlags).GetValue(dockArea);

            var totalWidth = ((Rect)dockArea.GetType().GetField("m_TabAreaRect", maxBindingFlags).GetValue(dockArea)).width;
            var x0 = 0f;
            var x1 = 0f;

            var tabs = GetTabList(EditorWindow.focusedWindow);
            for (int i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];

#if UNITY_2020_1_OR_NEWER
                var width = (float)getWidth.Invoke(dockArea, new[] { tabStyle, tab });
#else
                var width = (float)getWidth.Invoke(dockArea, new[] { tabStyle, i });
#endif               


                x0 = x1;
                x1 += width + 1;

                if (tab == EditorWindow.focusedWindow) break;
            }

            scrollOffset = Mathf.Min(scrollOffset, x0 - 12);
            scrollOffset = Mathf.Max(scrollOffset, 0);
            scrollOffset = Mathf.Max(scrollOffset, x1 - totalWidth + 12);

            scrollOffsetField.SetValue(dockArea, scrollOffset);

        }



        static void HandleSelectionChangeOnGUI()
        {
            if (selectionChangeHandled) return;

            selectionChangeHandled = true;

            UpdateInspectorTabHeaders();

        }
        static void FinishedDefaultHeaderGUI(Editor editor) => HandleSelectionChangeOnGUI();
        static void ProjectWindowItemOnGUI(string guid, Rect rect) => HandleSelectionChangeOnGUI();
        static bool selectionChangeHandled = true;

        static void OnPlaymodeStateChanged(PlayModeStateChange state) => EditorApplication.delayCall += UpdateInspectorTabHeaders;

        static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode) => UpdateInspectorTabHeaders();




#if !DISABLED
        [InitializeOnLoadMethod]
#endif
        static void Init()
        {
            var globalEventHandler = typeof(EditorApplication).GetField("globalEventHandler", maxBindingFlags);
            globalEventHandler.SetValue(null, (EditorApplication.CallbackFunction)globalEventHandler.GetValue(null) + OnKeyEvent);

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            EditorApplication.delayCall += UpdateDelayedMousePosition;

            Editor.finishedDefaultHeaderGUI -= FinishedDefaultHeaderGUI;
            Editor.finishedDefaultHeaderGUI += FinishedDefaultHeaderGUI;
            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUI;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;

            EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;

            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnSceneOpened;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnSceneOpened;

            Selection.selectionChanged += () => selectionChangeHandled = false;


            EditorApplication.delayCall += UpdateBrowserTabHeaders;
            EditorApplication.delayCall += UpdateInspectorTabHeadersPreventingReset;
        }




        [System.Serializable]
        class TabDescription
        {
            public string typeName;
            public object dockArea;
            int tabIndex;
            public string menuItemName;

            public string objectGuid = "";
            public int objectInstanceId;
            public bool isLockedToObject;

            public bool isInspector => typeName == inspectorType.Name;
            public bool isBrowser => typeName == browserType.Name;



            public TabDescription(EditorWindow window)
            {
                typeName = window.GetType().Name;
                dockArea = typeof(EditorWindow).GetField("m_Parent", maxBindingFlags).GetValue(window);
                tabIndex = ((List<EditorWindow>)dockAreaType.GetField("m_Panes", maxBindingFlags).GetValue(dockArea)).IndexOf(window);
                menuItemName = window.titleContent.text.Replace("/", " \u2215 ").Trim(' ');

                if (window.GetType() == browserType)
                {
                    var path = (string)browserType.GetMethod("GetActiveFolderPath", maxBindingFlags).Invoke(window, null);
                    objectGuid = AssetDatabase.AssetPathToGUID(path);

                    isLockedToObject = (bool)browserType.GetProperty("isLocked", maxBindingFlags).GetValue(window);

                }

                if (window.GetType() == inspectorType)
                    if (inspectorType.GetMethod("GetInspectedObject", maxBindingFlags).Invoke(window, null) is Object obj)
                    {
                        var path = obj.GetPath();

                        if (path != "")
                            objectGuid = AssetDatabase.AssetPathToGUID(path);
                        else
                            objectInstanceId = obj.GetInstanceID();

                        isLockedToObject = (bool)inspectorType.GetProperty("isLocked", maxBindingFlags).GetValue(window);

                    }

            }
            public TabDescription(string typeName, string menuItemName)
            {
                this.typeName = typeName;
                this.menuItemName = menuItemName;
            }
            public TabDescription(Object lockTo)
            {
                var isFolder = lockTo.GetType() == typeof(DefaultAsset);

                if (isFolder)
                {
                    typeName = browserType.Name;
                    objectGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(lockTo));
                }
                else
                {
                    typeName = inspectorType.Name;
                    objectInstanceId = lockTo.GetInstanceID();
                }

                isLockedToObject = true;
            }


            public EditorWindow CreateWindow(object dockArea, bool atStoredTabIndex = true)
            {
                if (dockArea == null) return null;

                var s_LastInteractedProjectBrowser = isBrowser ? browserType.GetField("s_LastInteractedProjectBrowser", maxBindingFlags).GetValue(null) : null;

                var window = (EditorWindow)ScriptableObject.CreateInstance(typeName);

                if (atStoredTabIndex)
                {
                    var totalTabs = ((List<EditorWindow>)dockAreaType.GetField("m_Panes", maxBindingFlags).GetValue(dockArea)).Count;
                    var index = Mathf.Clamp(tabIndex, 0, totalTabs);
                    dockAreaType.GetMethods(maxBindingFlags).First(r => r.Name == "AddTab" && r.GetParameters().Count() == 3).Invoke(dockArea, new[] { tabIndex, (object)window, true });
                }
                else
                    dockAreaType.GetMethods(maxBindingFlags).First(r => r.Name == "AddTab" && r.GetParameters().Count() == 2).Invoke(dockArea, new[] { (object)window, true });


                if (isBrowser)
                {
                    browserType.GetMethod("Init", maxBindingFlags).Invoke(window, null);

                    if (s_LastInteractedProjectBrowser != null)
                    {
                        var fi_m_DirectoriesAreaWidth = browserType.GetField("m_DirectoriesAreaWidth", maxBindingFlags);
                        fi_m_DirectoriesAreaWidth.SetValue(window, fi_m_DirectoriesAreaWidth.GetValue(s_LastInteractedProjectBrowser));


                        var fi_m_ListArea = browserType.GetField("m_ListArea", maxBindingFlags);
                        var pi_gridSize = fi_m_ListArea.FieldType.GetProperty("gridSize", maxBindingFlags);

                        var listAreaSource = fi_m_ListArea.GetValue(s_LastInteractedProjectBrowser);
                        var listAreaDest = fi_m_ListArea.GetValue(window);

                        if (listAreaSource != null && listAreaDest != null)
                            pi_gridSize.SetValue(listAreaDest, pi_gridSize.GetValue(listAreaSource));

                    }

                    if (objectGuid != "")
                    {
                        var iid = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(objectGuid)).GetInstanceID();

                        var m_ListAreaState = browserType.GetField("m_ListAreaState", maxBindingFlags).GetValue(window);

                        m_ListAreaState.GetType().GetField("m_SelectedInstanceIDs").SetValue(m_ListAreaState, new List<int> { iid });

                        browserType.GetMethod("OpenSelectedFolders", maxBindingFlags).Invoke(null, null);


                        if (isLockedToObject)
                            browserType.GetProperty("isLocked", maxBindingFlags).SetValue(window, true, null);
                    }

                }

                if (isInspector)
                {
                    Object obj = null;

                    if (objectInstanceId != 0)
                        obj = EditorUtility.InstanceIDToObject(objectInstanceId);
                    else
                        obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(objectGuid));

                    if (obj)
                    {
                        var prev = Selection.activeObject;
                        Selection.activeObject = obj;

                        inspectorType.GetProperty("isLocked", maxBindingFlags).SetValue(window, true, null);

                        Selection.activeObject = prev;
                    }

                }



                if (isBrowser)
                    UpdateTabHeader(window);

                window.Focus();

                if (isInspector)
                    UpdateInspectorTabHeadersPreventingReset();

                EnsureFocusedTabVisibleOnScroller();


                return window;
            }
            public EditorWindow CreateWindow(bool atStoredTabIndex = true) => CreateWindow(dockArea, atStoredTabIndex);



            [System.Serializable]
            public class ListHolderForJson
            {
                public List<TabDescription> list = new List<TabDescription>();
                public ListHolderForJson(List<TabDescription> list) => this.list = list;
            }
        }



        static object GetDockArea(EditorWindow window) => typeof(EditorWindow).GetField("m_Parent", maxBindingFlags).GetValue(window);
        static object GetDockArea() => GetDockArea(EditorWindow.mouseOverWindow);
        static List<EditorWindow> GetTabList(EditorWindow window) => dockAreaType.GetField("m_Panes", maxBindingFlags).GetValue(GetDockArea(window)) as List<EditorWindow>;
        static List<EditorWindow> GetTabList() => GetTabList(EditorWindow.mouseOverWindow);


        static List<EditorWindow> lockedBrowsers => Resources.FindObjectsOfTypeAll(browserType).Where(r => (bool)_isLockedBrowserProp.GetValue(r)).Select(r => r as EditorWindow).ToList();
        static PropertyInfo _isLockedBrowserProp = browserType.GetProperty("isLocked", maxBindingFlags);

        static List<EditorWindow> lockedInspectors => ((EditorWindow[])_getAllInspectorsMethod.Invoke(null, null)).Where(r => (bool)_isLockedInspectorProp.GetValue(r)).ToList();
        static MethodInfo _getAllInspectorsMethod = inspectorType.GetMethod("GetAllInspectorWindows", maxBindingFlags);
        static PropertyInfo _isLockedInspectorProp = inspectorType.GetProperty("isLocked", maxBindingFlags);


        static System.Type dockAreaType => typeof(Editor).Assembly.GetType("UnityEditor.DockArea");
        static System.Type inspectorType => typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        static System.Type browserType => typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        static System.Type hierarchyType => typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");


        const string version = "1.0.12";

    }
}
#endif
