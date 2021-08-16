Shader "Unlit/Butterfly_Fly_Anim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AnimSpeed("_AnimSpeed",Float)=1.0
        _AWidth("_AWidth",Float)=1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Off
        ZWrite Off
        Cull Off
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

            sampler2D _MainTex;
            float _AnimSpeed;
            float _AWidth;

            v2f vert (appdata v)
            {
                float3 p=v.vertex.xyz;
                float a=abs(p.x);
                p.y+=a*sin(_Time.y*_AnimSpeed)*_AWidth;
                v.vertex.xyz=p;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
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
