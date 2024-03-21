#if UNITY_EDITOR
/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using MelenitasDev.SoundsGood.Domain;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace MelenitasDev.SoundsGood.Editor
{
    public class GetStartedEditorWindow : EditorWindow
    {
        
        [MenuItem("Tools/Melenitas Dev/Sounds Good/Get Started!", false, 1)]
        public static void ShowWindow ()
        {
            GetStartedEditorWindow window = 
                (GetStartedEditorWindow)GetWindow(typeof(GetStartedEditorWindow));
            window.Show();
            window.titleContent = new GUIContent("Get Started!");
        }
        
        void OnGUI ()
        {
            GUILayout.Space(25);
            
            EditorWindowSharedTools.CenterText("GET STARTED!", 28, EditorWindowSharedTools.orange);
            
            GUILayout.Space(15);

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(6);
            EditorWindowSharedTools.CenterText("Welcome to SOUNDS GOOD,", 16, EditorWindowSharedTools.lightOrange);
            
            GUILayout.Space(8);
            
            string welcomeText = "the audio manager you've always dreamed of. " +
                                 "If you need any help getting started, you can refer to the documentation available in both English and Spanish.";
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6);
            EditorWindowSharedTools.CenterText(welcomeText, 12, Color.white);
            GUILayout.Space(6);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(6);
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            
            EditorWindowSharedTools.CenterText("Is something not working as it should?", 15, EditorWindowSharedTools.lightOrange);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Create Dependencies", GUILayout.Height(50)))
            {
                CreateDependencies();
            }
            
            GUILayout.Space(15);
            
            EditorWindowSharedTools.CenterText("↓ Check the documentation below ↓", 15, EditorWindowSharedTools.lightOrange);
            
            GUILayout.Space(15);
            
            if (GUILayout.Button("English (en)", GUILayout.Height(40)))
            {
                Application.OpenURL("https://melenitasdev.notion.site/melenitasdev/SOUNDS-GOOD-English-documentation-e2102ec5bafa411cb7991dba60081075");
            }
            GUILayout.Space(1);
            if (GUILayout.Button("Spanish (es)", GUILayout.Height(40)))
            {
                Application.OpenURL("https://melenitasdev.notion.site/melenitasdev/SOUNDS-GOOD-Documentaci-n-espa-ol-b8fd14728ba74050a55a4c4f41ec0727");
            }
            
            EditorWindowSharedTools.LogoBanner();
        }

        private void CreateDependencies ()
        {
            bool allRight = true;
            
            string resourcesFolderPath = "Assets/Resources/Melenitas Dev/Sounds Good";
            if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Melenitas Dev"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Melenitas Dev");
                AssetDatabase.CreateFolder("Assets/Resources/Melenitas Dev", "Sounds Good");
            }
            
            SoundDataCollection soundDataCollection = Resources.Load<SoundDataCollection>("Melenitas Dev/Sounds Good/SoundCollection");
            MusicDataCollection musicDataCollection = Resources.Load<MusicDataCollection>("Melenitas Dev/Sounds Good/MusicCollection");
            if (soundDataCollection == null)
            {
                Debug.Log("Sound Data Collection was not found. A new one has been created at the following path:\n" + resourcesFolderPath);
                soundDataCollection = CreateInstance<SoundDataCollection>();
                AssetDatabase.CreateAsset(soundDataCollection, $"{resourcesFolderPath}/SoundCollection.asset");
                allRight = false;
            }
            if (musicDataCollection == null)
            {
                Debug.Log("Music Data Collection was not found. A new one has been created at the following path:\n" + resourcesFolderPath);
                musicDataCollection = CreateInstance<MusicDataCollection>();
                AssetDatabase.CreateAsset(musicDataCollection, $"{resourcesFolderPath}/MusicCollection.asset");
                allRight = false;
            }
            
            string path = "Assets/Resources/Melenitas Dev/Sounds Good/Outputs";
            if (!AssetDatabase.IsValidFolder(path))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Melenitas Dev"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Melenitas Dev");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Melenitas Dev/Sounds Good"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Sounds Good");
                AssetDatabase.CreateFolder("Assets/Resources/Melenitas Dev", "Outputs");
            }
            
            OutputCollection outputCollection = Resources.Load<OutputCollection>($"Melenitas Dev/Sounds Good/Outputs/OutputCollection");
            if (outputCollection == null)
            {
                Debug.Log("Output Collection was not found. A new one has been created at the following path:\n" + path);
                outputCollection = CreateInstance<OutputCollection>();
                AssetDatabase.CreateAsset(outputCollection, $"{path}/OutputCollection.asset");
                EditorUtility.SetDirty(outputCollection);
                allRight = false;
            }

            AudioMixer mixer = Resources.Load<AudioMixer>("Melenitas Dev/Sounds Good/Outputs/Master");
            if (mixer == null)
            {
                Debug.LogWarning($"Master Audio Mixer was not found. You must create " +
                                 $"it with the name 'Master' on the following path:\n{path}");
                allRight = false;
            }

            Debug.Log(!allRight ? "All the dependencies are ready!" : "Everything is ok!");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif