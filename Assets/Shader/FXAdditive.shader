Shader "Custom/FXAdditive"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass
        {
            Blend SrcAlpha One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);// * _Brightness;
                col.a = col.a * _Brightness;
                //half gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                return col;
            }
            ENDCG
        }
    }
}