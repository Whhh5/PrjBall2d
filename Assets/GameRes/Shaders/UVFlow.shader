Shader "Effects/UVFlow"
{
    Properties
    {
        [HDR]_TintColor ("主颜色", Color) = (1,1,1,1)
        _MainTex ("主纹理", 2D) = "white" {}
        _MainSpeed("主纹理偏移",Vector)=(0,0,0,0)
        _MaskTex ("蒙版纹理", 2D) = "white" {}
        _MaskSpeed("蒙版纹理偏移",Vector)=(0,0,0,0)
        _Alpha("透明度",Range(0.00,1)) = 1


        [HideInInspector] _ColorMode("ColorMode",Float)=0  //Blend=0,Addtive=1 
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _TwoSide("TwoSide",Float)=0
        [HideInInspector]_Ztest ("Ztest", Float) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" "PreviewType"="Plane"}
        Blend [_SrcBlend] [_DstBlend]
        Cull [_TwoSide] Lighting Off ZWrite Off
        ZTest [_Ztest]
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            float _Alpha;

            float4 _MainSpeed;
            float4 _MaskSpeed;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color =  v.color*_TintColor;
                o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex)+ frac(float2(_MainSpeed.xy) * _Time.y);
                o.texcoord.zw = TRANSFORM_TEX(v.texcoord,_MaskTex) + frac(float2(_MaskSpeed.xy) * _Time.y);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col=tex2D(_MainTex, i.texcoord.xy)*i.color;
                fixed4 tex2 = tex2D (_MaskTex, i.texcoord.zw);
          
                col.a*=tex2.a*tex2.r *_Alpha;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    // CustomEditor "CustomBaseParticleGUI"
}
