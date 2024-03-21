#if UNITY_EDITOR
/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited,
 * but it can be used within your projects."
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MelenitasDev.SoundsGood.Domain;
using UnityEditor;
using UnityEngine;

namespace MelenitasDev.SoundsGood.Editor
{
    public class AudioCollectionEditorWindow : EditorWindow
    {
        private SoundDataCollection soundDataCollection;
        private MusicDataCollection musicDataCollection;

        private Dictionary<SoundData, bool> templateSoundDataCollection = new Dictionary<SoundData, bool>();
        private Dictionary<SoundData, bool> templateMusicDataCollection = new Dictionary<SoundData, bool>();

        private SoundData[] currentDataCollection =>
            inSoundSect ? soundDataCollection.Sounds : musicDataCollection.MusicTracks;

        private Dictionary<SoundData, bool> currentTemplateDataCollection =>
            inSoundSect ? templateSoundDataCollection : templateMusicDataCollection;

        private bool inSoundSect => currentSection == Sections.Sounds;
        
        private GUIStyle enabledButtonStyle;
        private GUIStyle greenButtonStyle;
        private GUIStyle redButtonStyle;
        private GUIStyle errorTextFieldStyle;

        private string searchTag;
        private AudioClip searchAudioClip;

        private SoundData soundDataDeleteTry;
        private bool tryDeleteAll = false;

        private Sections currentSection = Sections.Sounds;
        private Vector2 scrollPosition;

        private enum Sections
        {
            Sounds,
            Music
        }

        [MenuItem("Tools/Melenitas Dev/Sounds Good/Audio Collection")]
        public static void ShowWindow ()
        {
            // Get existing open window or if none, make a new one:
            AudioCollectionEditorWindow window =
                (AudioCollectionEditorWindow)GetWindow(typeof(AudioCollectionEditorWindow));
            window.Show();
            window.titleContent = new GUIContent("Audio Collection");
        }

        void OnEnable ()
        {
            tryDeleteAll = false;
            soundDataCollection = Resources.Load<SoundDataCollection>("Melenitas Dev/Sounds Good/SoundCollection");
            musicDataCollection = Resources.Load<MusicDataCollection>("Melenitas Dev/Sounds Good/MusicCollection");
            foreach (SoundData soundData in soundDataCollection.Sounds)
            {
                AudioClip[] clips = new AudioClip[soundData.Clips.Length];
                for (int i = 0; i < clips.Length; i++) clips[i] = soundData.Clips[i];
                templateSoundDataCollection.Add(new SoundData(soundData.Tag, clips,
                    soundData.CompressionPreset, soundData.ForceToMono), true);
            }

            foreach (SoundData soundData in musicDataCollection.MusicTracks)
            {
                AudioClip[] clips = new AudioClip[soundData.Clips.Length];
                for (int i = 0; i < clips.Length; i++) clips[i] = soundData.Clips[i];
                templateMusicDataCollection.Add(new SoundData(soundData.Tag, clips,
                    soundData.CompressionPreset, soundData.ForceToMono), true);
            }
        }

        void OnGUI ()
        {
            greenButtonStyle = new GUIStyle("Button")
            {
                normal =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.green * 0.5f)
                },
                active =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.green * 0.8f)
                }
            };
            redButtonStyle = new GUIStyle("Button")
            {
                normal =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.red * 0.5f)
                },
                active =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.red * 0.8f)
                }
            };
            enabledButtonStyle = new GUIStyle("Button")
            {
                normal =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.white * 0.5f)
                },
                active =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.white * 0.8f)
                }
            };
            errorTextFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                normal =
                {
                    textColor = Color.white,
                    background = MakeTexture(2, 2, Color.red * 0.5f)
                },
            };

            EditorGUILayout.Space(20);

            CenterText("AUDIO COLLECTION", 25, EditorWindowSharedTools.orange);

            EditorGUILayout.Space(5);

            CenterText("Manage and modify your audios", 11, EditorWindowSharedTools.lightOrange);

            PlayModeMessage();

            GUI.enabled = !Application.isPlaying;

            EditorGUILayout.Space(15);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            ToolSections();

            EditorGUILayout.Space(10);

            Search();

            EditorGUILayout.Space(7);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            bool noFounds = true;
            for (int i = 0; i < currentTemplateDataCollection.Count; i++)
            {
                if (currentTemplateDataCollection.ElementAt(i).Value)
                {
                    noFounds = false;
                    DrawSoundDataGUI(currentTemplateDataCollection.ElementAt(i).Key, i);
                    GUILayout.Space(3);
                }
            }

            if (noFounds)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.HelpBox("  No results found :(", MessageType.Info);
                EditorGUILayout.Space(3);
            }
            
            GUILayout.Space(5);

            if (!noFounds)
            {
                if (!tryDeleteAll)
                {
                    if (GUILayout.Button("Delete All", redButtonStyle, GUILayout.Height(40)))
                    {
                        tryDeleteAll = true;
                        scrollPosition = new Vector2(0, 999999);
                    }
                }
                else
                {
                    EditorGUILayout.BeginVertical("box");
                    GUILayout.Space(5);
                    string target = inSoundSect ? "SOUNDS" : "MUSIC TRACKS";
                    string advice = $"You will permanently delete all your {target}";
                    CenterText("Are you sure?", 14, new Color(0.85f, 0.25f, 0.25f));
                    CenterText(advice, 12, new Color(0.75f, 0.35f, 0.35f));
                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("I'm sure, Delete All", greenButtonStyle))
                    {
                        DeleteAll();
                        tryDeleteAll = false;
                    }
                    if (GUILayout.Button("Cancel", redButtonStyle))
                    {
                        tryDeleteAll = false;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    EditorGUILayout.EndVertical();
                }
            }

            GUILayout.Space(15);
            
            EditorGUILayout.EndScrollView();

            EditorGUI.EndDisabledGroup();

            EditorWindowSharedTools.LogoBanner();

            Repaint();
        }

        private void DrawSoundDataGUI (SoundData soundData, int index)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tag:", GUILayout.Width(30));
            soundData.Tag = EditorGUILayout.TextField(soundData.Tag,
                IsTagValid(soundData.Tag) ? GUI.skin.textField : errorTextFieldStyle);
            if (currentDataCollection[index].Tag != soundData.Tag)
            {
                if (GUILayout.Button("Undo", redButtonStyle, GUILayout.Width(60)))
                {
                    GUI.FocusControl("");
                    currentTemplateDataCollection.ElementAt(index).Key.Tag = currentDataCollection[index].Tag;
                }
            }

            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(45)))
            {
                List<AudioClip> newClips = currentTemplateDataCollection.ElementAt(index).Key.Clips.ToList();
                newClips.Add(null);
                currentTemplateDataCollection.ElementAt(index).Key.Clips = newClips.ToArray();
            }

            EditorGUILayout.EndHorizontal();

            UpdateTag(soundData, index);

            bool nullAudioClip = false;
            bool changesDetected = false;
            for (int i = 0; i < soundData.Clips.Length; i++)
            {
                if (!nullAudioClip) nullAudioClip = soundData.Clips[i] == null;
                if (soundData.Clips.Length != currentDataCollection[index].Clips.Length ||
                    soundData.Clips[i] != currentDataCollection[index].Clips[i])
                    changesDetected = true;

                EditorGUILayout.BeginHorizontal();
                soundData.Clips[i] =
                    (AudioClip)EditorGUILayout.ObjectField(soundData.Clips[i], typeof(AudioClip), false);
                if (soundData.Clips.Length > 1)
                {
                    if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(30)))
                    {
                        List<AudioClip> newClips = currentTemplateDataCollection.ElementAt(index).Key.Clips.ToList();
                        newClips.RemoveAt(i);
                        currentTemplateDataCollection.ElementAt(index).Key.Clips = newClips.ToArray();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(1);
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label($"Preset: {soundData.CompressionPreset.ToString()}");
            GUILayout.Label(soundData.ForceToMono ? "Output: Mono" : "Output: Stereo");
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1);

            if (changesDetected)
            {
                if (GUILayout.Button("Undo", redButtonStyle))
                {
                    currentTemplateDataCollection.ElementAt(index).Key.Clips = currentDataCollection[index].Clips;
                }

                if (!nullAudioClip)
                {
                    if (GUILayout.Button("Update Clips", greenButtonStyle))
                    {
                        currentDataCollection[index].Clips = currentTemplateDataCollection.ElementAt(index).Key.Clips;
                        EditorWindowSharedTools.ChangeAudioClipImportSettings(currentDataCollection[index].Clips,
                            currentDataCollection[index].CompressionPreset, currentDataCollection[index].ForceToMono);
                        SaveChanges(true);
                    }
                }
            }

            if (soundDataDeleteTry == null || soundDataDeleteTry != soundData)
            {
                if (GUILayout.Button("Delete"))
                {
                    soundDataDeleteTry = soundData;
                }
            }
            else if (soundDataDeleteTry == soundData)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Delete", greenButtonStyle))
                {
                    DeleteSoundData(soundData);
                    soundDataDeleteTry = null;
                }

                if (GUILayout.Button("Cancel", redButtonStyle))
                {
                    soundDataDeleteTry = null;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private bool IsTagValid (string tag)
        {
            if (string.IsNullOrEmpty(tag)) return false;
            if (Regex.IsMatch(tag, @"[^a-zA-Z0-9]")) return false;
            if (Regex.IsMatch(tag, "^[0-9]")) return false;
            if (!Regex.IsMatch(tag, @"[a-zA-Z]")) return false;
            return true;
        }

        private void UpdateTag (SoundData soundData, int index)
        {
            string newTag = soundData.Tag;

            if (!IsTagValid(newTag)) return;
            if (newTag == currentDataCollection[index].Tag) return;
            if (GUILayout.Button("Update Tag", greenButtonStyle))
            {
                currentDataCollection[index].Tag = newTag;

                string[] tags;
                int i = 0;
                tags = new string[currentDataCollection.Length];
                foreach (SoundData sound in currentDataCollection)
                {
                    tags[i] = sound.Tag;
                    i++;
                }

                using (EnumGenerator enumGenerator = new EnumGenerator())
                {
                    string enumName = currentSection == Sections.Sounds ? "SFX" : "Track";
                    enumGenerator.GenerateEnum(enumName, tags);
                }

                SaveChanges();
            }
        }

        private void Search ()
        {
            EditorGUILayout.BeginVertical("box");
            CenterText("Search", 15, Color.white);
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tag:", GUILayout.Width(30));
            searchTag = EditorGUILayout.TextField(searchTag);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Clip:", GUILayout.Width(65));
            searchAudioClip = (AudioClip)EditorGUILayout.ObjectField(searchAudioClip, typeof(AudioClip), false);
            if (searchAudioClip != null)
            {
                if (GUILayout.Button("Clean", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    searchAudioClip = null;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
            EditorGUILayout.EndVertical();

            var newShowValuesCollection = new Dictionary<SoundData, bool>();
            foreach (SoundData soundData in currentTemplateDataCollection.Keys)
            {
                if (!string.IsNullOrEmpty(searchTag) && searchAudioClip == null)
                    newShowValuesCollection.Add(soundData, soundData.Tag.Contains(searchTag.ToLower()));
                else if (string.IsNullOrEmpty(searchTag) && searchAudioClip != null)
                    newShowValuesCollection.Add(soundData, soundData.Clips.Any(clip => clip == searchAudioClip));
                else if (!string.IsNullOrEmpty(searchTag) && searchAudioClip != null)
                    newShowValuesCollection.Add(soundData,
                        soundData.Tag.Contains(searchTag.ToLower()) &&
                        soundData.Clips.Any(clip => clip == searchAudioClip));
                else
                    newShowValuesCollection.Add(soundData, true);
            }

            if (inSoundSect) templateSoundDataCollection = newShowValuesCollection;
            else templateMusicDataCollection = newShowValuesCollection;
        }

        private void DeleteSoundData (SoundData soundDataToRemove)
        {
            if (inSoundSect)
            {
                if (soundDataCollection == null) return;

                soundDataCollection.RemoveSound(soundDataToRemove.Tag);
                EditorUtility.SetDirty(soundDataCollection);
            }
            else
            {
                if (musicDataCollection == null) return;

                musicDataCollection.RemoveMusicTrack(soundDataToRemove.Tag);
                EditorUtility.SetDirty(musicDataCollection);
            }

            string[] tags;
            int i = 0;
            tags = new string[currentDataCollection.Length];
            foreach (SoundData sound in currentDataCollection)
            {
                tags[i] = sound.Tag;
                i++;
            }

            using (EnumGenerator enumGenerator = new EnumGenerator())
            {
                string enumName = inSoundSect ? "SFX" : "Track";
                enumGenerator.GenerateEnum(enumName, tags);
            }

            SaveChanges();
        }

        private void DeleteAll ()
        {
            if (inSoundSect)
            {
                if (soundDataCollection == null) return;

                soundDataCollection.RemoveAll();
                EditorUtility.SetDirty(soundDataCollection);
            }
            else
            {
                if (musicDataCollection == null) return;

                musicDataCollection.RemoveAll();
                EditorUtility.SetDirty(musicDataCollection);
            }
            
            string[] tags = Array.Empty<string>();
            using (EnumGenerator enumGenerator = new EnumGenerator())
            {
                string enumName = inSoundSect ? "SFX" : "Track";
                enumGenerator.GenerateEnum(enumName, tags);
            }
            
            SaveChanges();
        }

        private void ToolSections ()
        {
            EditorGUILayout.BeginHorizontal();
            if (currentSection == Sections.Sounds && !Application.isPlaying) GUI.enabled = false;
            if (GUILayout.Button("Sounds", currentSection == Sections.Sounds ? greenButtonStyle : enabledButtonStyle))
            {
                GUI.FocusControl("");
                currentSection = Sections.Sounds;
                tryDeleteAll = false;
                soundDataDeleteTry = null;
            }

            if (currentSection == Sections.Sounds && !Application.isPlaying) GUI.enabled = true;
            if (currentSection == Sections.Music && !Application.isPlaying) GUI.enabled = false;
            if (GUILayout.Button("Music", currentSection == Sections.Music ? greenButtonStyle : enabledButtonStyle))
            {
                GUI.FocusControl("");
                currentSection = Sections.Music;
                tryDeleteAll = false;
                soundDataDeleteTry = null;
            }

            if (currentSection == Sections.Music && !Application.isPlaying) GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void SaveChanges (bool setDirty = false)
        {
            Refresh();

            if (setDirty)
            {
                if (inSoundSect) EditorUtility.SetDirty(soundDataCollection);
                else EditorUtility.SetDirty(musicDataCollection);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Refresh ()
        {
            templateSoundDataCollection.Clear();
            templateMusicDataCollection.Clear();
            foreach (SoundData soundData in soundDataCollection.Sounds)
            {
                AudioClip[] clips = new AudioClip[soundData.Clips.Length];
                for (int i = 0; i < clips.Length; i++) clips[i] = soundData.Clips[i];
                templateSoundDataCollection.Add(new SoundData(soundData.Tag, clips,
                    soundData.CompressionPreset, soundData.ForceToMono), true);
            }

            foreach (SoundData soundData in musicDataCollection.MusicTracks)
            {
                AudioClip[] clips = new AudioClip[soundData.Clips.Length];
                for (int i = 0; i < clips.Length; i++) clips[i] = soundData.Clips[i];
                templateMusicDataCollection.Add(new SoundData(soundData.Tag, clips,
                    soundData.CompressionPreset, soundData.ForceToMono), true);
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