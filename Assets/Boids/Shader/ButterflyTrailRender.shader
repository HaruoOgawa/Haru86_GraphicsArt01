Shader "Unlit/ButterflyTrailRender"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Cull Off
        ZWrite Off
        ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha
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
            };

            struct v2g
            {
               float4 vertex : POSITION;
               float3 node_pos : TEXCOORD0;
               float3 node_nextPos : TEXCOORD1;
               float3 node_dir : TEXCOORD2;
               float3 node_nextDir : TEXCOORD3;
               float node_life : TEXCOORD4;
               float trail_ID : TEXCOORD5;
            };

            struct g2f{
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float node_life : TEXCOORD1;
                float trail_ID : TEXCOORD2;
            };

            struct node{
                float3 node_position;
                float node_life;
                int renderFlag;
            };

            struct trail{
                int nextCalNodeIndex;
            };

            sampler2D _MainTex;
            StructuredBuffer<node> _node_data_read;
            float _TrailWidth;
            float _nodeSegment;
            float _initNodeLife;
           
            int CalCorrectIndex(int trailIndex,int calIndex){
                //trailIndex*_nodeSegment+_nodeSegmentを-1したら治った
                int index=min(trailIndex*_nodeSegment+_nodeSegment-1,max(trailIndex*_nodeSegment,calIndex));
                return index;
            }

            v2g vert (appdata v, uint id : SV_INSTANCEID)
            {
                int nodeIndex=id;
                int trailIndex=(int)(floor(nodeIndex/_nodeSegment));
               
                node node_data0_1=_node_data_read[CalCorrectIndex(trailIndex,(int)nodeIndex-1)];
                node node_data00=_node_data_read[CalCorrectIndex(trailIndex,(int)nodeIndex)];
                node node_data01=_node_data_read[CalCorrectIndex(trailIndex,(int)nodeIndex+1)];
                node node_data02=_node_data_read[CalCorrectIndex(trailIndex,(int)nodeIndex+2)];
                
                float3 node_pos=node_data00.node_position;
                float3 node_nextPos=node_data01.node_position;
                float3 node_dir=normalize(node_data01.node_position-node_data0_1.node_position);
                float3 node_nextDir=normalize(node_data02.node_position-node_data00.node_position);
                float node_life=node_data00.node_life;

                v2g o;
                o.vertex = v.vertex;
                o.node_pos=node_pos;
                o.node_nextPos=node_nextPos;
                o.node_dir=node_dir;
                o.node_nextDir=node_nextDir;
                o.node_life=node_life;
                o.trail_ID=trailIndex;
                return o;
            }

            [maxvertexcount(4)]
            void geom(point v2g Input[1],inout TriangleStream<g2f> outputStream){
                float3 camDir=normalize(Input[0].node_pos-_WorldSpaceCameraPos.xyz);
                float3 camNextDir=normalize(Input[0].node_nextPos-_WorldSpaceCameraPos.xyz);
                float3 node_cam_sideDir=normalize(cross(camDir,Input[0].node_dir));
                float3 node_cam_nextSideDir=normalize(cross(camNextDir,Input[0].node_nextDir));

                float4 render_node_pos01=float4(Input[0].node_pos+node_cam_sideDir*_TrailWidth,1.0);
                float4 render_node_pos0_1=float4(Input[0].node_pos-node_cam_sideDir*_TrailWidth,1.0);
                float4 render_node_pos11=float4(Input[0].node_nextPos+node_cam_nextSideDir*_TrailWidth,1.0);
                float4 render_node_pos1_1=float4(Input[0].node_nextPos-node_cam_nextSideDir*_TrailWidth,1.0);


                g2f o;
                
                o.vertex=UnityObjectToClipPos(render_node_pos01);
                o.uv=float2(0,0);
                o.trail_ID=Input[0].trail_ID;
                o.node_life=Input[0].node_life;
                outputStream.Append(o);

                o.vertex=UnityObjectToClipPos(render_node_pos0_1);
                o.uv=float2(0,0);
                o.trail_ID=Input[0].trail_ID;
                o.node_life=Input[0].node_life;
                outputStream.Append(o);

                o.vertex=UnityObjectToClipPos(render_node_pos11);
                o.uv=float2(0,0);
                o.trail_ID=Input[0].trail_ID;
                o.node_life=Input[0].node_life;
                outputStream.Append(o);

                o.vertex=UnityObjectToClipPos(render_node_pos1_1);
                o.uv=float2(0,0);
                o.trail_ID=Input[0].trail_ID;
                o.node_life=Input[0].node_life;
                outputStream.Append(o);

                outputStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col=float4(1.,1.,1.,1.);
               
                float node_life_rate=i.node_life/_initNodeLife;
                col.a=node_life_rate; 
                col.a*=(i.trail_ID==50) ? 1.0 : 0.0;


                return col;
            }
            ENDCG
        }
    }
}
