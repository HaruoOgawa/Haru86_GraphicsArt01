Shader "Unlit/GPUFlower_Stem_Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color("_Color",color)=(1,1,1,1)
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
            #pragma geometry geom
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

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
                 float lifeTime : TEXCOORD9;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
            };

            struct StemVertex{
                float3 vertice;
                float3 tangent;
                float3 normal;
                float3 bioNormal;
                int index;
            };

            //花の数など
            struct StemManage{
                float stemLifeVal;
                float stemWaitTime;
                float signNum;
                int manageLifeCountFlag;
                int flowerCount;
                int flowerStartIndex;
                int leafCount;
                int leafStartIndex;
            };

            //花や茎を生成するための情報を載せる構造体
            struct StemData{
                int resampleIndex;
                int resampleGroupIndex;
                float3 position;
                float3 tangent;
                float3 normal;
                float3 bioNormal;
                int renderFlag;
                float lifeTime;
            };

            sampler2D _MainTex;
            float4 _Color;

            StructuredBuffer<StemVertex> _stemVertex_buffer;
            StructuredBuffer<StemManage> _read_stemManage_buffer;
            int _stemVertexCount;
            int _stemSegments;
            float _stemRadius;

            v2g vert (appdata v,uint id : SV_INSTANCEID)
            {
                StemVertex sVertex=_stemVertex_buffer[id];
                StemVertex nextVertex=_stemVertex_buffer[id+1];
                
                //get lifetime
                int stemNodeID=(id-sVertex.index)/_stemVertexCount;
                StemManage sManage=_read_stemManage_buffer[stemNodeID];
                float lifeTime=sManage.stemLifeVal;
                

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

                o.lifeTime=lifeTime;
                return o;
            }

            [maxvertexcount(72)]
            void geom(point v2g input[1],inout TriangleStream<g2f> outStream){
                
                float lifeTime=input[0].lifeTime;

                if(input[0].idInMyStem>0&&input[0].idInMyStem<_stemVertexCount-1){
                   g2f o;
                   float angleVal=(2.0*PI)/_stemSegments;

                   for(int i=0;i<_stemSegments;i++){
                        
                        float4 pos0=float4(
                            lifeTime*_stemRadius*normalize(input[0].normal*cos(angleVal*(float)(i))
                            +input[0].bioNormal*sin(angleVal*(float)(i)))
                            +input[0].vertex.xyz
                            ,1.0);

                        float4 pos1=float4(
                            lifeTime*_stemRadius*normalize(input[0].normal*cos(angleVal*(float)(i+1))
                            +input[0].bioNormal*sin(angleVal*(float)(i+1)))
                            +input[0].vertex.xyz
                            ,1.0);

                        float4 pos2=float4(
                            lifeTime*_stemRadius*normalize(input[0].nextNormal*cos(angleVal*(float)(i))
                            +input[0].nextBioNormal*sin(angleVal*(float)(i)))
                            +input[0].nextStemVertex.xyz
                            ,1.0);

                        float4 pos3=float4(
                            lifeTime*_stemRadius*normalize(input[0].nextNormal*cos(angleVal*(float)(i+1))
                            +input[0].nextBioNormal*sin(angleVal*(float)(i+1)))
                            +input[0].nextStemVertex.xyz
                            ,1.0);
                        
                        //first
                        o.vertex=UnityObjectToClipPos(pos0);
                        o.uv=float2(0,0);
                        o.normal=normalize(pos0-input[0].vertex.xyz);
                        o.worldPos=(mul(UNITY_MATRIX_M,pos0)).xyz;
                        outStream.Append(o);

                        o.vertex=UnityObjectToClipPos(pos1);
                        o.uv=float2(0,0);
                        o.normal=normalize(pos1-input[0].vertex.xyz);
                        o.worldPos=(mul(UNITY_MATRIX_M,pos1)).xyz;
                        outStream.Append(o);  

                        o.vertex=UnityObjectToClipPos(pos3);
                        o.uv=float2(0,0);
                        o.normal=normalize(pos3-input[0].nextStemVertex.xyz);
                        o.worldPos=(mul(UNITY_MATRIX_M,pos3)).xyz;
                        outStream.Append(o);    

                        outStream.RestartStrip();

                        //second
                        o.vertex=UnityObjectToClipPos(pos0);
                        o.uv=float2(0,0);
                        o.normal=normalize(pos0-input[0].vertex.xyz);
                        o.worldPos=(mul(UNITY_MATRIX_M,pos0)).xyz;
                        outStream.Append(o);

                        o.vertex=UnityObjectToClipPos(pos3);
                        o.uv=float2(0,0);
                        o.normal=normalize(pos3-input[0].nextStemVertex.xyz);
                        o.worldPos=(mul(UNITY_MATRIX_M,pos3)).xyz;
                        outStream.Append(o);  

                        o.vertex=UnityObjectToClipPos(pos2);
                        o.uv=float2(0,0);
                        o.normal=normalize(pos2-input[0].nextStemVertex.xyz);
                        o.worldPos=(mul(UNITY_MATRIX_M,pos2)).xyz;
                        outStream.Append(o);    

                        outStream.RestartStrip();
                   }

                }else{
                    g2f o;

                    o.vertex=input[0].vertex;
                    o.uv=float2(0,0);
                    o.normal=normalize(float3(0,0,1));
                    o.worldPos=(mul(UNITY_MATRIX_M,input[0].vertex)).xyz;
                    outStream.Append(o);

                    o.vertex=input[0].vertex;
                    o.uv=float2(0,0);
                    o.normal=normalize(float3(0,0,1));
                    o.worldPos=(mul(UNITY_MATRIX_M,input[0].vertex)).xyz;
                    outStream.Append(o);

                    o.vertex=input[0].vertex;
                    o.uv=float2(0,0);
                    o.normal=normalize(float3(0,0,1));
                    o.worldPos=(mul(UNITY_MATRIX_M,input[0].vertex)).xyz;
                    outStream.Append(o);

                    outStream.RestartStrip();

                     o.vertex=input[0].vertex;
                    o.uv=float2(0,0);
                    o.normal=normalize(float3(0,0,1));
                    o.worldPos=(mul(UNITY_MATRIX_M,input[0].vertex)).xyz;
                    outStream.Append(o);

                    o.vertex=input[0].vertex;
                    o.uv=float2(0,0);
                    o.normal=normalize(float3(0,0,1));
                    o.worldPos=(mul(UNITY_MATRIX_M,input[0].vertex)).xyz;
                    outStream.Append(o);

                    o.vertex=input[0].vertex;
                    o.uv=float2(0,0);
                    o.normal=normalize(float3(0,0,1));
                    o.worldPos=(mul(UNITY_MATRIX_M,input[0].vertex)).xyz;
                    outStream.Append(o);

                    outStream.RestartStrip();
                }
            }

            fixed4 frag (g2f i) : SV_Target
            {
                float4 col =float4(1,1,1,1);
                float3 lightDir=normalize(i.worldPos-_WorldSpaceLightPos0.xyz);
                float diff=dot(i.normal,lightDir);
                diff=(diff+1.0)*0.5;
                diff+=0.05;
                col.rgb*=diff*_Color.rgb;
                col=saturate(col);

                return col;
            }
            ENDCG
        }
    }
}
