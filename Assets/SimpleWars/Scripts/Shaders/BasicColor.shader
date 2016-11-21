Shader "Basic Color"
{
	Properties
	{
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
		struct vertexInput
	{
		float4 vertex : POSITION;
		fixed4 color : COLOR;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
	};


	v2f vert(vertexInput v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		return o;
	}

	sampler2D _MainTex;

	float4 frag(v2f IN) : COLOR
	{
		return IN.color;
	}
		ENDCG
	}
	}
}