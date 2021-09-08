Shader "Unlit/Flower"
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                float4 pos=v.vertex;
                float l=length(pos.xyz);
                pos.xz*=((sin(_Time.y)+1.0)*0.5*0.8+0.2);

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
