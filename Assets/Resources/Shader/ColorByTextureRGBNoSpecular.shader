Shader "DeZuidlanden/ColorByTextureRGBNoSpecular" 
{
	Properties 
	{
		_MainTex ("KleurCodering (RGB)", 2D) = "white" {}
		_BrickColor("Steen Kleur (R)",Color) = (0.2,0.2,0.2,0.2)
		_RoofColor("Dak Kleur (G)",Color) = (0,0,0,0)
		_WindowColor("Raam Kleur (B)", Color) = (0.6,0.6,0.6,0.6)
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _BrickColor;
		float4 _RoofColor;
		float4 _WindowColor;
		 
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			half4 c = tex2D (_MainTex, IN.uv_MainTex);

			float4 tempR = _BrickColor * c.r;
			float4 tempG = _RoofColor * c.g;
			float4 tempB = _WindowColor * c.b;
			
			
			o.Albedo = tempR+tempG+tempB;
			o.Alpha = c.a;
			
			half3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Normal = n;
		}
		ENDCG
	} 
	FallBack "Specular"
}
