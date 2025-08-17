Shader "Custom/SaturationBoost"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // 0 = 灰階, 1 = 原圖, >1 = 增加彩度
        _Saturation ("Saturation", Range(0, 10)) = 2.0
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half _Saturation;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Rec.601 luma (與你先前 sepia 使用的權重一致)
                half gray = dot(col.rgb, half3(0.299, 0.587, 0.114));

                // 彩度調整：gray + (col - gray) * saturation
                half3 outRGB = gray.xxx + (col.rgb - gray.xxx) * _Saturation;

                // 可選：避免高彩度造成超出範圍
                outRGB = saturate(outRGB);

                return fixed4(outRGB, col.a);
            }
            ENDCG
        }
    }
}
