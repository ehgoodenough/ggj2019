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

		// for wave effect
		//_ScanDistance("Scan Distance", float) = 0
		_EffectPos("Effect Position", Vector) = (1,1,1,1)
		_ScanSmoothAmount("Scan Smooth Amount", float) = 0
		_EdgeNoiseTex("Edge Noise Texture", 2D) = "white" {}
		_EdgeScale("Edge Scale", float) = 0
		_EdgeScaleX("Edge Scale X", float) = 0
		//_SaturationLevel("Saturation Level", float) = 0
		//_TargetSaturationLevel("Target saturation Level", float) = 0
		_EdgeGlowStrength("Edge glow strength", float) = 0
		_EdgeSpeed("Edge speed", float) = 0
		_EdgeGlowExp("Edge glow exp", float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZTest Always

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
				float4 ray : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 interpolatedRay : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.interpolatedRay = v.ray;
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

			float4 _EffectOrigin;
			float _ScanDistance;
			float _ScanSmoothAmount;
			sampler2D _EdgeNoiseTex;
			float _EdgeScale;
			float _EdgeScaleX;
			float _EdgeGlowStrength;
			float _EdgeSpeed;
			float _EdgeGlowExp;

			float _SaturationLevel; 
			float _TargetSaturationLevel;

			// 2D Random
			float random(fixed2 st) {
				return frac(sin(dot(st.xy,
					fixed2(12.9898, 78.233)))
					* 43758.5453123);
			}

			// 2D Noise based on Morgan McGuire @morgan3d
			// https://www.shadertoy.com/view/4dS3Wd
			float noise(fixed2 st) {
				fixed2 i = floor(st);
				fixed2 f = frac(st);

				// Four corners in 2D of a tile
				float a = random(i);
				float b = random(i + fixed2(1.0, 0.0));
				float c = random(i + fixed2(0.0, 1.0));
				float d = random(i + fixed2(1.0, 1.0));

				// Smooth Interpolation

				// Cubic Hermine Curve.  Same as SmoothStep()
				fixed2 u = f * f*(3.0 - 2.0*f);
				// u = smoothstep(0.,1.,f);

				// Mix 4 coorners percentages
				return lerp(a, b, u.x) +
					(c - a)* u.y * (1.0 - u.x) +
					(d - b) * u.x * u.y;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				// saturation wave stuff
				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv));
				float linearDepth = Linear01Depth(rawDepth);
				float4 wsDir = linearDepth * i.interpolatedRay;
				float3 wsPos = _WorldSpaceCameraPos + wsDir;

				float dist = distance(wsPos, _EffectOrigin);

				fixed4 texCol = tex2D(_MainTex, i.uv);
				
				float luminance = Luminance(texCol.rgb);
				float darkness = (1 - luminance);

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

				fixed4 dotsColor = lerp(fixed4(1, 1, 1, 1) * dotCol * steppedLuminance, round(Luminance(pow((texCol.rgb), _LuminancePower)) / inc) * inc, _LuminanceLerp);

				// ignore items on the color layer
				fixed4 halftoneEffectColor = colorBufferSample > .5 && depthDifference < .001 ? lerp(dotCol, texCol, .95) : dotsColor;

				fixed4 fullColor = lerp(dotCol, texCol, .95);

				float noiseLookup = atan2(wsPos.z - _EffectOrigin.z, wsPos.x - _EffectOrigin.x);
				_EdgeScale *= clamp((_ScanDistance - 1)/6, 0, 1);
				dist += noise(fixed2(noiseLookup * _EdgeScaleX, _Time.y * _EdgeSpeed)) * _EdgeScale;

				float glowAmount = 0;
				if (_ScanDistance > dist) {
					float distanceFromEdge = _ScanDistance - dist;
					float glowSize = 8;
					float normalized = 1 - clamp(distanceFromEdge / glowSize, 0, 1);
					glowAmount = pow(normalized, _EdgeGlowExp);
				}

				return lerp(lerp(halftoneEffectColor, fullColor, _TargetSaturationLevel + glowAmount * _EdgeGlowStrength), lerp(halftoneEffectColor, fullColor, _SaturationLevel + glowAmount * _EdgeGlowStrength), smoothstep(_ScanDistance, _ScanDistance + _ScanSmoothAmount, dist));            }
            ENDCG
        }
    }
}
