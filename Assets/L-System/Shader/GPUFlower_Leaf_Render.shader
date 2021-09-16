Shader "Unlit/GPUFlower_Leaf_Render"
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

            #define PI 3.14159265
            #define rot(a) float2x2(cos(a),-sin(a),sin(a),cos(a))

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
            StructuredBuffer<StemData> _read_stemDataLeaf_buffer;
            float4 _Color;

            float rand(float2 st){
                return frac(
                    sin(dot(st.xy,float2(12.9898,78.233)))*43758.5453123
                );
            }

            v2f vert (appdata v,uint id : SV_INSTANCEID)
            {
                StemData data=_read_stemDataLeaf_buffer[id];

                float4 pos=v.vertex;
                float l=length(pos.xyz);
                pos.xyz*=data.lifeTime*0.8+0.2;
                pos.xyz*=data.flowerSize;
                
                float angle=PI*(id%2);
                pos.xy=mul(rot(angle),pos.xy);
                pos.yz=mul(rot(
                    (id%2==1) ? (PI/4.0) : (-PI/4.0)
                ),pos.yz);

                pos.xz=mul(rot(
                    (rand(float2(id,0.123))*2.0-1.0)*(PI/6.0)
                ),pos.xz);

                pos.xyz=mul(float3x3(
                    data.bioNormal,
                    data.normal,
                    data.tangent
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
