using System.IO;
using FLZ.Audio;
using UnityEditor;
using UnityEngine;

public static class SoundCreator
{
    [MenuItem("Assets/Create/Audio/SFX")]
    private static void CreateSFX()
    {
        var selection = Selection.objects;

        foreach (var obj in selection)
        {
            SFX.Sound[] sounds = new SFX.Sound[1];
            sounds[0] = new SFX.Sound(obj as AudioClip);

            SFX newSFX = new SFX(sounds);

            string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj));
            string name = "/SFX_" + obj.name + ".asset";
            AssetDatabase.CreateAsset(newSFX, path + name);
            
            Selection.activeObject = newSFX;
        }
    }
}