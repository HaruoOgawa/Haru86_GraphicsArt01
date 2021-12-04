Shader "Hidden/GPUFlowers_ColorGap"
{
    Properties
    {
        [HDR] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        //pass1
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _GapOffsetX;
            float4 _GapOffsetY;
            float _gapOffsetPower;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 vecFromCenter=float2(0.0,0.0)-(i.uv-float2(0.5,0.5))*2.0;

                float2 st_R=i.uv+vecFromCenter*_gapOffsetPower*_GapOffsetX.r;
                float2 st_G=i.uv+vecFromCenter*_gapOffsetPower*_GapOffsetX.g;
                float2 st_B=i.uv+vecFromCenter*_gapOffsetPower*_GapOffsetX.b;

                fixed4 col = tex2D(_MainTex, i.uv);
                col.r+=tex2D(_MainTex,st_R);
                col.g+=tex2D(_MainTex,st_G);
                col.b+=tex2D(_MainTex,st_B);

                col.rgb*=0.5;
                
                return col;
            }
            ENDCG
        }
        //pass2
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _colorGapTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float4 gapCol=tex2D(_colorGapTexture,i.uv);

                col.rgb+=gapCol.rgb;
                col.rgb*=0.5;

                return col;
            }
            ENDCG
        }
    }
}
