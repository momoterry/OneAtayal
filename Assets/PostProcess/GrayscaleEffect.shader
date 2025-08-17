Shader "Custom/GrayscaleEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SepiaR("Sepia R Factor", Float) = 1.0
        _SepiaG("Sepia G Factor", Float) = 1.0
        _SepiaB("Sepia B Factor", Float) = 1.0
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

            // 宣告可編輯參數
            float _SepiaR;
            float _SepiaG;
            float _SepiaB;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float3 sepia = float3(gray * _SepiaR, gray * _SepiaG, gray * _SepiaB);
                return fixed4(sepia, col.a);
            }
            ENDCG
        }
    }
}
