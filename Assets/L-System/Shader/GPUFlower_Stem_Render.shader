Shader "Unlit/GPUFlower_Stem_Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color("_Color",color)=(1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            #define PI 3.14159265

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float idInMyStem : TEXCOORD1;
                float3 nextStemVertex : TEXCOORD2; 
                float3 tangent : TEXCOORD3;
                float3 normal : TEXCOORD4;
                float3 bioNormal : TEXCOORD5;
                float3 nextTangent : TEXCOORD6;
                float3 nextNormal : TEXCOORD7;
                float3 nextBioNormal : TEXCOORD8;
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
            float4 _Color;

            StructuredBuffer<StemVertex> _stemVertex_buffer;
            int _stemVertexCount;
            int _stemSegments;

            v2g vert (appdata v,uint id : SV_INSTANCEID)
            {
                StemVertex sVertex=_stemVertex_buffer[id];
                StemVertex nextVertex=_stemVertex_buffer[id+1];
                
                

                v2g o;
                //nowStem
                o.vertex = float4(sVertex.vertice,1.0);
                o.uv = v.uv;
                o.idInMyStem=sVertex.index;
                o.tangent=sVertex.tangent;
                o.normal=sVertex.normal;
                o.bioNormal=sVertex.bioNormal;
                
                //nextStem
                o.nextStemVertex=nextVertex.vertice;
                o.nextTangent=nextVertex.tangent;
                o.nextNormal=nextVertex.normal;
                o.nextBioNormal=nextVertex.bioNormal;
                return o;
            }

            // [maxvertexcount(48)]
            // void geom(point v2g input[1],inout TriangleStream<g2f> outStream){
                


            //     if(input[0].idInMyStem>0&&input[0].idInMyStem<_stemVertexCount-1){
            //        g2f o;
            //        float angleVal=(2.0*PI)/_stemSegments;

            //        for(int i=0;i<_stemSegments;i++){
            //             float4 pos0=float4(input[0].normal*cos(angleVal*(i))+input[0].bioNormal*sin(angleVal*(i)+input[0].vertex.xyz),1.0);
            //             float4 pos1=float4(input[0].normal*cos(angleVal*(i+1))+input[0].bioNormal*sin(angleVal*(i+1)+input[0].vertex.xyz),1.0);
            //             float4 pos2=float4(input[0].nextNormal*cos(angleVal*(i))+input[0].nextBioNormal*sin(angleVal*(i))+input[0].nextStemVertex.xyz,1.0);
            //             float4 pos3=float4(input[0].nextNormal*cos(angleVal*(i+1))+input[0].nextBioNormal*sin(angleVal*(i+1))+input[0].nextStemVertex.xyz,1.0);
                        
            //             //first
            //             o.vertex=UnityObjectToClipPos(pos0);
            //             o.uv=float2(0,0);
            //             outStream.Append(o);

            //             o.vertex=UnityObjectToClipPos(pos1);
            //             o.uv=float2(0,0);
            //             outStream.Append(o);  

            //             o.vertex=UnityObjectToClipPos(pos3);
            //             o.uv=float2(0,0);
            //             outStream.Append(o);    

            //             outStream.RestartStrip();

            //             //second
            //             o.vertex=UnityObjectToClipPos(pos0);
            //             o.uv=float2(0,0);
            //             outStream.Append(o);

            //             o.vertex=UnityObjectToClipPos(pos3);
            //             o.uv=float2(0,0);
            //             outStream.Append(o);  

            //             o.vertex=UnityObjectToClipPos(pos2);
            //             o.uv=float2(0,0);
            //             outStream.Append(o);    

            //             outStream.RestartStrip();
            //        }

            //     }else{
            //         g2f o;

            //         o.vertex=input[0].vertex;
            //         o.uv=float2(0,0);
            //         outStream.Append(o);

            //         o.vertex=input[0].vertex;
            //         o.uv=float2(0,0);
            //         outStream.Append(o);

            //         o.vertex=input[0].vertex;
            //         o.uv=float2(0,0);
            //         outStream.Append(o);

            //         outStream.RestartStrip();
            //     }
            // }

            [maxvertexcount(2)]
            void geom(point v2g input[1],inout LineStream<g2f> outStream){
                


                if(input[0].idInMyStem<_stemVertexCount-1){
                    g2f o;

                    o.vertex=UnityObjectToClipPos(input[0].vertex);
                    o.uv=float2(0,0);
                    outStream.Append(o);

                    o.vertex=UnityObjectToClipPos(float4(input[0].nextStemVertex,1.0));
                    o.uv=float2(0,0);
                    outStream.Append(o);    

                    outStream.RestartStrip();

                }else{
                    g2f o;

                    o.vertex=UnityObjectToClipPos(input[0].vertex);
                    o.uv=float2(0,0);
                    outStream.Append(o);

                    o.vertex=UnityObjectToClipPos(input[0].vertex);
                    o.uv=float2(0,0);
                    outStream.Append(o);

                    outStream.RestartStrip();
                }
            }

            //debug geom
            // [maxvertexcount(6)]
            // void geom(point v2g input[1],inout TriangleStream<g2f> outStream){
                
            //     float4 pos0=float4(-1.0,-1.0,0.0,1.0);
            //     float4 pos1=float4(-1.0,1.0,0.0,1.0);
            //     float4 pos2=float4(1.0,1.0,0.0,1.0);
            //     float4 pos3=float4(1.0,-1.0,0.0,1.0);
            
            //     g2f o;

            //     o.vertex=UnityObjectToClipPos(pos0);
            //     o.uv=float2(0,0);
            //     outStream.Append(o);

            //     o.vertex=UnityObjectToClipPos(pos1);
            //     o.uv=float2(0,0);
            //     outStream.Append(o);

            //     o.vertex=UnityObjectToClipPos(pos3);
            //     o.uv=float2(0,0);
            //     outStream.Append(o);    

            //     outStream.RestartStrip();



            //     o.vertex=UnityObjectToClipPos(pos0);
            //     o.uv=float2(0,0);
            //     outStream.Append(o);

            //     o.vertex=UnityObjectToClipPos(pos3);
            //     o.uv=float2(0,0);
            //     outStream.Append(o);

            //     o.vertex=UnityObjectToClipPos(pos2);
            //     o.uv=float2(0,0);
            //     outStream.Append(o);    

            //     outStream.RestartStrip();

                
            // }

            fixed4 frag (g2f i) : SV_Target
            {
                float4 col =_Color; 
                return col;
            }
            ENDCG
        }
    }
}
