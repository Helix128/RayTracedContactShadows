Shader "Hidden/Combine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _CS ("CS Buffer", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
		
        Pass
        {
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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
			float4 _Ambient;
			sampler2D _MainTex;
			sampler2D _CS;
			sampler2D _SSS;
			fixed4 frag(v2f i) : SV_Target
			{
			
			float cs = (tex2D(_MainTex,i.uv).r);
			float ss = tex2D(_SSS, i.uv).r;
			

			return min(cs,ss);
			
            }
            ENDCG
        }
    }
}
