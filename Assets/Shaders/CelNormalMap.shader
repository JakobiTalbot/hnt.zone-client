Shader "Custom/CelNormalMap"
{
	Properties
	{
		_Colour("Colour", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Glossiness", Float) = 32
		[HDR]
		_Ambient("Ambient", Color) = (0.4, 0.4, 0.4, 1)
		[HDR]
		_Specular("Specular", Color) = (0.9, 0.9, 0.9, 1)
		[HDR]
		_Rim("Rim", Color) = (1, 1, 1, 1)
		_RimAmount("RimAmount", Range(0,1)) = 0.716
		_RimThreshold("RimThreshold", Range(0,1)) = 0.1
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdpass
			#define SHADOWS_SCREEN
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD1;
				SHADOW_COORDS(2)
				float3 worldTangent : TEXCOORD3;
				float3 worldBinormal : TEXCOORD4;
			};

			sampler2D _MainTex;
			sampler2D _NormalMap;
			float4 _MainTex_ST;
			float4 _NormalMap_ST;

			fixed4 _Ambient;
			float4 _Specular;
			float4 _Rim;
			float4 _Colour;
			float _RimAmount;
			float _RimThreshold;
			float _Glossiness;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);

				float4x4 modelMat = unity_ObjectToWorld;
				float4x4 modelMatInv = unity_WorldToObject;

				o.worldTangent = normalize(mul(modelMat, float4(v.tangent.xyz, 0.0)).xyz);
				o.worldBinormal = normalize(cross(o.worldNormal, o.worldTangent) * v.tangent.w);

				TRANSFER_SHADOW(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				// normal calculations
				float4 encodedNormal = tex2D(_NormalMap, _NormalMap_ST.xy * i.uv.xy + _NormalMap_ST.zw);
				float3 localCoords = float3(2.0 * encodedNormal.a - 1.0, 2.0 * encodedNormal.g - 1.0, 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));

				float3x3 local2WorldTranspose = float3x3(
					i.worldTangent,
					i.worldBinormal,
					i.worldNormal);
				float3 normalDir = normalize(mul(localCoords, local2WorldTranspose));

				float4 sample = tex2D(_MainTex, i.uv);
				// light variables
				float3 normal = normalize(i.worldNormal + normalDir);
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float shadow = smoothstep(0.5, 1, SHADOW_ATTENUATION(i));
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
				float4 light = lightIntensity * _LightColor0;

				float3 viewDir = normalize(i.viewDir);
				float3 halfVec = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVec);

				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float4 specular = smoothstep(0.005, 0.01, specularIntensity) * _Specular;

				float4 rimDot = 1 - dot(viewDir, normal);
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _Rim;

				// sample the texture
				return _Colour * sample * (_Ambient + light + specular + rim);
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}