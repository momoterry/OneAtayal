Shader "Custom/LightEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(0, 2)) =1.0
    }
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend DstColor One

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.color = IN.color * _Color;
				OUT.texcoord = IN.texcoord;

				return OUT;
			}

			sampler2D _MainTex;
			float _Brightness;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D (_MainTex, IN.texcoord) * IN.color;
				c.rgb *= c.a * _Brightness;
				return c;
			}
		ENDCG
		}
	}
}
//Shader "Custom/LightEffect"
//{
//    Properties
//    {
//        _MainTex ("Texture", 2D) = "white" {}
//    }
//    SubShader
//    {
//        Tags { "Queue" = "Transparent" }
//        Pass
//        {
//            Blend DstColor One
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag

//            #include "UnityCG.cginc"

//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float2 uv : TEXCOORD0;
//            };

//            struct v2f
//            {
//                float2 uv : TEXCOORD0;
//                UNITY_FOG_COORDS(1)
//                float4 vertex : SV_POSITION;
//            };

//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//            float _Brightness;

//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                UNITY_TRANSFER_FOG(o,o.vertex);
//                return o;
//            }

//            half4 frag (v2f i) : SV_Target
//            {
//                half4 col = tex2D(_MainTex, i.uv);
//                col.rgb = col.rgb * col.a;
//                //half gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
//                return col;
//            }
//            ENDCG
//        }
//    }
//}
