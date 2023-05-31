Shader "Custom/CloseFade"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _FadeDistance("Fade Distance", Range(0, 1000)) = 50
        _FadeSpeed("Fade Speed", Range(0, 10)) = 2
        _TestMode("Test Mode", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 200

            CGPROGRAM

            #pragma surface surf Standard fullforwardshadows alpha:fade
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
            };

            half _Glossiness;
            half _Metallic;
            half _FadeDistance;
            half _FadeSpeed;
            half _TestMode;
            fixed4 _Color;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                float dist = abs(IN.worldPos.y - _WorldSpaceCameraPos.y);
                float distrat = dist / _FadeDistance;
                o.Alpha = floor(saturate(distrat)) * saturate((distrat - 1) * _FadeSpeed);
                o.Alpha = saturate(o.Alpha + _TestMode);
            }
            ENDCG
        }
            FallBack "Standard"
}