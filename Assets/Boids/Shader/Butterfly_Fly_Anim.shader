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
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "LightMode"="ForwardBase"}
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
                float id : TEXCOORD1;
                float4 col : COLOR;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float _AnimSpeed;
            float _AWidth;
            float4 _BaseColor;  
            float4 _SecondColor;
            float4 _ThirdColor;
            float _boidsScale;

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

            float4x4 inverse(float4x4 m) {
    float n11 = m[0][0], n12 = m[1][0], n13 = m[2][0], n14 = m[3][0];
    float n21 = m[0][1], n22 = m[1][1], n23 = m[2][1], n24 = m[3][1];
    float n31 = m[0][2], n32 = m[1][2], n33 = m[2][2], n34 = m[3][2];
    float n41 = m[0][3], n42 = m[1][3], n43 = m[2][3], n44 = m[3][3];

    float t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
    float t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
    float t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
    float t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

    float det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;
    float idet = 1.0f / det;

    float4x4 ret;

    ret[0][0] = t11 * idet;
    ret[0][1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * idet;
    ret[0][2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * idet;
    ret[0][3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * idet;

    ret[1][0] = t12 * idet;
    ret[1][1] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * idet;
    ret[1][2] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * idet;
    ret[1][3] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * idet;

    ret[2][0] = t13 * idet;
    ret[2][1] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * idet;
    ret[2][2] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * idet;
    ret[2][3] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * idet;

    ret[3][0] = t14 * idet;
    ret[3][1] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * idet;
    ret[3][2] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * idet;
    ret[3][3] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * idet;

    return ret;
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
                boids_objectToWorld._11_22_33_44=_boidsScale;

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
                
                float3 normal=v.normal;
                /*float4x4 boids_objectToWorldNormal=inverse(boids_objectToWorld);
                normal=(mul(boids_objectToWorldNormal,float4(normal,0.0))).xyz;*/

                v2f o;
                o.vertex = mul(UNITY_MATRIX_VP,p);
                o.uv = v.uv;
                o.id=id;
                o.col=float4(1.,1.,1.,1.);
                float uid=fmod(id,2.0);
                uid=floor(uid);
                o.col.rgb=(uid==0.0) ? _BaseColor.rgb : float3(0.,0.,0.) + (uid==1.0) ? _SecondColor.rgb : float3(0.,0.,0.) + (uid==2.0) ? _ThirdColor.rgb : float3(0.,0.,0.);
                o.normal=normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb+=i.col.rgb;
                /* float3 lightDir=_WorldSpaceLightPos0.xyz;
                float diff=max(dot(i.normal,lightDir),0.5);
                float3 lightCol=_LightColor0;*/
                //col.rgb*=diff;
                return col;
            }
            ENDCG
        }
    }
}
