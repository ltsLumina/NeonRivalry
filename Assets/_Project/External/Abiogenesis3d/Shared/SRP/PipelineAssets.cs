#if UNITY_PIPELINE_URP
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Abiogenesis3d
{
    public static class PipelineAssets
    {
        public static bool ShouldUseHDR()
        {
            var asset = GetUrpAsset();
            return (
                asset &&
                asset.supportsHDR &&
                SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR)
            );
        }

        public static List<UniversalRenderPipelineAsset> GetUrpAssets()
        {
            var assets = new List<UniversalRenderPipelineAsset>();
            int levels = QualitySettings.names.Length;

            for (int level = 0; level < levels; level++)
            {
                var asset = QualitySettings.GetRenderPipelineAssetAt(level) as UniversalRenderPipelineAsset;

                if (!asset) asset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
                if (!asset) continue;

                assets.Add(asset);
            }

            return assets;
        }

        public static UniversalRenderPipelineAsset GetUrpAsset()
        {
            var currentLevel = QualitySettings.GetQualityLevel();
            var asset = QualitySettings.GetRenderPipelineAssetAt(currentLevel) as UniversalRenderPipelineAsset;
            if (!asset) asset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            return asset;
        }
    }
}
#endif