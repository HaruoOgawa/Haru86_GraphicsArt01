Shader "Hidden/ImageEffect_Butterfly_Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
       // _BlurTexelSize("_BlurTexelSize",int)=1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

         //pass 0/////////////
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
            float4 _MainTex_TexelSize;
            float _BlurTexelSize;

            float _blurRange;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0.,0.,0.,1.);
                float centerToDist=length(float2(0.,0.)-(i.uv*2.0-1.0))*_blurRange;
                for(int x=-_BlurTexelSize;x<_BlurTexelSize;x++){
                    for(int y=-_BlurTexelSize;y<_BlurTexelSize;y++){
                        col.rgb+=tex2D(_MainTex,float2(i.uv.x+_MainTex_TexelSize.x*x , i.uv.y+_MainTex_TexelSize.y*y)).rgb*exp(centerToDist);
                    }    
                }
                col.rgb/=pow(_BlurTexelSize+2,2);
                return col;
            }
            ENDCG
        }

        //pass 1/////////////
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
            float4 _MainTex_TexelSize;
            float _ColorGapVal_R;
            float _ColorGapVal_G;
            float _ColorGapVal_B;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 st=i.uv;

                float2 st_R=st+float2(_ColorGapVal_R,_ColorGapVal_R);
                float2 st_G=st+float2(_ColorGapVal_G,_ColorGapVal_G);
                float2 st_B=st+float2(_ColorGapVal_B,_ColorGapVal_B);


                fixed4 col = tex2D(_MainTex, st);
                col.r=tex2D(_MainTex,st_R).r;
                col.g+=tex2D(_MainTex,st_G).g;
                col.b+=tex2D(_MainTex,st_B).b;

                col.rgb/=3.0;

                return col;
            }
            ENDCG
        }

        //pass 2/////////////
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
            float4 _MainTex_TexelSize;
            float _vignetteRange;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 st=i.uv+float2(_vignetteRange,_vignetteRange);
                fixed4 col = tex2D(_MainTex, st);
                // float centerToDist=length(float2(0.,0.)-(i.uv*2.0-1.0))*_vignetteRange;
                // col.rgb=col.rgb*exp(-centerToDist);
                return col;
            }
            ENDCG
        }

        //pass 3/////////////
         Pass
        {
          //  Blend SrcColor OneMinusSrcColor

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
            float4 _MainTex_TexelSize;

            sampler2D _blurRenderTexture;
            sampler2D _chromaticAberrationRenderTexture;
            sampler2D _vignetteRenderTexture;

            float _blurPower;
            float _chromaticAberrationPower;
            float _vignettePower;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 blurCol=tex2D(_blurRenderTexture,i.uv);
                float4 chromaticAberrationCol = tex2D(_chromaticAberrationRenderTexture, i.uv);
                float4 vignetteCol=tex2D(_vignetteRenderTexture,i.uv);
                
                float4 mainCol=tex2D(_MainTex,i.uv);
                mainCol.rgb+=blurCol.rgb*_blurPower;
                mainCol.rgb+=chromaticAberrationCol.rgb*_chromaticAberrationPower;
                mainCol.rgb+=vignetteCol.rgb*_vignettePower;

                return mainCol;
            }
            ENDCG
        }
    }
}
