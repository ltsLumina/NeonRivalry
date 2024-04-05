#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Abiogenesis3d
{
    class ShaderPreprocessor : IPreprocessShaders
    {
        public int callbackOrder { get { return 0; } }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (shader.name.Contains("Abiogenesis3d"))
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    #if UNITY_PIPELINE_URP
                    var keyword_URP = new ShaderKeyword("UNITY_PIPELINE_URP");
                    if (!data[i].shaderKeywordSet.IsEnabled(keyword_URP))
                        data[i].shaderKeywordSet.Enable(keyword_URP);
                    #endif

                    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
                    {
                        var keyword_WEBGL = new ShaderKeyword("UNITY_WEBGL");
                        if (!data[i].shaderKeywordSet.IsEnabled(keyword_WEBGL))
                            data[i].shaderKeywordSet.Enable(keyword_WEBGL);
                    }
                }
            }
        }
    }
}
#endif
