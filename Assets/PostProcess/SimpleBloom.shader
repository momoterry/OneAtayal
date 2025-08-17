Shader "Custom/SimpleBloom"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _Threshold ("Threshold", Range(0, 5)) = 1.0
        _SoftKnee ("Soft Knee", Range(0, 1)) = 0.5
        _Intensity ("Intensity", Range(0, 5)) = 1.2
        _Radius ("Blur Radius (px)", Range(0, 8)) = 2.0
        [NoScaleOffset]_BloomTex ("BloomTex", 2D) = "black" {} // Composite 時用
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Cull Off ZWrite Off ZTest Always

        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        float4 _MainTex_TexelSize; // (1/w,1/h,w,h)
        sampler2D _BloomTex;
        float4 _BloomTex_TexelSize;

        float _Threshold;
        float _SoftKnee;
        float _Intensity;
        float _Radius;

        // 計算亮度（近似感知亮度；也可用你的 Rec.601 權重）
        inline half Luma(half3 c)
        {
            return dot(c, half3(0.2126, 0.7152, 0.0722)); // Rec.709，HDR 常用
        }

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv  : TEXCOORD0;
        };

        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv  = v.texcoord.xy;
            return o;
        }
        ENDCG

        // -------- Pass 0: Prefilter（高亮擷取 + Soft Knee） --------
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_prefilter
            #pragma target 3.0

            half3 ApplyThreshold(half3 rgb)
            {
                half lum = Luma(rgb);

                // Soft knee：在 [threshold - knee, threshold] 範圍內平滑過渡
                half knee = _Threshold * _SoftKnee + 1e-5;
                half soft = saturate((lum - (_Threshold - knee)) / (2*knee));
                soft = soft * soft; // 平滑曲線（近似）

                half t = max(lum - _Threshold, 0.0) + soft * knee;
                // 只保留亮部；用比例避免色偏
                return rgb * (t / max(lum, 1e-5));
            }

            fixed4 frag_prefilter(v2f i) : SV_Target
            {
                half3 c = tex2D(_MainTex, i.uv).rgb;
                half3 hi = ApplyThreshold(c);
                return fixed4(hi, 1);
            }
            ENDCG
        }

        // -------- Pass 1: Blur Horizontal --------
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_blur_h
            #pragma target 3.0

            fixed4 frag_blur_h(v2f i) : SV_Target
            {
                float2 texel = float2(_MainTex_TexelSize.x, 0);
                float r = _Radius;

                // 5-tap 高斯近似（權重：0.204, 0.304, 0.093，各自分配到對稱位置）
                half3 sum = 0;
                sum += tex2D(_MainTex, i.uv - 2*texel*r).rgb * 0.093;
                sum += tex2D(_MainTex, i.uv - 1*texel*r).rgb * 0.304;
                sum += tex2D(_MainTex, i.uv).rgb             * 0.204;
                sum += tex2D(_MainTex, i.uv + 1*texel*r).rgb * 0.304;
                sum += tex2D(_MainTex, i.uv + 2*texel*r).rgb * 0.093;

                return fixed4(sum, 1);
            }
            ENDCG
        }

        // -------- Pass 2: Blur Vertical --------
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_blur_v
            #pragma target 3.0

            fixed4 frag_blur_v(v2f i) : SV_Target
            {
                float2 texel = float2(0, _MainTex_TexelSize.y);
                float r = _Radius;

                half3 sum = 0;
                sum += tex2D(_MainTex, i.uv - 2*texel*r).rgb * 0.093;
                sum += tex2D(_MainTex, i.uv - 1*texel*r).rgb * 0.304;
                sum += tex2D(_MainTex, i.uv).rgb             * 0.204;
                sum += tex2D(_MainTex, i.uv + 1*texel*r).rgb * 0.304;
                sum += tex2D(_MainTex, i.uv + 2*texel*r).rgb * 0.093;

                return fixed4(sum, 1);
            }
            ENDCG
        }

        // -------- Pass 3: Composite（把 Bloom 加回原圖） --------
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_composite
            #pragma target 3.0

            fixed4 frag_composite(v2f i) : SV_Target
            {
                half3 src   = tex2D(_MainTex,  i.uv).rgb;
                half3 bloom = tex2D(_BloomTex, i.uv).rgb;

                // Additive；若不想抬黑，可改為 src + (1 - exp(-bloom * intensity))
                half3 outRGB = src + bloom * _Intensity;
                return fixed4(outRGB, 1);
            }
            ENDCG
        }
    }
}
