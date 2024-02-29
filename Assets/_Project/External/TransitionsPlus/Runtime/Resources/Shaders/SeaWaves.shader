Shader "TransitionsPlus/SeaWaves"
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
            Name "Sea Waves"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ TEXTURE GRADIENT_OPACITY GRADIENT_TIME GRADIENT_SPATIAL_RADIAL GRADIENT_SPATIAL_HORIZONTAL GRADIENT_SPATIAL_VERTICAL
            #pragma multi_compile_local _ TOON

            #include "TransitionsCommon.cginc"
        

            fixed4 frag (v2f i) : SV_Target
            {
                RotateUV(i.uv);

                fixed fade = 0;
                for (int k=0;k<_Splits;k++) {
                    float direction = ((uint)k % 2) * 2.0 - 1.0;
                    fixed thisFade = sin(i.uv.x * PI * 2.0 + PI * (float)k / _Splits + _Time.z * direction) * 0.1;
                    thisFade -= i.uv.y - _T * (1.0 + 0.2 * _Splits) - (1.0 - 0.1 * _Splits) - (0.1 * k);
                    thisFade = thisFade > 1.0;
                    thisFade /= (k + 1);
                    fade = max(fade, thisFade);
                }
                
                fade = saturate(fade);
                fixed4 color = ComputeOutputColor(i.uv, i.noiseUV, fade);

                return color; 

            }
            ENDCG
        }
    }
}
