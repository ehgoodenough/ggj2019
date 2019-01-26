Shader "Custom/Halftone"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ColorBuffer("Color Buffer", 2D) = "white" {}
		_DepthBuffer("Depth Buffer", 2D) = "black" {}

		_GridScale("Grid scale", Float) = 1
		_NumLevels("Num levels", Float) = 5
		_DotSizeMult("Dot size multiplier", Float) = .96
		_LuminancePower("Luminance power", Float) = 2
		_LuminanceLevels("Luminance levels", Float) = 3
		_LuminanceLerp("Luminance lerp", Float) = .5
			
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

            sampler2D _MainTex;
			sampler2D _DotTexture;
			sampler2D _CameraDepthTexture;
			sampler2D _ColorBuffer;
			sampler2D _DepthBuffer;
			float _GridScale;
			float _NumLevels;
			float _DotSizeMult;
			float _LuminancePower;
			float _LuminanceLerp;
			float _LuminanceLevels;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 texCol = tex2D(_MainTex, i.uv);
				
				float darkness = (1 - Luminance(texCol.rgb));

				float scaleIncrement = (1 / _NumLevels);

				float scale = round((darkness) / scaleIncrement) * scaleIncrement;
				scale = smoothstep(0., 1., scale);
				//_GridScale /= scale + .001;

				float stagger = fmod(floor(i.uv.y * _GridScale), 2);

				fixed2 dotUv = fixed2(i.uv.x * (_ScreenParams.x / _ScreenParams.y), i.uv.y) * _GridScale;
				dotUv.x += stagger * .5;
				float stepVal = scale * _DotSizeMult;
				float dotCol = step(stepVal, length(frac(dotUv) - .5));
				float inc = 1 / _LuminanceLevels;
				float steppedLuminance = round(Luminance(pow((texCol.rgb), _LuminancePower)) / inc) * inc;

				float depthDifference = tex2D(_CameraDepthTexture, i.uv).r - tex2D(_DepthBuffer, i.uv).r;
				float colorBufferSample = tex2D(_ColorBuffer, i.uv).r;

				fixed4 halftoneColor = lerp(fixed4(1, 1, 1, 1) * dotCol * steppedLuminance, floor(Luminance(pow((texCol.rgb), _LuminancePower)) / inc) * inc, _LuminanceLerp);
				return colorBufferSample > .5 && depthDifference < -.001 ? lerp(dotCol, texCol, .95) : halftoneColor;
            }
            ENDCG
        }
    }
}
