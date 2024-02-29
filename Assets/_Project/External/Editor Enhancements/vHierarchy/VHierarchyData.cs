#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static VHierarchy.Libs.VUtils;

namespace VHierarchy
{
    public class VHierarchyData : ScriptableObject
    {
        public SerializeableDicitonary<string, SceneData> sceneDatasByGuid = new SerializeableDicitonary<string, SceneData>();
        public Dictionary<Scene, SceneData> sceneDatasByScene = new Dictionary<Scene, SceneData>();

        [System.Serializable]
        public class SceneData
        {
            public SerializeableDicitonary<string, GameObjectData> goDatasByGlobalId = new SerializeableDicitonary<string, GameObjectData>();
            public SerializeableDicitonary<int, GameObjectData> goDatasByInstanceId = new SerializeableDicitonary<int, GameObjectData>(); // serializable so prefabs don't loose their icons on playmode enter

        }

        [System.Serializable]
        public class GameObjectData
        {
            public Color color => VHierarchyIconEditor.GetColor(iColor);
            public int iColor;
            public string icon = "";

        }
    }
}
#endif