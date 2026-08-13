Shader "Custom/BallSpiralMerge"
{
    Properties
    {

        _BallA ("小球A", 2D) = "white" {}
        _BallB ("小球B", 2D) = "white" {}
        _BallBig ("最终大球", 2D) = "white" {}


        _BallAStart ("小球A初始位置", Vector) = (0.25, 0.5, 0, 0)
        _BallBStart ("小球B初始位置", Vector) = (0.75, 0.5, 0, 0)
        _MergeCenter ("融合中心", Vector) = (0.5, 0.5, 0, 0)
        _SmallRadius ("小球半径", Range(0.05, 0.35)) = 0.14
        _BigRadius ("大球半径", Range(0.15, 0.5)) = 0.28
        _FusionSmoothness ("融合平滑度", Range(0.005, 0.08)) = 0.025


        _SpiralStrength ("螺旋强度", Range(0, 2.0)) = 0.8
        _SpiralRotations ("旋转圈数", Range(0, 3)) = 1.5
        _VortexDistortion ("漩涡扭曲强度", Range(0, 0.3)) = 0.12
        _FlowSpeed ("流动速度", Range(0, 6)) = 2.5
        _NoiseScale ("噪波密度", Range(1, 12)) = 4
        _EdgeMelt ("边缘融化", Range(0, 0.08)) = 0.02


        _MergeProgress ("融合进度", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _BallA;
            sampler2D _BallB;
            sampler2D _BallBig;

            float4 _BallAStart;
            float4 _BallBStart;
            float4 _MergeCenter;
            float _SmallRadius;
            float _BigRadius;
            float _FusionSmoothness;

            float _SpiralStrength;
            float _SpiralRotations;
            float _VortexDistortion;
            float _FlowSpeed;
            float _NoiseScale;
            float _EdgeMelt;

            float _MergeProgress;

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float smin(float a, float b, float k)
            {
                float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
                return lerp(b, a, h) - k * h * (1.0 - h);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _MergeProgress;

                float2 center = _MergeCenter.xy;

                // --------------------------
                // 1. 双小球螺旋运动
                // --------------------------
                float angle = t * _SpiralRotations * UNITY_PI * 2;

                float distA = distance(_BallAStart.xy, center);
                float distB = distance(_BallBStart.xy, center);

                float spiralFactor = _SpiralStrength * t;

                float2 dirA = normalize(center - _BallAStart.xy);
                float2 dirB = normalize(center - _BallBStart.xy);

                float2 tangentA = float2(-dirA.y, dirA.x);
                float2 tangentB = float2(-dirB.y, dirB.x);

                float2 posA = _BallAStart.xy + dirA * t * distA + tangentA * spiralFactor * distA * 0.5;
                float2 posB = _BallBStart.xy + dirB * t * distB + tangentB * spiralFactor * distB * 0.5;

                posA = lerp(posA, center, smoothstep(0.35, 0.95, t));
                posB = lerp(posB, center, smoothstep(0.35, 0.95, t));

                // --------------------------
                // 2. 漩涡扭曲 UV
                // --------------------------
                float2 vortexUV = uv - center;
                float vortexDist = length(vortexUV);

                float vortexAngle = atan2(vortexUV.y, vortexUV.x);
                float vortexRot = angle * _SpiralStrength * smoothstep(0.2, 0.6, t);

                vortexAngle += vortexRot * (1 - vortexDist);

                float2 twistedUV = center + float2(cos(vortexAngle), sin(vortexAngle)) * vortexDist;

                float distortFactor = _VortexDistortion * (1 - abs(t - 0.5) * 2);

                float2 noiseUV = uv * _NoiseScale + _Time.x * _FlowSpeed * float2(1, 0.7);
                float n1 = noise(noiseUV);
                float n2 = noise(noiseUV + 50.0);

                float2 noiseOffset = float2(n1 - 0.5, n2 - 0.5) * distortFactor;

                twistedUV += noiseOffset;

                // --------------------------
                // 3. 形态融合 SDF
                // --------------------------
                float radiusA = lerp(_SmallRadius, _BigRadius * 0.65, smoothstep(0.3, 0.8, t));
                float radiusB = lerp(_SmallRadius, _BigRadius * 0.65, smoothstep(0.3, 0.8, t));
                float radiusBig = lerp(_SmallRadius * 1.2, _BigRadius, smoothstep(0.6, 1.0, t));

                float dA = length(twistedUV - posA) - radiusA;
                float dB = length(twistedUV - posB) - radiusB;
                float dBig = length(twistedUV - center) - radiusBig;

                float edgeNoise = (n1 - 0.5) * _EdgeMelt;
                dA += edgeNoise;
                dB += edgeNoise;
                dBig += edgeNoise * 0.5;

                float mergedSmall = smin(dA, dB, _FusionSmoothness);
                float finalSDF = lerp(mergedSmall, dBig, smoothstep(0.5, 0.9, t));

                // --------------------------
                // 4. 纹理采样
                // --------------------------
                float2 uvA = (twistedUV - posA) / (2 * radiusA) + 0.5;
                float2 uvB = (twistedUV - posB) / (2 * radiusB) + 0.5;
                float2 uvBig = (lerp(twistedUV, uv, smoothstep(0.85, 1, t)) - center) / (1.5 * radiusBig) + 0.5;

                fixed4 colA = tex2D(_BallA, uvA);
                fixed4 colB = tex2D(_BallB, uvB);
                fixed4 colBig = tex2D(_BallBig, uvBig);

                // --------------------------
                // 5. 纹理混合
                // --------------------------
                float wA = 1 - smoothstep(0, _FusionSmoothness * 2, dA);
                float wB = 1 - smoothstep(0, _FusionSmoothness * 2, dB);
                float wTotal = wA + wB + 0.001;

                fixed4 mixedSmall = (colA * wA + colB * wB) / wTotal;
                fixed4 finalCol = lerp(mixedSmall, colBig, smoothstep(0.4, 0.95, t));

                // --------------------------
                // 6. 漩涡发光边缘
                // --------------------------
                float glowMask = smoothstep(0.3, 0.5, t) * (1 - smoothstep(0.5, 0.7, t));
                float edgeGlow = smoothstep(0, 0.02, finalSDF) * smoothstep(0.06, 0, finalSDF);

                finalCol.rgb += float3(0.45, 0.6, 0.9) * edgeGlow * glowMask * 1.8f;

                // --------------------------
                // 7. Alpha
                // --------------------------
                float alpha = 1 - smoothstep(0, 0.005, finalSDF);
                finalCol.a *= alpha;

                return finalCol;
            }

            ENDCG
        }
    }
}