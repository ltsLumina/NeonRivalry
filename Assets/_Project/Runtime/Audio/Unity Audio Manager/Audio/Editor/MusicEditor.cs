// using FLZ.Audio.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace FLZ.Audio
// {
//     [CustomEditor(typeof(Music))]
//     public class MusicEditor : AudioPreviewEditor
//     {
//         private Music _music;
//
//         private SerializedProperty _intro;
//         private SerializedProperty _clip;
//         
//         protected override void OnEnable()
//         {
//             _music = target as Music;
//
//             base.OnEnable();
//             
//             _intro = serializedObject.FindProperty("Intro");
//             _clip = serializedObject.FindProperty("Clip");
//         }
//
//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();
//             
//             EditorGUI.BeginChangeCheck();
//
//             float volume;
//             float pitch = _music.Pitch;
//             bool loop = _music.Loop;
//             float panStereo = _music.PanStereo;
//             
//             GUILayout.BeginVertical("box");
//             EditorGUILayout.PropertyField(_intro, new GUIContent("Intro"));
//             EditorGUILayout.PropertyField(_clip, new GUIContent("Clip"));
//             GUILayout.EndVertical();
//
//             GUILayout.Space(10);
//
//             GUILayout.BeginVertical("box");
//             loop = EditorGUILayout.Toggle("Loop", _music.Loop);
//             volume = EditorGUILayout.Slider("Volume", _music.Volume, 0, 1);
//             pitch = EditorGUILayout.Slider("Pitch", _music.Pitch, 0.05f, 10);
//             panStereo = EditorGUILayout.Slider("Pan Stereo", _music.PanStereo, -1f, 1);
//             GUILayout.EndVertical();
//
//             GUILayout.Space(10);
//             
//             EditorGUI.EndDisabledGroup();
//             
//             if (EditorGUI.EndChangeCheck())
//             {
//                 Undo.RecordObject(_music, "Undo Inspector");
//                 _music.Volume = volume;
//                 _music.Pitch = pitch;
//                 _music.Loop = loop;
//                 _music.PanStereo = panStereo;
//                 
//                 EditorUtility.SetDirty(this);
//             }
//             
//             if (GUI.changed)
//                 serializedObject.ApplyModifiedProperties();
//             
//             EditorGUILayout.Space();
//             DrawPreview();
//         }
//
//         protected override void PlayPreview()
//         {
//             if (_music.Clip == null)
//                 return;
//
//             var soundSettings = _music.GetSoundSettings();
//
//             _audioPreviewSource.ApplySettings(soundSettings);
//             _audioPreviewSource.Play();
//             
//             base.PlayPreview();
//         }
//     }
// }