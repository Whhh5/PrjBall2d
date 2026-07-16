Shader "Effects/Particle"
{
    Properties
    {
        [HDR]_TintColor ("主颜色", Color) = (1,1,1,1)
        _MainTex ("主纹理", 2D) = "white" {}
        _Alpha("透明度",Range(0.00,1)) = 1
        [HideInInspector] _ColorMode("ColorMode",Float)=0  //Blend=0,Addtive=1 
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _TwoSide("TwoSide",Float)=0
        [HideInInspector]_Ztest ("Ztest", Float) = 2

        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        // 由于历史原因，ColorMask默认RGB
		[HideInInspector] _ColorMask ("Color Mask", Float) = 14


    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="true" "Queue"="Transparent" "PreviewType"="Plane"}
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]
        Cull [_TwoSide]
        Lighting Off ZWrite Off
        ZTest [_Ztest]

        Stencil{
        Ref [_Stencil]
        Pass[_StencilOp]
        Comp[_StencilComp]
        ReadMask[_StencilReadMask]
        WriteMask[_StencilWriteMask]
        }

        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
     

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            float _Alpha;
            float4 _MainTex_ST;
            fixed _ColorMode;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color ;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col=fixed4(0,0,0,0);

                if(_ColorMode<0.5)
                {
                   col = i.color * tex2D(_MainTex, i.texcoord)*_TintColor;
                   col.a = saturate(col.a*_Alpha); // alpha should not have double-brightness applied to it, but we can't fix that legacy behavior without breaking everyone's effects, so instead clamp the output to get sensible HDR behavior (case 967476)
                }
                else
                {
                   col = i.color * _TintColor * tex2D(_MainTex, i.texcoord);
                   col.a = saturate(col.a*_Alpha);
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    // CustomEditor "CustomBaseParticleGUI"
}
