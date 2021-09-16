Shader "Unlit/PetalGPUParticle_Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color("_Color",Color)=(1,1,1,1)
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
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            #define PI 3.14159265
            #define rot(a) float2x2(cos(a),-sin(a),sin(a),cos(a))

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

            struct PetalAnimation{
                float petalLifeTime;
                float3 position;
                float3 rotation;
                float4 petalColor;
                float3 petalAngular;
                float3 petalVelocity;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            StructuredBuffer<PetalAnimation> _read_petalAnim_buffer;
            
            v2f vert (appdata v,uint id : SV_INSTANCEID)
            {
                PetalAnimation petal=_read_petalAnim_buffer[id];

                float4 pos=v.vertex;
                pos.xyz*=0.75;
                pos.yz=mul(rot(petal.rotation.x),pos.yz);
                pos.xz=mul(rot(petal.rotation.y),pos.xz);
                pos.xy=mul(rot(petal.rotation.z),pos.xy);
                pos.xyz+=petal.position;
                
                

                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv =v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                return col;
            }
            ENDCG
        }
    }
}
