#if UNITY_EDITOR
#if UNITY_PIPELINE_URP
// https://forum.unity.com/threads/urp-adding-a-renderfeature-from-script.1117060/
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Abiogenesis3d
{
    public static class SetupRenderFeatures
    {
        public static void SetDownsamplingToNone(UniversalRenderPipelineAsset urpAsset)
        {
            FieldInfo fieldInfo_OpaqueDownsampling = typeof(UniversalRenderPipelineAsset).GetField("m_OpaqueDownsampling", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo_OpaqueDownsampling != null)
            {
                if ((Downsampling)fieldInfo_OpaqueDownsampling.GetValue(urpAsset) != Downsampling.None)
                {
                    Debug.Log("Setting OpaqueDownsampling to None");
                    fieldInfo_OpaqueDownsampling.SetValue(urpAsset, Downsampling.None);
                }
            }
        }

        // public static List<T> AddAndGetRendererFeatures<T>() where T : ScriptableRendererFeature
        public static List<T> AddAndGetRendererFeatures<T>(List<UniversalRenderPipelineAsset> urpAssets) where T : ScriptableRendererFeature
        {
            List<T> features = new List<T>();

            var handledDataObjects = new List<ScriptableRendererData>();

            foreach (var urpAsset in urpAssets)
            {
                // Do NOT use asset.LoadBuiltinRendererData().
                // It's a trap, see: https://github.com/Unity-Technologies/Graphics/blob/b57fcac51bb88e1e589b01e32fd610c991f16de9/Packages/com.unity.render-pipelines.universal/Runtime/Data/UniversalRenderPipelineAsset.cs#L719
                var data = getDefaultRenderer(urpAsset);

                // This is needed in case multiple renderers share the same renderer data object.
                // If they do then we only handle it once.
                if (handledDataObjects.Contains(data))
                    continue;

                handledDataObjects.Add(data);

                bool found = false;
                foreach (var feature in data.rendererFeatures)
                {
                    if (feature is T)
                    {
                        found = true;
                        features.Add(feature as T);
                        break;
                    }
                }

                if (!found)
                {
                    var feature = ScriptableObject.CreateInstance<T>();
                    feature.name = typeof(T).Name;
                    addRenderFeature(data, feature);
                    features.Add(feature);
                }
            }

            return features;
        }

        /// <summary>
        /// Gets the default renderer index.
        /// Thanks to: https://forum.unity.com/threads/urp-adding-a-renderfeature-from-script.1117060/#post-7184455
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        static int getDefaultRendererIndex(UniversalRenderPipelineAsset asset)
        {
            return (int)typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(asset);
        }

        /// <summary>
        /// Gets the renderer from the current pipeline asset that's marked as default.
        /// Thanks to: https://forum.unity.com/threads/urp-adding-a-renderfeature-from-script.1117060/#post-7184455
        /// </summary>
        /// <returns></returns>
        static ScriptableRendererData getDefaultRenderer(UniversalRenderPipelineAsset asset)
        {
            if (asset)
            {
                ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset)
                        .GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(asset);
                int defaultRendererIndex = getDefaultRendererIndex(asset);

                return rendererDataList[defaultRendererIndex];
            }
            else
            {
                Debug.LogError("No Universal Render Pipeline is currently active.");
                return null;
            }
        }

        /// <summary>
        /// Based on Unity add feature code.
        /// See: AddComponent() in https://github.com/Unity-Technologies/Graphics/blob/d0473769091ff202422ad13b7b764c7b6a7ef0be/com.unity.render-pipelines.universal/Editor/ScriptableRendererDataEditor.cs#180
        /// </summary>
        /// <param name="data"></param>
        /// <param name="feature"></param>
        static void addRenderFeature(ScriptableRendererData data, ScriptableRendererFeature feature)
        {
            // Let's mirror what Unity does.
            var serializedObject = new SerializedObject(data);

            var renderFeaturesProp = serializedObject.FindProperty("m_RendererFeatures"); // Let's hope they don't change these.
            var renderFeaturesMapProp = serializedObject.FindProperty("m_RendererFeatureMap");

            serializedObject.Update();

            // Store this new effect as a sub-asset so we can reference it safely afterwards.
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(data))
                AssetDatabase.AddObjectToAsset(feature, data);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out var guid, out long localId);

            // Grow the list first, then add - that's how serialized lists work in Unity
            renderFeaturesProp.arraySize++;
            var componentProp = renderFeaturesProp.GetArrayElementAtIndex(renderFeaturesProp.arraySize - 1);
            componentProp.objectReferenceValue = feature;

            // Update GUID Map
            renderFeaturesMapProp.arraySize++;
            var guidProp = renderFeaturesMapProp.GetArrayElementAtIndex(renderFeaturesMapProp.arraySize - 1);
            guidProp.longValue = localId;

            // Force save / refresh
            if (EditorUtility.IsPersistent(data))
            {
                AssetDatabase.SaveAssetIfDirty(data);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
#endif
