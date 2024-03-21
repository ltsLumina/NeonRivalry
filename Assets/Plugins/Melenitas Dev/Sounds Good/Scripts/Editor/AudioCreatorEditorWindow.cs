#if UNITY_EDITOR
/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MelenitasDev.SoundsGood.Domain;
using UnityEditor;
using UnityEngine;

namespace MelenitasDev.SoundsGood.Editor
{
    public class AudioCreatorEditorWindow : EditorWindow
    {
        private MusicDataCollection musicDataCollection;
        private SoundDataCollection soundDataCollection;

        private string tag = "";
        private List<AudioClip> audioClipList = new List<AudioClip>();

        private bool showCompressionSection;
        private CompressionPreset compressionPreset = CompressionPreset.FrequentSound;
        private bool forceToMono = false;
        
        private GUIStyle enabledButtonStyle;
        private GUIStyle disabledButtonStyle;
        private GUIStyle errorTextFieldStyle;
        private GUIStyle greyBoxLabelStyle;
        
        private bool creationSuccess;
        private string resultMessage = null;
        
        private Sections currentSection = Sections.Sounds;

        private enum Sections
        { 
            Sounds,
            Music
        }
        
        [MenuItem("Tools/Melenitas Dev/Sounds Good/Audio Creator")]
        static void ShowWindow ()
        {
            // Get existing open window or if none, make a new one:
            AudioCreatorEditorWindow window = (AudioCreatorEditorWindow)GetWindow(typeof(AudioCreatorEditorWindow));
            window.Show();
            window.titleContent = new GUIContent("Audio Creator");
            window.LoadOrCreateAudioCollections();
        }

        void OnGUI ()
        {
            disabledButtonStyle = new GUIStyle("Button");
            disabledButtonStyle.normal.textColor = Color.white;
            disabledButtonStyle.normal.background = MakeTexture(2, 2, Color.green * 0.5f);
            disabledButtonStyle.active.textColor = Color.white;
            disabledButtonStyle.active.background = MakeTexture(2, 2, Color.green * 0.8f);

            enabledButtonStyle = new GUIStyle("Button");
            enabledButtonStyle.normal.textColor = Color.white;
            enabledButtonStyle.normal.background = MakeTexture(2, 2, Color.white * 0.5f);
            enabledButtonStyle.active.textColor = Color.white;
            enabledButtonStyle.active.background = MakeTexture(2, 2, Color.white * 0.8f);

            errorTextFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                normal =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.red * 0.5f)
                },
            };

            greyBoxLabelStyle = new GUIStyle("box");
            greyBoxLabelStyle.normal.textColor = Color.white;
            greyBoxLabelStyle.normal.background = MakeTexture(2, 2, new Color(0.15f, 0.15f, 0.15f, 0.5f));

            EditorGUILayout.Space(20);

            CenterText("AUDIO CREATOR", 25, EditorWindowSharedTools.orange);

            EditorGUILayout.Space(5);

            Subtitle();

            PlayModeMessage();

            GUI.enabled = !Application.isPlaying;

            EditorGUILayout.Space(15);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            ToolSections();

            EditorGUILayout.Space(10);

            CreatorFields();

            EditorGUILayout.Space(10);

            ResultMessage();

            EditorGUI.EndDisabledGroup();

            EditorWindowSharedTools.LogoBanner();
        }

        private void Subtitle ()
        {
            string subtitle = currentSection == Sections.Sounds ? "Create new sound" : "Create new music track";
            CenterText(subtitle, 11, EditorWindowSharedTools.lightOrange);
        }
        
        private void PlayModeMessage ()
        {
            if (!UnityEngine.Application.isPlaying) return;
            EditorGUILayout.Space(10);
            CenterText("It cannot be used in Play mode", 13, new Color(0.65f, 0.25f, 0.25f));
        }

        private void ToolSections ()
        {
            EditorGUILayout.BeginHorizontal();
            if (currentSection == Sections.Sounds && !Application.isPlaying) GUI.enabled = false;
            if (GUILayout.Button("Sounds", currentSection == Sections.Sounds ? disabledButtonStyle : enabledButtonStyle))
            {
                currentSection = Sections.Sounds;
                compressionPreset = CompressionPreset.FrequentSound;
            }
            if (currentSection == Sections.Sounds && !Application.isPlaying) GUI.enabled = true;
            if (currentSection == Sections.Music && !Application.isPlaying) GUI.enabled = false;
            if (GUILayout.Button("Music", currentSection == Sections.Music ? disabledButtonStyle : enabledButtonStyle))
            {
                currentSection = Sections.Music;
                forceToMono = false;
                compressionPreset = CompressionPreset.AmbientMusic;
            }
            if (currentSection == Sections.Music && !Application.isPlaying) GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void CompressionPresetLabel ()
        {
            GUIStyle titleLabelStyle = new GUIStyle(EditorStyles.label);
            titleLabelStyle.fontSize = 15;
            GUIStyle enumStyle = new GUIStyle(EditorStyles.popup);
            enumStyle.fontSize = 13;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Preset:", titleLabelStyle, GUILayout.Width(125));
            compressionPreset = (CompressionPreset)
                EditorGUILayout.EnumPopup(compressionPreset, enumStyle);
            EditorGUILayout.EndHorizontal();
            string presetInfo = null;
            switch (compressionPreset)
            {
                case CompressionPreset.AmbientMusic:
                    presetInfo = "Music that is generally long and heavy that will be played for a long time.";
                    break;
                case CompressionPreset.FrequentSound:
                    presetInfo = "Sound that is generally short, not very heavy and will " +
                                 "be played many times (shot, steps, UI...)";
                    break;
                case CompressionPreset.OccasionalSound:
                    presetInfo = "A sound that is generally short, not very heavy, and will not be played very frequently";
                    break;
            }

            GUILayout.Box(presetInfo, greyBoxLabelStyle);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Force to Mono:", titleLabelStyle, GUILayout.Width(125), GUILayout.Height(15));
            forceToMono = EditorGUILayout.Toggle(forceToMono);
            EditorGUILayout.EndHorizontal();
            GUILayout.Box("It will optimize the audio memory space, but it will no longer " +
                          "be stereo (not recommended for ambient music)", greyBoxLabelStyle);
        }
        
        private void CreatorFields ()
        {
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tag:", GUILayout.Width(30));
            bool tagValid = IsTagValid(tag);
            if (string.IsNullOrEmpty(tag)) tagValid = true;
            tag = EditorGUILayout.TextField(tag, tagValid ? GUI.skin.textField : errorTextFieldStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(3);
            
            for (int i = 0; i < audioClipList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                audioClipList[i] = (AudioClip)EditorGUILayout
                    .ObjectField(GUIContent.none, audioClipList[i], typeof(AudioClip), false);
            
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(30)))
                {
                    audioClipList.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayoutOption[] options = { GUILayout.Width(60), GUILayout.Height(30) };
            if (GUILayout.Button("+", options))
            {
                audioClipList.Add(null);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontSize = 16;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            showCompressionSection = EditorGUILayout.Foldout(showCompressionSection, " Compression Preset", foldoutStyle);
            EditorGUILayout.EndHorizontal();
            if (showCompressionSection)
            {
                EditorGUILayout.Space(1);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space(5);
                CompressionPresetLabel();
                EditorGUILayout.Space(1);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            string buttonTitle = currentSection == Sections.Sounds ? "CREATE SOUND" : "CREATE MUSIC TRACK";
            if (GUILayout.Button(buttonTitle, GUILayout.Height(30)))
            {
                if (!string.IsNullOrEmpty(tag) && !IsTagValid(tag))
                {
                    resultMessage = "Tag cannot contain special characters or start with a number";
                    creationSuccess = false;
                    return;
                }
                
                LoadOrCreateAudioCollections();
                
                List<AudioClip> validAudioClips = audioClipList.Where(audioClip => audioClip != null).ToList();

                if (currentSection == Sections.Sounds)
                    creationSuccess = soundDataCollection.CreateSound(validAudioClips.ToArray(), tag, 
                        compressionPreset, forceToMono, out resultMessage);
                else
                    creationSuccess = musicDataCollection.CreateMusicTrack(validAudioClips.ToArray(), tag, 
                        compressionPreset, forceToMono, out resultMessage);

                if (!creationSuccess) return;
                
                EditorWindowSharedTools.ChangeAudioClipImportSettings(validAudioClips.ToArray(), 
                    compressionPreset, forceToMono);
                    
                string[] tags;
                if (currentSection == Sections.Sounds)
                {
                    int i = 0;
                    tags = new string[soundDataCollection.Sounds.Length];
                    foreach (SoundData sound in soundDataCollection.Sounds)
                    {
                        tags[i] = sound.Tag;
                        i++;
                    }
                }
                else
                {
                    int i = 0;
                    tags = new string[musicDataCollection.MusicTracks.Length];
                    foreach (SoundData sound in musicDataCollection.MusicTracks)
                    {
                        tags[i] = sound.Tag;
                        i++;
                    }
                }
                    
                using (EnumGenerator enumGenerator = new EnumGenerator())
                {
                    string enumName = currentSection == Sections.Sounds ? "SFX" : "Track";
                    enumGenerator.GenerateEnum(enumName, tags);
                }
                audioClipList.Clear();
                
                if (currentSection == Sections.Sounds) EditorUtility.SetDirty(soundDataCollection);
                else EditorUtility.SetDirty(musicDataCollection);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private bool IsTagValid (string tag)
        {
            if (string.IsNullOrEmpty(tag)) return false;
            if (Regex.IsMatch(tag, @"[^a-zA-Z0-9]")) return false;
            if (Regex.IsMatch(tag, "^[0-9]")) return false;
            if (!Regex.IsMatch(tag, @"[a-zA-Z]")) return false;
            return true;
        }
        
        private void ResultMessage ()
        {
            if (!string.IsNullOrEmpty(resultMessage))
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("  " + resultMessage, creationSuccess ? MessageType.Info : MessageType.Error);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Clear", GUILayout.Height(20), GUILayout.Width(70)))
                {
                    resultMessage = null;
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }
        
        private void LoadOrCreateAudioCollections ()
        {
            string resourcesFolderPath = "Assets/Resources/Melenitas Dev/Sounds Good";
            if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Melenitas Dev"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Melenitas Dev");
                AssetDatabase.CreateFolder("Assets/Resources/Melenitas Dev", "Sounds Good");
                AssetDatabase.Refresh();
            }

            // Attempt to load the audio collections from Resources folder
            soundDataCollection = Resources.Load<SoundDataCollection>("Melenitas Dev/Sounds Good/SoundCollection");
            musicDataCollection = Resources.Load<MusicDataCollection>("Melenitas Dev/Sounds Good/MusicCollection");
            

            // If the sound collection doesn't exist, create a new one and store it
            if (soundDataCollection == null)
            {
                soundDataCollection = CreateInstance<SoundDataCollection>();

                // Create a new asset file and save defaultTwitchSettings to Resources folder
                AssetDatabase.CreateAsset(soundDataCollection, $"{resourcesFolderPath}/SoundCollection.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            // If the music collection doesn't exist, create a new one and store it
            if (musicDataCollection == null)
            {
                musicDataCollection = CreateInstance<MusicDataCollection>();

                // Create a new asset file and save defaultTwitchSettings to Resources folder
                AssetDatabase.CreateAsset(musicDataCollection, $"{resourcesFolderPath}/MusicCollection.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
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
        
        private Texture2D MakeTexture (int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
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