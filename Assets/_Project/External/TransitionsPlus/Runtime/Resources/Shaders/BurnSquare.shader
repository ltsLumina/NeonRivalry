Shader "TransitionsPlus/BurnSquare"
{
    Properties
    {
        [HideInInspector] _T("Progress", Range(0, 1)) = 0
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _NoiseTex ("Noise", 2D) = "white" {}
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
            Name "Square Burn"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ TEXTURE GRADIENT_OPACITY GRADIENT_TIME GRADIENT_SPATIAL_RADIAL GRADIENT_SPATIAL_HORIZONTAL GRADIENT_SPATIAL_VERTICAL
            #pragma multi_compile_local _ TOON

            #define GRADIENT_ON_TIME
            #define MAX_CENTERS 32

            #include "TransitionsCommon.cginc"
        
            float4 _Centers[MAX_CENTERS]; // xy and radius multiplier

            int _CentersCount;

            fixed4 frag (v2f i) : SV_Target
            {

                RotateUV(i.uv);

                fixed t = 1.0 - _T;

                // compute distance
                float aspect = GetAspectRatio();
                float minDist = 10;
                for(int k=0; k<_CentersCount; k++) {
                    float2 delta = abs(_Centers[k].xy - i.uv);
                    delta.x *= aspect;
                    float dist = max(delta.x, delta.y);
                    float centerRadius = _Centers[k].z;
                    dist *= centerRadius;
                    float timeShift = _Centers[k].w;
                    dist += saturate( (t - timeShift) / (1.0 - timeShift) );
                    if (dist < minDist) {
                        minDist = dist;
                    }
                }

                fixed radius = ComputeNoise(i.noiseUV);
                radius *= 0.01 / (t + 0.001);

                fixed fade = saturate(radius / minDist);

                fixed4 color = ComputeOutputColor(i.uv, i.noiseUV, fade);

                return color; 

            }
            ENDCG
        }
    }
}
