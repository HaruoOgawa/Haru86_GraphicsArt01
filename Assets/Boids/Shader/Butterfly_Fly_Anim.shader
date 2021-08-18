Shader "Unlit/Butterfly_Fly_Anim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AnimSpeed("_AnimSpeed",Float)=1.0
        _AWidth("_AWidth",Float)=1.0
        [HDR] _BaseColor("_BaseColor",Color)=(1.,1.,1.,1.)
        [HDR] _SecondColor("_SecondColor",Color)=(1.,1.,1.,1.)
        [HDR] _ThirdColor("_ThirdColor",Color)=(1.,1.,1.,1.)
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
            #pragma multi_compile_instancing
            
            
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
                float id : TEXCOORD1;
                float4 col : COLOR;
            };

            sampler2D _MainTex;
            float _AnimSpeed;
            float _AWidth;
            float4 _BaseColor;  
            float4 _SecondColor;
            float4 _ThirdColor;

            struct Butterfly{
                float3 position;
                float3 velocity;
            };
                    // StructuredBuffer<float4x4> _ButterflyBuffer;
            StructuredBuffer<Butterfly> _boidsBuffer;
            StructuredBuffer<float3> _boidsForce;

            #define rot(a) float2x2(cos(a),sin(a),-sin(a),cos(a))
            #define PI 3.1415926535

            float4x4 GetRotMatrix(float3 angle){
                //fai Φ
                //theta θ
                //pisi Ψ

                float sfai=sin(angle.x); float stheta=sin(angle.y); float spusi=sin(angle.z);
                float cfai=cos(angle.x); float ctheta=cos(angle.y); float cpusi=cos(angle.z);
                
                float4x4 rotateMatrix=float4x4(
                    float4(ctheta*cpusi+stheta*sfai*spusi,-ctheta*spusi+stheta*sfai*spusi,stheta*cfai,0.0),
                    float4(cfai*spusi,cfai*cpusi,-sfai,0.0),
                    float4(-stheta*cpusi+ctheta*sfai*spusi,stheta*spusi+ctheta*sfai*cpusi,ctheta*cfai,0.0),
                    float4(0.0,0.0,0.0,1.0)
                );

                return rotateMatrix;
            }

            v2f vert (appdata v,uint id : SV_InstanceID)
            {
                float4 p=v.vertex;
               
               float t=sin(_Time.y*_AnimSpeed+(float)id)*sign(p.x)*_AWidth;
               float2x2 rot=float2x2(
                   float2(cos(t),-sin(t)),
                   float2(sin(t),cos(t))
               );
               p.xy=mul(rot,p.xy);


                //boid matrix
                float4x4 boids_objectToWorld=(float4x4)0;
                //scale
                boids_objectToWorld._11_22_33_44=1.0;

                //rotation
                float3 force=normalize(_boidsForce[id])*2.0*PI-PI;
                float theta_x=atan2(force.y,force.z);
                theta_x=2.0*PI-theta_x;
                float theta_y=atan2(force.x,force.z);
                
                float4x4 rotMat=GetRotMatrix(float3(theta_x,theta_y,0.0));
                boids_objectToWorld=mul(rotMat,boids_objectToWorld);

                //transform
                boids_objectToWorld._14_24_34=_boidsBuffer[id].position;

                p=mul(boids_objectToWorld,p);
                

                v2f o;
                o.vertex = mul(UNITY_MATRIX_VP,p);
                o.uv = v.uv;
                o.id=id;
                o.col=float4(1.,1.,1.,1.);
                float uid=fmod(id,2.0);
                uid=floor(uid);
                o.col.rgb=(uid==0.0) ? _BaseColor.rgb : float3(0.,0.,0.) + (uid==1.0) ? _SecondColor.rgb : float3(0.,0.,0.) + (uid==2.0) ? _ThirdColor.rgb : float3(0.,0.,0.);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb+=i.col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
