Shader "Hidden/GrabPass_Butterfly_Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha


        Tags{"RenderType"="Transparent" "Queue"="Transparent+999999"}

        GrabPass{"_Butterfly_GrabPass_Texture"}

        //pass 0/////////////
       /* Pass
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
                float4 grabPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos=ComputeGrabScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            sampler2D _Butterfly_GrabPass_Texture;
            float4 _Butterfly_GrabPass_Texture_TexelSize;

            float _BlurTexelSize;

            float _blurRange;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0.,0.,0.,1.);
                float4 st=i.grabPos;

                float centerToDist=length(float2(0.,0.)-(st.xy*2.0-1.0))*_blurRange;

                for(int x=-_BlurTexelSize;x<_BlurTexelSize;x++){
                    for(int y=-_BlurTexelSize;y<_BlurTexelSize;y++){
                       
                        st.xy=float2(st.x+_Butterfly_GrabPass_Texture_TexelSize.x*x , st.y+_Butterfly_GrabPass_Texture_TexelSize.y*y);

                        col.rgb+=tex2D(_Butterfly_GrabPass_Texture,st).rgb*exp(centerToDist);
                    }    
                }
                col.rgb/=pow(_BlurTexelSize+2,2);
                return col;
            }
            ENDCG
        }*/

        //pass 1/////////////
        /* Pass
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
                float4 grabPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos=ComputeGrabScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            sampler2D _Butterfly_GrabPass_Texture;

            float _ColorGapVal_R;
            float _ColorGapVal_G;
            float _ColorGapVal_B;

            float _colorGapAlpha;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 st=i.grabPos;

                float4 st_R=st+float4(_ColorGapVal_R,_ColorGapVal_R,0.,0.);
                float4 st_G=st+float4(_ColorGapVal_G,_ColorGapVal_G,0.,0.);
                float4 st_B=st+float4(_ColorGapVal_B,_ColorGapVal_B,0.,0.);


                fixed4 col = tex2Dproj(_Butterfly_GrabPass_Texture, st);
                col.r=tex2Dproj(_Butterfly_GrabPass_Texture,st_R).r;
                col.g+=tex2Dproj(_Butterfly_GrabPass_Texture,st_G).g;
                col.b+=tex2Dproj(_Butterfly_GrabPass_Texture,st_B).b;

               // col=float4(0.,0.,0.,1.);

                //col.rgb/=3.0;

                col.a=_colorGapAlpha;

                return col;
            }
            ENDCG
        }*/

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
                float4 grabPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos=ComputeGrabScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            sampler2D _Butterfly_GrabPass_Texture;
            float _vignetteRange;
            float _vignetteAlpha;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 st=i.grabPos;
                float2 uv=float2(i.grabPos.x/i.grabPos.w,i.grabPos.y/i.grabPos.w);
                uv=uv*2.0-1.0;

                fixed4 col = tex2Dproj(_Butterfly_GrabPass_Texture, st);
                float centerToDist=length(uv)*_vignetteRange;
               // col.rgb=col.rrr/centerToDist;
                col.rgb=col.rgb*exp(-centerToDist);

                // float2 test_st=float2(i.grabPos.x/i.grabPos.w,i.grabPos.y/i.grabPos.w);
                // test_st=test_st*2.0-1.0;
                // col.rgb=float3(test_st,0.);

                col.a=_vignetteAlpha;
                return col;
            }
            ENDCG
        } 

        //pass 3//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         /*Pass
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
                float4 grabPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos=ComputeGrabScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            sampler2D _Butterfly_GrabPass_Texture;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 mainCol=float4(1.,1.,1.,1.);
                float4 st=i.grabPos;
                mainCol=tex2Dproj(_Butterfly_GrabPass_Texture,st);
                return mainCol;
            }
            ENDCG
        }*/
    }
}
