Shader "DeZuidlanden/ColorByTextureRGB" 
{
	Properties 
	{
		_Color("Selection color",Color) = (255,255,255,255)
		_MainTex ("KleurCodering (RGB)", 2D) = "white" {}
		_BrickColor("Steen Kleur (R)",Color) = (0.2,0.2,0.2,255)
		_RoofColor("Dak Kleur (G)",Color) = (0,0,0,0)
		_WindowColor("Raam Kleur (B)", Color) = (0.6,0.6,0.6,255)
		_Specularity("Dak reflectie",Range(0.01,1))= 0.5 
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
		half _Specularity;
		fixed4 _Color;
		fixed4 _BrickColor;
		fixed4 _RoofColor;
		fixed4 _WindowColor;
		
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
			o.Albedo += _Color;
			o.Alpha = c.a;
			o.Specular = _Specularity;
			
			half3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Normal = n;
			
			
		}
		ENDCG
	} 
	FallBack "Specular"
}
