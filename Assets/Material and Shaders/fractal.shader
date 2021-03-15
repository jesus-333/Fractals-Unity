Shader "Explorer/fractal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Area("Area", vector) = (0, 0, 4, 4)
        _scale_time_a("scale_time_a", Float) = 1
        _scale_time_b("scale_time_b", Float) = 1
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

            #define product(a, b, scale_time_a, scale_time_b) float2((a.x*b.x-a.y*b.y), (a.x*b.y+a.y*b.x))
            #define conjugate(a) float2(a.x, -a.y)
            #define divide(a, b) float2(((a.x*b.x+a.y*b.y)/(b.x*b.x+b.y*b.y)), ((a.y*b.x-a.x*b.y)/(b.x*b.x+b.y*b.y)))

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

            float4 _Area;
            sampler2D _MainTex;
            float scale_time_a, scale_time_b;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 c = _Area.xy + (i.uv - 0.5) * _Area.zw;
                float2 z;
                float iter;

                for (iter = 0; iter < 255; iter++){
                    z = product(z, z, scale_time_a, scale_time_b) + c;

                    if(length(z) > 2) break;
                }

                return iter/255;
            }
            ENDCG
        }
    }
}
