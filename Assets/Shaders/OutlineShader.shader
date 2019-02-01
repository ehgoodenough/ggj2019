Shader "Custom/Outline"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SceneTex("Scene Texture", 2D) = "white" {}
		_OutlineDepthBuffer("Outline Depth Buffer", 2D) = "black" {}
		_OutlineColor("Outline Color", Color) = (1, 0, 0, 0)
		_OutlineStrength("Outline Strength", Float) = 2
	}
		SubShader
		{
			Cull off
			ZWrite Off
			ZTest Always

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

				sampler2D _CameraDepthTexture;
				sampler2D _OutlineDepthBuffer;

				sampler2D _MainTex;
				float2 _MainTex_TexelSize;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				half frag(v2f i) : SV_Target
				{
					const float blurIterations = 20;
					float colorAmount = 0;
					float outlineDepthSample = 0;

					for (int j = 0; j < blurIterations; j++)
					{
						float2 samplePosition = i.uv + float2((j - .5 * blurIterations) * _MainTex_TexelSize.x, 0);
						colorAmount += tex2D(_MainTex, samplePosition).r / blurIterations;
						outlineDepthSample = max(outlineDepthSample, tex2D(_OutlineDepthBuffer, samplePosition).x);
					}

					// Manual z test to cull against objects in front of the object being outlined
					float sceneDepthSample = tex2D(_CameraDepthTexture, i.uv).x;
					float depthDifference = outlineDepthSample - sceneDepthSample;

					// There's a bit of fudge here, you get flickering on the outline sometimes otherwise
					return (depthDifference < -0.001 ? 0.0 : colorAmount);
				}
				ENDCG
			}

			GrabPass {}

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

				sampler2D _MainTex;
				sampler2D _SceneTex;

				// This has to be named _GrabTexture due to Unity convention.
				sampler2D _GrabTexture;
				float2 _GrabTexture_TexelSize;

				float4 _OutlineColor;
				float _OutlineStrength;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					const float blurIterations = 20;

					if (tex2D(_MainTex, i.uv.xy).r > 0)
					{
						return tex2D(_SceneTex, i.uv);
					}

					float colorAmount = 0;
					float2 grabUv = i.uv;

					#if !UNITY_UV_STARTS_AT_TOP
					grabUv.y = 1 - grabUv.y;
					#endif

					for (int j = 0; j < blurIterations; j++)
					{
						// Render texture is upside down for some reason
						colorAmount += tex2D(_GrabTexture, grabUv + float2(0, (j - .5 * blurIterations) * _GrabTexture_TexelSize.y)).r / blurIterations;
					}

					return lerp(tex2D(_SceneTex, i.uv), _OutlineColor, colorAmount * _OutlineStrength);
				}
				ENDCG
			}
		}
}