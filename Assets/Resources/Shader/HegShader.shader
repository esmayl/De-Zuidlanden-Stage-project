Shader "DeZuidlanden/HegShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailMap ("Detail map", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _DetailMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DetailMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Albedo *= tex2D(_DetailMap,IN.uv_DetailMap).rgba*1;
			o.Alpha = c.a+tex2D(_DetailMap,IN.uv_DetailMap).a*1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
