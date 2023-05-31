Shader "Custom/CloseFade"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _UVs("UV Scale", float) = 1.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _FadeDistance("Fade Distance", Range(0, 1000)) = 50
        _FadeSpeed("Fade Speed", Range(0, 10)) = 2
        _TestMode("Test Mode", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            LOD 200
            ZWrite Off

            CGPROGRAM

            #pragma surface surf Standard fullforwardshadows alpha:fade
            #pragma target 3.0



            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
                float3 worldNormal;
            };

            half _Glossiness;
            half _Metallic;
            half _FadeDistance;
            half _FadeSpeed;
            half _TestMode;
            float _UVs;
            fixed4 _Color;


            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                //---- World Space (Aligned) Evaluation Here -----

                float3 Pos = IN.worldPos / (-1.0 * abs(_UVs));

                float3 c00 = tex2D(_MainTex, IN.worldPos / 10);

                float3 c1 = tex2D(_MainTex, Pos.yz).rgb;
                float3 c2 = tex2D(_MainTex, Pos.xz).rgb;
                float3 c3 = tex2D(_MainTex, Pos.xy).rgb;

                float alpha21 = abs(IN.worldNormal.x);
                float alpha23 = abs(IN.worldNormal.z);

                float3 c21 = lerp(c2, c1, alpha21).rgb;
                float3 c23 = lerp(c21, c3, alpha23).rgb;


                fixed3 c = c23 * _Color;
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