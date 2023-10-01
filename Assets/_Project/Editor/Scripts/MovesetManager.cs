using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lumina.Debugging
{
/// <summary>
///     This EditorWindow is intended to be used to easily modify existing moves and movesets.
///     It is designed using the UI Toolkit.
///     <para>https://www.youtube.com/watch?v=mTjYA3gC1hA</para>
/// </summary>
public class MovesetManager : EditorWindow
{
    [MenuItem("Tools/Attacking/Moveset Manager")]
    static void ShowWindow()
    {
        var window = GetWindow<MovesetManager>();
        window.titleContent = new ("TITLE");
        window.Show();
    }

    void OnGUI() { throw new NotImplementedException(); }

    #region Utility
    void CreateMoveListView()
    {
        FindAllMoves(out MoveData[] moves);

        ListView movesList = rootVisualElement.Query<ListView>("move-list").First();

        movesList.makeItem = () => new Label();
        movesList.bindItem = (element, i) => (element as Label).text = moves[i].name;

        movesList.itemsSource     = moves;
        movesList.fixedItemHeight = 16;
        movesList.selectionType   = SelectionType.Single;

        movesList.selectionChanged += enumerable =>
        {
            foreach (var move in enumerable)
            {
                Box moveInfoBox = rootVisualElement.Query<Box>("move-info").First();
                moveInfoBox.Clear();

                MoveData moveData = move as MoveData;

                SerializedObject   serializedMove = new SerializedObject(moveData);
                SerializedProperty moveProperty   = serializedMove.GetIterator();
                moveProperty.Next(true);
            }
        };
    }

    void FindAllMoves(out MoveData[] moves)
    {
        var guids = AssetDatabase.FindAssets("t:MoveData");

        moves = new MoveData[guids.Length];

        for (var i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            moves[i] = AssetDatabase.LoadAssetAtPath<MoveData>(path);
        }
    }

    void FindAllMovesets()
    {
        var guids = AssetDatabase.FindAssets("t:Moveset");

        var movesets = new Moveset[guids.Length];

        for (var i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            movesets[i] = AssetDatabase.LoadAssetAtPath<Moveset>(path);
        }
    }

    void LoadMoveImage(Texture texture)
    {
        var moveImagePreview = rootVisualElement.Query<Image>("preview").First();
        moveImagePreview.image = texture;
    }
    #endregion
}
}
