Shader "Custom/MultipleBallSpiralMerge"
{
    Properties
    {
        [NoScaleOffset] _BallTexArray ("小球纹理数组 (2DArray)", 2DArray) = "white" {}
        _BallBig ("最终大球", 2D) = "white" {}

        _BallCount ("小球数量", Int) = 2
        _StartRadius ("初始分布半径", Range(0.1, 0.5)) = 0.25
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

            // 最大支持小球数量，可按需调整
            #define MAX_BALL_COUNT 8

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

            // 小球纹理数组
            UNITY_DECLARE_TEX2DARRAY(_BallTexArray);
            sampler2D _BallBig;

            int _BallCount;
            float _StartRadius;
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
                // 1. 多小球螺旋运动位置计算
                // --------------------------
                float angle = t * _SpiralRotations * UNITY_PI * 2;
                float spiralFactor = _SpiralStrength * t;
                float radiusSmall = lerp(_SmallRadius, _BigRadius * 0.65, smoothstep(0.3, 0.8, t));

                float2 ballPositions[MAX_BALL_COUNT];

                for (int idx = 0; idx < MAX_BALL_COUNT; idx++)
                {
                    if (idx >= _BallCount) break;

                    // 按数量均匀分配初始角度，沿圆周逆时针分布
                    float startAngle = (float)idx / _BallCount * UNITY_PI * 2;
                    // 若需对齐原双球左右分布，可改为：
                    // float startAngle = (float)idx / _BallCount * UNITY_PI * 2 + UNITY_PI;
                    float2 startPos = center + float2(cos(startAngle), sin(startAngle)) * _StartRadius;

                    float dist = _StartRadius;
                    float2 dir = normalize(center - startPos);
                    float2 tangent = float2(-dir.y, dir.x);

                    float2 pos = startPos + dir * t * dist + tangent * spiralFactor * dist * 0.5;
                    pos = lerp(pos, center, smoothstep(0.35, 0.95, t));

                    ballPositions[idx] = pos;
                }

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
                // 3. SDF融合 + 多球纹理混合
                // --------------------------
                float radiusBig = lerp(_SmallRadius * 1.2, _BigRadius, smoothstep(0.6, 1.0, t));
                float edgeNoise = (n1 - 0.5) * _EdgeMelt;

                float mergedSmall = 1.0;
                fixed4 mixedSmall = fixed4(0, 0, 0, 0);
                float wTotal = 0.001;

                for (int j = 0; j < MAX_BALL_COUNT; j++)
                {
                    if (j >= _BallCount) break;

                    float d = length(twistedUV - ballPositions[j]) - radiusSmall;
                    d += edgeNoise;
                    mergedSmall = smin(mergedSmall, d, _FusionSmoothness);

                    // 采样对应索引的小球纹理
                    float2 ballUV = (twistedUV - ballPositions[j]) / (2 * radiusSmall) + 0.5;
                    fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_BallTexArray, float3(ballUV, j));
                    float w = 1 - smoothstep(0, _FusionSmoothness * 2, d);

                    mixedSmall += col * w;
                    wTotal += w;
                }
                mixedSmall /= wTotal;

                float dBig = length(twistedUV - center) - radiusBig;
                dBig += edgeNoise * 0.5;

                float finalSDF = lerp(mergedSmall, dBig, smoothstep(0.5, 0.9, t));

                // --------------------------
                // 4. 大球纹理采样与最终混合
                // --------------------------
                float2 uvBig = (lerp(twistedUV, uv, smoothstep(0.85, 1, t)) - center) / (1.5 * radiusBig) + 0.5;
                fixed4 colBig = tex2D(_BallBig, uvBig);
                fixed4 finalCol = lerp(mixedSmall, colBig, smoothstep(0.4, 0.95, t));

                // --------------------------
                // 5. 漩涡发光边缘
                // --------------------------
                float glowMask = smoothstep(0.3, 0.5, t) * (1 - smoothstep(0.5, 0.7, t));
                float edgeGlow = smoothstep(0, 0.02, finalSDF) * smoothstep(0.06, 0, finalSDF);

                finalCol.rgb += float3(0.45, 0.6, 0.9) * edgeGlow * glowMask * 1.8f;

                // --------------------------
                // 6. Alpha
                // --------------------------
                float alpha = 1 - smoothstep(0, 0.005, finalSDF);
                finalCol.a *= alpha;

                return finalCol;
            }

            ENDCG
        }
    }
}