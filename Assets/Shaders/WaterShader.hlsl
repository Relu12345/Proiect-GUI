#pragma shader_feature_local _ _CustomFunctionShading

sampler2D _MainTex;

struct VertexOutput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

struct FragmentOutput
{
    float4 fragColor : SV_Target;
};

FragmentOutput CustomFunctionFragment(VertexOutput i)
{
    FragmentOutput o;

    // Sample the texture
    float4 col = tex2D(_MainTex, i.uv);
                
    // Apply any other effects or calculations here
                
    o.fragColor = col;

    return o;
}
