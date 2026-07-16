Shader "Custom/SquareLavaFlow_WorldSpace"
{
    Properties
    {
        _MainTex ("岩浆纹理", 2D) = "white" {}
        _NoiseTex ("扰动噪波", 2D) = "white" {}
        [Toggle(USE_XY_PLANE)] _UseXYPlane ("使用XY平面(Quad立面)", Float) = 0
        _WorldTileScale ("世界空间平铺缩放", Float) = 1.0
        _FlowSpeed ("流动速度", Vector) = (0.1, 0.2, 0, 0)
        _Turbulence ("扰动强度", Float) = 0.15
        _LavaColor1 ("暗岩浆", Color) = (0.8, 0.2, 0, 1)
        _LavaColor2 ("亮岩浆", Color) = (1, 0.7, 0.1, 1)
        _EdgeGlow ("边缘灼烧强度", Float) = 0.3
        _GlowColor ("火光颜色", Color) = (1, 0.4, 0, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_XY_PLANE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 localUV      : TEXCOORD0;
                float3 worldPos     : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                SAMPLER(sampler_MainTex);
                SAMPLER(sampler_NoiseTex);
                TEXTURE2D(_MainTex);
                TEXTURE2D(_NoiseTex);
                float4 _MainTex_ST;
                float4 _NoiseTex_ST;
                float _UseXYPlane;
                float _WorldTileScale;
                float2 _FlowSpeed;
                float _Turbulence;
                float4 _LavaColor1;
                float4 _LavaColor2;
                float _EdgeGlow;
                float4 _GlowColor;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.localUV = input.uv;
                output.worldPos = TransformObjectToWorld(input.positionOS.xyz);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // ========== 根据开关选择世界坐标轴向 ==========
                float2 worldUV;
                #ifdef USE_XY_PLANE
                    worldUV = input.worldPos.xy * _WorldTileScale;
                #else
                    worldUV = input.worldPos.xz * _WorldTileScale;
                #endif

                // 应用Tiling和Offset
                float2 mainUV = worldUV * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 noiseUV = worldUV * _NoiseTex_ST.xy + _NoiseTex_ST.zw;

                // 流动偏移
                float2 flowOffset = _Time.y * _FlowSpeed;
                mainUV += flowOffset;
                noiseUV += flowOffset * 0.5;

                // 噪波扭曲
                half noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;
                float2 distortUV = mainUV + (noise - 0.5) * _Turbulence;

                // 岩浆颜色混合
                half lavaTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortUV).r;
                half3 lavaCol = lerp(_LavaColor1.rgb, _LavaColor2.rgb, lavaTex);

                // ========== 方块边缘发光（本地UV计算，每个方块独立） ==========
                float uvX = abs(input.localUV.x - 0.5) * 2;
                float uvY = abs(input.localUV.y - 0.5) * 2;
                float edge = max(uvX, uvY);
                edge = saturate((edge - 0.8) * 5);
                half3 edgeGlow = _GlowColor.rgb * edge * _EdgeGlow;

                half3 finalCol = lavaCol + edgeGlow;
                half alpha = saturate(lavaTex + edge * 0.4);
                return half4(finalCol, alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/Unlit/Transparent"
}