Shader "Custom/LeftRightTintPreserveDetail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseTint ("Base Tint", Color) = (0,0,1,1)      // Blue
        _FillTint ("Fill Tint", Color) = (0.5,0,0.5,1)  // Purple
        _Fill ("Fill Amount", Range(0,1)) = 0
        _TintStrength ("Tint Strength", Range(0,1)) = 1 // 1 = full tint, 0 = no tint
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseTint;
            float4 _FillTint;
            float _Fill;
            float _TintStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                // Determine which tint applies
                float t = step(i.uv.x, _Fill);
                float3 tint = lerp(_BaseTint.rgb, _FillTint.rgb, t);

                // Apply tint by multiplying, preserving detail
                float3 finalRGB = lerp(tex.rgb, tex.rgb * tint, _TintStrength);

                return float4(finalRGB, tex.a);
            }
            ENDCG
        }
    }
}