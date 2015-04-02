//
// 2 Parallax mapped materials with adjustable features
// Tomasz Stobierski 2014
//
// NOTE - by default most features are disabled
// You can enable them below uncommenting property block associated with a feature and the feature itself by uncommenting its #define keyword
// So, to make any shader variation:
// 1. copy shader code into new shader file
// 2. adjust its filename and shader name below (using 20 shaders with name like "Relief Pack/Customizable Shader 2 Layers (PBL)" would be confusing)
// 3. comment/uncomment property blocks for features you'd like to use
// 4. comment/uncomment #define keywords to enable/disable features
//
// note that there are some important switches in #define section (HDR cubemaps switch, linear lighting switch and so on)
//
Shader "Relief Pack/Customizable Shader 2 Layers (PBL)" {
    Properties {
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)		
		// takes effect only with RTP_PM_SHADING defined below
		_ExtrudeHeight ("Extrude Height", Range(0.001,0.1)) = 0.04
				
				
				
		//
		// complementary lighting
		//
		// look for RTP_COMPLEMENTARY_LIGHTS define below
		//  (using complementary lighting you probably don't need IBL diffuse)
		// (comment below properties when the feature is enabled and you want to get it synced globally by RTP scripts)
		RTP_ReflexLightDiffuseSoftness ("Complementary diffuse softness", Range(0,1)) = 0.5
		RTP_ReflexLightDiffuseColor1 ("Complementary light diffuse 1 (RGB+A strength)", Color) = (1, 1, 1, 0.05)
		RTP_ReflexLightDiffuseColor2 ("Complementary light diffuse 2 (RGB+A strength)", Color) = (1, 1, 1, 0.05)
		RTP_BackLightStrength ("Negative light power", Range(0,1)) = 0
		
		rtp_customAmbientCorrection ("Ambient correction", Color) = (0, 0, 0, 1)				



    	//
    	// main textures
    	//    
		_MainTex ("Texture A", 2D) = "white" {}
		_BumpMap ("Bumpmap A", 2D) = "bump" {}
		_HeightMap ("Heightmap A", 2D) = "black" {}
		
		_MainTex2 ("Texture B", 2D) = "white" {}
		_BumpMap2 ("Bumpmap B", 2D) = "bump" {}
		_HeightMap2 ("Heightmap B", 2D) = "black" {}
		
		
		
		// near distance values (used with global colormap, perlin normals or uv blend feature)
		// UNCOMMENT when using one of these features or you won't get control over many features)
/*
		_TERRAIN_distance_start ("Near distance start", Float) = 0
		_TERRAIN_distance_transition ("       fade length", Float) = 20
*/
		// far distance values
		_TERRAIN_distance_start_bumpglobal ("Far distance start", Float) = 0
		_TERRAIN_distance_transition_bumpglobal ("       distance transition", Float) = 50



		//
		// global colormap
		// to enable/disable feature in shader - look for COLOR_MAP define below
		//
/*		
		_ColorMapGlobal ("Global colormap", 2D) = "grey" {}
		// uncomment when shader used w/o RTP
		_GlobalColorMapBlendValues ("       blending (near,mid,far,-)", Vector) = (0.3,0.6,0.8,0)
		_GlobalColorMapSaturation ("       saturation near", Range(0,1)) = 1	
		_GlobalColorMapSaturationFar ("       saturation far", Range(0,1)) = 1
		_GlobalColorMapBrightness ("       brightness near", Range(0,2)) = 1
		_GlobalColorMapBrightnessFar ("       brightness far", Range(0,2)) = 1		
		_GlobalColorMapNearMIP ("       MIP level below far", Range(0, 8)) = 0
		_GlobalColorMapDistortByPerlin ("       Distort by perlin", Range(0, 0.02)) = 0.01
		////////
*/
	
	
	
		//
		// perlin normalmap (RG combined with wetmask in B channel and reflection map in A channel)
		// to enable/disable feature in shader - look for GLOBAL_PERLIN define below
		//
/*
		_BumpMapGlobal ("Combined texture (RG perlin, B wet, A refl)", 2D) = "black" {}
		rtp_perlin_start_val ("       Perlin start val", Range(0,1)) = 0.3
		_BumpMapGlobalScale ("       Perlin tiling scale", Range( 0.01,0.25) ) = 0.1
		_BumpMapGlobalStrength0 ("       Layer 1 perlin normal", Range(0,2)) = 0.3
		_BumpMapGlobalStrength1 ("       Layer 2 perlin normal", Range(0,2)) = 0.3
*/
	
	
	
    	//
		// per layer adjustement PBL
		//
		// Layer 1
		_LayerColor0 ("Layer 1 color", Color) = (0.5,0.5,0.5,1)
		_LayerSaturation0 ("       saturation", Range(0,2)) = 1
		_Spec0 ("       spec multiplier", Range(0,4)) = 1
		RTP_gloss_mult0 ("       gloss multiplier", Range(0,4)) = 1
		RTP_gloss2mask0 ("       spec mask from gloss", Range(0,1)) = 0
		_LayerBrightness2Spec0 ("       spec mask from albedo", Range(0,1)) = 0
		RTP_gloss_shaping0 ("       gloss shaping", Range(0,1)) = 0.5
		_FarSpecCorrection0 ("       far spec correction", Range(-1,1)) = 0
		RTP_Fresnel0 ("       fresnel", Range(0,1)) = 0
		RTP_FresnelAtten0 ("           fresnel attenuate by gloss", Range(0,1)) = 0
		RTP_DiffFresnel0 ("       diffuse scattering", Range(0,1)) = 0
		RTP_IBL_bump_smoothness0 ("       IBL / Refl bump smooth", Range(0,1)) = 0.7
		RTP_IBL_SpecStrength0 ("       IBL spec / Refl exposure", Range(0,8)) = 1
		RTP_IBL_DiffuseStrength0 ("       IBL diffuse exposure", Range(0,2)) = 1
		_LayerAlbedo2SpecColor0 ("          color from albedo (metal tint)", Range(0,1)) = 0
		// UV blend layer1
		// to enable/disable feature in shader - look for RTP_UV_BLEND define below
/*
		_MixBlend0 ("Layer1 UV blend", Range(0,1)) = 0.5
		_MixScale0 ("     tiling", Range(0.02,0.25)) = 0.125
		_MixSaturation0 ("     saturation", Range(0,2)) = 1
		_MixBrightness0 ("     brightness", Range(0.5,3.5)) = 2
		_MixReplace0 ("     replace at far", Range(0,1)) = 0.25
*/
	
		// Layer 2
		_LayerColor1 ("Layer 2 color", Color) = (0.5,0.5,0.5,1)
		_LayerSaturation1 ("       saturation", Range(0,2)) = 1
		_Spec1 ("       spec multiplier", Range(0,4)) = 1
		RTP_gloss_mult1 ("       gloss multiplier", Range(0,4)) = 1
		RTP_gloss2mask1 ("       spec mask from gloss", Range(0,1)) = 0
		_LayerBrightness2Spec1 ("       spec mask from albedo", Range(0,1)) = 0
		RTP_gloss_shaping1 ("       gloss shaping", Range(0,1)) = 0.5
		_FarSpecCorrection1 ("       far spec correction", Range(-1,1)) = 0
		RTP_Fresnel1 ("       fresnel", Range(0,1)) = 0
		RTP_FresnelAtten1 ("           fresnel attenuate by gloss", Range(0,1)) = 0
		RTP_DiffFresnel1 ("       diffuse scattering", Range(0,1)) = 0
		RTP_IBL_bump_smoothness1 ("       IBL / Refl bump smooth", Range(0,1)) = 0.7
		RTP_IBL_SpecStrength1 ("       IBL spec / Refl exposure", Range(0,8)) = 1
		RTP_IBL_DiffuseStrength1 ("       IBL diffuse exposure", Range(0,2)) = 1
		_LayerAlbedo2SpecColor1 ("          color from albedo (metal tint)", Range(0,1)) = 0
		// UV blend layer2
		// to enable/disable feature in shader - look for RTP_UV_BLEND define below		_MixBlend1 ("Layer2 UV blend", Range(0,1)) = 0.5
/*
		_MixScale1 ("     tiling", Range(0.02,0.25)) = 0.125
		_MixSaturation1 ("     saturation", Range(0,2)) = 1
		_MixBrightness1 ("     brightness", Range(0.5,3.5)) = 2
		_MixReplace1 ("     replace at far", Range(0,1)) = 0.25
*/
    	//
		// EOF per layer adjustement PBL
		//		
	
	
	
		//
		// emissive properties
		// to enable/disable feature in shader - look for RTP_EMISSION define below
		//
/*
		// Layer 1
		_LayerEmission0 ("Layer 1 emission", Range(0,1)) = 0
		_LayerEmissionColor0 ("       glow color", Color) = (0.5,0.5,0.5,0)
		// Layer 2
		_LayerEmission1 ("Layer 2 emission", Range(0,1)) = 0
		_LayerEmissionColor1 ("       glow color", Color) = (0.5,0.5,0.5,0)
*/

		//
		// water/wet
		// to enable/disable feature in shader - look for RTP_EMISSION define below
		//
/*
		TERRAIN_GlobalWetness ("Wetness", Range(0,1)) = 1
		TERRAIN_WaterLevel ("       Water level", Range(0,2)) = 0.5
		TERRAIN_WaterLevelSlopeDamp ("       Water level slope damp", Range(0.1, 2)) = 0.1
		TERRAIN_WaterEdge ("       Water level sharpness", Range(1,4)) = 1
		// when GLOBAL_PERLIN is defined below texture is ommited (flow bumps are taken from combined perlin texture)
      	TERRAIN_FlowingMap ("      Flowingmap (water bumps)", 2D) = "gray" {}
		// to enable/disable flowmap feature in shader - look for FLOWMAP define below
		//_FlowMap ("      FlowMap (RG+BA)", 2D) = "grey" {}
	  	//TERRAIN_FlowSpeedMap ("       Flow Speed (map)", Range(0, 1)) = 0.1		
		TERRAIN_FlowSpeed ("       Flow speed", Range(0,1)) = 0.5
		TERRAIN_FlowCycleScale ("       Flow cycle scale", Range(0.5,4)) = 1
		TERRAIN_FlowScale ("       Flow tex tiling", Float) = 1
		TERRAIN_FlowMipOffset ("       Flow tex filter", Range(0,4)) = 1
		TERRAIN_mipoffset_flowSpeed ("       Filter by flow speed", Range(0,4)) = 1
		TERRAIN_WetDarkening ("       Water surface darkening", Range(0.1,0.9)) = 0.5
      	
		TERRAIN_WaterColor ("       Water color (A - fresnel)", Color) = (0.5, 0.7, 1, 0.5)
		TERRAIN_WaterOpacity ("       Water opacity", Range(0,1)) = 0.2
		TERRAIN_WaterEmission ("       Water emission", Range(0,1)) = 0
		
		TERRAIN_WaterSpecularity ("       Water spec boost", Range(-1,1)) = 0.2
		TERRAIN_WaterGloss ("       Water gloss boost", Range(-1,1)) = 0.3
		
		TERRAIN_Flow ("       Flow strength", Range(0, 1)) = 0.1
		TERRAIN_Refraction ("       Water refraction", Range(0,0.04)) = 0.02
		TERRAIN_WaterIBL_SpecWaterStrength ("       IBL spec / Refl - water", Range(0,8)) = 1
		
		TERRAIN_WetSpecularity ("       Wet spec boost", Range(-1,1)) = 0.1
		TERRAIN_WetGloss ("       Wet gloss boost", Range(-1,1)) = 0.1
		
		TERRAIN_WetFlow ("       Wet flow", Range(0, 1)) = 0
		TERRAIN_WetRefraction ("       Wet refraction factor", Range(0,1)) = 0
		TERRAIN_WaterIBL_SpecWetStrength ("       IBL spec / Refl - wet", Range(0,8)) = 1
		
		TERRAIN_WaterGlossDamper ("       Hi-freq / distance gloss damper", Range(0,1)) = 0
*/
		//
		// EOF water/wet
		// 
	
							
		//
      	// rain feature
      	// to enable/disable feature in shader - look for RTP_WET_RIPPLE_TEXTURE define below (RTP_WETNESS must be defined, too)
      	//
/*
		TERRAIN_RippleMap ("Ripplemap", 2D) = "gray" {}
		TERRAIN_RainIntensity ("Rain intensity", Range(0,1)) = 1
		TERRAIN_WetDropletsStrength ("       Rain on wet", Range(0,1)) = 0.1
		TERRAIN_DropletsSpeed ("       Anim Speed", Float) = 15
		TERRAIN_RippleScale ("       Ripple tex tiling", Range(0.25,8)) = 1
*/
	
	
		//
		// static reflections (taken from A channel of _BumpMapGlobal texture)
		// to enable/disable feature in shader - look for RTP_REFLECTION define below 
		// this feature can be treated as simple (no need for cubemap texture sampler) alternative for IBL specular cubemaps
		//
/*
		TERRAIN_ReflColorA ("Reflection color A (Emissive RGB)", Color) = (0.5,0.5,0.5,0)
		TERRAIN_ReflColorB ("       color B (Diffuse RGBA)", Color) = (0.0,0.5,0.9,0)
		TERRAIN_ReflColorC ("       color C (Diffuse RGBA)", Color) = (0.3,0.6,0.9,0)
		// when GLOBAL_PERLIN is defined below texture is defined in another part of property block (reflection map is taken from A channel of combined perlin texture)
		_BumpMapGlobal ("       Refl texture (comb. texure A channel)", 2D) = "black" {}
		TERRAIN_ReflColorCenter ("       gradient center", Range(0.1, 0.9)) = 0.5
		TERRAIN_ReflGlossAttenuation ("       roughness attenuation", Range(0, 1)) = 0.5
		TERRAIN_ReflectionRotSpeed ("       Reflection rotation speed", Range(0, 2)) = 0.3
*/



		//
		// IBL
		// to enable/disable feature in shader - look for RTP_IBL_DIFFUSE and RTP_IBL_SPEC defines below  
		// (note that using IBL diffuse you probably don't need complementary lighting)
		//
/*
		// below cubemap is not used when RTP_SKYSHOP_SYNC define below is used
		_DiffCubeIBL ("Custom IBL diffuse cubemap", CUBE) = "black" {}
		TERRAIN_IBL_DiffAO_Damp("Diffuse IBL AO damp", Range(0,1)) = 0
		// in case of RTP_SKYSHOP_SYNC you don't need to fill below cubemap, but you can overwrite it with custom value
		_SpecCubeIBL ("Custom IBL spec cubemap", CUBE) = "black" {}
		TERRAIN_IBLRefl_SpecAO_Damp("Spec IBL / Refl AO damp", Range(0,1)) = 0
*/
	
	
	
		//
		// vertical texturing
		// to enable/disable feature in shader - look for VERTICAL_TEXTURE define below  		
		//
/*
		_VerticalTexture ("Vertical texture (RGB)", 2D) = "grey" {}
		_VerticalTextureTiling ("       Texture tiling", Float) = 50
		_VerticalTextureGlobalBumpInfluence ("       Perlin distortion", Range(0,0.3)) = 0.01
		_VerticalTextureStrength0 ("       Layer 1 strength", Range(0,1)) = 0.5
		_VerticalTextureStrength1 ("       Layer 2 strength", Range(0,1)) = 0.5
*/
	
	
	
		//
		// snow (set globally by RTP scripts)
		// to enable/disable feature in shader - look for RTP_SNOW define below  		
		//
/*
		rtp_snow_strength ("Snow strength", Range(0,2)) = 1
		
		// uncomment when shader used w/o RTP
		rtp_global_color_brightness_to_snow ("       Global color influence", Range(0, 0.3)) = 0
		////////
		
		rtp_snow_slope_factor ("       Slope damp factor", Range(0,15)) = 2
		// in [m] (where snow start to appear)
		rtp_snow_height_treshold ("       Coverage height theshold", Float) = -100
		rtp_snow_height_transition ("       Coverage height length", Float) = 300
		rtp_snow_color("       Color", Color) = (0.9,0.9,1,1)
		rtp_snow_specular("       Spec (gloss mask)", Range(0, 1)) = 0.4
		rtp_snow_gloss("       Gloss", Range(0.01,1)) = 0.2
		
		rtp_snow_fresnel("       Fresnel", Range(0.01,1)) = 0.2
		rtp_snow_diff_fresnel("       Diffuse scattering", Range(0,2)) = 0.5
		rtp_snow_IBL_DiffuseStrength("       IBL diffuse exposure", Range(0,2)) = 0.25
		rtp_snow_IBL_SpecStrength("       IBL spec / Refl exposure", Range(0,8)) = 0.25
		
		rtp_snow_edge_definition ("       Edges definition", Range(0.25,20)) = 2
		//rtp_snow_deep_factor("       Deep factor", Range(0,6)) = 2 // not used in shader
*/



		//
		// caustics
		// to enable/disable feature in shader - look for RTP_CAUSTICS define below  		
		//
/*
		TERRAIN_CausticsAnimSpeed(" Caustics anim speed", Range(0, 10)) = 2
		TERRAIN_CausticsColor("       Color (RGB)", Color) = (1,1,1,0)
		TERRAIN_CausticsWaterLevel("       Water Level", Float) = 0
		TERRAIN_CausticsWaterLevelByAngle("       Water level by slope", Range(0,8)) = 4
		TERRAIN_CausticsWaterShallowFadeLength("       Shallow fade length", Range(0.1, 10)) = 1
		TERRAIN_CausticsWaterDeepFadeLength("       Deep fade length", Range(1, 100)) = 20
		TERRAIN_CausticsTilingScale("       Texture tiling", Float) = 1 //Range(0.5, 4)) = 2
		TERRAIN_CausticsTex("       Caustics texture", 2D) = "black" {}
*/
  
  
  
		//
		// global ambient emissive map
		// to enable/disable feature in shader - look for RTP_AMBIENT_EMISSIVE_MAP define below  		
		//
/*
		_AmbientEmissiveMapGlobal("Global ambient emissive map", 2D) = "black" {}
		_AmbientEmissiveMultiplier("       emission brightness", Range(0,4)) = 0.5
		_AmbientEmissiveRelief("       normal/height mod", Range(0,1)) = 0.5
		_shadow_distance_start ("       shadows distance", Float) = 20
		_shadow_distance_transition ("       shadows fade length", Range(0,30)) = 30
		_shadow_value ("       shadows blending", Range(0,1)) = 0
*/

   
         
    	// params for underlying terrain (used for global maps and in geom blend mode)
 		//_TERRAIN_PosSize ("Terrain pos (xz to XY) & size(xz to ZW)", Vector) = (0,0,1000,1000)
		//_TERRAIN_Tiling ("Terrain tiling (XY) & offset(ZW)", Vector) = (3,3,0,0)       
    }
    SubShader {
	Tags { "RenderType" = "Opaque" }
	CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert

	#pragma exclude_renderers flash
	#pragma glsl
	#pragma target 3.0

	// define either PM or simple shading mode. You can also compile 2 versions to be switched runtime (globally by RTP LOD manager script)
	#define RTP_PM_SHADING
	//#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING

	// complementary lights (you'll probably want to turn it off every time you use IBL diffuse)	
	#define RTP_COMPLEMENTARY_LIGHTS
	// AO can be taken from arbitrary vertex color channel
	#define VERTEX_COLOR_AO_DAMP IN.color.g
	// switch to choose we're render in linear or gamma
	//#define RTP_COLORSPACE_LINEAR
	
	// comment if you don't need global color map
	//#define COLOR_MAP
	// if not defined global color map will be blended (lerp)
	//#define COLOR_MAP_BLEND_MULTIPLY
	
	// makes usage of _BumpMapGlobal texture (RG - perlin bumpmap, B watermask, A - reflection map)
	// practical when used on larger areas of geom blend, this switch makes also perlin global texture to be used as water bumpmaps
	//#define GLOBAL_PERLIN
	
	// uv blending
	//#define RTP_UV_BLEND

	// water features	
	//#define RTP_WETNESS
	// enable below if you don't want to use water flow
	//#define SIMPLE_WATER
	// rain droplets
	//#define RTP_WET_RIPPLE_TEXTURE	  
	// if defined we don't use terrain wetmask (_BumpMapGlobal B channel), but B channel of vertex color
	// (to get it from combined texture B channel you need to define GLOBAL_PERLIN)
	#define VERTEX_COLOR_TO_WATER_COVERAGE IN.color.b	
	// you can ask shader to use flowmap to control direction of flow (can be dependent on this flowmap along uv coords defined there)
	//#define FLOWMAP

	
	// IBL lighting - diffuse cubemap used (you'll probably want to turn it off every time you use complementary lights)	
	//#define RTP_IBL_DIFFUSE
	// IBL lighting - specular / reflections cubemap used
	//#define RTP_IBL_SPEC
	// if not defined we will decode LDR cubemaps (RGB only)
	#define IBL_HDR_RGBM
	// if you'd like to integrate Skyshop IBL features (global Sky object)
	//#define RTP_SKYSHOP_SYNC	
	// use if you'd like global skyshop's cubemaps to be rotates accordingly to skyshop settings
	//#define RTP_SKYSHOP_SKY_ROTATION	
	
	// static reflection (A channel of  _BumpMapGlobal texture)
	//#define RTP_REFLECTION
	// if RTP_REFLECTION is enabled - below define control rotation "skymap" around
	#define RTP_ROTATE_REFLECTION
	
	//  layer emissiveness
	//#define RTP_EMISSION
	// when wetness is defined and fuild on surface is emissive we can mod its emisiveness by output normal (wrinkles of flowing "water")
	// below define change the way we treat output normals (works fine for "lava" like emissive fuilds)
	//#define RTP_FUILD_EMISSION_WRAP	
	
	// vertical texturing
	//#define VERTICAL_TEXTURE
	
	// dynamic snow
	//#define RTP_SNOW
	// you can optionally define source vertex color to control snow coverage (useful when you need to mask snow under any kind of shelters)
	//#define VERTEX_COLOR_TO_SNOW_COVERAGE IN.color.a

	// caustics	
	//#define RTP_CAUSTICS

	// PBL visibility function - not necessary if you'd like to configure fast shader (makes shader more physically accurate - boosts a bit spec behaviour in sereval conditions)
	#define RTP_PBL_VISIBILITY_FUNCTION
	// fresnel used for direct lighting (in forward only !)
	#define RTP_PBL_FRESNEL
	// when you don't use solution like Lux which handles PBL in deferred, leave below define uncommented
	#define RTP_DEFERRED_PBL_NORMALISATION
	
	// global ambient emissive map
	//#define RTP_AMBIENT_EMISSIVE_MAP
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

	#define TWO_LAYERS
	//#define NOSPEC_BLEED
	
#ifdef RTP_SKYSHOP_SYNC
	// SH IBL lighting taken under permission from Skyshop MarmosetCore.cginc
	uniform float3		_SH0;
	uniform float3		_SH1;
	uniform float3		_SH2;
	uniform float3		_SH3;
	uniform float3		_SH4;
	uniform float3		_SH5;
	uniform float3		_SH6;
	uniform float3		_SH7;
	uniform float3		_SH8;	
	float3 SHLookup(float3 dir) {
		//l = 0 band (constant)
		float3 result = _SH0.xyz;

		//l = 1 band
		result += _SH1.xyz * dir.y;
		result += _SH2.xyz * dir.z;
		result += _SH3.xyz * dir.x;

		//l = 2 band
		float3 swz = dir.yyz * dir.xzx;
		result += _SH4.xyz * swz.x;
		result += _SH5.xyz * swz.y;
		result += _SH7.xyz * swz.z;
		float3 sqr = dir * dir;
		result += _SH6.xyz * ( 3.0*sqr.z - 1.0 );
		result += _SH8.xyz * ( sqr.x - sqr.y );
		
		return abs(result);
	}	
	uniform float4 _ExposureIBL;	
#endif
	
	struct Input {
	float4 texCoords;
	
	float3 viewDir;
	float4 _auxDir;
	float4 TtoW0_TtoW2;
	
	float4 color:COLOR;
	};
	
	float RTP_BackLightStrength;
	fixed3 rtp_customAmbientCorrection;
	float _ExtrudeHeight;
	
	//
	// complementary lighting (uncomment property block when used)
	//
	float RTP_ReflexLightDiffuseSoftness;
	fixed4 RTP_ReflexLightDiffuseColor1;
	fixed4 RTP_ReflexLightDiffuseColor2;

	//
	// main section
	//    
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _HeightMap;
	sampler2D _MainTex2;
	sampler2D _BumpMap2;
	sampler2D _HeightMap2;
	
	//
	// per layer adjustement PBL
	//
	// Layer 1
	fixed3 _LayerColor0;
	float _LayerSaturation0;
	float _Spec0; 
	float RTP_gloss_mult0;
	float RTP_gloss2mask0;
	float _LayerBrightness2Spec0;
	float RTP_gloss_shaping0;

	float _FarSpecCorrection0;
	float RTP_Fresnel0;
	float RTP_FresnelAtten0;
	float RTP_DiffFresnel0;
	float RTP_IBL_bump_smoothness0;
	float RTP_IBL_SpecStrength0;
	float RTP_IBL_DiffuseStrength0;
	float _LayerAlbedo2SpecColor0;
			
	// Layer 2
	fixed3 _LayerColor1;
	float _LayerSaturation1;
	float _Spec1; 
	float RTP_gloss_mult1;
	float RTP_gloss2mask1;
	float _LayerBrightness2Spec1;
	float RTP_gloss_shaping1;
			
	float _FarSpecCorrection1;
	float RTP_Fresnel1;
	float RTP_FresnelAtten1;
	float RTP_DiffFresnel1;
	float RTP_IBL_bump_smoothness1;
	float RTP_IBL_SpecStrength1;
	float RTP_IBL_DiffuseStrength1;
	float _LayerAlbedo2SpecColor1;
	//
	// EOF per layer adjustement PBL
	//		
	
	//
	// emissive properties
	//
	// Layer 1
	float _LayerEmission0;
	fixed3 _LayerEmissionColor0;
	// Layer 2
	float _LayerEmission1;
	fixed3 _LayerEmissionColor1;

	//
	// water/wet
	//
	float TERRAIN_GlobalWetness;
	// general flow direction
	sampler2D _FlowMap;
  	sampler2D TERRAIN_FlowingMap;
	float TERRAIN_FlowSpeed;
  	float TERRAIN_FlowSpeedMap;
	float TERRAIN_FlowCycleScale;
	float TERRAIN_FlowScale;
	float TERRAIN_FlowMipOffset;
	float TERRAIN_mipoffset_flowSpeed;
	float TERRAIN_WetDarkening;
  	
	fixed4 TERRAIN_WaterColor;
	float TERRAIN_WaterOpacity;
	float TERRAIN_WaterEmission;
	
	float TERRAIN_WaterLevel;
	float TERRAIN_WaterLevelSlopeDamp;
	float TERRAIN_WaterEdge;
	
	float TERRAIN_WaterSpecularity;
	float TERRAIN_WaterGloss;
	float TERRAIN_WaterGlossDamper;
	
	float TERRAIN_Flow;
	float TERRAIN_Refraction;
	float TERRAIN_WaterIBL_SpecWaterStrength;
	
	float TERRAIN_WetSpecularity;
	float TERRAIN_WetGloss;
	
	float TERRAIN_WetFlow;
	float TERRAIN_WetRefraction;
	float TERRAIN_WaterIBL_SpecWetStrength;
	//
	// EOF water/wet
	// 	
	
	//
  	// rain feature
  	//
	sampler2D TERRAIN_RippleMap;
	float TERRAIN_RainIntensity;
	float TERRAIN_WetDropletsStrength;
	float TERRAIN_DropletsSpeed;
	float TERRAIN_RippleScale;
	
	//
	// colormap global
	//
	sampler2D _ColorMapGlobal;
	// can be set globaly by ReliefTerrain script
	float4 _GlobalColorMapBlendValues;
	float _GlobalColorMapNearMIP; 
	float _GlobalColorMapDistortByPerlin; 
	float _GlobalColorMapSaturation;
	float _GlobalColorMapSaturationFar	; 
	float _GlobalColorMapBrightness;
	float _GlobalColorMapBrightnessFar;
	
	//
	// perlin ( + watermask & reflection)
	//
	sampler2D _BumpMapGlobal;
	float4 _BumpMapGlobal_TexelSize;
	float _BumpMapGlobalScale;
	float _BumpMapGlobalStrength0;
	float _BumpMapGlobalStrength1;
	float rtp_perlin_start_val;		
	
	//
	// UV blend
	//
	float _MixBlend0;
	float _MixBlend1;
	float _MixScale0;
	float _MixScale1;
	float _MixSaturation0;
	float _MixSaturation1;
	float _MixBrightness0;
	float _MixBrightness1;
	float _MixReplace0;
	float _MixReplace1;
	
	//
	// reflection
	//
	fixed4 TERRAIN_ReflColorA;
	fixed4 TERRAIN_ReflColorB;
	fixed4 TERRAIN_ReflColorC;
	float TERRAIN_ReflColorCenter;
	float TERRAIN_ReflGlossAttenuation;
	float TERRAIN_ReflectionRotSpeed;

	//
	// IBL
	//
	samplerCUBE _DiffCubeIBL;
	samplerCUBE _SpecCubeIBL;
	float TERRAIN_IBL_DiffAO_Damp;
	float TERRAIN_IBLRefl_SpecAO_Damp;
	float4x4	_SkyMatrix;// set globaly by skyshop		
	
	sampler2D _VerticalTexture;
	float _VerticalTextureTiling;
	float _VerticalTextureGlobalBumpInfluence;
	float _VerticalTextureStrength0;
	float _VerticalTextureStrength1;
	
	//
	// snow
	//
	float rtp_global_color_brightness_to_snow;
	float rtp_snow_strength;
	float rtp_snow_slope_factor;
	// in [m] (where snow start to appear)
	float rtp_snow_height_treshold;
	float rtp_snow_height_transition;
	fixed4 rtp_snow_color;
	float rtp_snow_specular;
	float rtp_snow_gloss;
	
	float rtp_snow_fresnel;
	float rtp_snow_diff_fresnel;
	float rtp_snow_IBL_DiffuseStrength;
	float rtp_snow_IBL_SpecStrength;
	
	float rtp_snow_edge_definition;
	float rtp_snow_deep_factor;

	//
	// caustics
	//
	float TERRAIN_CausticsAnimSpeed;
	fixed4 TERRAIN_CausticsColor;
	float TERRAIN_CausticsWaterLevel;
	float TERRAIN_CausticsWaterLevelByAngle;
	float TERRAIN_CausticsWaterShallowFadeLength;
	float TERRAIN_CausticsWaterDeepFadeLength;
	float TERRAIN_CausticsTilingScale;
	sampler2D TERRAIN_CausticsTex;
	
	//
	// global ambient emissive map
	//
	sampler2D _AmbientEmissiveMapGlobal;
	float _AmbientEmissiveMultiplier;
	float _AmbientEmissiveRelief;
	float _shadow_distance_start;
	float _shadow_distance_transition;
	float _shadow_value;
		
	// RTP terrain specific
	float _TERRAIN_distance_start;
	float _TERRAIN_distance_transition;
	float _TERRAIN_distance_start_bumpglobal;
	float _TERRAIN_distance_transition_bumpglobal;			

	// used for global maps and height blend	
	float4 _TERRAIN_PosSize;
	float4 _TERRAIN_Tiling;
	sampler2D _TERRAIN_HeightMap;
	sampler2D _TERRAIN_Control;		
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// aux functions & lighting

// quick gamma to linear approx of pow(n,2.2) function
inline float FastToLinear(float t) {
		t *= t * (t * 0.305306011 + 0.682171111) + 0.012522878;
		return t;
}

half3 DecodeRGBM(float4 rgbm)
{
	#ifdef IBL_HDR_RGBM
		// gamma/linear RGBM decoding
		#if defined(RTP_COLORSPACE_LINEAR)
	    	return rgbm.rgb * FastToLinear(rgbm.a) * 8;
	    #else
	    	return rgbm.rgb * rgbm.a * 8;
	    #endif
	#else
    	return rgbm.rgb;
	#endif
}

half2 normalEncode (half3 n)
{
    half scale = 1.7777;
    half2 enc = n.xy / (n.z+1);
    enc /= scale;
    enc = enc*0.5+0.5;
    return enc;
}

half3 normalDecode(half2 enc)
{
    half scale = 1.7777;
    half3 nn =
        enc.xyy*half3(2*scale,2*scale,0) +
        half3(-scale,-scale,1);
    half g = 2.0 / dot(nn.xyz,nn.xyz);
    half3 n;
    n.xy = g*nn.xy;
    n.z = g-1;
    return n;
}

struct RTPSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Alpha;
	float2 RTP;
	half3 SpecColor;
};

fixed3 RTP_ambLight;
inline fixed4 LightingCustomBlinnPhong_PrePass (in RTPSurfaceOutput s, half4 light)
{
	#if defined(RTP_DEFERRED_PBL_NORMALISATION)
		fixed spec = light.a;
	#else
		// we assume Lux is used with its compressed specular luminance term
		fixed spec = exp2(light.a) - 1;
	#endif
	
	fixed4 c;
	s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
	c.rgb = (s.Albedo * light.rgb + light.rgb * s.SpecColor * spec) *s.RTP.y + rtp_customAmbientCorrection*0.5;
	#if defined(LIGHTMAP_OFF) && !defined(RTP_AMBIENT_EMISSIVE_MAP)
		c.rgb += (s.Albedo*RTP_ambLight)*(1-s.RTP.y);
	#endif
	c.a = s.Alpha + spec * _SpecColor.a;
	return c;
}

inline fixed4 LightingCustomBlinnPhong (in RTPSurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	fixed diff = dot (s.Normal, lightDir); // n_dot_l
	#if defined (RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
		float diffBack = diff<0 ? diff*RTP_BackLightStrength : 0;
	#endif
	diff = saturate(diff);
	
	float n_dot_l=diff;
	float n_dot_h = saturate(dot (s.Normal, h));
	float h_dot_l = dot (h, lightDir);
	// hacking spec normalisation to get quiet a dark spec for max roughness (will be 0.25/16)
	float specular_power=exp2(10*s.Specular+1) - 1.75;
	float normalisation_term = specular_power / 16.0f; // /8.0f in equations, but we multiply (atten * 2) in lighting below
	float blinn_phong = pow( n_dot_h, specular_power );    // n_dot_h is the saturated dot product of the normal and half vectors
	float specular_term = normalisation_term * blinn_phong;
	
	#ifdef RTP_PBL_FRESNEL
		// fresnel
		//float exponential = pow( 1.0f - h_dot_l, 5.0f ); // Schlick's approx to fresnel
		// below pow 4 looks OK and is cheaper than pow() call
		float exponential = 1.0f - h_dot_l;
		exponential*=exponential;
		exponential*=exponential;
		// skyshop fit (I'd like people to get similar results in gamma / linear)
		#if defined(RTP_COLORSPACE_LINEAR)
			exponential=0.03+0.97*exponential;
		#else
			exponential=0.25+0.75*exponential;
		#endif
		float pbl_fresnel_term = lerp (1.0f, exponential,  s.RTP.x); // o.RTP.x - _Fresnel
	#endif
		
	#ifdef RTP_PBL_VISIBILITY_FUNCTION
		// visibility
		float n_dot_v = saturate(dot (s.Normal, viewDir));
		float alpha = 1.0f / ( sqrt( 3.1415/4 * specular_power + 3.1415/2 ) );
		float pbl_visibility_term = ( n_dot_l * ( 1.0f - alpha ) + alpha ) * ( n_dot_v * ( 1.0f - alpha ) + alpha ); // Both dot products should be saturated
		pbl_visibility_term = 1.0f / pbl_visibility_term;	
	#endif
	
	float spec = specular_term * pbl_fresnel_term * pbl_visibility_term * diff;
	
	fixed4 c;
	#if defined (RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
		c.rgb = s.Albedo * diffBack;
	#else
		c.rgb = 0;
	#endif
	s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
	#ifdef RTP_COLORSPACE_LINEAR
		// s.RTP.y - self - shadow atten from surf()
		c.rgb += (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.SpecColor.rgb) * (min(atten, s.RTP.y) * 2)  + rtp_customAmbientCorrection*0.5;
	#else
		// shape ^2 spec golor (to fit IBL and not overbright spot for high glossy)
		// s.RTP.y - self - shadow atten from surf()
		c.rgb += (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.SpecColor.rgb * s.SpecColor.rgb) * (min(atten, s.RTP.y) * 2)  + rtp_customAmbientCorrection*0.5;
	#endif
	c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;

	#if defined (RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
		//		
		// reflex lights
		//
		float3 normForDiffuse=lerp(s.Normal, float3(0,0,1), RTP_ReflexLightDiffuseSoftness);
		float3 normForSpec=s.Normal;//lerp(s.Normal, float3(0,0,1), RTP_ReflexLightSpecSoftness);
		//normForSpec=normalize(normForSpec);
		float sGloss=saturate(dot(s.SpecColor,0.3));
		float glossDiff1=(sGloss+1)*RTP_ReflexLightDiffuseColor1.a;
		float glossDiff2=(sGloss+1)*RTP_ReflexLightDiffuseColor2.a;
			
		fixed3 complColor=lerp(RTP_ReflexLightDiffuseColor1.rgb*glossDiff1, RTP_ReflexLightDiffuseColor2.rgb*glossDiff2, dot(normForDiffuse.xy, lightDir.xy)*0.5+0.5);
		c.rgb += s.Albedo * complColor * (abs(normForDiffuse.z)*0.6+0.4)*(-lightDir.z*0.3+0.7);
//		lightDir.y=-0.7; // 45 degrees
//		lightDir=normalize(lightDir);
//		
//		float3 lightDirRefl;
//		float3 refl_rot;
//		refl_rot.x=0.86602540378443864676372317075294;// = sin(+120deg);
//		refl_rot.y=-0.5; // = cos(+/-120deg);
//		refl_rot.z=-refl_rot.x;
//		
//		// 1st reflex
//		lightDirRefl.x=dot(lightDir.xz, refl_rot.yz);
//		lightDirRefl.y=lightDir.y;
//		lightDirRefl.z=dot(lightDir.xz, refl_rot.xy);	
//		diff = abs( dot (normForDiffuse, lightDirRefl) )*glossDiff1; 
//		float3 reflexRGB=RTP_ReflexLightDiffuseColor1.rgb * diff * diff;
//		c.rgb += s.Albedo * reflexRGB;
//		
//		// 2nd reflex
//		lightDirRefl.x=dot(lightDir.xz, refl_rot.yx);
//		lightDirRefl.z=dot(lightDir.xz, refl_rot.zy);	
//		diff = abs ( dot (normForDiffuse, lightDirRefl) )*glossDiff2;
//		reflexRGB=RTP_ReflexLightDiffuseColor2.rgb * diff * diff;
//		c.rgb += s.Albedo * reflexRGB;
	#endif
	
	return c;
}

inline half4 LightingCustomBlinnPhong_DirLightmap (RTPSurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
{
	UNITY_DIRBASIS
	half3 scalePerBasisVector;
	
	color.rgb*=rtp_customAmbientCorrection*2+1;
	half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector) + rtp_customAmbientCorrection*0.5;
	
	half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);

	// PBL	
	half3 h = normalize (lightDir + viewDir);
	fixed diff = dot (s.Normal, lightDir); // n_dot_l
	diff = saturate(diff);
	
	float n_dot_l=diff;
	float n_dot_h = saturate(dot (s.Normal, h));
	float h_dot_l = dot (h, lightDir);
	// hacking spec normalisation to get quiet a dark spec for max roughness (will be 0.25/16)
	float specular_power=exp2(10*s.Specular+1) - 1.75;
	float normalisation_term = specular_power / 16.0f; // /8.0f in equations, but we multiply (atten * 2) in lighting below
	float blinn_phong = pow( n_dot_h, specular_power );    // n_dot_h is the saturated dot product of the normal and half vectors
	float specular_term = normalisation_term * blinn_phong;
	
	#ifdef RTP_PBL_FRESNEL
		// fresnel
		//float exponential = pow( 1.0f - h_dot_l, 5.0f ); // Schlick's approx to fresnel
		// below pow 4 looks OK and is cheaper than pow() call
		float exponential = 1.0f - h_dot_l;
		exponential*=exponential;
		exponential*=exponential;
		// skyshop fit (I'd like people to get similar results in gamma / linear)
		#if defined(RTP_COLORSPACE_LINEAR)
			exponential=0.03+0.97*exponential;
		#else
			exponential=0.25+0.75*exponential;
		#endif
		float pbl_fresnel_term = lerp (1.0f, exponential,  s.RTP.x); // o.RTP.x - _Fresnel
	#endif
		
	#ifdef RTP_PBL_VISIBILITY_FUNCTION
		// visibility
		float n_dot_v = saturate(dot (s.Normal, viewDir));
		float alpha = 1.0f / ( sqrt( 3.1415/4 * specular_power + 3.1415/2 ) );
		float pbl_visibility_term = ( n_dot_l * ( 1.0f - alpha ) + alpha ) * ( n_dot_v * ( 1.0f - alpha ) + alpha ); // Both dot products should be saturated
		pbl_visibility_term = 1.0f / pbl_visibility_term;	
	#endif
	
	float spec = specular_term * diff * pbl_fresnel_term * pbl_visibility_term;
	
	// specColor used outside in the forward path, compiled out in prepass
	#ifdef RTP_COLORSPACE_LINEAR
		specColor = lm * s.SpecColor.rgb * spec;
	#else
		// shape ^2 spec golor (to fit IBL and not overbright spot for high glossy)
		specColor = lm * s.SpecColor.rgb * s.SpecColor.rgb * spec;
	#endif

	#if defined(RTP_DEFERRED_PBL_NORMALISATION)
		// spec from the alpha component is used to calculate specular
		// in the Lighting*_Prepass function, it's not used in forward
		return half4(lm, spec)*s.RTP.y; // s.RTP.y - self - shadow atten from surf()
	#else
		// (part taken from Lux)
		// spec from the alpha component is used to calculate specular
		// in the Lighting*_Prepass function, it's not used in forward
		// we have to compress spec like we do in the "Intrenal-PrepassLighting" shader
		return half4(lm, log2(spec + 1))*s.RTP.y; // s.RTP.y - self - shadow atten from surf()
	#endif

}
//
// EOF lighting & misc functions
//
///////////////////////////////////////	

	float4 _MainTex_ST;
	void vert (inout appdata_full v, out Input o) {
	    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
			UNITY_INITIALIZE_OUTPUT(Input, o);
		#endif

		o.texCoords.xy=TRANSFORM_TEX(v.texcoord, _MainTex);
	
		float3 Wpos=mul(_Object2World, v.vertex).xyz;
		o._auxDir.x=length(_WorldSpaceCameraPos.xyz-Wpos);
		o._auxDir.y = Wpos.y;
		
		o.texCoords.zw=Wpos.xz;
		
		#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS) || defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal.xyz );				
		#endif
		#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
		  	float3 TtoW0 = mul(rotation, ((float3x3)_Object2World)[0].xyz)*unity_Scale.w;
	  		float3 TtoW2 = mul(rotation, ((float3x3)_Object2World)[2].xyz)*unity_Scale.w;		
			o.TtoW0_TtoW2.xy = normalEncode(TtoW0);
			o.TtoW0_TtoW2.zw = normalEncode(TtoW2);
  		#endif

		#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS)
			o._auxDir.zw = normalEncode(( mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz) ).xyz);
		#endif
	}

	void surf (Input IN, inout RTPSurfaceOutput o) {
		o.Normal=float3(0,0,1); o.Albedo=0;	o.Emission=0; o.Specular=0.01; o.Alpha=0;
		o.RTP.xy=float2(0,1); o.SpecColor=0;
		half o_Gloss=0;	

		float _uv_Relief_w=saturate((IN._auxDir.x - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN) || defined(RTP_UV_BLEND)
			float _uv_Relief_z=saturate((IN._auxDir.x - _TERRAIN_distance_start) / _TERRAIN_distance_transition);
			float _uv_Relief_wz_no_overlap=_uv_Relief_w*_uv_Relief_z;
			_uv_Relief_z=1-_uv_Relief_z;
		#else
			float _uv_Relief_z=1-_uv_Relief_w;
		#endif

		#if defined(GLOBAL_PERLIN)		
			float4 global_bump_val=tex2D(_BumpMapGlobal, IN.texCoords.xy*_BumpMapGlobalScale);
			#if !defined(RTP_SIMPLE_SHADING)
				global_bump_val.rg=global_bump_val.rg*0.6 + tex2D(_BumpMapGlobal, IN.texCoords.xy*_BumpMapGlobalScale*8).rg*0.4;
			#endif
		#endif

		float2 globalUV=(IN.texCoords.zw-_TERRAIN_PosSize.xy)/_TERRAIN_PosSize.zw;
		#ifdef COLOR_MAP
			float global_color_blend=lerp( lerp(_GlobalColorMapBlendValues.y, _GlobalColorMapBlendValues.x, _uv_Relief_z*_uv_Relief_z), _GlobalColorMapBlendValues.z, _uv_Relief_w);
			#if defined(RTP_SIMPLE_SHADING) || !defined(GLOBAL_PERLIN)
				float4 global_color_value=tex2D(_ColorMapGlobal, globalUV);
				global_color_value=lerp(tex2Dlod(_ColorMapGlobal, float4(globalUV, _GlobalColorMapNearMIP.xx)), global_color_value, _uv_Relief_w);
			#else
				float4 global_color_value=tex2D(_ColorMapGlobal, globalUV+(global_bump_val.rg-float2(0.5f, 0.5f))*_GlobalColorMapDistortByPerlin);
				global_color_value=lerp(tex2Dlod(_ColorMapGlobal, float4(globalUV+(global_bump_val.rg-float2(0.5f, 0.5f))*_GlobalColorMapDistortByPerlin, _GlobalColorMapNearMIP.xx)), global_color_value, _uv_Relief_w);
			#endif
			
			//float perlin2global_color=abs((global_bump_val.r-0.4)*5);
			//perlin2global_color*=perlin2global_color;
			//float GlobalColorMapSaturationByPerlin = saturate( lerp(_GlobalColorMapSaturation, _GlobalColorMapSaturationFar, _uv_Relief_w) -perlin2global_color*_GlobalColorMapSaturationByPerlin);
			float GlobalColorMapSaturationByPerlin = lerp(_GlobalColorMapSaturation, _GlobalColorMapSaturationFar, _uv_Relief_w);
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, GlobalColorMapSaturationByPerlin);
			global_color_value.rgb*=lerp(_GlobalColorMapBrightness, _GlobalColorMapBrightnessFar, _uv_Relief_w);
		#endif		
		
		#if defined(GLOBAL_PERLIN)
      		float perlinmask=tex2Dbias(_BumpMapGlobal, float4(IN.texCoords.xy/16, _uv_Relief_w.xx*2)).r;  		
      	#else
      		#if defined(RTP_WETNESS) && !defined(SIMPLE_WATER)
      			float perlinmask=tex2D(TERRAIN_FlowingMap, IN.texCoords.xy/8).a;
      		#else
      			float perlinmask=0;
      		#endif
      	#endif
   		float3 norm_far=float3(0,0,1);

		#if defined(TWO_LAYERS)
	      	float2 tH;
	      	tH.x=tex2D(_HeightMap, IN.texCoords.xy).a;
	    	tH.y=tex2D(_HeightMap2, IN.texCoords.xy).a;
	      	#if !defined(RTP_SIMPLE_SHADING)
	      	#if defined(GEOM_BLEND)
	      		float eh=max(0.001, _ExtrudeHeight*(1-IN.color.a));
	      	#else
	      		float eh=_ExtrudeHeight;
	      	#endif		      	
	      	float2 uv=IN.texCoords.xy + ParallaxOffset(tH.x, eh, IN.viewDir.xyz);
	      	float2 uv2=IN.texCoords.xy + ParallaxOffset(tH.y, eh, IN.viewDir.xyz);
	      	#endif
	      	float2 control=float2(IN.color.r, 1-IN.color.r);
	      	control*=(tH+0.01);
      		float2 control_orig=control;		
	      	control*=control;
	      	control*=control;
	      	control*=control;
	      	control/=dot(control, 1);
	      	#ifdef NOSPEC_BLEED
				float2 control_nobleed=saturate(control-float2(0.5,0.5))*2;
			#else
				float2 control_nobleed=control;
			#endif
	      	float actH=dot(control, tH);
	    #else
	      	float actH=tex2D(_HeightMap, IN.texCoords.xy).a;
	      	#if !defined(RTP_SIMPLE_SHADING)
	      	#if defined(GEOM_BLEND)
	      		float eh=max(0.001, _ExtrudeHeight*(1-IN.color.a));
	      	#else
	      		float eh=_ExtrudeHeight;
	      	#endif		      	
	      	float2 uv=IN.texCoords.xy + ParallaxOffset(actH, eh, IN.viewDir.xyz);
	      	#endif
		#endif
      		
      	float2 rayPos;
      	
		// simple fresnel rim (w/o bumpmapping)
		IN.viewDir=normalize(IN.viewDir);
		float diffFresnel = 1.0f - IN.viewDir.z;
		diffFresnel*=diffFresnel;
		diffFresnel*=diffFresnel;
		
		#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS)
			float3 flat_dir = normalDecode(IN._auxDir.zw);
			#if defined(RTP_WETNESS)
				float wetSlope=1-dot(norm_far, flat_dir.xyz);
			#endif
		#endif
		
		#if defined(GLOBAL_PERLIN)
			norm_far.xy = global_bump_val.rg*3-1.5;
			norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));			
		#endif
		
		#ifdef RTP_CAUSTICS
		float damp_fct_caustics;
   		#if defined(RTP_WETNESS)
			float damp_fct_caustics_inv;
		#endif
		{
			float norm=saturate(1-flat_dir.z);
			norm*=norm;
			norm*=norm;  
			float CausticsWaterLevel=TERRAIN_CausticsWaterLevel+norm*TERRAIN_CausticsWaterLevelByAngle;
			damp_fct_caustics=saturate((IN._auxDir.y-CausticsWaterLevel+TERRAIN_CausticsWaterDeepFadeLength)/TERRAIN_CausticsWaterDeepFadeLength);
			float overwater=saturate(-(IN._auxDir.y-CausticsWaterLevel-TERRAIN_CausticsWaterShallowFadeLength)/TERRAIN_CausticsWaterShallowFadeLength);
			damp_fct_caustics*=overwater;
       		#if defined(RTP_WETNESS)
				damp_fct_caustics_inv=1-overwater;
			#endif
			damp_fct_caustics*=saturate(flat_dir.z+0.1)*0.9+0.1;
		}
		#endif			
		
		// snow initial step
		#ifdef RTP_SNOW
			float3 norm_for_snow=norm_far*0.3;
			norm_for_snow.z+=0.7;
			#if defined(VERTEX_COLOR_TO_SNOW_COVERAGE)
				rtp_snow_strength*=VERTEX_COLOR_TO_SNOW_COVERAGE;
			#endif	
			float snow_const = 0.5*rtp_snow_strength;
			snow_const-=perlinmask;
			float snow_height_fct=saturate((rtp_snow_height_treshold - IN._auxDir.y)/rtp_snow_height_transition)*4;
			snow_height_fct=snow_height_fct<0 ? 0 : snow_height_fct;
			snow_const -= snow_height_fct;
			
			float snow_val;
			#ifdef COLOR_MAP
				snow_val = snow_const + rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow.xxx)+rtp_snow_strength*2;
			#else
				rtp_global_color_brightness_to_snow=0;
				snow_val = snow_const + rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow+rtp_snow_strength*2;
			#endif
			snow_val -= rtp_snow_slope_factor*( 1 - dot(norm_for_snow, flat_dir.xyz) );
	
			float snow_depth=snow_val-1;
			snow_depth=snow_depth<0 ? 0:snow_depth*6; 
			
			//float snow_depth_lerp=saturate(snow_depth-rtp_snow_deep_factor);
	
			fixed3 rtp_snow_color_tex=rtp_snow_color.rgb;
		#endif		
		
		#ifdef RTP_UV_BLEND
			float blendVal=(1.0-_uv_Relief_z*0.3);
			#if defined(TWO_LAYERS)
				blendVal *= dot( control, float2(_MixBlend0, _MixBlend1) );
			#else
				blendVal *= _MixBlend0;
			#endif
			#if defined(GLOBAL_PERLIN)
				blendVal*=saturate((global_bump_val.r*global_bump_val.g*2+0.3));
			#endif

			#if defined(TWO_LAYERS)
				float2 MixScaleRouted=float2(_MixScale0, _MixScale1);
			#else
				float MixScaleRouted=_MixScale0;
			#endif
		#endif		
		
		// layer emission - init step
		#ifdef RTP_EMISSION
			#if defined(TWO_LAYERS)
				float emission_valA=dot(control, float2(_LayerEmission0, _LayerEmission1) );
				half3 _LayerEmissionColor=control.x * _LayerEmissionColor0 + control.y * _LayerEmissionColor1;
				float layer_emission = emission_valA;
			#else
				half3 _LayerEmissionColor=_LayerEmissionColor0;
				float layer_emission = _LayerEmission0;
			#endif
		#endif		
		
		#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			// gloss vs. fresnel dependency
			#if defined(TWO_LAYERS)			
				float fresnelAtten=dot(control, float2(RTP_FresnelAtten0, RTP_FresnelAtten1) );
				o.RTP.x=dot(control, float2(RTP_Fresnel0, RTP_Fresnel1) );
			#else
				float fresnelAtten=RTP_FresnelAtten0;
				o.RTP.x=RTP_Fresnel0;
			#endif
		#endif

      	#if defined(RTP_SIMPLE_SHADING)
      		rayPos=IN.texCoords.xy;
      	#else
			#if defined(TWO_LAYERS)		      	
	      		rayPos=lerp(uv, uv2, control.y);
      		#else
    	  		rayPos=uv;
      		#endif
      	#endif
      	
	    #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
	        float p = 0;
	        float _WaterOpacity=0;
		#endif
		
		////////////////////////////////
		// water
		//
		#ifdef RTP_WETNESS
			#if defined(VERTEX_COLOR_TO_WATER_COVERAGE) || !defined(GLOBAL_PERLIN)
				float water_mask=IN.color.b;
			#else
				float mip_selector_tmp=saturate(_uv_Relief_w-1);// bug in compiler for forward pass, we have to specify mip level indirectly (can't be treated constant)
				float water_mask=tex2Dlod(_BumpMapGlobal, float4(globalUV*(1-2*_BumpMapGlobal_TexelSize.xx)+_BumpMapGlobal_TexelSize.xx, mip_selector_tmp.xx)).b;
			#endif
			#if defined(TWO_LAYERS)		      	
				float2 water_splat_control=control;
				float2 water_splat_control_nobleed=control_nobleed;
			#endif
			float TERRAIN_LayerWetStrength=saturate( 2*(1 - water_mask-perlinmask*(1-TERRAIN_GlobalWetness)) )*TERRAIN_GlobalWetness;
			float2 roff=0;
			float2 flowOffset	=0;

			wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
			float _RippleDamp=saturate(TERRAIN_LayerWetStrength*2-1)*saturate(1-wetSlope*4)*_uv_Relief_z;
			TERRAIN_RainIntensity*=_RippleDamp;
			TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
			TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
			#ifdef RTP_CAUSTICS
				TERRAIN_WaterLevel*=damp_fct_caustics_inv;
			#endif				
			TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH*0.25);
			
			p = saturate((TERRAIN_WaterLevel - actH -(1-actH)*perlinmask*0.5)*TERRAIN_WaterEdge);
			p*=p;
	        _WaterOpacity=TERRAIN_WaterOpacity*p;
			#if defined(RTP_EMISSION)
				float wEmission = TERRAIN_WaterEmission*p;
				layer_emission = lerp( layer_emission, wEmission, _WaterOpacity);
				layer_emission = max( layer_emission, wEmission*(1-_WaterOpacity) );
			#endif					
			#if !defined(RTP_SIMPLE_SHADING) && !defined(SIMPLE_WATER)
				float2 flowUV=lerp(IN.texCoords.xy, rayPos.xy, 1-p*0.5)*TERRAIN_FlowScale;
				float _Tim=frac(_Time.x*TERRAIN_FlowCycleScale)*2;
				float ft=abs(frac(_Tim)*2 - 1);
				float2 flowSpeed=clamp((flat_dir.xy+0.01)*4,-1,1)/TERRAIN_FlowCycleScale;
				#ifdef FLOWMAP
					float4 vec=tex2D(_FlowMap, flowUV)*2-1;
					flowSpeed+=lerp(vec.xy, vec.zw, IN.color.r)*float2(-1,1)*TERRAIN_FlowSpeedMap;
				#endif
				flowUV*=TERRAIN_FlowScale;
				flowSpeed*=TERRAIN_FlowSpeed*TERRAIN_FlowScale;
				float rtp_mipoffset_add = (1-saturate(dot(flowSpeed, flowSpeed)*TERRAIN_mipoffset_flowSpeed))*TERRAIN_mipoffset_flowSpeed;
				rtp_mipoffset_add+=(1-TERRAIN_LayerWetStrength)*8;
				rtp_mipoffset_add+=TERRAIN_FlowMipOffset;
				#if defined(GLOBAL_PERLIN)
					flowOffset=tex2Dbias(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx)*flowSpeed, rtp_mipoffset_add.xx)).rg*2-1;
					flowOffset=lerp(flowOffset, tex2Dbias(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.25, rtp_mipoffset_add.xx)).rg*2-1, ft);
				#else
					flowOffset=tex2Dbias(TERRAIN_FlowingMap, float4(flowUV+frac(_Tim.xx)*flowSpeed, rtp_mipoffset_add.xx)).ag*2-1;
					flowOffset=lerp(flowOffset, tex2Dbias(TERRAIN_FlowingMap, float4(flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.25, rtp_mipoffset_add.xx)).ag*2-1, ft);
				#endif
				#ifdef RTP_SNOW
					flowOffset*=saturate(1-snow_val);
				#endif							
				flowOffset*=lerp(TERRAIN_WetFlow, TERRAIN_Flow, p)*_uv_Relief_z*TERRAIN_LayerWetStrength;
			#endif
			
			#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
				float2 rippleUV = IN.texCoords.xy*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
			    float4 Ripple;
			  	{
			  	 	Ripple = tex2D(TERRAIN_RippleMap, rippleUV);
				    Ripple.xy = Ripple.xy * 2 - 1;
				
				    float DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
				    float TimeFrac = DropFrac - 1.0f + Ripple.z;
				    float DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
				    float FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
				  	roff = Ripple.xy * FinalFactor * 0.35f;
				  	
				  	rippleUV+=float2(0.25,0.25);
			  	 	Ripple = tex2D(TERRAIN_RippleMap, rippleUV);
				    Ripple.xy = Ripple.xy * 2 - 1;
				
				    DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
				    TimeFrac = DropFrac - 1.0f + Ripple.z;
				    DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
				    FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
				  	roff += Ripple.xy * FinalFactor * 0.35f;
			  	}
			  	roff*=4*_RippleDamp*lerp(TERRAIN_WetDropletsStrength, 1, p);
			  	#ifdef RTP_SNOW
			  		roff*=saturate(1-snow_val);
			  	#endif
			  	roff+=flowOffset;
			#else
				roff = flowOffset;
			#endif
			
			#if !defined(RTP_SIMPLE_SHADING)
				flowOffset=TERRAIN_Refraction*roff*max(p, TERRAIN_WetRefraction);
				#if !defined(RTP_TRIPLANAR)
					rayPos.xy+=flowOffset;
				#endif
			#endif
		#endif
		// water
		///////////////////////////////////////////	      	
	    
	    //
	    // diffuse color
	    //
		#if defined(TWO_LAYERS)		      	
	      	fixed4 col = control.x*tex2D(_MainTex, rayPos.xy) + control.y*tex2D(_MainTex2, rayPos.xy);
	    #else
	      	fixed4 col = tex2D(_MainTex, rayPos.xy);
	    #endif

	    // UV blend
		#if defined(RTP_UV_BLEND)
			#if defined(TWO_LAYERS)
				fixed4 colBlend = control.x * tex2D(_MainTex, IN.texCoords.xy * _MixScale0) + control.y * tex2D(_MainTex2, IN.texCoords.xy * _MixScale1);
				float3 colBlendDes=lerp((dot(colBlend.rgb, 0.33333)).xxx, colBlend.rgb, dot(control, float2(_MixSaturation0, _MixSaturation1)));
				float repl = dot( control, float2(_MixReplace0, _MixReplace1) );
				repl *= _uv_Relief_wz_no_overlap;
		        float3 blendNormal = UnpackNormal( tex2D(_BumpMap, IN.texCoords.xy*_MixScale0)*control.x + tex2D (_BumpMap2, IN.texCoords.xy*_MixScale1)*control.y );
				col.rgb=lerp(col.rgb, col.rgb*colBlendDes*dot(control, float2(_MixBrightness0, _MixBrightness1) ), lerp(blendVal, 1, repl));  
			#else
				fixed4 colBlend = tex2D(_MainTex, IN.texCoords.xy * _MixScale0);
				float3 colBlendDes=lerp((dot(colBlend.rgb, 0.33333)).xxx, colBlend.rgb, _MixSaturation0);
				float repl = _MixReplace0;
				repl *= _uv_Relief_wz_no_overlap;
				float3 blendNormal = UnpackNormal( tex2D(_BumpMap, IN.texCoords.xy*_MixScale0));
				col.rgb=lerp(col.rgb, col.rgb*colBlendDes*_MixBrightness0, lerp(blendVal, 1, repl));
			#endif
			col.rgb = lerp( col.rgb, colBlend.rgb , repl );
		#endif		    
	    
		#ifdef VERTICAL_TEXTURE
			float2 vert_tex_uv=float2(0, IN._auxDir.y/_VerticalTextureTiling);
			#ifdef GLOBAL_PERLIN
				vert_tex_uv += _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
			#endif
			half3 vert_tex=tex2D(_VerticalTexture, vert_tex_uv).rgb;
			#if defined(TWO_LAYERS)
				float _VerticalTextureStrength=dot(control, float2(_VerticalTextureStrength0, _VerticalTextureStrength1));
			#else
				float _VerticalTextureStrength=_VerticalTextureStrength0;
			#endif
			col.rgb=lerp( col.rgb, col.rgb*vert_tex*2, _VerticalTextureStrength );
		#endif
			    
      	fixed3 colAlbedo=0;
      	
      	//
      	// PBL specularity
      	//
      	float glcombined=col.a;
		#if defined(RTP_UV_BLEND)			
			glcombined=lerp(glcombined, colBlend.a, repl*0.5);					
		#endif		    
		#if defined(RTP_COLORSPACE_LINEAR)
		//glcombined=FastToLinear(glcombined);
		#endif
		#if defined(TWO_LAYERS)		      	
			float RTP_gloss2mask = dot(control, float2(RTP_gloss2mask0, RTP_gloss2mask1) );
			float _Spec = dot(control_nobleed, float2(_Spec0, _Spec1)); // anti-bleed subtraction
			float RTP_gloss_mult = dot(control, float2(RTP_gloss_mult0, RTP_gloss_mult1) );
			float RTP_gloss_shaping = dot(control, float2(RTP_gloss_shaping0, RTP_gloss_shaping1) );
		#else
			float RTP_gloss2mask = RTP_gloss2mask0;
			float _Spec = _Spec0;
			float RTP_gloss_mult = RTP_gloss_mult0;
			float RTP_gloss_shaping = RTP_gloss_shaping0;
		#endif
		float gls = saturate(glcombined * RTP_gloss_mult);
		o_Gloss =  lerp(1, gls, RTP_gloss2mask) * _Spec;
		
		float2 gloss_shaped=float2(gls, 1-gls);
		gloss_shaped=gloss_shaped*gloss_shaped*gloss_shaped;
		gls=lerp(gloss_shaped.x, 1-gloss_shaped.y, RTP_gloss_shaping);
		o.Specular = saturate(gls);
		// gloss vs. fresnel dependency
		#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			o.RTP.x*=lerp(1, 1-fresnelAtten, o.Specular*0.9+0.1);
		#endif
		half colDesat=dot(col.rgb,0.33333);
		#if defined(TWO_LAYERS)		 		
			col.rgb=lerp(colDesat.xxx, col.rgb, dot(control, float2(_LayerSaturation0, _LayerSaturation1)) );	
			float brightness2Spec=dot(control, float2(_LayerBrightness2Spec0, _LayerBrightness2Spec1));
        #else
			col.rgb=lerp(colDesat.xxx, col.rgb, _LayerSaturation0);
			float brightness2Spec=_LayerBrightness2Spec0;
        #endif
		o_Gloss*=lerp(1, colDesat, brightness2Spec);
		colAlbedo=col.rgb;
		#if defined(TWO_LAYERS)		 		
			col.rgb*=(control.x*_LayerColor0 + control.y*_LayerColor1)*2;
	      	
	        o.Normal = UnpackNormal( tex2D(_BumpMap, rayPos.xy)*control.x + tex2D (_BumpMap2, rayPos.xy)*control.y );
        #else
			col.rgb*=_LayerColor0*2;
	      	
	        o.Normal = UnpackNormal( tex2D(_BumpMap, rayPos.xy) );
        #endif
		#if defined(RTP_UV_BLEND)
			o.Normal=lerp(o.Normal, blendNormal, repl);
		#endif        
        #ifdef RTP_SNOW
        	o.Normal = lerp( o.Normal, float3(0,0,1), saturate(snow_depth)*0.5 );
        #endif
      	
		////////////////////////////////
		// water
		//
        #if defined(RTP_WETNESS)
			#ifdef RTP_CAUSTICS
				TERRAIN_WetSpecularity*=damp_fct_caustics_inv;
				TERRAIN_WetGloss*=damp_fct_caustics_inv;
				TERRAIN_WaterSpecularity*=damp_fct_caustics_inv;
				TERRAIN_WaterGloss*=damp_fct_caustics_inv;
			#endif
	  		float porosity = 1-saturate(o.Specular * 4 - 1);
			#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
	        o.RTP.x = lerp(o.RTP.x, TERRAIN_WaterColor.a, TERRAIN_LayerWetStrength);
	        #endif
			float wet_fct = saturate(TERRAIN_LayerWetStrength*2-0.4);
			float glossDamper=lerp( (1-TERRAIN_WaterGlossDamper), 1, _uv_Relief_z); // odległość>near daje całkowite tłumienie
	        o.Specular += lerp(TERRAIN_WetGloss, TERRAIN_WaterGloss, p) * wet_fct * glossDamper; // glossiness boost
	        o.Specular=saturate(o.Specular);
	        o_Gloss += lerp(TERRAIN_WetSpecularity, TERRAIN_WaterSpecularity, p) * wet_fct; // spec boost
	        o_Gloss=max(0, o_Gloss);
	  		
	  		// col - saturation, brightness
	  		half3 col_sat=col.rgb*col.rgb; // saturation z utrzymaniem jasności
	  		col_sat*=dot(col.rgb,1)/dot(col_sat,1);
	  		wet_fct=saturate(TERRAIN_LayerWetStrength*(2-perlinmask));
	  		porosity*=0.5;
	  		col.rgb=lerp(col.rgb, col_sat, wet_fct*porosity);
			col.rgb*=1-wet_fct*TERRAIN_WetDarkening*(porosity+0.5);
					  		
	        // col - colorisation
	        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p);
	        
 			// col - opacity
			col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
			colAlbedo=lerp(colAlbedo, col.rgb, _WaterOpacity); // potrzebne do spec color				
	        
	        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
	        o.Normal.xy+=roff;
        #endif
		// water
		////////////////////////////////      	
		
		float3 norm_snowCov=o.Normal;

				
		#if defined(TWO_LAYERS)
			float _BumpMapGlobalStrengthPerLayer=dot(control, float2(_BumpMapGlobalStrength0, _BumpMapGlobalStrength1));
		#else
			float _BumpMapGlobalStrengthPerLayer=_BumpMapGlobalStrength0;
		#endif
		#if !defined(RTP_SIMPLE_SHADING)
			{
			float3 tangentBase = normalize(cross(float3(0.0,1.0,0.0), norm_far));
			float3 binormalBase = normalize(cross(norm_far, tangentBase));
			float3 combinedNormal = tangentBase * o.Normal.x + binormalBase * o.Normal.y + norm_far * o.Normal.z;
			o.Normal = lerp(o.Normal, combinedNormal, lerp(rtp_perlin_start_val,1, _uv_Relief_w)*_BumpMapGlobalStrengthPerLayer);
			}
		#else
			o.Normal+=norm_far*lerp(rtp_perlin_start_val,1, _uv_Relief_w)*_BumpMapGlobalStrengthPerLayer;	
		#endif


		#ifdef COLOR_MAP
			float colBrightness=dot(col,1);
			#ifdef RTP_WETNESS
				global_color_blend*=(1-_WaterOpacity);
			#endif
			
			// basic global colormap blending
			#ifdef COLOR_MAP_BLEND_MULTIPLY
				col.rgb=lerp(col.rgb, col.rgb*global_color_value.rgb*2, global_color_blend);
			#else
				col.rgb=lerp(col.rgb, global_color_value.rgb, global_color_blend);
			#endif

			#ifdef RTP_IBL_DIFFUSE
				half3 colBrightnessNotAffectedByColormap=col.rgb*colBrightness/max(0.01, dot(col.rgb,float3(1,1,1)));
			#endif
		#else
			#ifdef RTP_IBL_DIFFUSE
				half3 colBrightnessNotAffectedByColormap=col.rgb;
			#endif
		#endif		
		
		#ifdef RTP_SNOW
			//rayPos.xy=lerp(rayPos.xy, IN.texCoords.xy, snow_depth_lerp);
		
			#ifdef COLOR_MAP
				snow_val = snow_const + rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow.xxx)+rtp_snow_strength*2;
			#else
				snow_val = snow_const + rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow+rtp_snow_strength*2;
			#endif
			
			snow_val -= rtp_snow_slope_factor*saturate( 1 - dot( (norm_snowCov*0.7+norm_for_snow*0.3), flat_dir.xyz) - 0*dot( norm_for_snow, flat_dir.xyz));
			
			snow_val=saturate(snow_val);
			snow_val=pow(abs(snow_val), rtp_snow_edge_definition);
			
			#ifdef COLOR_MAP
				half3 global_color_value_desaturated=dot(global_color_value.rgb, 0.37);//0.3333333); // będzie trochę jasniej
				#ifdef COLOR_MAP_BLEND_MULTIPLY
					rtp_snow_color_tex=lerp(rtp_snow_color_tex, rtp_snow_color_tex*global_color_value_desaturated.rgb*2, min(0.4,global_color_blend*0.7) );
				#else
					rtp_snow_color_tex=lerp(rtp_snow_color_tex, global_color_value_desaturated.rgb, min(0.4,global_color_blend*0.7) );
				#endif
			#endif
	
			col.rgb=lerp( col.rgb, rtp_snow_color_tex, snow_val );
			#ifdef RTP_IBL_DIFFUSE
				colBrightnessNotAffectedByColormap=lerp( colBrightnessNotAffectedByColormap, rtp_snow_color_tex, snow_val );
			#endif
			
			float3 snow_normal=o.Normal;
			snow_normal=norm_for_snow + 2*snow_normal*(_uv_Relief_z*0.5+0.5);
			
			snow_normal=normalize(snow_normal);
			o.Normal=lerp(o.Normal, snow_normal, snow_val);
			float rtp_snow_specular_distAtten=rtp_snow_specular;
			o_Gloss=lerp(o_Gloss, rtp_snow_specular, snow_val);
			// przeniesione pod emisję (która zależy od specular materiału _pod_ śniegiem)
			//o.Specular=lerp(o.Specular, rtp_snow_gloss, snow_val);
			#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			o.RTP.x=lerp(o.RTP.x, rtp_snow_fresnel, snow_val);
			#endif
			float snow_damp=saturate(1-snow_val*2);
		#endif
				
		// emission of layer (inside)
		#ifdef RTP_EMISSION
			#ifdef RTP_SNOW
				layer_emission *= snow_damp*0.9+0.1; // delikatna emisja poprzez snieg
			#endif
			
			#if defined(RTP_WETNESS)
				layer_emission *= lerp(o.Specular, 1, p) * 2;
				// zróżnicowanie koloru na postawie animowanych normalnych
				#ifdef RTP_FUILD_EMISSION_WRAP
					float norm_fluid_val=lerp( 0.7, saturate(dot(o.Normal.xy*4, o.Normal.xy*4)), _uv_Relief_z);
					o.Emission += (col.rgb + _LayerEmissionColor.rgb ) * ( norm_fluid_val*p+0.15 ) * layer_emission * 4;
				#else
					float norm_fluid_val=lerp( 0.5, (o.Normal.x+o.Normal.y), _uv_Relief_z);
					o.Emission += (col.rgb + _LayerEmissionColor.rgb ) * ( saturate(norm_fluid_val*2*p)*1.2+0.15 ) * layer_emission * 4;
				#endif
			#else
				layer_emission *= o.Specular * 2;
				o.Emission += (col.rgb + _LayerEmissionColor.rgb*0.2 ) * layer_emission * 4;
			#endif
			layer_emission = max(0, 1 - layer_emission);
			o_Gloss *= layer_emission;
			o.Specular *= layer_emission;
			col.rgb *= layer_emission;
		#endif		
		
		// przeniesione pod emisję (która zależy od specular materiału _pod_ śniegiem)
		#ifdef RTP_SNOW
			o.Specular=lerp(o.Specular, rtp_snow_gloss, snow_val);
		#endif	
			
		o.Normal=normalize(o.Normal);
		o.Albedo=col.rgb;
		
		#if defined(VERTEX_COLOR_AO_DAMP)
			o.RTP.y=VERTEX_COLOR_AO_DAMP;
		#endif
				
		// ^4 shaped diffuse fresnel term for soft surface layers (grass)
		#if defined(TWO_LAYERS)		
			float _DiffFresnel=dot( control, float2(RTP_DiffFresnel0, RTP_DiffFresnel1) );
		#else
			float _DiffFresnel=RTP_DiffFresnel0;
		#endif
		// diffuse fresnel term for snow
		#ifdef RTP_SNOW
			_DiffFresnel=lerp(_DiffFresnel, rtp_snow_diff_fresnel, snow_val);
		#endif
		float diffuseScatteringFactor=1.0 + diffFresnel*_DiffFresnel;
		o.Albedo *= diffuseScatteringFactor;
		#ifdef RTP_IBL_DIFFUSE
			colBrightnessNotAffectedByColormap *= diffuseScatteringFactor;
		#endif
		
		// spec color from albedo (metal tint)
		#if defined(TWO_LAYERS)
			float Albedo2SpecColor=dot(control, float2(_LayerAlbedo2SpecColor0, _LayerAlbedo2SpecColor1) );
		#else
			float Albedo2SpecColor=_LayerAlbedo2SpecColor0;
		#endif
		#ifdef RTP_SNOW
			Albedo2SpecColor*=snow_damp;
		#endif
		#ifdef RTP_WETNESS
			colAlbedo=lerp(colAlbedo, o.Albedo, p);
		#endif
		#if defined(TWO_LAYERS)
			o_Gloss=lerp( saturate(o_Gloss+4*dot(control_nobleed, float2(_FarSpecCorrection0,_FarSpecCorrection1) )), o_Gloss, (1-_uv_Relief_w)*(1-_uv_Relief_w) );
		#else
			o_Gloss=lerp( saturate(o_Gloss+4*_FarSpecCorrection0), o_Gloss, (1-_uv_Relief_w)*(1-_uv_Relief_w) );
		#endif
		float colAlbedoRGBmax=max(max(colAlbedo.r, colAlbedo.g), colAlbedo.b);
		float3 colAlbedoNew=lerp(half3(1,1,1), colAlbedo.rgb/colAlbedoRGBmax.xxx, saturate(colAlbedoRGBmax*4)*Albedo2SpecColor);
		half3 SpecColor=_SpecColor.rgb*o_Gloss*colAlbedoNew*colAlbedoNew; // spec color for IBL/Refl
		o.SpecColor=SpecColor;
		
		#if defined(RTP_AMBIENT_EMISSIVE_MAP)
			float4 eMapVal=tex2D(_AmbientEmissiveMapGlobal, globalUV);
			o.Emission+=o.Albedo*eMapVal.rgb*_AmbientEmissiveMultiplier*lerp(1, saturate(o.Normal.z*o.Normal.z-(1-actH)*(1-o.Normal.z*o.Normal.z)), _AmbientEmissiveRelief);
			float pixel_trees_shadow_val=saturate((IN._auxDir.x - _shadow_distance_start) / _shadow_distance_transition);
			pixel_trees_shadow_val=lerp(1, eMapVal.a, pixel_trees_shadow_val);
			float o_RTP_y_without_shadowmap_distancefade=o.RTP.y*lerp(1, eMapVal.a, _shadow_value);
			o.RTP.y*=lerp((1-_shadow_value), 1, pixel_trees_shadow_val);
		#else
			#define o_RTP_y_without_shadowmap_distancefade (o.RTP.y)
		#endif		
				
		#if defined(TWO_LAYERS)	
			float IBL_bump_smoothness=dot(control, float2(RTP_IBL_bump_smoothness0, RTP_IBL_bump_smoothness1) );
			#ifdef RTP_IBL_DIFFUSE
				float RTP_IBL_DiffStrength=dot(control, float2(RTP_IBL_DiffuseStrength0, RTP_IBL_DiffuseStrength1) );
			#endif
			#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
				// anti-bleed subtraction
				float RTP_IBL_SpecStrength=dot(control_nobleed, float2(RTP_IBL_SpecStrength0, RTP_IBL_SpecStrength1) );
			#endif
		#else
			float IBL_bump_smoothness=RTP_IBL_bump_smoothness0;
			#ifdef RTP_IBL_DIFFUSE
				float RTP_IBL_DiffStrength=RTP_IBL_DiffuseStrength0;
			#endif
			#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
				// anti-bleed subtraction
				float RTP_IBL_SpecStrength=RTP_IBL_SpecStrength0;
			#endif
		#endif
		
		#if defined(RTP_IBL_DIFFUSE) || defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
			float3 IBLNormal=lerp(o.Normal, float3(0,0,1), IBL_bump_smoothness);
			
			float3 TtoW0=normalDecode(IN.TtoW0_TtoW2.xy);
			float3 TtoW2=normalDecode(IN.TtoW0_TtoW2.zw);
			float3 TtoW1=cross(TtoW0, TtoW2);
			float3 normalW=float3(dot(TtoW0, IBLNormal), dot(TtoW1, IBLNormal), dot(TtoW2,IBLNormal));
		#endif
		// lerp IBL values with wet / snow
		#ifdef RTP_IBL_DIFFUSE
			#ifdef RTP_SNOW
				RTP_IBL_DiffStrength=lerp(RTP_IBL_DiffStrength, rtp_snow_IBL_DiffuseStrength, snow_val);
			#endif
	   		#if defined(RTP_SKYSHOP_SKY_ROTATION)
				float3 normalWR = _SkyMatrix[0].xyz*normalW.x + _SkyMatrix[1].xyz*normalW.y + _SkyMatrix[2].xyz*normalW.z;
			#else
				float3 normalWR = normalW;
			#endif					
			#ifdef RTP_SKYSHOP_SYNC
				half3 IBLDiffuseCol = SHLookup(normalWR)*RTP_IBL_DiffStrength;
			#else
				half3 IBLDiffuseCol = DecodeRGBM(texCUBElod(_DiffCubeIBL, float4(normalWR,0)))*RTP_IBL_DiffStrength;
			#endif
			IBLDiffuseCol*=colBrightnessNotAffectedByColormap * lerp(1, o_RTP_y_without_shadowmap_distancefade, TERRAIN_IBL_DiffAO_Damp);
			#ifdef RTP_SKYSHOP_SYNC
				IBLDiffuseCol*=_ExposureIBL.x;
			#endif	
			#ifndef RTP_IBL_SPEC
			o.Emission += IBLDiffuseCol.rgb;
			#else
			// dodamy za chwilę poprzez introplację, która zachowa energie
			#endif
		#endif
		#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
			#ifdef RTP_WETNESS
				RTP_IBL_SpecStrength=lerp(RTP_IBL_SpecStrength, TERRAIN_WaterIBL_SpecWetStrength, TERRAIN_LayerWetStrength);
				RTP_IBL_SpecStrength=lerp(RTP_IBL_SpecStrength, TERRAIN_WaterIBL_SpecWaterStrength, p*p);
			#endif
			#ifdef RTP_SNOW
				RTP_IBL_SpecStrength=lerp(RTP_IBL_SpecStrength, rtp_snow_IBL_SpecStrength, snow_val);
			#endif
		#endif		
		
		// kompresuję odwrotnie mip blur (łatwiej osiągnąć "lustro")
		#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
			float o_SpecularInvSquared = (1-o.Specular)*(1-o.Specular);
			float3 viewW=float3(dot(TtoW0, IN.viewDir), dot(TtoW1, IN.viewDir), dot(TtoW2, IN.viewDir));
			float3 reflVec=reflect(-viewW, normalW);
			#if defined(RTP_SKYSHOP_SKY_ROTATION)
			 	reflVec = _SkyMatrix[0].xyz*reflVec.x + _SkyMatrix[1].xyz*reflVec.y + _SkyMatrix[2].xyz*reflVec.z;
			#endif		
		#endif
		
		#ifdef RTP_IBL_SPEC
			float n_dot_v = saturate(dot (IBLNormal, (IN.viewDir.z<0? -IN.viewDir.xyz:IN.viewDir.xyz)));
			// float exponential = pow( 1.0f - n_dot_v, 5.0f ); // Schlick's approx to fresnel
			// below pow 4 looks OK and is cheaper than pow() call
			float exponential = 1.0f - n_dot_v;
			exponential*=exponential;
			exponential*=exponential;	
			
			// skyshop fit (I'd like people to get similar results in gamma / linear)
			#if defined(RTP_COLORSPACE_LINEAR)
				exponential=0.03+0.97*exponential;
			#else
				exponential=0.25+0.75*exponential;
			#endif
			float spec_fresnel = lerp (1.0f, exponential, o.RTP.x);
			half3 IBLSpecCol = DecodeRGBM(texCUBElod (_SpecCubeIBL, float4(reflVec, o_SpecularInvSquared*(6-exponential*o.RTP.x*3))))*RTP_IBL_SpecStrength;
			IBLSpecCol.rgb*=spec_fresnel * SpecColor * lerp(1, o_RTP_y_without_shadowmap_distancefade, TERRAIN_IBLRefl_SpecAO_Damp);
			#ifdef RTP_SKYSHOP_SYNC
				IBLSpecCol.rgb*=_ExposureIBL.y;
			#endif			
			#ifdef RTP_IBL_DIFFUSE
				// link difuse and spec IBL together with energy conservation
				o.Emission += saturate(1-IBLSpecCol.rgb) * IBLDiffuseCol.rgb + IBLSpecCol.rgb;
			#else
				o.Emission+=IBLSpecCol.rgb;
			#endif
		#endif
	
	    #if defined(RTP_REFLECTION)
			float2 mip_selectorRefl=o_SpecularInvSquared*(8-diffFresnel*o.RTP.x*4);
			#ifdef RTP_ROTATE_REFLECTION
				float3 refl_rot;
				refl_rot.x=sin(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.y=cos(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.z=-refl_rot.x;
				float2 tmpRefl;
				tmpRefl.x=dot(reflVec.xz, refl_rot.yz);
				tmpRefl.y=dot(reflVec.xz, refl_rot.xy);
				float t=tex2Dlod(_BumpMapGlobal, float4(tmpRefl*0.5+0.5, mip_selectorRefl)).a;
			#else
				float t=tex2Dlod(_BumpMapGlobal, float4(reflVec.xz*0.5+0.5, mip_selectorRefl)).a;
			#endif	
			#ifdef RTP_IBL_SPEC
				half rim=spec_fresnel;
			#else
				float n_dot_v = saturate(dot (IBLNormal, (IN.viewDir.z<0? -IN.viewDir.xyz:IN.viewDir.xyz)));
				// float exponential = pow( 1.0f - n_dot_v, 5.0f ); // Schlick's approx to fresnel
				// below pow 4 looks OK and is cheaper than pow() call
				float exponential = 1.0f - n_dot_v;
				exponential*=exponential;
				exponential*=exponential;	
				half rim= lerp(1, exponential, o.RTP.x);
		    #endif
		    float downSideEnvelope=saturate(reflVec.y*3);
		    t *= downSideEnvelope;
		    rim *= downSideEnvelope*0.7+0.3;
			#if defined(RTP_SIMPLE_SHADING)
				rim*=RTP_IBL_SpecStrength*_uv_Relief_z;
			#else
				rim*=RTP_IBL_SpecStrength;
			#endif
			rim-=o_SpecularInvSquared*rim*TERRAIN_ReflGlossAttenuation; // attenuate low gloss
			
			half3 reflCol;
			reflCol=lerp(TERRAIN_ReflColorB.rgb, TERRAIN_ReflColorC.rgb, saturate(TERRAIN_ReflColorCenter-t) / TERRAIN_ReflColorCenter );
			reflCol=lerp(reflCol.rgb, TERRAIN_ReflColorA.rgb, saturate(t-TERRAIN_ReflColorCenter) / (1-TERRAIN_ReflColorCenter) );
			o.Emission += reflCol * SpecColor * lerp(1, o_RTP_y_without_shadowmap_distancefade, TERRAIN_IBLRefl_SpecAO_Damp) * rim * 2;
		#endif		
		
		#ifdef RTP_CAUSTICS
		{
			float tim=_Time.x*TERRAIN_CausticsAnimSpeed;
			rayPos.xy=IN.texCoords.zw*TERRAIN_CausticsTilingScale;
			#ifdef RTP_VERTALPHA_CAUSTICS
				float3 _Emission=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(tim, tim) ).aaa;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(-tim, -tim*0.873) ).aaa;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*1.1+float2(tim, 0) ).aaa;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*0.5+float2(0, tim*0.83) ).aaa;
			#else
				float3 _Emission=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(tim, tim) ).rgb;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(-tim, -tim*0.873) ).rgb;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*1.1+float2(tim, 0) ).rgb;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*0.5+float2(0, tim*0.83) ).rgb;
			#endif
			_Emission=saturate(_Emission-1.55);
			_Emission*=_Emission;
			_Emission*=_Emission;
			_Emission*=TERRAIN_CausticsColor.rgb*8;
			_Emission*=damp_fct_caustics;
			//_Emission*=(1-_uv_Relief_w);
			o.Emission+=_Emission;
		} 
		#endif		
		
		#if defined(UNITY_PASS_PREPASSBASE)	 || defined(UNITY_PASS_PREPASSFINAL)
			o.Specular=max(0.01, o.Specular);
		#endif
	
		#if defined(UNITY_PASS_PREPASSFINAL)	
			#if defined(RTP_DEFERRED_PBL_NORMALISATION)
				o.Specular=1-o.Specular;
				o.Specular*=o.Specular;
				o.Specular=1-o.Specular;
				// hacking spec normalisation to get quiet a dark spec for max roughness (will be 0.25/16)
				float specular_power=exp2(10*o.Specular+1) - 1.75;
				float normalisation_term = specular_power / 8.0f;
				o.SpecColor*=normalisation_term;
			#endif
		#endif		
		
		#if defined(GEOM_BLEND)
			#if defined(BLENDING_HEIGHT)
				float4 terrain_coverage=tex2D(_TERRAIN_Control, globalUV);
				float2 tiledUV=(IN.texCoords.zw-_TERRAIN_PosSize.xy+_TERRAIN_Tiling.zw)/_TERRAIN_Tiling.xy;
				float4 splat_control1=terrain_coverage * tex2D(_TERRAIN_HeightMap, tiledUV) * IN.color.a;
				#if defined(TWO_LAYERS)
					float4 splat_control2=float4(control_orig, 0, 0) * (1-IN.color.a);
				#else
					float4 splat_control2=float4(actH+0.01, 0, 0, 0) * (1-IN.color.a);
				#endif

				float blend_coverage=dot(terrain_coverage, 1);
				if (blend_coverage>0.1) {

					splat_control1*=splat_control1;
					splat_control1*=splat_control1;
					splat_control1*=splat_control1;
					splat_control2*=splat_control2;
					splat_control2*=splat_control2;
					splat_control2*=splat_control2;

					float normalize_sum=dot(splat_control1, 1)+dot(splat_control2, 1);
					splat_control1 /= normalize_sum;
					splat_control2 /= normalize_sum;

					o.Alpha=dot(splat_control2,1);
					o.Alpha=lerp(1-IN.color.a, o.Alpha, saturate((blend_coverage-0.1)*4) );
				} else {
					o.Alpha=1-IN.color.a;
				}
			#else
				o.Alpha=1-IN.color.a;
			#endif		

			#if defined(UNITY_PASS_PREPASSFINAL)
				o.SpecColor*=o.Alpha;
			#endif		
		#endif
		

	}
	ENDCG
      
    } 
    Fallback "Diffuse"
}
