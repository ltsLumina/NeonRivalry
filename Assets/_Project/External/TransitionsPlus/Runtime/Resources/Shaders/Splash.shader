Shader "TransitionsPlus/Splash"
{
    Properties
    {
        [HideInInspector] _T("Progress", Range(0, 1)) = 0
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _NoiseTex ("Noise", 2D) = "white" {}
        [HideInInspector] _SplashTex ("Splash Pattern", 2D) = "black" {}
        [HideInInspector] _GradientTex ("Gradient Tex", 2D) = "white" {}
        [HideInInspector] _Color("Color", Color) = (0,0,0)
        [HideInInspector] _VignetteIntensity("Vignette Intensity", Range(0,1)) = 0.5
        [HideInInspector] _NoiseIntensity("Noise Intensity", Range(0,1)) = 0.5
        [HideInInspector] _ToonIntensity("Toon Intensity", Float) = 1
        [HideInInspector] _ToonDotIntensity("Toon Dot Intensity", Float) = 1
        [HideInInspector] _AspectRatio("Aspect Ratio", Float) = 1
        [HideInInspector] _Distortion("Distortion", Float) = 1
        [HideInInspector] _ToonDotRadius("Toon Dot Radius", Float) = 0
        [HideInInspector] _ToonDotCount("Toon Dot Count", Float) = 0
        [HideInInspector] _Contrast("Constrast", Float) = 1
        [HideInInspector] _CellDivisions("Cell Divisions", Float) = 32
        [HideInInspector] _Spread("Spread", Float) = 64
        [HideInInspector] _RotationMultiplier("Rotation", Float) = 0
        [HideInInspector] _Rotation("Rotation", Float) = 0
        [HideInInspector] _Splits("Splits", Float) = 2
        [HideInInspector] _Seed("Seed", Float) = 0
        [HideInInspector] _CentersCount("Seed", Int) = 1
        [HideInInspector] _TimeMultiplier("Time Multiplier", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "Splash"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ TEXTURE GRADIENT_OPACITY GRADIENT_TIME GRADIENT_SPATIAL_RADIAL GRADIENT_SPATIAL_HORIZONTAL GRADIENT_SPATIAL_VERTICAL
            #pragma multi_compile_local _ TOON

            #include "TransitionsCommon.cginc"

            sampler2D _SplashTex;

            #define MAX_CENTERS 32
            float4 _Centers[MAX_CENTERS]; // xy and radius multiplier

            int _CentersCount;

            fixed4 frag (v2f i) : SV_Target
            {
                float aspect = _AspectRatio ? GetAspectRatio() : 1.0;
                i.uv.x = 0.5 + (i.uv.x - 0.5) * aspect;
                i.uv.y = 0.5 + (i.uv.y - 0.5) / aspect;

                RotateUV(i.uv);

                fixed t = 0.99999 - _T;

                fixed radius = ComputeNoise(i.noiseUV);
                float expand = 0.01 / (t + 0.001);
                radius *= expand;

                fixed fade = 0;

                // compute distance
                for(int k=0; k<_CentersCount; k++) {
                    float2 delta = abs(_Centers[k].xy - i.uv);
                    float dist = max(delta.x, delta.y);
                    float centerRadius = _Centers[k].z;
                    dist *= centerRadius;
                    float timeShift = _Centers[k].w;
                    dist += saturate( (t - timeShift) / (1.0 - timeShift) );

                    fixed thisFade = saturate(radius / dist);

                    float2 maskUV = saturate( (i.uv - (_Centers[k].xy - thisFade)) / (0.0001 + thisFade * 2.0));
                    fixed mask = tex2Dlod(_SplashTex, float4(maskUV, 0, 0)).a;
                    fade = max(fade, mask);
                }

                fixed4 color = ComputeOutputColor(i.uv, i.noiseUV, fade);

                return color; 

            }
            ENDCG
        }
    }
}
