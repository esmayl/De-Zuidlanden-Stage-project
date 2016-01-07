Shader "DeZuidlanden/SpecularShader" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB)", 2D) = "white" {}
        _SpecularTex ("Specular (RGB)", 2D) = "gray" {}
        _BumpMap ("Normal (Normal)", 2D) = "bump" {}
        _Gloss ("Gloss Value", Range(0.2,1)) = 0.5
    }
 
    SubShader{
        Tags { "RenderType" = "Opaque" }
   
        CGPROGRAM

			#pragma surface surf ColourSpec
           
            struct Input
            {
                float2 uv_MainTex;
            };

			struct SurfaceOutputColourSpec {
                fixed3 Albedo;
                fixed3 Normal;
                fixed3 Specular;
                fixed3 Emission;
                fixed Gloss;
                fixed Alpha;
            };
           
            sampler2D _MainTex, _SpecularTex, _BumpMap;
           
		    fixed4 _Color;
            float _Gloss;
            float _Cutoff;


            inline fixed4 LightingColourSpec (SurfaceOutputColourSpec s, fixed3 lightDir, fixed3 viewDir, fixed atten)
            {
                clip(s.Alpha - _Cutoff);
               
                viewDir = normalize(viewDir);
                lightDir = normalize(lightDir);
                float NdotL = saturate(dot(s.Normal, lightDir));
                float3 h = normalize(lightDir + viewDir);
               
                float specBase = saturate(dot(s.Normal, h));
 
                float3 spec = s.Specular * pow(specBase, s.Gloss * 128) * _LightColor0.rgb;
 
                fixed4 c;
                c.rgb = ((s.Albedo * _LightColor0.rgb * NdotL) + (spec)) * (atten * 2);
                c.a = s.Alpha;
                return c;
            }
       

            void surf (Input IN, inout SurfaceOutputColourSpec o)
            {
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb*_Color.rgb;
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
                o.Gloss = _Gloss;
            }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}