Shader "Unlit/GPUFlower_Flowers_Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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

            struct StemData{
                int index;
                float3 position;
                float3 tangent;
                float3 normal;
                float3 bioNormal;
            };

            sampler2D _MainTex;
            StructuredBuffer<StemData> _stemData_buffer;

            v2f vert (appdata v,uint id : SV_INSTANCEID)
            {
                StemData data=_stemData_buffer[id];

                float4 pos=v.vertex;
                float l=length(pos.xyz);
                pos.xz*=((sin(_Time.y)+1.0)*0.5*0.8+0.2);
                pos.xyz+=data.position*100.0;

                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               fixed4 col = tex2D(_MainTex, i.uv);
               return col;
            }
            ENDCG
        }
    }
}
