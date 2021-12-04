Shader "Unlit/CPU_FlowerSimulation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color("_Color",Color)=(1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "LightMode"="ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
            };


            struct MultiFlower_Data{
                float3 position;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            StructuredBuffer<float4x4> _multiFlower_Data;
            float4 _Color;

            v2f vert (appdata v,uint id:SV_INSTANCEID)
            {
                v.vertex=mul(_multiFlower_Data[id],v.vertex);
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal=UnityObjectToWorldNormal(v.normal);
                o.worldPos=mul(UNITY_MATRIX_M,v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col =float4(1,1,1,1);
                float3 lightDir=normalize(i.worldPos-_WorldSpaceLightPos0.xyz);
                float diff=dot(i.normal,lightDir);
                diff=(diff+1.0)*0.5;
                diff+=0.05;
                col.rgb*=diff*_Color.rgb;
                
                //col.rgb=float3(1,0,0);
                // float3 n=normalize(i.normal);
                // col.rgb=n;

                return col;
            }
            ENDCG
        }
    }
}
