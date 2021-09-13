Shader "Unlit/GPUFlower_Flowers_Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color("_Color",Color)=(1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
        LOD 100
        Cull Off

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

           struct StemData{
                int resampleIndex;
                int resampleIndexInStem;
                float3 position;
                float3 tangent;
                float3 normal;
                float3 bioNormal;
                int renderFlag;
                float lifeTime;
                float flowerSize;
            };

            sampler2D _MainTex;
            StructuredBuffer<StemData> _read_stemDataFlower_buffer;
            float4 _Color;

            v2f vert (appdata v,uint id : SV_INSTANCEID)
            {
                StemData data=_read_stemDataFlower_buffer[id];

                float4 pos=v.vertex;
                float l=length(pos.xyz);
                // pos.xz*=((sin(_Time.y)+1.0)*0.5*0.8+0.2);
                // pos.y*=((sin(_Time.y)+1.0)*0.5*0.3+0.7);
                pos.xz*=data.lifeTime*0.8+0.2;
                pos.y*=data.lifeTime*0.3+0.7;
                //  pos.xz*=data.lifeTime;
                // pos.y*=data.lifeTime;
                pos.xyz*=data.flowerSize;
                
                pos.xyz=mul(float3x3(
                    data.normal,
                    data.tangent,
                    data.bioNormal
                ),pos.xyz)+data.position;

                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
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
                return col;
            }
            ENDCG
        }
    }
}
