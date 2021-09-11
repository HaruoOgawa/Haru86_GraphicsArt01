Shader "Unlit/GPUFlower_Stem_Render"
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
            #pragma geometry geom
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            struct StemVertex{
                float3 vertice;
                float3 tangent;
                float3 normal;
                float3 bioNormal;
                int index;
            };

            sampler2D _MainTex;
            StructuredBuffer<StemVertex> _stemVertex_buffer;

            v2g vert (appdata v,uint id : SV_INSTANCEID)
            {
                StemVertex sVertex=_stemVertex_buffer[id];

                v.vertex.xyz=sVertex.vertice;

                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            [maxvertexcount(1)]
            void geom(point v2g input[1],inout PointStream<g2f> outStream){
                g2f o;
                o.vertex=input[0].vertex;
                o.uv=float2(0,0);
                outStream.Append(o);

                outStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
