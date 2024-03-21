#if UNITY_EDITOR
/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System.Collections.Generic;
using MelenitasDev.SoundsGood.Domain;
using UnityEditor;
using UnityEngine;

namespace MelenitasDev.SoundsGood.Editor
{
    public class OutputManagerEditorWindow : EditorWindow
    {
        private OutputCollection outputCollection;

        private Dictionary<OutputData, bool> lastRefreshedOutputsDict = new Dictionary<OutputData, bool>();
        private Vector2 scrollPosition;

        private GUIStyle redBoxStyle;
        private GUIStyle greenBoxStyle;

        [MenuItem("Tools/Melenitas Dev/Sounds Good/Output Manager")]
        public static void ShowWindow ()
        {
            OutputManagerEditorWindow window = 
                (OutputManagerEditorWindow)GetWindow(typeof(OutputManagerEditorWindow));
            window.Show();
            window.titleContent = new GUIContent("Output Manager");
        }

        void OnEnable ()
        {
            string path = "Melenitas Dev/Sounds Good/Outputs";
            outputCollection = Resources.Load<OutputCollection>($"{path}/OutputCollection");
            LoadOutputs();
        }

        void OnGUI ()
        {
            redBoxStyle = new GUIStyle(GUI.skin.box);
            greenBoxStyle = new GUIStyle(GUI.skin.box);
            redBoxStyle.normal.background = MakeTex(2, 2, new Color(0.58f, 0.15f, 0.15f, 0.3f));
            greenBoxStyle.normal.background = MakeTex(2, 2, new Color(0.15f, 0.58f, 0.15f, 0.3f));

            EditorGUILayout.Space(20);
            
            CenterText("OUTPUTS MANAGER", 25, EditorWindowSharedTools.orange);
            
            GUILayout.Space(5);
            
            CenterText("Manage your audio outputs", 11, EditorWindowSharedTools.lightOrange);
            
            PlayModeMessage();
            
            GUI.enabled = !Application.isPlaying;
            
            GUILayout.Space(15);
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            if (GUILayout.Button("Reload Database", GUILayout.Height(45)))
            {
                LoadOutputs();
            }
            if (GUILayout.Button("Check Exposed Volumes", GUILayout.Height(24)))
            {
                CheckExposedVolumes();
            }

            if (outputCollection == null) return;

            GUILayout.Space(15);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var outputData in lastRefreshedOutputsDict)
            {
                bool exposed = outputData.Value;

                EditorGUILayout.BeginVertical(exposed ? greenBoxStyle : redBoxStyle);
                CenterText(outputData.Key.Name, 15, exposed ? Color.green : Color.red);
                string exposedText = exposed
                    ? "Volume is exposed correctly!"
                    : "Error: Volume is not exposed";
                CenterText(exposedText, 11, exposed ? Color.green : Color.red);
                GUILayout.Space(3);
                EditorGUILayout.EndVertical();
                GUILayout.Space(3);
            }
            EditorGUILayout.EndScrollView();
            EditorGUI.EndDisabledGroup();

            EditorWindowSharedTools.LogoBanner();
        }

        private void CheckExposedVolumes ()
        {
            lastRefreshedOutputsDict.Clear();
            foreach (OutputData outputData in outputCollection.Outputs)
            {
                bool exposed = outputData.Output.audioMixer.GetFloat(outputData.Name.Replace(" ", ""), out float value);
                lastRefreshedOutputsDict.Add(outputData, exposed);
            }
        }
        
        private void LoadOutputs()
        {
            outputCollection = Resources.Load<OutputCollection>($"Melenitas Dev/Sounds Good/Outputs/OutputCollection");
            if (outputCollection != null)
            {
                outputCollection.LoadOutputs();
                GenerateEnum();
                CheckExposedVolumes();
                EditorUtility.SetDirty(outputCollection);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
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
            
            outputCollection = CreateInstance<OutputCollection>();
            AssetDatabase.CreateAsset(outputCollection, $"{path}/OutputCollection.asset");
            
            outputCollection.LoadOutputs();
            GenerateEnum();
            CheckExposedVolumes();
            EditorUtility.SetDirty(outputCollection);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void GenerateEnum ()
        {
            string[] outputNames = new string[outputCollection.Outputs.Length];
            int i = 0;
            foreach (OutputData outputData in outputCollection.Outputs)
            {
                outputNames[i] = outputData.Name.Replace(" ", "");
                i++;
            }
            
            using (EnumGenerator enumGenerator = new EnumGenerator())
            {
                enumGenerator.GenerateEnum("Output", outputNames);
            }
        }

        private void PlayModeMessage ()
        {
            if (!Application.isPlaying) return;
            EditorGUILayout.Space(10);
            CenterText("It cannot be used in Play mode", 13, new Color(0.65f, 0.25f, 0.25f));
        }
        
        private void CenterText (string text, int fontSize, Color color)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = color;
            style.focused.textColor = color;
            style.hover.textColor = color;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = fontSize;
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
#endif