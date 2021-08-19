Shader "Custom/Chrysanthemum_Flower"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 5.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        struct v2s{
            UNITY_POSITION(pos);
           
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        #ifdef SHADER_API_D3D11
        //StructuredBuffer<float4x4> _flower_buffer;
        #endif

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            //StructuredBuffer<float4x4> _flower_buffer;
            UNITY_DEFINE_INSTANCED_PROP(float4x4,_flower_buffer)
        UNITY_INSTANCING_BUFFER_END(Props)

        v2s vert(appdata_full v,uint id : SV_INSTANCEID){
            v2s o;
            UNITY_INITIALIZE_OUTPUT(v2s,o);
            float4 vertex=v.vertex;
           // #ifdef SHADER_API_D3D11
             vertex=mul(_flower_buffer[id],vertex);
           // #endif
            o.pos=UnityObjectToClipPos(vertex);

            return o;

        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
