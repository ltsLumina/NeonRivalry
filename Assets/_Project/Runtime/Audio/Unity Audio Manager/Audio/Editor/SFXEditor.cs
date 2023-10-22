using FLZ.Utils;
using UnityEditor;
using UnityEngine;

namespace FLZ.Audio.Editor
{
    [CustomEditor(typeof(SFX))]
    public class SFXEditor : AudioPreviewEditor
    {
        private SFX _sfx;
        private SoundSettings _lastSettings;
        
        private SerializedProperty _mixerProp;
        private SerializedProperty _soundsProp;
        private SerializedProperty _volumeProp;
        private SerializedProperty _pitchProp;
        private SerializedProperty _delayProp;

        private bool _showVolume;
        
        private TimeSinceEditor _timeSinceLastPlay;

        protected override void OnEnable()
        {
            _sfx = target as SFX;
            
            base.OnEnable();

            _mixerProp = serializedObject.FindProperty("Mixer");
            _soundsProp = serializedObject.FindProperty("_sounds");
            
            _volumeProp = serializedObject.FindProperty("VolumeRange");
            _pitchProp = serializedObject.FindProperty("PitchRange");
            _delayProp = serializedObject.FindProperty("DelayRange");

            EditorApplication.update += OnUpdate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (_audioPreviewSource.clip != null && _timeSinceLastPlay > _audioPreviewSource.clip.length / _audioPreviewSource.pitch)
            {
                if (_sfx.Loop)
                {
                    PlayPreview();
                }
                else
                {
                    StopPreview();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // SOUNDS LIST
            EditorGUILayout.PropertyField(_soundsProp, new GUIContent("Sounds"));
            EditorGUILayout.PropertyField(_mixerProp, new GUIContent("Mixer"));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            
            // VOLUME
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Volume", EditorStyles.boldLabel);
            var globalVolume = EditorGUILayout.Slider("Global volume", _sfx.GlobalVolume, 0, 1);
            var randomizeVolume = EditorGUILayout.Toggle("Randomize", _sfx.RandomizeVolume);
        
            EditorGUI.BeginDisabledGroup(!randomizeVolume);
            EditorGUILayout.PropertyField(_volumeProp, new GUIContent("Range"));
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();

            GUILayout.Space(10);
            
            
            // PITCH
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Pitch", EditorStyles.boldLabel);
            var randomizePitch = EditorGUILayout.Toggle("Randomize", _sfx.RandomizePitch);
            
            EditorGUI.BeginDisabledGroup(!randomizePitch);
                EditorGUILayout.PropertyField(_pitchProp, new GUIContent("Range"));
            EditorGUI.EndDisabledGroup();
            
            GUILayout.EndVertical();

            GUILayout.Space(10);
            
            
            // DELAY 
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Delay", EditorStyles.boldLabel);
            var randomizeDelay = EditorGUILayout.Toggle("Randomize", _sfx.RandomizeDelay);

            EditorGUI.BeginDisabledGroup(!randomizeDelay);
            EditorGUILayout.PropertyField(_delayProp, new GUIContent("Delay"));
            EditorGUI.EndDisabledGroup();
                
            GUILayout.EndVertical();
            
            GUILayout.Space(10);

            // MISC
            GUILayout.BeginVertical("box");

            GUILayout.Label("Misc", EditorStyles.boldLabel);
            var panStereo = EditorGUILayout.Slider("Pan Stereo", _sfx.PanStereo, -1f, 1f);
            var loop = EditorGUILayout.Toggle("Loop", _sfx.Loop);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            
            EditorGUI.BeginDisabledGroup(_sfx == null || _audioPreviewSource == null || _sfx.GetSoundCount() < 1 || _sfx.HasAnyNullClip());
            DrawPreview();
            EditorGUI.EndDisabledGroup();

            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_sfx, "Undo Inspector");
                _sfx.GlobalVolume = globalVolume;
                _sfx.RandomizeVolume = randomizeVolume;
                _sfx.RandomizePitch = randomizePitch;
                _sfx.RandomizeDelay = randomizeDelay;
                _sfx.Loop = loop;
                _sfx.PanStereo = panStereo;
                
                EditorUtility.SetDirty(this);
            }
            
            EditorGUILayout.Space();
            if (_sfx.HasAnyNullClip())
                EditorGUILayout.HelpBox("Sounds list has a null audio clip", MessageType.Error);
            
            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }
        
        protected override void PlayPreview()
        {
            var soundSettings = _sfx.GetSoundSettings();
            _audioPreviewSource.ApplySettings(soundSettings);
            _audioPreviewSource.outputAudioMixerGroup = _sfx.Mixer;
            
            _lastSettings = soundSettings;
            _delay = soundSettings.Delay;
            _timeSinceLastPlay = 0 - soundSettings.Delay;

            base.PlayPreview();
        }

        protected override void StopPreview()
        {
            _timeSinceLastPlay = float.MinValue;
            base.StopPreview();
        }

        protected override void DrawLabels()
        {
            base.DrawLabels();
            
            GUILayout.Label($"Delay: {_lastSettings.Delay:0.00}s", _labelStyle);
        }
    }
}
