sampler ColorMapSampler;

int WIDTH = 640;
int HEIGHT = 480;

const float PI = 3.141592654;

float4 Nothing(float2 coords0: TEXCOORD0, float4 color0: COLOR0): COLOR0 
{
	float4 color = tex2D(ColorMapSampler, coords0.xy);
	return color;
}

float4 Sharpen(float2 coords0: TEXCOORD0, float4 color0: COLOR0): COLOR0 
{
	float4 color = tex2D(ColorMapSampler, coords0.xy);
	float factor = 20;
	color -= tex2D(ColorMapSampler, coords0.xy + 0.0001f) * factor;
	color += tex2D(ColorMapSampler, coords0.xy - 0.0001f) * factor;

	return color;
}

float4 CRT(float4 color: COLOR0, float2 texCoord : TEXCOORD0): COLOR0 
{ 
	float2 texCoordOffset = float2(0.0015f, 0.0f); 
	float r = tex2D(ColorMapSampler, texCoord - texCoordOffset).r; 
	float g = tex2D(ColorMapSampler, texCoord).g; 
	float b = tex2D(ColorMapSampler, texCoord + texCoordOffset).b; 
	float4 imageColor = float4(r, g, b, 1); 
 
	//float4 scanlineColor = 1.5f * float4(1, 1, 1, 1.0f) * abs(sin(texCoord.y * 768) + 0.0f); 
	//float4 scanlineColor = 0.3f + abs(sin(texCoord.y * 768));
	float4 scanlineColor = 0.15f + saturate(HEIGHT / 1.5f * texCoord.y % 2 + 0.2f); //640 is texture width
	
	return color * imageColor * scanlineColor; 
} 

//float2 radialDistortion(float2 coord, float2 pos) 
//{ 
//	float distortion = 0.125f; 
//	float2 cc = pos - 0.5f; 
//	float dist = dot(cc, cc) * distortion;
//	if ((dist < coord.x * (distortion + 1.0f)) &&\
//		(dist < coord.y * (distortion + 1.0f)) &&\
//		(dist < (distortion + 1.0f) - coord.x * (distortion+1.0f)) &&\
//		(dist < (distortion + 1.0f) - coord.y * (distortion+1.0f)))
//		return coord * (pos + cc * (1.0f + dist) * dist) / pos;
//	else return float2(0.5f, 0.5f); 
//} 
 
float4 Distort(float4 color: COLOR0, float2 texCoord: TEXCOORD0): COLOR0 
{ 
	float distortionAmount = 0.25f; 
	float2 cc = texCoord - 0.5f; //center
	float dist = dot(cc, cc) * distortionAmount; //distance
	if ((dist < texCoord.x * (2.0f)) &&\
		(dist < texCoord.y * (2.0f)) &&\
		(dist < (2.0f) - texCoord.x * (2.0f)) &&\
		(dist < (2.0f) - texCoord.y * (2.0f))) //determine internal area
		return tex2D(ColorMapSampler, texCoord * (texCoord + cc * (1.0f + dist) * dist) / texCoord);
	else return float4(0.0f, 0.0f, 0.0f, 0.0f); //outer area is fully transparent
	
	//texCoord = radialDistortion(texCoord, texCoord); 
	//return tex2D(ColorMapSampler, texCoord.xy);  
}

float4 LCD(float2 coords0: TEXCOORD0, float4 color0: COLOR0): COLOR0 
{
	const int brighten_scanlines = 12;
	const int brighten_lcd = 4;
	const float3 offsets = PI * 1.75f * float3(1.0/2, 1.0/2 - 2.0/3, 1.0/2 - 4.0/3);
	float2 omega = PI * 2.75f * 640;
	float3 res = tex2D(ColorMapSampler, coords0).xyz;
	float2 angle = coords0 * omega;
	float yfactor = (brighten_scanlines + sin(angle.y)) / (brighten_scanlines + 1);
	float3 xfactors = (brighten_lcd + sin(angle.x + offsets)) / (brighten_lcd + 1);
	float3 color = yfactor * xfactors * res;
	return float4(color.x, color.y, color.z, 1.0);
}

float4 Amber(float2 texCoord: TEXCOORD0, float4 color: COLOR0): COLOR0 
{
	const float colors = 256.0f;
	half3 ink = half3(0.55f, 0.41f, 0.0f);
	half3 c11 = tex2D(ColorMapSampler, texCoord).xyz;
	half lct = floor(colors * length(c11)) / colors;
	return half4(lct * ink, 1);
}

float4 Green(float2 texCoord: TEXCOORD0, float4 color: COLOR0): COLOR0 
{
	const float colors = 256.0f;
	half3 ink = half3(0.32, 0.50, 0.0);  
	half3 c11 = tex2D(ColorMapSampler, texCoord).xyz;
	half lct = floor(colors * length(c11)) / colors;
	return half4(lct*ink, 1);
}

float4 Orange(float2 texCoord: TEXCOORD0, float4 color: COLOR0): COLOR0 
{
	const float colors = 256.0f;
	half3 ink = half3(0.57, 0.28, 0.0);  
	half3 c11 = tex2D(ColorMapSampler, texCoord).xyz;
	half lct = floor(colors * length(c11)) / colors;
	return half4(lct*ink, 1);
}

float4 Bright(float2 texCoord: TEXCOORD0, float4 color: COLOR0): COLOR0 
{
	float factor = 0.15f;
	float level = 0.5f;
	float4 color0 = tex2D(ColorMapSampler, texCoord.xy);
	if (color0.r > level) { color0 += factor; }
	if (color0.g > level) { color0 += factor; }
	if (color0.b > level) { color0 += factor; }
	return saturate(color0);
}

technique Main {
	pass Nothing { PixelShader = compile ps_2_0 Nothing(); }
	//pass Amber { PixelShader = compile ps_2_0 Amber(); }
	//pass Green { PixelShader = compile ps_2_0 Green(); }
	//pass Orange { PixelShader = compile ps_2_0 Orange(); }
	//pass Sharpen { PixelShader = compile ps_2_0 Sharpen(); }
	//pass CRT { PixelShader = compile ps_2_0 CRT(); }
	pass Brighten { PixelShader = compile ps_2_0 Bright(); }
	pass LCD { PixelShader = compile ps_2_0 LCD(); }
	pass Barrel { PixelShader = compile ps_2_0 Distort(); }
}