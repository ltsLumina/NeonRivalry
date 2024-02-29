Shader "TransitionsPlus/Cube"
{
    Properties
    {
        [HideInInspector] _T("Progress", Range(0, 1)) = 0
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _MaskTex ("Texture", 2D) = "white" {}
        [HideInInspector] _NoiseTex ("Noise", 2D) = "white" {}
        [HideInInspector] _FirstCameraTex ("First Camera Tex", 2D) = "white" {}
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
        [HideInInspector] _Center("Center", Vector) = (0,0,0)
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
            Name "Cube"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ TEXTURE GRADIENT_OPACITY GRADIENT_TIME GRADIENT_SPATIAL_RADIAL GRADIENT_SPATIAL_HORIZONTAL GRADIENT_SPATIAL_VERTICAL

            #include "TransitionsCommon.cginc"

            #define BIG 9999999
            float3 Rotate(float3 v, float angle) {
                float s = sin(angle);
                float c = cos(angle);
                float newX = v.x * c + v.z * s;
                float newZ = -v.x * s + v.z * c;
                return float3(newX, v.y, newZ);
            }

            float2 IntersectionQuad(float3 rayOrigin, float3 rayDirection, float3 v0, float3 v1, float3 v2, float3 v3, float angle, float d, out float t) {

                t = BIG;
                v0 = Rotate(v0, angle);
                v1 = Rotate(v1, angle);
                v2 = Rotate(v2, angle);
                v3 = Rotate(v3, angle);

                v0.z += d;
                v1.z += d;
                v2.z += d;
                v3.z += d;

                float3 quadNormal = normalize(cross(v1 - v0, v2 - v0));
                float denominator = dot(rayDirection, quadNormal);
                if (abs(denominator) < 1e-6) return -1;
                t = dot(v0 - rayOrigin, quadNormal) / denominator;
                if (t < 0) {
                    t = BIG;
                    return -1;
                }

                float3 intersectionPoint = rayOrigin + t * rayDirection;

                float3 e1 = v1 - v0;
                float3 e2 = v3 - v0;
                float3 i1 = intersectionPoint - v0;

                float u = dot(i1, e1) / dot(e1, e1);
                float v = dot(i1, e2) / dot(e2, e2);

                if (u >= 0.0 && u <= 1.0 && v >= 0.0 && v <= 1.0) {
                    return float2(u, v);
                }

                t = BIG;
                return -1;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayOrigin = float3(0, 0, -2);
                float3 rayDirection = normalize(float3(i.uv.xy - 0.5, 0.5));

                float angle = -0.5 * PI * _T;
                float t0;
                float d = sin(PI * _T);

                float3 v0 = float3(-1, -1, -1);
                float3 v1 = float3( 1, -1, -1);
                float3 v2 = float3(-1,  1, -1);
                float3 v3 = float3( 1,  1, -1);
                float2 uv = IntersectionQuad(rayOrigin, rayDirection, v0, v1, v3, v2, angle, d, t0);

                float t1;
                float3 w0 = float3(-1, -1, 1);
                float3 w1 = float3(-1, -1, -1);
                float3 w2 = float3(-1,  1, -1);
                float3 w3 = float3(-1,  1, 1);
                float2 uv2 = IntersectionQuad(rayOrigin, rayDirection, w0, w1, w2, w3, angle, d, t1);

                fixed4 color = uv.x < 0 ? fixed4(0,0,0,1) : tex2D(_FirstCameraTex, uv);
                fixed4 finalColor = uv2.x < 0 ? fixed4(0,0,0,1) : ComputeOutputColor(uv2, uv2, 1.0);

                color.rgb = lerp(color.rgb, finalColor.rgb, t1 < t0);

                return color;

            }
            ENDCG
        }
    }
}
