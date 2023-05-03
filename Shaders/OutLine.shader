Shader "SyminCustom/OutLine"
{
    Properties
    {
        [HDR]_LineColor("Outline Color",Color) = (2,1,0,1)
        _LineWidth("Outline Width",float) = 0
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
            //"Queue" = "Opaque"
            "IgnoreProjector" = "True"
            }
        LOD 100
        
        //描边的模版测试
        Pass
        {
            Name "TestLine"
            Tags {
                //灯光配合RenderObject
                "LightMode"="TestLine" 
                "RenderType"="Opaque"
                }
            LOD 100
            ColorMask 0
            ZWrite Off
            ZTest Off

            Stencil{
                Ref 1
                Comp Always
                pass replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _LineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP,v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                clip(_LineWidth - 1);
                return half4(0.5,0,0,1);
            }
            ENDHLSL
        }
//通过模版测试的边缘进行着色
        Pass
        {
            Name "FillLine"
            Tags {
                //灯光配合RenderObject
                "LightMode"="FillLine" 
                "RenderType"="Opaque+20"
                }
            LOD 100
            ZTest Off

            Stencil{
                Ref 1
                Comp notEqual
                pass keep
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            half4 _LineColor;
            float _LineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                
                half4 mPos = v.vertex;
                half4 clipPos = mul(UNITY_MATRIX_MVP,v.vertex);
                
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal.xyz);
                //通过NDC空间获取归一化的线宽控制
                float3 ndcNormal = normalize(mul((float3x3)UNITY_MATRIX_P,viewNormal.xyz))*clipPos.w;
                //修正屏幕比例
                float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));//将近裁剪面右上角位置的顶点变换到观察空间
                float aspect = abs(nearUpperRight.y / nearUpperRight.x);//求得屏幕宽高比
                ndcNormal.x *= aspect;
                
                clipPos.xy += _LineWidth * ndcNormal.xy*0.01;
                o.vertex = clipPos;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                clip(_LineWidth - 1);
                return _LineColor;
            }
            ENDHLSL
        }
    }
}
