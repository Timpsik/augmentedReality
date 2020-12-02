Shader "Custom/Diffuse" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}

    [Enum(Equal,3,NotEqual,6)] _StencilTest ("Stencil Test", int) = 6
}
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 150

            Stencil {
            Ref 1
            Comp [_StencilTest]
        }

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;

struct Input {
    float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

//Fallback "Mobile/VertexLit"
}