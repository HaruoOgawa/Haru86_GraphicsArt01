Shader "Unlit/Chrysanthemum_Flower 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _FirstColor("_FirstColor",COLOR)=(1.,1.,1.,1.)
        [HDR] _SecondColor("_SecondColor",COLOR)=(1.,1.,1.,1.)
        [HDR] _ThirdColor("_ThirdColor",COLOR)=(1.,1.,1.,1.)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "lighting.cginc"

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
                float4 col : COLOR;
                   float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _FirstColor;
            float4 _SecondColor;
            float4 _ThirdColor;
            StructuredBuffer<float4x4> _flower_buffer;

            v2f vert (appdata v,uint id : SV_INSTANCEID)
            {
                float uid=(float)id;
                uid=fmod(uid,2.0);
                uid=floor(uid);
                v2f o;
                v.vertex=mul(_flower_buffer[id],v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.col=(uid==0) ? _FirstColor : float4(0,0,0,1) + (uid==1) ? _SecondColor : float4(0,0,0,1) + (uid==2) ? _ThirdColor : float4(0,0,0,1);
                o.uv =v.uv; 
                o.normal=UnityObjectToWorldNormal(v.normal);
                 return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(1.,1.,1.,1.);
                float3 lightDir=_WorldSpaceLightPos0.xyz;
                float diff=max(dot(i.normal,lightDir),0.0);
                float3 lightCol=_LightColor0;
                col.rgb*=diff*lightCol;
                col.rgb+=i.col.rgb;
                 return col;
            }
            ENDCG
        }
    }
}
