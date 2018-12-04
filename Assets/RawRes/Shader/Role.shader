Shader "SLGGame/Role"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Gloss ("Gloss", Range(2, 128)) = 16
		_SpecularFresnel ("SpecularFresnel", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="true" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 worldViewDir : TEXCOORD2;
				float3 worldLightDir : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 worldPos = UnityObjectToWorldDir(o.vertex).xyz;
				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				o.worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				return o;
			}

			float calcFresnel(float3 viewDir, float3 halfVector, float _SpecularFresnel) 
			{
				float fresnel = 1.0 - dot(viewDir, halfVector);
				fresnel = pow(fresnel, 5.0);
				fresnel += _SpecularFresnel * (1.0 - fresnel);
				return fresnel;
			}

			float _Gloss;
			float _SpecularFresnel;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float diffuse = max(0, dot(i.worldNormal, i.worldLightDir)) * 0.5 + 0.5;
				float3 halfVector = normalize(i.worldLightDir + i. worldViewDir);
				float specular = pow(max(0, dot(halfVector, i.worldNormal)) , _Gloss);
				float fresnel = calcFresnel(i.worldViewDir, halfVector, _SpecularFresnel);

				col.rgb *= (diffuse + specular * fresnel);
				return col;
			}
			ENDCG
		}
	}
	Fallback"Diffuse"
}
