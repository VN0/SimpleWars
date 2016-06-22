// VacuumShaders 2016
// https://www.facebook.com/VacuumShaders

Shader "Hidden/VacuumShaders/ResizePro/Normalmap" 
{
	Properties 
	{
		_MainTex("", 2D) = "white" {}
	}
	 


	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert( appdata_img v ) 
	{ 
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);		
		
		o.uv =  v.texcoord.xy;	
		#if UNITY_UV_STARTS_AT_TOP
		if(_MainTex_TexelSize.y < 0.0)
			o.uv.y = 1.0 - o.uv.y;
		#endif

		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 c = tex2D(_MainTex, i.uv);
		c.x = c.a;
		c.a = 1;

		return c;
	}

	ENDCG
	

	SubShader    
	{				
		Pass
	    {
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
	    	#pragma fragment frag
			ENDCG

		} //Pass

	} //SubShader
	 
} //Shader
