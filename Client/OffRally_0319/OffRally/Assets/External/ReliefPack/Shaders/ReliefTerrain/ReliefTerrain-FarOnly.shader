//
// Relief Terrain Shader - version for far distances - used only in Unity4, Unity5
// Tomasz Stobierski 2014
//
Shader "Relief Pack/ReliefTerrain-FarOnly" {
Properties {
	_Control ("Control (RGBA)", 2D) = "red" {} 
	_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
	// used in fallback on old cards
	_MainTex ("BaseMap (RGB)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
}

/* INIT
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//
//
// POM / PM / SIMPLE shading
//
//
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
SubShader {
	Tags {
		"SplatCount" = "12"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
	LOD 700
	Fog { Mode Off }
	CGPROGRAM

	#pragma surface surf CustomBlinnPhong vertex:vert finalcolor:customFog  noforwardadd nolightmap noambient
	// U5 fog handling
	#pragma multi_compile_fog	
	#include "UnityCG.cginc"

	#pragma target 3.0
	#pragma glsl
	#pragma only_renderers d3d9 opengl gles flash d3d11
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	//#define RTP_POM_SHADING_LO
	//#define RTP_PM_SHADING
	//#define RTP_SIMPLE_SHADING
	
	// for geom blend (early exit from surf function)
	//#define COLOR_EARLY_EXIT
	// tangents approximation
	#define APPROX_TANGENTS
	
	#define FAR_ONLY

	#include "RTP_Base.cginc"

	ENDCG

///astar AddFar
Fog { Mode Off }
ZWrite Off
CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert finalcolor:customFog decal:blend noforwardadd nolightmap noambient
	// U5 fog handling
	#pragma multi_compile_fog	
	#include "UnityCG.cginc"
	   
	#pragma target 3.0
	#pragma glsl
	#pragma only_renderers d3d9 opengl gles flash d3d11
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	//#define RTP_PM_SHADING
	//#define RTP_SIMPLE_SHADING
	
	// for geom blend (early exit from sur function)
	//#define COLOR_EARLY_EXIT
	
	#define APPROX_TANGENTS
	
	#define FAR_ONLY		
		
	#include "RTP_AddBase.cginc"

ENDCG  	
//astar/ // AddFar

// (not used / commented below when addshadow surf keyword used above)
///astar SHADOW PASSES
	// Pass to render object as a shadow caster
	Pass {
		Name "Caster"
		Tags { "LightMode" = "ShadowCaster" }
		Offset 1, 1
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual Cull Off

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"
#define UNITY_PASS_SHADOWCASTER

#define RTP_CUT_HOLES

struct v2f { 
	V2F_SHADOW_CASTER;
	#ifdef RTP_CUT_HOLES
	float2  uv : TEXCOORD1;
	#endif
};

uniform float4 _MainTex_ST;

v2f vert( appdata_base v )
{
	v2f o;
	//v.vertex.y+=20;
	TRANSFER_SHADOW_CASTER(o)
	#ifdef RTP_CUT_HOLES
	o.uv =v.texcoord;
	#endif
	return o;
}

#ifdef RTP_CUT_HOLES
uniform sampler2D _ColorMapGlobal;
#endif

float4 frag( v2f i ) : COLOR
{
	#ifdef RTP_CUT_HOLES
	float4 global_color_value=tex2D(_ColorMapGlobal, i.uv);
	clip(global_color_value.a-0.001f);
	#endif	
	SHADOW_CASTER_FRAGMENT(i)
}
ENDCG

}

	// Pass to render object as a shadow collector
	Pass {
		Name "ShadowCollector"
		Tags { "LightMode" = "ShadowCollector" }
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcollector
#define UNITY_PASS_SHADOWCOLLECTOR
#define SHADOW_COLLECTOR_PASS
#include "UnityCG.cginc"

/astar
// Shadow Softener part
#pragma target 3.0
// Define the Shadow Filter
#define SOFTENER_FILTER PCF8x8
// Include Shadow Softener
#include "../../../Shadow Softener/Shaders/ShadowSoftener.cginc"
astar/

#define RTP_CUT_HOLES

struct v2f {
	V2F_SHADOW_COLLECTOR;
	#ifdef RTP_CUT_HOLES	
	float2  uv : TEXCOORD5;
	#endif
};

v2f vert (appdata_base v)
{
	v2f o;
	TRANSFER_SHADOW_COLLECTOR(o)
	#ifdef RTP_CUT_HOLES
	o.uv = v.texcoord;
	#endif
	return o;
}

#ifdef RTP_CUT_HOLES
uniform sampler2D _ColorMapGlobal;
#endif

fixed4 frag (v2f i) : COLOR
{
	#ifdef RTP_CUT_HOLES
	float4 global_color_value=tex2D(_ColorMapGlobal, i.uv);
	clip(global_color_value.a-0.001f);
	#endif	
	
	SHADOW_COLLECTOR_FRAGMENT(i)
}
ENDCG

}
//astar/ // SHADOW PASSES

	
}
// EOF POM / PM / SIMPLE shading
*/ // INIT


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//
//
// CLASSIC shading
//
//
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
SubShader {
	Tags {
		"SplatCount" = "12"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
	LOD 100

CGPROGRAM
	#pragma surface surf Lambert vertex:vert noforwardadd nolightmap noambient
	#include "UnityCG.cginc"
	
	#pragma only_renderers d3d9 opengl gles flash d3d11
		
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#define ADDITIONAL_FEATURES_IN_FALLBACKS

	#ifdef ADDITIONAL_FEATURES_IN_FALLBACKS	
		// comment if you don't need global color map
		#define COLOR_MAP
		// if not defined global color map will be blended (lerp)
		#define COLOR_MAP_BLEND_MULTIPLY
		
		#define RTP_CUT_HOLES
		
		//#define RTP_SNOW
	#endif
	
/////////////////////////////////////////////////////////////////////
	
sampler2D _Control, _Control1;
sampler2D _SplatA0,_SplatA1,_SplatA2,_SplatA3;
float4 _TERRAIN_ReliefTransform;
half _Shininess;
float4 _Spec0123;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef COLOR_MAP
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
sampler2D _ColorMapGlobal;
#endif
#ifdef RTP_SNOW
float rtp_snow_strength;
float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer0123;
float4 rtp_snow_strength_per_layer4567;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;
fixed4 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 _uv_Relief;
	float4 snowDir;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_Relief.xy=mul(_Object2World, v.vertex).xz / _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
			
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		o.snowDir.xyz = normalize( mul(_Object2World, float4(v.normal.xyz,0)).xyz );
		o.snowDir.w = v.vertex.y;
	#endif	
/////////////////////////////////////////////////////////////////////
}

void surf (Input IN, inout SurfaceOutput o) {
	float4 splat_control = tex2D(_Control1, IN.uv_Control);
	
 	float total_coverage=dot(splat_control, 1);
	
	#if defined(COLOR_MAP) || defined(RTP_SNOW)
		float global_color_blend=_GlobalColorMapBlendValues.y*total_coverage;
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control);
		#ifdef RTP_CUT_HOLES
		clip(global_color_value.a-0.001f);
		#endif				
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif	
		
	#ifdef RTP_SNOW
		float snow_val=rtp_snow_strength*2;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.snowDir.w)/rtp_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow);
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= rtp_snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		// due to arithmetic op limit
		#ifndef RTP_CUT_HOLES
		snow_val*=snow_val;
		#endif
		
	 	fixed4 col;
		col = splat_control.r * lerp(tex2D(_SplatA0, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer0123.x );
		col += splat_control.g * lerp(tex2D(_SplatA1, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer0123.y );
		col += splat_control.b * lerp(tex2D(_SplatA2, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer0123.z );
		col += splat_control.a * lerp(tex2D(_SplatA3, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer0123.w );
				
		global_color_value.rgb=lerp(global_color_value.rgb, rtp_snow_color.rgb, snow_val);
	#else		
	 	fixed4 col;
		col = splat_control.r * tex2D(_SplatA0, IN._uv_Relief.xy);
		col += splat_control.g * tex2D(_SplatA1, IN._uv_Relief.xy);
		col += splat_control.b * tex2D(_SplatA2, IN._uv_Relief.xy);
		col += splat_control.a * tex2D(_SplatA3, IN._uv_Relief.xy);
	#endif
	
	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col.rgb=lerp(col.rgb, col.rgb*global_color_value.rgb*2, global_color_blend);
		#else
			col.rgb=lerp(col.rgb, global_color_value.rgb, global_color_blend);
		#endif		
	#endif	
		
	o.Albedo = col.rgb;
	//o.Gloss = col.a*total_coverage;
	//o.Specular = dot(_Spec0123, splat_control);
}
ENDCG  

///* AddPass
ZTest LEqual
//Offset -1,-1
CGPROGRAM
	#pragma surface surf Lambert vertex:vert decal:add noforwardadd nolightmap noambient
	#include "UnityCG.cginc"
	
	#pragma only_renderers d3d9 opengl gles flash d3d11
		
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#define ADDITIONAL_FEATURES_IN_FALLBACKS

	#ifdef ADDITIONAL_FEATURES_IN_FALLBACKS	
		// comment if you don't need global color map
		#define COLOR_MAP
		// if not defined global color map will be blended (lerp)
		#define COLOR_MAP_BLEND_MULTIPLY
		
		#define RTP_CUT_HOLES
		
		//#define RTP_SNOW
	#endif
/////////////////////////////////////////////////////////////////////
	
sampler2D _Control, _Control2;
sampler2D _SplatB0,_SplatB1,_SplatB2,_SplatB3;
float4 _TERRAIN_ReliefTransform;
half _Shininess;
float4 _Spec4567;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef COLOR_MAP
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
sampler2D _ColorMapGlobal;
#endif
#ifdef RTP_SNOW
float rtp_snow_strength;
float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer0123;
float4 rtp_snow_strength_per_layer4567;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;
fixed4 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 _uv_Relief;
	float4 snowDir;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_Relief.xy=mul(_Object2World, v.vertex).xz / _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		o.snowDir.xyz = normalize( mul(_Object2World, float4(v.normal.xyz,0)).xyz );
		o.snowDir.w = v.vertex.y;
	#endif	
/////////////////////////////////////////////////////////////////////
	
}

void surf (Input IN, inout SurfaceOutput o) {
	float4 splat_control = tex2D(_Control2, IN.uv_Control);
	
 	float total_coverage=dot(splat_control, 1);
	
	#if defined(COLOR_MAP) || defined(RTP_SNOW)
		float global_color_blend=_GlobalColorMapBlendValues.y*total_coverage;
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control);
		#ifdef RTP_CUT_HOLES
		clip(global_color_value.a-0.001f);
		#endif				
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif	
		
	#ifdef RTP_SNOW
		float snow_val=rtp_snow_strength*2;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.snowDir.w)/rtp_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow);
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= rtp_snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		// due to arithmetic op limit
		#ifndef RTP_CUT_HOLES
		snow_val*=snow_val;
		#endif
		
	 	fixed4 col;
		col = splat_control.r * lerp(tex2D(_SplatB0, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer4567.x );
		col += splat_control.g * lerp(tex2D(_SplatB1, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer4567.y );
		col += splat_control.b * lerp(tex2D(_SplatB2, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer4567.z );
		col += splat_control.a * lerp(tex2D(_SplatB3, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer4567.w );
				
		global_color_value.rgb=lerp(global_color_value.rgb, rtp_snow_color.rgb, snow_val);
	#else		
	 	fixed4 col;
		col = splat_control.r * tex2D(_SplatB0, IN._uv_Relief.xy);
		col += splat_control.g * tex2D(_SplatB1, IN._uv_Relief.xy);
		col += splat_control.b * tex2D(_SplatB2, IN._uv_Relief.xy);
		col += splat_control.a * tex2D(_SplatB2, IN._uv_Relief.xy);
	#endif
	
	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col.rgb=lerp(col.rgb, col.rgb*global_color_value.rgb*2, global_color_blend);
		#else
			col.rgb=lerp(col.rgb, global_color_value.rgb, global_color_blend);
		#endif		
	#endif	
		
	o.Albedo = col.rgb;
	//o.Gloss = col.a*total_coverage;
	//o.Specular = dot(_Spec4567, splat_control);
}
ENDCG  
//*/ // AddPass

}
// EOF CLASSIC shading

// Fallback to Diffuse
Fallback "Diffuse"
}