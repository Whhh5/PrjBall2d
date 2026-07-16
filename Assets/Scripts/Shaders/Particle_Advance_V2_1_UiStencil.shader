Shader "Fruits/0EffectShader/UiShader/Particle_Advance_V2_1_UIStencil"
{
    Properties
    {
        [HDR]_TintColor("_TintColor", Color) = (0.75, 0.75, 0.75, 1)
        [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
        Main_TillingOffset("Main_TillingOffset", Vector) = (1, 1, 0, 0)
        _Width("Width", Int) = 1
        _Height("Height", Int) = 1
        _Tile("Tile", Range(-2, 2)) = 0
        _Rotation("Rotation", Range(0, 360)) = 0
        [ToggleUI]_______________________________2("-------------------------------", Float) = 0
        [NoScaleOffset]_WaveTexture("_WaveTexture", 2D) = "white" {}
        Wave_TilingOffset("Wave_TilingOffset", Vector) = (1, 1, 0, 0)
        Wave_Value("Wave_Value", Float) = 0
        [ToggleUI]_______________________________1("--------------------------------", Float) = 0
        [NoScaleOffset]_DissolveTex("_DissolveTex", 2D) = "white" {}
        Dissolve_TilingOffset("Dissolve_TilingOffset", Vector) = (1, 1, 0, 0)
        _DissRotation("DissRotation", Range(0, 360)) = 0
        Dissolve_Value("Dissolve_Value", Range(-1, 1)) = -0.5
        Dissolve_Smooth("Dissolve_Smooth", Range(0, 1)) = 0.1
        DissolveSide("DissolveSide", Range(0, 1)) = 0.1
        [HDR]DissolveColor("DissolveColor", Color) = (5.992157, 0, 0, 1)
        [ToggleUI]______________________________("------------------------------", Float) = 0
        [NoScaleOffset]_MaskTexture("_MaskTexture", 2D) = "white" {}
        Mask_TillingOffset("Mask_TillingOffset", Vector) = (1, 1, 0, 0)
        Mask_Value("Mask_Value", Range(1, 10)) = 1
        _Mask_Rotation("Mask_Rotation", Range(0, 360)) = 0
        [ToggleUI]_______________________________3("---------------------------------", Float) = 0
        [ToggleUI]Boolean_a0c7fbd5e5b84802acf08ee1c31217a8("Boolean", Float) = 0
        _InvFade("SoftParticles", Range(0.01, 3)) = 1
        [ToggleUI]_______________________________4("-----------------------------------", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        [KeywordEnum(Normal, Auto, CustomData)]MAIN_OFFSET_MODE("Main_Offset_Mode", Float) = 0
        [KeywordEnum(Normal, Auto, CustomData)]WAVE_OFFSET_MODE("Wave_Offset_Mode", Float) = 0
        [KeywordEnum(Normal, Auto, CustomData)]DISSOLVE_OFFSET_MODE("Dissolve_Offset_Mode", Float) = 0
        [KeywordEnum(FlipBook, Normal)]_MAINTEXTILINGSTYLE("MainTexTilingStyle", Float) = 0
        [KeywordEnum(Soft, Normal)]ENUM_757C0F1933D44CD6AF8836FFAC72D329("SoftParticles", Float) = 1
        //Stencil
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
    }
    SubShader
    {
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Pass"
            Tags {}

            // Render State
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            #pragma shader_feature_local MAIN_OFFSET_MODE_NORMAL MAIN_OFFSET_MODE_AUTO MAIN_OFFSET_MODE_CUSTOMDATA
            #pragma shader_feature_local WAVE_OFFSET_MODE_NORMAL WAVE_OFFSET_MODE_AUTO WAVE_OFFSET_MODE_CUSTOMDATA
            #pragma shader_feature_local DISSOLVE_OFFSET_MODE_NORMAL DISSOLVE_OFFSET_MODE_AUTO DISSOLVE_OFFSET_MODE_CUSTOMDATA
            #pragma shader_feature_local _MAINTEXTILINGSTYLE_FLIPBOOK _MAINTEXTILINGSTYLE_NORMAL
            #pragma shader_feature_local ENUM_757C0F1933D44CD6AF8836FFAC72D329_SOFT ENUM_757C0F1933D44CD6AF8836FFAC72D329_NORMAL
            
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_COLOR
           
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT

            #define REQUIRE_DEPTH_TEXTURE
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv2 : TEXCOORD2;
                float4 color : COLOR;
                uint instanceID : INSTANCEID_SEMANTIC;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float4 texCoord0;
                float4 texCoord2;
                float4 color;
                uint instanceID : CUSTOM_INSTANCE_ID;
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            };

            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
                float4 uv0;
                float4 uv2;
                float4 VertexColor;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float3 interp0 : TEXCOORD0;
                float4 interp1 : TEXCOORD1;
                float4 interp2 : TEXCOORD2;
                float4 interp3 : TEXCOORD3;
                uint instanceID : CUSTOM_INSTANCE_ID;
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;

                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif

                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                output.positionCS = input.positionCS;
                output.interp0.xyz = input.positionWS;
                output.interp1.xyzw = input.texCoord0;
                output.interp2.xyzw = input.texCoord2;
                output.interp3.xyzw = input.color;

                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif

                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif

                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif

                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif

                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp0.xyz;
                output.texCoord0 = input.interp1.xyzw;
                output.texCoord2 = input.interp2.xyzw;
                output.color = input.interp3.xyzw;
                output.instanceID = input.instanceID;
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;

                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif

                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif

                return output;
            }


            CBUFFER_START(UnityPerMaterial)
                float4 _TintColor;
                float4 _MainTex_TexelSize;
                float4 Main_TillingOffset;
                float _Width;
                float _Height;
                float _Tile;
                float _Rotation;

                float4 _WaveTexture_TexelSize;
                float4 Wave_TilingOffset;
                float Wave_Value;

                float4 _DissolveTex_TexelSize;
                float4 Dissolve_TilingOffset;
                float _DissRotation;
                float Dissolve_Value;
                float Dissolve_Smooth;
                float DissolveSide;
                float4 DissolveColor;

                float4 _MaskTexture_TexelSize;
                float4 Mask_TillingOffset;
                float Mask_Value;
                float _Mask_Rotation;

                float Boolean_a0c7fbd5e5b84802acf08ee1c31217a8;
                float _InvFade;

            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_WaveTexture);
            SAMPLER(sampler_WaveTexture);
            TEXTURE2D(_DissolveTex);
            SAMPLER(sampler_DissolveTex);
            TEXTURE2D(_MaskTexture);
            SAMPLER(sampler_MaskTexture);

            // Graph Functions

            void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            struct Bindings_TilingOffset
            {
                float3 TimeParameters;
            };

            void SG_TilingOffset_49d09e87fa74d9c40982fcebf14e7929(float4 Vector4_5c5530cddbac4caf873f8d410fe36a1e, float Vector1_a560f99024f4465194b6f92bd7b0188a,
                                          Bindings_TilingOffset IN, out float2 OutVector2_1, out float2 OutVector21_2,
                                          out float2 OutVector22_3, out float2 OutVector23_4)
            {
                float4 _Property_e447fc2e68ae40e99b563dd4098438f2_Out_0 = Vector4_5c5530cddbac4caf873f8d410fe36a1e;
                float _Split_b0fabcc92a304643afdb1519d0596a85_R_1 = _Property_e447fc2e68ae40e99b563dd4098438f2_Out_0[0];
                float _Split_b0fabcc92a304643afdb1519d0596a85_G_2 = _Property_e447fc2e68ae40e99b563dd4098438f2_Out_0[1];
                float _Split_b0fabcc92a304643afdb1519d0596a85_B_3 = _Property_e447fc2e68ae40e99b563dd4098438f2_Out_0[2];
                float _Split_b0fabcc92a304643afdb1519d0596a85_A_4 = _Property_e447fc2e68ae40e99b563dd4098438f2_Out_0[3];
                float2 _Vector2_4b3897ed21814c4a919bddbf58389fbd_Out_0 = float2(_Split_b0fabcc92a304643afdb1519d0596a85_R_1, _Split_b0fabcc92a304643afdb1519d0596a85_G_2);
                float2 _Vector2_04e8b07e32584cffa6fad22278dcb78c_Out_0 = float2(_Split_b0fabcc92a304643afdb1519d0596a85_B_3, _Split_b0fabcc92a304643afdb1519d0596a85_A_4);
                float _Property_1c46f938e83f4d5a89b1711572cd623d_Out_0 = Vector1_a560f99024f4465194b6f92bd7b0188a;
                float2 _Multiply_4bc1158e3c9443f49588928b347a35bc_Out_2;
                Unity_Multiply_float(_Vector2_04e8b07e32584cffa6fad22278dcb78c_Out_0, (_Property_1c46f938e83f4d5a89b1711572cd623d_Out_0.xx), _Multiply_4bc1158e3c9443f49588928b347a35bc_Out_2);
                float2 _Multiply_b5a9184c88c444c79c3e5da2282f20b4_Out_2;
                Unity_Multiply_float(_Vector2_04e8b07e32584cffa6fad22278dcb78c_Out_0, (IN.TimeParameters.x.xx), _Multiply_b5a9184c88c444c79c3e5da2282f20b4_Out_2);
                OutVector2_1 = _Vector2_4b3897ed21814c4a919bddbf58389fbd_Out_0;
                OutVector21_2 = _Vector2_04e8b07e32584cffa6fad22278dcb78c_Out_0;
                OutVector22_3 = _Multiply_4bc1158e3c9443f49588928b347a35bc_Out_2;
                OutVector23_4 = _Multiply_b5a9184c88c444c79c3e5da2282f20b4_Out_2;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_Add_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A + B;
            }

            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }

            void Unity_Lerp_float(float A, float B, float T, out float Out)
            {
                Out = lerp(A, B, T);
            }

            void Unity_Flipbook_InvertY_float(float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
            {
                Tile = floor(fmod(Tile + 0.000001, Width * Height));
                float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
                float tileX = (Tile - Width * floor(Tile * tileCount.x));
                float tileY = (Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
                Out = (UV + float2(tileX, tileY)) * tileCount;
            }

            void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
            {
                //rotation matrix
                Rotation = Rotation * (3.1415926f / 180.0f);
                UV -= Center;
                float s = sin(Rotation);
                float c = cos(Rotation);

                //center rotation matrix
                float2x2 rMatrix = float2x2(c, -s, s, c);
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = rMatrix * 2 - 1;

                //multiply the UVs by the rotation matrix
                UV.xy = mul(UV.xy, rMatrix);
                UV += Center;

                Out = UV;
            }

            struct Bindings_ParticlesColor_78a8a3404575f784682cbccbfbe09340
            {
                float4 VertexColor;
            };

            void SG_ParticlesColor_78a8a3404575f784682cbccbfbe09340(float4 Vector4_b24be748635143f4a17e0bbed5461239, float4 Vector4_abd6f1cdef8b457287141daac1dba918,
                    Bindings_ParticlesColor_78a8a3404575f784682cbccbfbe09340 IN, out float3 RGB_1, out float Alpha_2)
            {
                float4 _Property_78d213b9eb71479484b4df47046e4d55_Out_0 = Vector4_b24be748635143f4a17e0bbed5461239;
                float4 _Multiply_90ba5f7654c44f559a1d30826cd6aed4_Out_2;
                Unity_Multiply_float(IN.VertexColor, _Property_78d213b9eb71479484b4df47046e4d55_Out_0, _Multiply_90ba5f7654c44f559a1d30826cd6aed4_Out_2);
                float4 _Property_09ae09e6ea9346f7ad2160db8881ae5f_Out_0 = Vector4_abd6f1cdef8b457287141daac1dba918;
                float4 _Multiply_8b45346c0d3d49bf9e308422359243f8_Out_2;
                Unity_Multiply_float(_Multiply_90ba5f7654c44f559a1d30826cd6aed4_Out_2, _Property_09ae09e6ea9346f7ad2160db8881ae5f_Out_0, _Multiply_8b45346c0d3d49bf9e308422359243f8_Out_2);
                float _Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_R_1 = _Multiply_8b45346c0d3d49bf9e308422359243f8_Out_2[0];
                float _Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_G_2 = _Multiply_8b45346c0d3d49bf9e308422359243f8_Out_2[1];
                float _Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_B_3 = _Multiply_8b45346c0d3d49bf9e308422359243f8_Out_2[2];
                float _Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_A_4 = _Multiply_8b45346c0d3d49bf9e308422359243f8_Out_2[3];
                float3 _Vector3_453a8572f94446bdb75d1fb73f9ba035_Out_0 = float3(_Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_R_1, _Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_G_2,
                    _Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_B_3);
                RGB_1 = _Vector3_453a8572f94446bdb75d1fb73f9ba035_Out_0;
                Alpha_2 = _Split_ebc6ce90cf714dbdb1b4bc7c03c0f157_A_4;
            }

            void Unity_OneMinus_float4(float4 In, out float4 Out)
            {
                Out = 1 - In;
            }

            void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A - B;
            }

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            void Unity_Smoothstep_float4(float4 Edge1, float4 Edge2, float4 In, out float4 Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }

            void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
            {
                Out = lerp(A, B, T);
            }

            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
            {
                Out = lerp(A, B, T);
            }

            void Unity_Power_float4(float4 A, float4 B, out float4 Out)
            {
                Out = pow(abs(A), B);
            }

            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }

            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }

            struct Bindings_SoftParticles_016d70ff6530ba448beb5b5ae55a6b7f
            {
                float4 ScreenPosition;
            };

            void SG_SoftParticles_016d70ff6530ba448beb5b5ae55a6b7f(float Vector1_dae21aa74dd84c169871c362da07e048, Bindings_SoftParticles_016d70ff6530ba448beb5b5ae55a6b7f IN, out float Out_1)
            {
                float _SceneDepth_94e0ba927f9e464eaeaaef05a2dee19b_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_94e0ba927f9e464eaeaaef05a2dee19b_Out_1);
                float4 _ScreenPosition_af072112703846ca8d274105841b299f_Out_0 = IN.ScreenPosition;
                float _Split_566b13e1562f474dab2228df6ab73805_R_1 = _ScreenPosition_af072112703846ca8d274105841b299f_Out_0[0];
                float _Split_566b13e1562f474dab2228df6ab73805_G_2 = _ScreenPosition_af072112703846ca8d274105841b299f_Out_0[1];
                float _Split_566b13e1562f474dab2228df6ab73805_B_3 = _ScreenPosition_af072112703846ca8d274105841b299f_Out_0[2];
                float _Split_566b13e1562f474dab2228df6ab73805_A_4 = _ScreenPosition_af072112703846ca8d274105841b299f_Out_0[3];
                float _Subtract_b46992213bf0441dba61ce03c9fab91d_Out_2;
                Unity_Subtract_float(_SceneDepth_94e0ba927f9e464eaeaaef05a2dee19b_Out_1, _Split_566b13e1562f474dab2228df6ab73805_A_4, _Subtract_b46992213bf0441dba61ce03c9fab91d_Out_2);
                float _Property_7562c637141f43aba64feb11ded7fa4a_Out_0 = Vector1_dae21aa74dd84c169871c362da07e048;
                float _Multiply_8b4b98cca6d2427683e8ca75bafa2328_Out_2;
                Unity_Multiply_float(_Subtract_b46992213bf0441dba61ce03c9fab91d_Out_2, _Property_7562c637141f43aba64feb11ded7fa4a_Out_0, _Multiply_8b4b98cca6d2427683e8ca75bafa2328_Out_2);
                float _Saturate_d935a73ce40d4ea99a4bb91f56669461_Out_1;
                Unity_Saturate_float(_Multiply_8b4b98cca6d2427683e8ca75bafa2328_Out_2, _Saturate_d935a73ce40d4ea99a4bb91f56669461_Out_1);
                Out_1 = _Saturate_d935a73ce40d4ea99a4bb91f56669461_Out_1;
            }

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _Property_1155e89c61bb436dbb2013c336dffe9f_Out_0 = IsGammaSpace() ? LinearToSRGB(_TintColor) : _TintColor;
                UnityTexture2D _Property_82f6f932ded04943817a361e6395bb37_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
                UnityTexture2D _Property_1e2bb92df20346178bea155cbbfad36c_Out_0 = UnityBuildTexture2DStructNoScale(_WaveTexture);
                float4 _Property_2bf74902e05641fc93ac025e8eb20cec_Out_0 = Wave_TilingOffset;
                float4 _UV_36f43b2dc7d8435ca8259881dc912de6_Out_0 = IN.uv2;

                float _Split_ac6a7a35b72f4d9e99b3860c409ee922_R_1 = _UV_36f43b2dc7d8435ca8259881dc912de6_Out_0[0];
                float _Split_ac6a7a35b72f4d9e99b3860c409ee922_G_2 = _UV_36f43b2dc7d8435ca8259881dc912de6_Out_0[1];
                float _Split_ac6a7a35b72f4d9e99b3860c409ee922_B_3 = _UV_36f43b2dc7d8435ca8259881dc912de6_Out_0[2];
                float _Split_ac6a7a35b72f4d9e99b3860c409ee922_A_4 = _UV_36f43b2dc7d8435ca8259881dc912de6_Out_0[3];

                Bindings_TilingOffset _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15;
                _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15.TimeParameters = IN.TimeParameters;
                float2 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector2_1;
                float2 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector21_2;
                float2 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector22_3;
                float2 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector23_4;
                SG_TilingOffset_49d09e87fa74d9c40982fcebf14e7929(_Property_2bf74902e05641fc93ac025e8eb20cec_Out_0, _Split_ac6a7a35b72f4d9e99b3860c409ee922_G_2,
                 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15,
                 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector2_1,
                 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector21_2,
                 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector22_3,
                 _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector23_4);

                #if defined(WAVE_OFFSET_MODE_NORMAL)
                float2 _WaveOffsetMode_18e9ccc0633e4c77b9a2e6850c152c03_Out_0 = _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector21_2;
                #elif defined(WAVE_OFFSET_MODE_AUTO)
                float2 _WaveOffsetMode_18e9ccc0633e4c77b9a2e6850c152c03_Out_0 = _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector23_4;
                #else
                float2 _WaveOffsetMode_18e9ccc0633e4c77b9a2e6850c152c03_Out_0 = _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector22_3;
                #endif


                float2 _TilingAndOffset_365dc9c3ef534093b044b1b84e14a870_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, _TilingOffset_2502bcc9c459479c9d873a1b9fd44e15_OutVector2_1, _WaveOffsetMode_18e9ccc0633e4c77b9a2e6850c152c03_Out_0,
                    _TilingAndOffset_365dc9c3ef534093b044b1b84e14a870_Out_3);

                float4 _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1e2bb92df20346178bea155cbbfad36c_Out_0.tex,
                    _Property_1e2bb92df20346178bea155cbbfad36c_Out_0.samplerstate,
                    _TilingAndOffset_365dc9c3ef534093b044b1b84e14a870_Out_3);
                float _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_R_4 = _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_RGBA_0.r;
                float _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_G_5 = _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_RGBA_0.g;
                float _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_B_6 = _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_RGBA_0.b;
                float _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_A_7 = _SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_RGBA_0.a;

                float _Property_5edf7bfd2a5c4dfc88e8f460facd0b4c_Out_0 = Wave_Value;

                float4 _Multiply_fae430a302fa40fa9fa82fdbc6fec292_Out_2;
                Unity_Multiply_float(_SampleTexture2D_c16c71f2122b46cebd470f818b6320f1_RGBA_0, (_Property_5edf7bfd2a5c4dfc88e8f460facd0b4c_Out_0.xxxx),
                                                                                                                 _Multiply_fae430a302fa40fa9fa82fdbc6fec292_Out_2);

                float2 _TilingAndOffset_b4ef80e38606431bbeea6c4239e30bb1_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(1, 1), float2(0, 0), _TilingAndOffset_b4ef80e38606431bbeea6c4239e30bb1_Out_3);

                float2 _Add_e54ac3a9e1154b03a20e035c1b117048_Out_2;
                Unity_Add_float2((_Multiply_fae430a302fa40fa9fa82fdbc6fec292_Out_2.xy), _TilingAndOffset_b4ef80e38606431bbeea6c4239e30bb1_Out_3, _Add_e54ac3a9e1154b03a20e035c1b117048_Out_2);

                float4 _Property_f2dcdb4fc9114a009d9c45cab920563e_Out_0 = Main_TillingOffset;

                Bindings_TilingOffset _TilingOffset_b76af07443924163bdf0533bae88b224;
                _TilingOffset_b76af07443924163bdf0533bae88b224.TimeParameters = IN.TimeParameters;
                float2 _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector2_1;
                float2 _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector21_2;
                float2 _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector22_3;
                float2 _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector23_4;
                SG_TilingOffset_49d09e87fa74d9c40982fcebf14e7929(_Property_f2dcdb4fc9114a009d9c45cab920563e_Out_0, _Split_ac6a7a35b72f4d9e99b3860c409ee922_R_1,
                     _TilingOffset_b76af07443924163bdf0533bae88b224,
                     _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector2_1,
                     _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector21_2,
                     _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector22_3,
                     _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector23_4);

                #if defined(MAIN_OFFSET_MODE_NORMAL)
                float2 _MainOffsetMode_63e6121102724865a3c9c00213d4f2a2_Out_0 = _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector21_2;
                #elif defined(MAIN_OFFSET_MODE_AUTO)
            float2 _MainOffsetMode_63e6121102724865a3c9c00213d4f2a2_Out_0 = _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector23_4;
                #else
            float2 _MainOffsetMode_63e6121102724865a3c9c00213d4f2a2_Out_0 = _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector22_3;
                #endif


                float2 _TilingAndOffset_d76a94cf95dc4dc68c864f233e85f3ff_Out_3;
                Unity_TilingAndOffset_float(_Add_e54ac3a9e1154b03a20e035c1b117048_Out_2, _TilingOffset_b76af07443924163bdf0533bae88b224_OutVector2_1,
_MainOffsetMode_63e6121102724865a3c9c00213d4f2a2_Out_0,
_TilingAndOffset_d76a94cf95dc4dc68c864f233e85f3ff_Out_3);

                float _Property_a9603126b00844649c8c889b62580978_Out_0 = _Width;

                float _Property_b2fc4d98706b4bae893bcd2575d05c95_Out_0 = _Height;

                float _Multiply_ed935631b1274f74b7e03dee7bf38b42_Out_2;
                Unity_Multiply_float(_Property_a9603126b00844649c8c889b62580978_Out_0, _Property_b2fc4d98706b4bae893bcd2575d05c95_Out_0, _Multiply_ed935631b1274f74b7e03dee7bf38b42_Out_2);

                float _Subtract_a90510207af84049884670b1319ef2fb_Out_2;
                Unity_Subtract_float(_Multiply_ed935631b1274f74b7e03dee7bf38b42_Out_2, 1, _Subtract_a90510207af84049884670b1319ef2fb_Out_2);

                float _Property_7dc37c3d77bf4577b53685724e84517e_Out_0 = _Tile;

                float _Lerp_d342d8c5db78489aa790e2f89d14f97f_Out_3;
                Unity_Lerp_float(0, _Subtract_a90510207af84049884670b1319ef2fb_Out_2, _Property_7dc37c3d77bf4577b53685724e84517e_Out_0, _Lerp_d342d8c5db78489aa790e2f89d14f97f_Out_3);

                float2 _Flipbook_99ace008807b4d0298098e3eb68cb40a_Out_4;
                float2 _Flipbook_99ace008807b4d0298098e3eb68cb40a_Invert = float2(0, 1);
                Unity_Flipbook_InvertY_float(_TilingAndOffset_d76a94cf95dc4dc68c864f233e85f3ff_Out_3, _Property_a9603126b00844649c8c889b62580978_Out_0,
                    _Property_b2fc4d98706b4bae893bcd2575d05c95_Out_0, _Lerp_d342d8c5db78489aa790e2f89d14f97f_Out_3,
                    _Flipbook_99ace008807b4d0298098e3eb68cb40a_Invert,
                    _Flipbook_99ace008807b4d0298098e3eb68cb40a_Out_4);

                float _Property_651ec7288bf244588a0b9ad97e25a33f_Out_0 = _Rotation;

                float2 _Rotate_28fe181e9576446791f81887ca9f69c0_Out_3;
                Unity_Rotate_Degrees_float(_Flipbook_99ace008807b4d0298098e3eb68cb40a_Out_4, float2(0.5, 0.5), _Property_651ec7288bf244588a0b9ad97e25a33f_Out_0,
                                                                                        _Rotate_28fe181e9576446791f81887ca9f69c0_Out_3);

                float4 _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_RGBA_0 = SAMPLE_TEXTURE2D(_Property_82f6f932ded04943817a361e6395bb37_Out_0.tex,
                                                                                    _Property_82f6f932ded04943817a361e6395bb37_Out_0.samplerstate,
                                                                                    _Rotate_28fe181e9576446791f81887ca9f69c0_Out_3);
                float _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_R_4 = _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_RGBA_0.r;
                float _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_G_5 = _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_RGBA_0.g;
                float _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_B_6 = _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_RGBA_0.b;
                float _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_A_7 = _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_RGBA_0.a;

                Bindings_ParticlesColor_78a8a3404575f784682cbccbfbe09340 _ParticlesColor_86b77ff862b146abb40078b6215bb4fc;
                _ParticlesColor_86b77ff862b146abb40078b6215bb4fc.VertexColor = IN.VertexColor;
                float3 _ParticlesColor_86b77ff862b146abb40078b6215bb4fc_RGB_1;
                float _ParticlesColor_86b77ff862b146abb40078b6215bb4fc_Alpha_2;
                SG_ParticlesColor_78a8a3404575f784682cbccbfbe09340(_Property_1155e89c61bb436dbb2013c336dffe9f_Out_0, _SampleTexture2D_56eee8a3026042cf90bf1c3ba348a8ad_RGBA_0,
                                                                      _ParticlesColor_86b77ff862b146abb40078b6215bb4fc,
                                                                      _ParticlesColor_86b77ff862b146abb40078b6215bb4fc_RGB_1,
                                                                      _ParticlesColor_86b77ff862b146abb40078b6215bb4fc_Alpha_2);

                float4 _Property_10d2fffe8d3042f5b1a52b478f53d8e0_Out_0 = IsGammaSpace() ? LinearToSRGB(DissolveColor) : DissolveColor;

                UnityTexture2D _Property_31e6bbbdb16841dd9c1ee84b066cdacf_Out_0 = UnityBuildTexture2DStructNoScale(_DissolveTex);

                float4 _Property_94c45287c5fc44c2a300e8f5a4876e9a_Out_0 = Dissolve_TilingOffset;

                Bindings_TilingOffset _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1;
                _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1.TimeParameters = IN.TimeParameters;
                float2 _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector2_1;
                float2 _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector21_2;
                float2 _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector22_3;
                float2 _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector23_4;
                SG_TilingOffset_49d09e87fa74d9c40982fcebf14e7929(_Property_94c45287c5fc44c2a300e8f5a4876e9a_Out_0, _Split_ac6a7a35b72f4d9e99b3860c409ee922_B_3,
                                                                  _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1, _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector2_1,
                                                                  _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector21_2,
                                                                  _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector22_3,
                                                                  _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector23_4);


                #if defined(DISSOLVE_OFFSET_MODE_NORMAL)
                float2 _DissolveOffsetMode_4e915cdab4484ff4bdba2a15294aee74_Out_0 = _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector21_2;
                #elif defined(DISSOLVE_OFFSET_MODE_AUTO)
            float2 _DissolveOffsetMode_4e915cdab4484ff4bdba2a15294aee74_Out_0 = _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector23_4;
                #else
            float2 _DissolveOffsetMode_4e915cdab4484ff4bdba2a15294aee74_Out_0 = _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector22_3;
                #endif


                float2 _TilingAndOffset_c037b196e3444ea1a5276ab0a1dfdc37_Out_3;
                Unity_TilingAndOffset_float(_Add_e54ac3a9e1154b03a20e035c1b117048_Out_2, _TilingOffset_3a75eee2b1774d8595ce06fdc411d1b1_OutVector2_1,
                                                         _DissolveOffsetMode_4e915cdab4484ff4bdba2a15294aee74_Out_0,
                                                         _TilingAndOffset_c037b196e3444ea1a5276ab0a1dfdc37_Out_3);

                float _Property_afa074809b8848cbb6d6f39c21310c94_Out_0 = _DissRotation;

                float2 _Rotate_727c23b94a974e5a93cbc131f1ff12c4_Out_3;
                Unity_Rotate_Degrees_float(_TilingAndOffset_c037b196e3444ea1a5276ab0a1dfdc37_Out_3, float2(0.5, 0.5), _Property_afa074809b8848cbb6d6f39c21310c94_Out_0,
                                      _Rotate_727c23b94a974e5a93cbc131f1ff12c4_Out_3);

                float4 _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_RGBA_0 = SAMPLE_TEXTURE2D(_Property_31e6bbbdb16841dd9c1ee84b066cdacf_Out_0.tex,
                                                      _Property_31e6bbbdb16841dd9c1ee84b066cdacf_Out_0.samplerstate,
                                                      _Rotate_727c23b94a974e5a93cbc131f1ff12c4_Out_3);
                float _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_R_4 = _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_RGBA_0.r;
                float _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_G_5 = _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_RGBA_0.g;
                float _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_B_6 = _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_RGBA_0.b;
                float _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_A_7 = _SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_RGBA_0.a;

                float4 _OneMinus_4806741049944ebc87bc27f33a3ca27c_Out_1;
                Unity_OneMinus_float4(_SampleTexture2D_e5c2221a2afa47fa9d6aed27a2681d77_RGBA_0, _OneMinus_4806741049944ebc87bc27f33a3ca27c_Out_1);

                float _Property_4f26d27e59614563b970135f3e72e054_Out_0 = Dissolve_Smooth;

                float4 _Subtract_7ddf57d8138d4df08a239ed6a3693e5c_Out_2;
                Unity_Subtract_float4(_OneMinus_4806741049944ebc87bc27f33a3ca27c_Out_1, (_Property_4f26d27e59614563b970135f3e72e054_Out_0.xxxx), _Subtract_7ddf57d8138d4df08a239ed6a3693e5c_Out_2);

                float _Property_412e224b54594fd780ad9737ba666094_Out_0 = Dissolve_Value;

                #if defined(DISSOLVE_OFFSET_MODE_NORMAL)
                float _DissolveOffsetMode_5e6743d71d7341e5b032c045393a45a5_Out_0 = _Property_412e224b54594fd780ad9737ba666094_Out_0;
                #elif defined(DISSOLVE_OFFSET_MODE_AUTO)
            float _DissolveOffsetMode_5e6743d71d7341e5b032c045393a45a5_Out_0 = _Property_412e224b54594fd780ad9737ba666094_Out_0;
                #else
            float _DissolveOffsetMode_5e6743d71d7341e5b032c045393a45a5_Out_0 = _Split_ac6a7a35b72f4d9e99b3860c409ee922_A_4;
                #endif


                float _Property_414260767b8a494588175b070954f51b_Out_0 = DissolveSide;

                float _Add_61995e55ac6444038dcee95ac6a19463_Out_2;
                Unity_Add_float(_DissolveOffsetMode_5e6743d71d7341e5b032c045393a45a5_Out_0, _Property_414260767b8a494588175b070954f51b_Out_0, _Add_61995e55ac6444038dcee95ac6a19463_Out_2);

                float4 _Smoothstep_bb38dcc64c814fceb1636d20ac860115_Out_3;
                Unity_Smoothstep_float4(_OneMinus_4806741049944ebc87bc27f33a3ca27c_Out_1, _Subtract_7ddf57d8138d4df08a239ed6a3693e5c_Out_2, (_Add_61995e55ac6444038dcee95ac6a19463_Out_2.xxxx),
                                              _Smoothstep_bb38dcc64c814fceb1636d20ac860115_Out_3);

                float4 _OneMinus_17952381ca48425d906679220776d962_Out_1;
                Unity_OneMinus_float4(_Smoothstep_bb38dcc64c814fceb1636d20ac860115_Out_3, _OneMinus_17952381ca48425d906679220776d962_Out_1);

                float3 _Lerp_064e71f8b19c4e29a559c95e97b08914_Out_3;
                Unity_Lerp_float3(_ParticlesColor_86b77ff862b146abb40078b6215bb4fc_RGB_1, (_Property_10d2fffe8d3042f5b1a52b478f53d8e0_Out_0.xyz),
                   (_OneMinus_17952381ca48425d906679220776d962_Out_1.xyz),
                   _Lerp_064e71f8b19c4e29a559c95e97b08914_Out_3);

                float _Property_299f8ec089f344c78b6d1fd311e6061c_Out_0 = Dissolve_Value;

                float4 _Smoothstep_db59427558e6459fb7acfa7c574d4b99_Out_3;
                Unity_Smoothstep_float4(_OneMinus_4806741049944ebc87bc27f33a3ca27c_Out_1, _Subtract_7ddf57d8138d4df08a239ed6a3693e5c_Out_2, (_Property_299f8ec089f344c78b6d1fd311e6061c_Out_0.xxxx),
                                                _Smoothstep_db59427558e6459fb7acfa7c574d4b99_Out_3);

                float4 _Lerp_94df40724fd84963baa957532bc6bc03_Out_3;
                Unity_Lerp_float4(float4(0, 0, 0, 0), (_ParticlesColor_86b77ff862b146abb40078b6215bb4fc_Alpha_2.xxxx), _Smoothstep_db59427558e6459fb7acfa7c574d4b99_Out_3,
                                                                                                                   _Lerp_94df40724fd84963baa957532bc6bc03_Out_3);

                UnityTexture2D _Property_f5aa8c1a08e1453280fab6a932fc55c9_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTexture);

                float4 _Property_1902bf8ff4e84f8d94877c7220a6067a_Out_0 = Mask_TillingOffset;

                float _Split_f1a49587071e4a07881edbe8ae01f0be_R_1 = _Property_1902bf8ff4e84f8d94877c7220a6067a_Out_0[0];
                float _Split_f1a49587071e4a07881edbe8ae01f0be_G_2 = _Property_1902bf8ff4e84f8d94877c7220a6067a_Out_0[1];
                float _Split_f1a49587071e4a07881edbe8ae01f0be_B_3 = _Property_1902bf8ff4e84f8d94877c7220a6067a_Out_0[2];
                float _Split_f1a49587071e4a07881edbe8ae01f0be_A_4 = _Property_1902bf8ff4e84f8d94877c7220a6067a_Out_0[3];

                float2 _Vector2_c4c6a235299a440eb12286824195d50a_Out_0 = float2(_Split_f1a49587071e4a07881edbe8ae01f0be_R_1, _Split_f1a49587071e4a07881edbe8ae01f0be_G_2);

                float2 _Vector2_7bb47216fe724512bdf0d604c00a6971_Out_0 = float2(_Split_f1a49587071e4a07881edbe8ae01f0be_B_3, _Split_f1a49587071e4a07881edbe8ae01f0be_A_4);

                float2 _TilingAndOffset_cbf79f7e5d1f47098292ff049f96df8b_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, _Vector2_c4c6a235299a440eb12286824195d50a_Out_0, _Vector2_7bb47216fe724512bdf0d604c00a6971_Out_0,
                                                                                                                      _TilingAndOffset_cbf79f7e5d1f47098292ff049f96df8b_Out_3);

                float _Property_6fb3e7f29fa34d9a976e1d1319c54ce4_Out_0 = _Mask_Rotation;

                float2 _Rotate_733bdaa1f42341c98074916dade032a7_Out_3;
                Unity_Rotate_Degrees_float(_TilingAndOffset_cbf79f7e5d1f47098292ff049f96df8b_Out_3, float2(0.5, 0.5), _Property_6fb3e7f29fa34d9a976e1d1319c54ce4_Out_0,
                                  _Rotate_733bdaa1f42341c98074916dade032a7_Out_3);

                float4 _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_f5aa8c1a08e1453280fab6a932fc55c9_Out_0.tex,
                                                                                                       _Property_f5aa8c1a08e1453280fab6a932fc55c9_Out_0.samplerstate,
                                                                                                       _Rotate_733bdaa1f42341c98074916dade032a7_Out_3);
                float _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_R_4 = _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_RGBA_0.r;
                float _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_G_5 = _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_RGBA_0.g;
                float _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_B_6 = _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_RGBA_0.b;
                float _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_A_7 = _SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_RGBA_0.a;

                float _Property_a0765e57d5764ec7b7834820e0d25dba_Out_0 = Mask_Value;

                float4 _Power_65227ce9a36f4ece89a9d9ba3d5ff197_Out_2;
                Unity_Power_float4(_SampleTexture2D_0c12e49e6e554dd5ae0e6b42a2cde65d_RGBA_0, (_Property_a0765e57d5764ec7b7834820e0d25dba_Out_0.xxxx), _Power_65227ce9a36f4ece89a9d9ba3d5ff197_Out_2);

                float4 _Multiply_a29c2ee577c246be850dd719bcb6b99b_Out_2;
                Unity_Multiply_float(_Lerp_94df40724fd84963baa957532bc6bc03_Out_3, _Power_65227ce9a36f4ece89a9d9ba3d5ff197_Out_2, _Multiply_a29c2ee577c246be850dd719bcb6b99b_Out_2);

                float _Property_faeb50f148a44acb86350abb44da396e_Out_0 = _InvFade;

                Bindings_SoftParticles_016d70ff6530ba448beb5b5ae55a6b7f _SoftParticles_4c2591ffae5d46d7b917f7f34e7db387;
                _SoftParticles_4c2591ffae5d46d7b917f7f34e7db387.ScreenPosition = IN.ScreenPosition;

                float _SoftParticles_4c2591ffae5d46d7b917f7f34e7db387_Out_1;
                SG_SoftParticles_016d70ff6530ba448beb5b5ae55a6b7f(_Property_faeb50f148a44acb86350abb44da396e_Out_0, _SoftParticles_4c2591ffae5d46d7b917f7f34e7db387,
                                                                                                                                 _SoftParticles_4c2591ffae5d46d7b917f7f34e7db387_Out_1);

                #if defined(ENUM_757C0F1933D44CD6AF8836FFAC72D329_SOFT)
                float _SoftParticles_f66579bd12b1421d9533e51826c7e0f2_Out_0 = _SoftParticles_4c2591ffae5d46d7b917f7f34e7db387_Out_1;
                #else
            float _SoftParticles_f66579bd12b1421d9533e51826c7e0f2_Out_0 = 1;
                #endif

                float4 _Multiply_c3373c111b404fcf9b2c4dd0c953f6b0_Out_2;
                Unity_Multiply_float(_Multiply_a29c2ee577c246be850dd719bcb6b99b_Out_2, (_SoftParticles_f66579bd12b1421d9533e51826c7e0f2_Out_0.xxxx), _Multiply_c3373c111b404fcf9b2c4dd0c953f6b0_Out_2);
                // #endif
                surface.BaseColor = _Lerp_064e71f8b19c4e29a559c95e97b08914_Out_3;
                surface.Alpha = (_Multiply_c3373c111b404fcf9b2c4dd0c953f6b0_Out_2).x;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                output.WorldSpacePosition = input.positionWS;
                output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                output.uv0 = input.texCoord0;
                output.uv2 = input.texCoord2;
                output.VertexColor = input.color;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value

                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif

                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}