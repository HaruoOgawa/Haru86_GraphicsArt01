Shader "Unlit/Butterfly_Fly_Anim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AnimSpeed("_AnimSpeed",Float)=1.0
        _AWidth("_AWidth",Float)=1.0
        [HDR] _BaseColor("_BaseColor",Color)=(1.,1.,1.,1.)
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
            float4 _BaseColor;

            v2f vert (appdata v)
            {
                float3 p=v.vertex.xyz;
                
                // float a=abs(p.x);
                // p.y+=a*sin(_Time.y*_AnimSpeed)*_AWidth;

               float t=sin(_Time.y*_AnimSpeed)*sign(p.x)*_AWidth;
               float2x2 rot=float2x2(
                   float2(cos(t),-sin(t)),
                   float2(sin(t),cos(t))
               );
               p.xy=mul(rot,p.xy);

                v.vertex.xyz=p;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb+=_BaseColor.rgb;
                return col;
            }
            ENDCG
        }
    }
}
