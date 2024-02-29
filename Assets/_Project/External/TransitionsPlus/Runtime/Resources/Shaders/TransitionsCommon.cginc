            #include "UnityCG.cginc"

            #define dot2(x) dot(x,x)
            #define PI 3.1415927
            #define SCREEN_CENTER 0.5.xx

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                float3 wpos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize; // x = 1/width, y = 1/height, z = width, w = height

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            sampler2D _GradientTex;
            sampler2D _MaskTex;
            float4 _MaskTex_TexelSize;

            sampler2D _FirstCameraTex;

            fixed _T, _TimeMultiplier;
            fixed4 _Color; 
            fixed _NoiseIntensity;
            fixed _VignetteIntensity;
            fixed _ToonIntensity;
            fixed _ToonDotIntensity;
            fixed _ToonDotRadius;
            fixed _ToonDotCount;
            fixed _Rotation, _RotationMultiplier;
            fixed _Distortion;
            fixed _AspectRatio;
            fixed _Contrast;
            int _Seed;
            int _Splits;            
            float2 _Center;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS = UnityObjectToClipPos(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _NoiseTex);
                o.wpos = mul(unity_ObjectToWorld, v.positionOS).xyz;
                return o;
            }

float Rand(float2 p) {
	float3 p3  = frac(float3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}

float2 Rotate(float2 uv, float angle) {
    float si, co;
    sincos(angle, si, co);
    float2x2 rotationMatrix = float2x2(
                co, -si,
                si, co
            );

    float2 rotatedUV = mul(rotationMatrix, uv);
    return rotatedUV;
}

float GetAspectRatio() {
    return _ScreenParams.x / _ScreenParams.y;
}

void RotateUV(inout float2 uv) {
    uv.y = _AspectRatio ? (uv.y - 0.5) * GetAspectRatio() + 0.5 : uv.y;
    float distortion = 1.0 - dot2(uv - 0.5.xx) * _Distortion;
    float angle = distortion * (_Rotation + (_T - 1.0) * _RotationMultiplier);
    uv = Rotate(uv - 0.5, angle) + 0.5;
}

float ComputeJitter(float2 uv) {
    uv *= _ScreenParams.xy;
    const float3 magic = float3( 0.06711056, 0.00583715, 52.9829189 );
    float jitter = frac( magic.z * frac( dot( uv, magic.xy ) ) ) - 0.5;
    return jitter;
}

fixed ComputeNoise(float2 uv) {
    fixed noise = tex2D(_NoiseTex, uv).r;
    noise = lerp(1.0, noise, _NoiseIntensity);
    return noise;
}

fixed ComputeVignetteWithNoise(float2 uv, float2 noiseUV, fixed T) {
    float aspect = _AspectRatio ? GetAspectRatio() : 1.0;
    uv.x = (uv.x - 0.5) * aspect;
    uv.y = (uv.y - 0.5);
    float t = dot2(uv);
    t *= ComputeNoise(noiseUV);
    t += 0.0001;
    t /= (1.00001 - T);

    return lerp(1.0, t, _VignetteIntensity);
}

fixed ComputeToonDot(float2 uv, float dotRadius) {
    uv.x *= GetAspectRatio();
    float2 ij = (floor(uv * _ToonDotCount) + 0.5) / _ToonDotCount;
    float d = distance(uv, ij);
    float radius = dotRadius / _ToonDotCount;
    fixed toonDot = smoothstep(radius * 0.95, radius * 1.05, d);
    return toonDot;
}


fixed4 ComputeOutputColor(float2 uv, float2 noiseUV, fixed fade) {
    #if TEXTURE
        fixed4 tex = tex2D(_MainTex, noiseUV);
        fixed4 color = _Color * tex;
    #else
        fixed4 color = _Color;
    #endif

    // apply toon dot
    #if TOON
        fixed toonDot = ComputeToonDot(noiseUV, _ToonDotRadius * fade);
        fade += (1.0 - toonDot * _ToonDotIntensity) * fade;

        // apply toon gradient
        if (fade > 0.3) {
            fade = ceil((fade * 255.0) / _ToonIntensity);
        } else {
            fade = floor( (fade * 255.0) / _ToonIntensity);
        }
        fade *= (_ToonIntensity / 255.0);
    #endif

    fade += _T >= _TimeMultiplier;

    fade = saturate(fade);
    
    // apply contrast
    fade = saturate(0.5 + (fade - 0.5) * _Contrast);

    fade = smoothstep(0, 1, fade);
    
    // apply gradient
    #if GRADIENT_TIME
        color.rgb *= tex2D(_GradientTex, float2(_T, 0));
    #elif GRADIENT_OPACITY
        color.rgb *= tex2D(_GradientTex, float2(fade, 0));
    #elif GRADIENT_SPATIAL_RADIAL
        color.rgb *= tex2D(_GradientTex, float2(distance(uv, 0.5.xx), 0));
    #elif GRADIENT_SPATIAL_HORIZONTAL
        color.rgb *= tex2D(_GradientTex, float2(uv.x, 0));
    #elif GRADIENT_SPATIAL_VERTICAL
        color.rgb *= tex2D(_GradientTex, float2(uv.y, 0));
    #endif

    // return final color with fade
    color.a *= fade;
    color = saturate(color);
    
    return color;
}

            