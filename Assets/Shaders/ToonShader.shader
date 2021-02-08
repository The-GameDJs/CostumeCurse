Shader "Custom/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness("Brightness", Range(0,1)) = 0.3
        _Strength("Strength", Range(0,1)) = 0.5
        _Color("Color", COLOR) = (1,1,1,1)

        [HDR]
		_SpecularColor("Specular Color", COLOR) = (0.9,0.9,0.9,1)
		// Controls the size of the specular reflection.
		_Glossiness("Glossiness", Float) = 32

        [HDR]
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 0.716
    }
    SubShader
    {
        Tags
        { 
            "RenderType" = "Opaque"
			"PassFlags" = "OnlyDirectional"
            "RenderPipeline" = "UniversalRenderPipeline" 
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
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
                float3 positionWS : TEXCOORD2;
                half3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            float _Strength;
            float4 _Color;

            float4 _SpecularColor;
            float _Glossiness;

            float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;

            float3 GetWorldSpaceViewDir(float3 positionWS) 
            {
                if (unity_OrthoParams.w == 0) 
                {
                    // Perspective
                    return _WorldSpaceCameraPos - positionWS;
                } 
                else 
                {
                    // Orthographic
                    float4x4 viewMat = GetWorldToViewMatrix();
                    return viewMat[2].xyz;
                }
            }

            float Toon(float3 normal, float3 lightDir) {
                float NdotL = max(0.0,dot(normalize(normal), normalize(lightDir)));

                return NdotL > 0 ? 1 : 0;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.viewDir = GetWorldSpaceViewDir(v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);

                float NdotL = Toon(i.worldNormal, -TransformWorldToShadowCoord(i.positionWS.xyz));

                // Sample the texture
                half4 col = tex2D(_MainTex, i.uv);

                float lightIntensity = smoothstep(0, 0.01, NdotL);
                float4 light = lightIntensity * _MainLightColor;

                // Calculate specular reflection
                float3 halfVector = normalize(-TransformWorldToShadowCoord(i.positionWS.xyz) + viewDir);
				float NdotH = dot(normal, halfVector);
				// Multiply _Glossiness by itself to allow artist to use smaller
				// glossiness values in the inspector
				float specularIntensity = pow(abs(NdotH * lightIntensity), _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;	

                // Calculate rim lighting.
				float rimDot = 1 - dot(viewDir, normal);
				// We only want rim to appear on the lit side of the surface,
				// so multiply it by NdotL, raised to a power to smoothly blend it.
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimDot);
				float4 rim = rimIntensity * _RimColor;

                col *=  (light + specular + rim) * _Strength * _Color + _Brightness;
                return col;
            }

            ENDHLSL
        }
    }
}
