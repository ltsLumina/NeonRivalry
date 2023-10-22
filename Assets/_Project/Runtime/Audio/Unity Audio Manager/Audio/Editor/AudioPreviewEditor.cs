using System.IO;
using UnityEditor;
using UnityEngine;

namespace FLZ.Audio.Editor
{
    public class AudioPreviewEditor : UnityEditor.Editor
    {
        protected AudioSource _audioPreviewSource;
        private static bool _autoPlay;
        
        private AudioClip _lastClip;
        private string _extension;
        
        protected bool _isPlaying;
        protected float _delay = 0;

        private readonly string AUDIOPREVIEW_AUTOPLAY;
        protected GUIStyle _labelStyle;


        protected virtual void OnEnable()
        {
            _audioPreviewSource = EditorUtility.CreateGameObjectWithHideFlags("Audio Preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
            _autoPlay = EditorPrefs.GetBool(AUDIOPREVIEW_AUTOPLAY, true);

            if (_autoPlay)
            {
                PlayPreview();
            }
        }
        
        protected virtual void OnDisable()
        {
            DestroyImmediate(_audioPreviewSource.gameObject);
        }
        
        protected virtual void DrawPreview()
        {
            GUILayout.BeginVertical("box");
            GUIStyle previewStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
    
            GUILayout.Label("Preview", previewStyle);
    
            GUILayout.Space(6);
    
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (_audioPreviewSource.isPlaying)
            {
                GUI.color = new Color(0.65f,0.76f,0.66f);
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Animation.Play").image))
            {
                PlayPreview();
            }
            GUI.color = Color.white;

            EditorGUI.BeginDisabledGroup(!_isPlaying);
           
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_PreMatQuad@2x").image, GUILayout.MaxHeight(20)))
            {
                StopPreview();
            }

            EditorGUI.EndDisabledGroup();

            _autoPlay = GUILayout.Toggle(_autoPlay, EditorGUIUtility.IconContent("d_preAudioAutoPlayOff"), "Button");
            EditorPrefs.SetBool(AUDIOPREVIEW_AUTOPLAY, _autoPlay);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
    
            GUILayout.Space(5);
    
            if (_audioPreviewSource.clip != null)
            {
                DrawLabels();
                GUILayout.EndVertical();
            }
    
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }
        

        protected virtual void PlayPreview()
        {
            if (_lastClip != _audioPreviewSource.clip)
            {
                _lastClip = _audioPreviewSource.clip;
                _extension = Path.GetExtension(AssetDatabase.GetAssetPath(_audioPreviewSource.clip));
            }
            
            if (_audioPreviewSource.loop)
            {
                _audioPreviewSource.loop = false;
            }
            
            // Unmute the editor if muted
            if (EditorUtility.audioMasterMute)
                EditorUtility.audioMasterMute = false;

            _audioPreviewSource.PlayDelayed(_delay);
            _isPlaying = true;
            
            Repaint();
        }

        protected virtual void StopPreview()
        {
            _audioPreviewSource.Stop();
            _isPlaying = false;
            
            Repaint();
        }


        protected virtual void DrawLabels()
        {
            GUIStyle miniLabelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
    
            GUIStyle miniGreyLabelstyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            _labelStyle = _isPlaying ? miniLabelStyle : miniGreyLabelstyle;
                
            GUILayout.BeginVertical();
            var clip = _audioPreviewSource.clip;

            GUILayout.BeginHorizontal();

            // Clip name
            GUILayout.Label(new GUIContent(
                $" {clip.name}{_extension} ({clip.length / _audioPreviewSource.pitch :0.00}s)", 
                EditorGUIUtility.IconContent(_isPlaying ? "d_SceneViewAudio On@2x" : "d_SceneViewAudio@2x").image), 
                new GUIStyle(EditorStyles.miniBoldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                    
                },  GUILayout.MaxHeight(25));
            GUILayout.EndHorizontal();
            
            Color oldColor = GUI.contentColor;
            if (_audioPreviewSource.volume == 0)
            {
                GUI.contentColor = Color.red;
            }
            
            // Volume
            GUILayout.Label("Volume: " + _audioPreviewSource.volume.ToString("0.00"), _labelStyle);
            GUI.contentColor = oldColor;
 
            // Pitch
            GUILayout.Label("Pitch: " + _audioPreviewSource.pitch.ToString("0.00"), _labelStyle);
    
            // Pan stereo
            GUILayout.Label("Pan stereo: " + _audioPreviewSource.panStereo.ToString("0.00"), _labelStyle);
        }
    }
}