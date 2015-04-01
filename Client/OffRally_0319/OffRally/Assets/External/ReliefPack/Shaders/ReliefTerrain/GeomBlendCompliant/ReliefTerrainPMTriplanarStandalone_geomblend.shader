//
// Vertex paint / 4 layers version of RTP shader (optionally 2 or 3),
// in triplanar mode (enabled below) can be easily adopted for voxel terrains (I believe)
// most features are disabled by default, but if you need then (for example dynamic snow or so)
// - go on and look around to quickly customize it
//
// geom blend version - good for complex caverns inside terrain
// with 4 layers, coverage is realised using color RGB vertex color channels (layers 1-3), 4th layer coverage is defined as (1-(R+G+B))
// you can also use automatic coverage defined by mesh normals (local or global - LOCAL_SPACE_UV define keyword), for example WNORMAL_COVERAGE_X_Z_Ypos_Yneg
// vertex color A channel is used for blending
//
// (C) Tomasz Stobierski 2014
//
//
Shader "Relief Pack/GeometryBlend_ReliefTerrainPMTriplanarStandalone" {
	//
	// as RTP has so many features, number of material properties are huge (and not managed via script but default material property inspector)
	// for this reason commented out are those unused (disabled in shader code)
	// Uncomment properties needed and look for #defines section below to add/remove features manually
	// Sometimes they depends on each other and can't be used together, so beware (as you adjust them on your own, not using RTP_LODmanager)
	//
	Properties {
		//
		// analog to RTP Settings / Main
		//
		_TERRAIN_ReliefTransformTriplanarZ ("Triplanar tile size", Float) = 3
		// near distance
		_TERRAIN_distance_start ("Near distance fade start", Float) = 0
		_TERRAIN_distance_transition ("       Near distance fade length", Float) = 20
		_RTP_MIP_BIAS ("MIP Bias", Range(-1,1)) = 0
		// PM/POM
		_TERRAIN_ExtrudeHeight ("Parallax Mapping extrude height", Range(0.001,0.3)) = 0.06
		
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)		
		 // (uncomment property when used independently from terrain)
//		rtp_customAmbientCorrection ("Ambient correction", Color) = (0, 0, 0, 1)		
		
		// spec IBL (uncomment property block when used)
/*
		TERRAIN_IBLRefl_SpecAO_Damp ("Spec IBL / Refl AO damp", Range(0,1)) = 0
		_SpecCubeIBL ("IBL spec cubemap (leave blank for skyshop's)", Cube) = "black" {}
*/		
	
		// complementary lighting (uncomment property block when used independently from terrain)
/*
		RTP_ReflexLightDiffuseSoftness ("Reflex diffuse softness", Range(0,1)) = 0.5
		RTP_ReflexLightDiffuseColor1 ("Reflex light diffuse 1 (RGB+A strength)", Color) = (1, 1, 1, 0.05)
		RTP_ReflexLightDiffuseColor2 ("Reflex light diffuse 2 (RGB+A strength)", Color) = (1, 1, 1, 0.05)
		
		RTP_ReflexLightSpecColor ("Reflex light specular (RGB+A strength)", Color) = (1, 1, 1, 0.05)
		RTP_ReflexLightSpecularity ("Reflex light glossiness", Range(2,40)) = 0.1
		
		RTP_BackLightStrength ("Negative light power", Range(0,1)) = 0
*/	
	
/*
		RTP_AOsharpness ("Fake AO 2 HB sharpness", Range(0, 10)) = 1
		RTP_AOamp ("Fake AO 2 HB value", Range(0,2)) = 0.5
		RTP_AO_0123 ("       Per layer AO (XYZW - layers 0-3)", Vector) = (0.2, 0.2, 0.2, 0.2)	
*/	

/*
		EmissionRefractFiltering ("Emission refraction filtering", Range(0, 8)) = 2
		EmissionRefractAnimSpeed ("Emission refraction anim speed", Range(0,20)) = 2
*/	
		
		// detail textures / normal maps (combined), heightmap (combined)
		// you can use either atlas (to save 3 tex samplers)
//		_SplatAtlasA ("Detail atlas (RGB+A spec)", 2D) = "black" {}
		// or up to 4 textures (look for RTP_USE_COLOR_ATLAS define below)
		_SplatA0 ("Detailmap 0 (RGB+A spec)", 2D) = "black" {}
		_SplatA1 ("Detailmap 1 (RGB+A spec)", 2D) = "black" {}
		_SplatA2 ("Detailmap 2 (RGB+A spec)", 2D) = "black" {}
		_SplatA3 ("Detailmap 3 (RGB+A spec)", 2D) = "black" {}
		_BumpMap01 ("Bumpmap combined 0+1 (RG+BA)", 2D) = "grey" {}
		_BumpMap23 ("Bumpmap combined 2+3 (RG+BA)", 2D) = "grey" {}
		_TERRAIN_HeightMap ("Heightmap combined (RGBA - layers 0-3)", 2D) = "white" {}
		
		
		// per layer settings + PBL
		_Spec0123 ("Layer spec multiplier (XYZW - layers 0-3)", Vector) = (1, 1, 1, 1)
		RTP_gloss_mult0123 ("       gloss multiplier", Vector) = (1, 1, 1, 1)
		RTP_gloss2mask0123 ("       spec mask from gloss", Vector) = (0, 0, 0, 0)
		_LayerBrightness2Spec0123 ("       spec mask from albedo", Vector) = (0, 0, 0, 0)
		RTP_gloss_shaping0123 ("       gloss shaping", Vector) = (0.5, 0.5, 0.5, 0.5)
		
		_FarSpecCorrection0123 ("       far spec correction", Vector) = (0, 0, 0, 0)
		RTP_Fresnel0123 ("       fresnel", Vector) = (0, 0, 0, 0)
		RTP_FresnelAtten0123 ("           fresnel attenuate by gloss", Vector) = (0, 0, 0, 0)
		RTP_DiffFresnel0123 ("       diffuse scattering", Vector) = (0, 0, 0, 0)
//		RTP_IBL_bump_smoothness0123 ("       IBL / Refl bump smooth", Vector) = (0.7, 0.7, 0.7, 0.7)
//		RTP_IBL_SpecStrength0123 ("       IBL spec / Refl exposure", Vector) = (1, 1, 1, 1)
		_LayerAlbedo2SpecColor0123 ("          color from albedo (metal tint)", Vector) = (0, 0, 0, 0)		
		
		_LayerBrightness0123 ("          layer brightness", Vector) = (1, 1, 1, 1)		
		_LayerSaturation0123 ("          layer saturation", Vector) = (1, 1, 1, 1)		
		_MIPmult0123 ("       MIP offset at far distance", Vector) = (0,0,0,0)
		
/*
		_LayerEmission0123 ("Layer emission", Vector) = (0,0,0,0)
		_LayerEmissionColor0 ("       glow color0", Color) = (0.5,0.5,0.5,0)
		_LayerEmissionColor1 ("       glow color1", Color) = (0.5,0.5,0.5,0)
		_LayerEmissionColor2 ("       glow color2", Color) = (0.5,0.5,0.5,0)
		_LayerEmissionColor3 ("       glow color3", Color) = (0.5,0.5,0.5,0)
		_LayerEmissionRefractStrength0123 ("       hot air refract strength", Vector) = (0,0,0,0)
		_LayerEmissionRefractHBedge0123 ("          on layer edges only", Vector) = (0,0,0,0)
*/
	
		// UV blend
		_blend_multiplier ("UV blend multiplier", Range(0,1)) = 1
		// (0.2 means that one blended tile is 5 detail tiles)
		_MixScale0123 ("       UV blend tiling (XYZW - layers 0-3)", Vector) = (0.2, 0.2, 0.2, 0.2)
		_MixBlend0123 ("       UV blend value (XYZW - layers 0-3)", Vector) = (0.5, 0.5, 0.5, 0.5)
		_MixSaturation0123 ("       UV blend saturation (XYZW - layers 0-3)", Vector) = (1.0, 1.0, 1.0, 1.0)
		_MixBrightness0123 ("       UV blend brightness (XYZW - layers 0-3)", Vector) = (2.0, 2.0, 2.0, 2.0)
		_MixReplace0123 ("       UV blend replace (XYZW - layers 0-3)", Vector) = (0, 0, 0, 0)

		// global colormap
		_ColorMapGlobal ("Global colormap (RGBA)", 2D) = "white" {}
		_GlobalColorMapBlendValues ("       blending near/mid/far (XYZ)", Vector) = (0.3,0.6,0.8,0)
		_GlobalColorMapSaturation ("       saturation", Range(0,1)) = 0.8
		_GlobalColorMapBrightness ("       brightness", Range(0,1)) = 0.8
		_GlobalColorMapNearMIP ("       near MIP level", Range(0,1)) = 0.8
		_GlobalColorMapDistortByPerlin ("       distort by perlin", Range(0,0.02)) = 0.005
		//_GlobalColorMapSaturationByPerlin ("       saturation by perlin", Range(0,1)) = 0.1
		// global colormap - per layer
		_GlobalColorPerLayer0123 ("Global colormap per layer (XYZW - layers 0-3)", Vector) = (1.0, 1.0, 1.0, 1.0)
		// adv global colormap blending
/*
		_GlobalColorBottom0123 ("       Height level - bottom (XYZW - layers 0-3)", Vector) = (0, 0, 0, 0)
		_GlobalColorTop0123 ("       Height level - top (XYZW - layers 0-3)", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GlobalColorColormapLoSat0123 ("       colormap saturation LO", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GlobalColorColormapHiSat0123 ("       colormap saturation HI", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GlobalColorLayerLoSat0123 ("       layer saturation LO", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GlobalColorLayerHiSat0123 ("       layer saturation HI", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GlobalColorLoBlend0123 ("       Blending for LO", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GlobalColorHiBlend0123 ("       Blending for HI", Vector) = (1.0, 1.0, 1.0, 1.0)	
*/
						
		// perlin normal mapping (channels RG) with wetmask (G) and reflection map (A)
		_BumpMapGlobal ("Perlin normal combined w. water & reflection map (RG+B+A)", 2D) = "black" {}
		// mid / far distance definitions
		_TERRAIN_distance_start_bumpglobal ("       Far distance fade start (perlin)", Float) = 24
		rtp_perlin_start_val ("       Starting value (for start=0 only !)", Float) = 1
		_TERRAIN_distance_transition_bumpglobal ("       Far distance fade length", Float) = 50
		// (0.1 means that one perlin tile is 10 detail tiles)
		_BumpMapGlobalScale ("       Perlin normal tiling", Float) = 0.1
		rtp_mipoffset_globalnorm_offset ("       MIP offset", Range(0,5)) = 0
		_FarNormalDamp ("       Far normal damp", Range(0,1)) = 0
		_BumpMapGlobalStrength0123 ("       Perlin normal strength per layer (XYZW - layers 0-3)", Vector) = (0.3, 0.3, 0.3, 0.3)
		
/*		
		// superdetail
		_SuperDetailTiling ("Superdetail tiling", Float) = 2
		_SuperDetailStrengthNormal0123 ("       Superdetail strength per layer (XYZW - layers 0-3)", Vector) = (0.5, 0.5, 0.5, 0.5)
*/

/*	
		// reflection
		//
		TERRAIN_ReflColorA ("Reflection color A (Emissive RGB)", Color) = (0.5,0.5,0.5,0)
		TERRAIN_ReflColorB ("       color B (Diffuse RGBA)", Color) = (0.0,0.5,0.9,0)
		TERRAIN_ReflColorC ("       color C (Diffuse RGBA)", Color) = (0.3,0.6,0.9,0)
		TERRAIN_ReflColorCenter ("       gradient center", Range(0.1, 0.9)) = 0.5
		TERRAIN_ReflGlossAttenuation ("       roughness attenuation", Range(0, 1)) = 0.5
		TERRAIN_ReflectionRotSpeed ("       Reflection rotation speed", Range(0, 2)) = 0.3
*/

/*	
		// water/wet
		//
		TERRAIN_GlobalWetness ("Global wetness", Range(0,1)) = 1
		TERRAIN_WetHeight_Treshold ("       Height threshold [units]", Float) = -500
		TERRAIN_WetHeight_Transition ("       Height transition", Float) = 20
		TERRAIN_FlowSpeed ("       Flow speed", Range(0,4)) = 0.5
		TERRAIN_FlowCycleScale ("       Flow cycle scale", Range(0.5,4)) = 1
		TERRAIN_FlowScale ("       Flow tex tiling", Range(0.25,8)) = 1
		TERRAIN_FlowMipOffset ("       Flow tex filter", Range(0,4)) = 1
		TERRAIN_mipoffset_flowSpeed ("       Filter by flow speed", Range(0,0.25)) = 0.1
		TERRAIN_WetDarkening ("       Water surface darkening", Range(0.1,0.9)) = 0.5
		
		TERRAIN_RippleMap ("Ripplemap (RGB)", 2D) = "white" {}
		//TERRAIN_RainIntensity ("Rain intensity", Range(0,1)) = 1  // commented to be controlled globally by ReliefTerrain script
		TERRAIN_WetDropletsStrength ("       Rain on wet", Range(0,1)) = 0.1
		TERRAIN_DropletsSpeed ("       Anim Speed", Float) = 15
		TERRAIN_RippleScale ("       Ripple tex tiling", Range(0.25,8)) = 1
		

		// water - per layer
		TERRAIN_LayerWetStrength0123 ("Water strengh per layer (XYZW - 0123)", Vector) = (1,1,1,1)
		TERRAIN_WaterColor0 ("       Color layer 0", Color) = (0.5, 0.7, 1, 0.5)
		TERRAIN_WaterColor1 ("       Color layer 1", Color) = (0.5, 0.7, 1, 0.5)
		TERRAIN_WaterColor2 ("       Color layer 2", Color) = (0.5, 0.7, 1, 0.5)
		TERRAIN_WaterColor3 ("       Color layer 3", Color) = (0.5, 0.7, 1, 0.5)
		TERRAIN_WaterOpacity0123 ("       Water opacity (XYZW - 0123)", Vector) = (0.2,0.2,0.2,0.2)
		TERRAIN_WaterEmission0123 ("       Water emission (XYZW - 0123)", Vector) = (0,0,0,0)
		
		TERRAIN_WaterLevel0123 ("       Water level (XYZW - 0123)", Vector) = (0.5,0.5,0.5,0.5)
		TERRAIN_WaterLevelSlopeDamp0123 ("       Water level slope damp (XYZW - 0123)", Vector) = (0.1,0.1,0.1,0.1)
		TERRAIN_WaterEdge0123 ("       Water level sharpness (XYZW - 0123)", Vector) = (1,1,1,1)
		
		TERRAIN_WaterSpecularity0123 ("       Water spec boost (XYZW - 0123)", Vector) = (0.1,0.1,0.1,0.1)
		TERRAIN_WaterGloss0123 ("       Water gloss boost (XYZW - 0123)", Vector) = (0.2,0.2,0.2,0.2)
		
		TERRAIN_Flow0123 ("       Flow strength (XYZW - 0123)", Vector) = (1, 1, 1, 1)
		TERRAIN_Refraction0123 ("       Water refraction (XYZW - 0123)", Vector) = (0.02, 0.02, 0.02, 0.02)
		TERRAIN_WaterIBL_SpecWaterStrength0123 ("       IBL spec / Refl - water", Vector) = (1, 1, 1, 1)
		
		TERRAIN_WetSpecularity0123 ("       Wet spec boost (XYZW - 0123)", Vector) = (0.1,0.1,0.1,0.1)
		TERRAIN_WetGloss0123 ("       Wet gloss boost (XYZW - 0123)", Vector) = (0.2,0.2,0.2,0.2)
		
		TERRAIN_WetFlow0123 ("       Wet flow (XYZW - 0123)", Vector) = (1, 1, 1, 1)
		TERRAIN_WetRefraction0123 ("       Wet refraction factor (XYZW - 0123)", Vector) = (0.5, 0.5, 0.5, 0.5)
		TERRAIN_WaterIBL_SpecWetStrength0123 ("       IBL spec / Refl - wet", Vector) = (0.5, 0.5, 0.5, 0.5)
		
		TERRAIN_WaterGlossDamper0123 ("       Hi-freq / distance gloss damper", Vector) = (0, 0, 0, 0)
*/

/*
		// vertical texturing
		//
		_VerticalTexture ("Vertical texture (RGB)", 2D) = "grey" {}
		_VerticalTextureTiling ("       Texture tiling", Float) = 50
		_VerticalTextureGlobalBumpInfluence ("       Perlin distortion", Range(0,0.3)) = 0.01
		_VerticalTexture0123 ("       Strength per layer (XYZW - 0123)", Vector) = (0.5, 0.5, 0.5, 0.5)
*/		

		// snow
		//
		//rtp_snow_strength ("Snow strength", Range(0,1)) = 1 // commented to be controlled globally by ReliefTerrain script
		rtp_snow_strength_per_layer0123 (" Snow strength per layer (XYZW - 0123)", Vector) = (1, 1, 1, 1)
		rtp_global_color_brightness_to_snow ("       Global color brightness to snow", Range(0,1)) = 1
		rtp_snow_slope_factor ("       Slope damp factor", Range(0,4)) = 2
		// in [m] (where snow start to appear)
		//rtp_snow_height_treshold ("       Coverage height theshold", Float) = -100 // commented to be controlled globally by ReliefTerrain script
		//rtp_snow_height_transition ("       Coverage height length", Float) = 300 // commented to be controlled globally by ReliefTerrain script
		rtp_snow_color("       Color", Color) = (0.9,0.9,1,1)
		rtp_snow_specular("       Spec (gloss mask)", Range(0, 1)) = 0.4
		rtp_snow_gloss("       Gloss", Range(0.01,1)) = 0.2
		
		rtp_snow_fresnel("       Fresnel", Range(0.01,1)) = 0.2
		rtp_snow_diff_fresnel("       Diffuse scattering", Range(0,2)) = 0.5
//		rtp_snow_IBL_SpecStrength("       IBL spec / Refl exposure", Range(0,8)) = 0.25
		
		rtp_snow_edge_definition ("       Edges definition", Range(0.25,20)) = 2
		rtp_snow_deep_factor("       Deep factor", Range(0,6)) = 2

/*		
		// caustics
		//
		TERRAIN_CausticsAnimSpeed(" Caustics anim speed", Range(0, 10)) = 2
		TERRAIN_CausticsColor("       Color (RGB)", Color) = (1,1,1,0)
		//TERRAIN_CausticsWaterLevel("       Water Level", Float) = 0 // commented to be controlled globally by ReliefTerrain script
		//TERRAIN_CausticsWaterLevelByAngle("       Water level by slope", Range(0,8)) = 4 // commented to be controlled globally by ReliefTerrain script
		//TERRAIN_CausticsWaterShallowFadeLength("       Shallow fade length", Range(0.1, 10)) = 1 // commented to be controlled globally by ReliefTerrain script
		//TERRAIN_CausticsWaterDeepFadeLength("       Deep fade length", Range(1, 100)) = 20 // commented to be controlled globally by ReliefTerrain script
		TERRAIN_CausticsTilingScale("       Texture tiling", Range(0.5, 4)) = 2
		TERRAIN_CausticsTex("       Caustics texture", 2D) = "black" {}
*/

		// used with VERTEX_COLOR_BLEND define switch below
		//TERRAIN_VertexColorBlend("Color blended by per vertex", Color) = (0.5,0.5,0.5,0)
		
    	// params for underlying terrain (used for global maps and in geom blend mode)
 		_TERRAIN_PosSize ("Terrain pos (xz to XY) & size(xz to ZW)", Vector) = (0,0,1000,1000)
		_TERRAIN_Tiling ("Terrain tiling (XY) & offset(ZW)", Vector) = (3,3,0,0)       
		
		// used for heightblend (BLENDING_HEIGHT define keyword below)
		_TERRAIN_HeightMap ("Terrain HeightMap (combined)", 2D) = "white" {}
		_TERRAIN_Control ("Terrain splat controlMap", 2D) = "black" {}		

	}
	
	SubShader {
		Tags {
			"Queue"="Transparent-100"
			"RenderType" = "Opaque"
		}

		Offset -1,-1
		ZTest LEqual
		LOD 700
		Fog { Mode Off }
		CGPROGRAM
		// add "noforwardadd" below if you agree to compromise additional lighting quality (but with multiple lights in forward we'll have to render in many passes, too)
		// you can also add "noambient" below if you use complementary lighting (or diffuse IBL)
		#pragma surface surf CustomBlinnPhong vertex:vert decal:blend finalcolor:customFog
		// U5 fog handling
		#pragma multi_compile_fog			
		// fullforwardshadows
		#pragma target 3.0
		// target platform renderers (refer to surface shader in Unity docs)
		#pragma exclude_renderers flash
		#pragma glsl

		// switch between shader level of detail using Shader.SetKeyword or using material keyword handling in Unity4.////2+
		#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
		#include "UnityCG.cginc"
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// defines section below allows for shader configuration. These are specific to this triplanar standalone shader
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// self explainable - detail colors 2,3 and bumpmap 23 not used then (shader will run a bit faster)
		// R vertex color used only (1st layer), 2nd layer coverage taken as (1-R)
		//#define USE_2_LAYERS_ONLY
		// as above, but we/re using RGB vertex color channels, A is used for blending
		//#define USE_3_LAYERS_ONLY
		
		// edges can be heightblended with underlying terrain (with one set of four layers - using one splat control map only)
		#define BLENDING_HEIGHT		
		
		// when water or snow is used you can use below defines to specify vertex channel that handles coverage (by default A channel)
		// NOTE that vertex color channel specified interferes with one of the layer splat control (4th by default), so it' only does make sense using with USE_2_LAYERS_ONLY or USE_3_LAYERS_ONLY defines
		//#define VERTEX_COLOR_TO_WATER_COVERAGE IN.color.a
		//#define VERTEX_COLOR_TO_SNOW_COVERAGE IN.color.a
		
		// ambient AO can be taken from arbitrary vertex color channel (you need to use only 3 or 2 usable layers then)
		//#define VERTEX_COLOR_AO_DAMP IN.color.a
		// diffuse color can be also affected by constant color (TERRAIN_VertexColorBlend shader property variable).
		// Level of multiplicative blending is driven from arbitrary vertex color channel defined below
		//#define VERTEX_COLOR_BLEND IN.color.a
		
		// we're texturing in local space
		//#define LOCAL_SPACE_UV
		
		//
		// coverage variants (define only one at time !) taken from normals
		// using one of below variants shader will derive vertex colors (coverage) from mesh normals (local or global)
		// so - you don't have to provide mesh with any mapping or vertex colors, normals are enough
		//
		// forces 3 layers only, side is first layer, top (floor) is 2nd layer, bottom (ceil) is 3rd layer
//		#define WNORMAL_COVERAGE_XZ_Ypos_Yneg
		// forces 3 layers only, X side is first layer, Z side is 2nd, top (floor) + bottom (ceil) is 3rd layer
//		#define WNORMAL_COVERAGE_X_Z_YposYneg
		// forces 4 layers X side is first layer, Z side is 2nd, top (floor) is 3rd, bottom (ceil) is 4th layer
//		#define WNORMAL_COVERAGE_X_Z_Ypos_Yneg
		// forces 2 layers X side is first layer,  top (floor) + bottom (ceil) is 2nd layer
//		#define WNORMAL_COVERAGE_XZ_YposYneg
		
		
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// defines section below allows for  shader configuration. These are regular RTP shader specific defines (the same you'll find in RTP_Base.cginc which are configured by LODmanager)
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// ATLASING to save 3 texture samplers
//#define RTP_USE_COLOR_ATLAS

// uv blending
// IMPORTANT - for UV blend routing look for "UV blend routing defines section" below
#define RTP_UV_BLEND
// blending at far distance only
//#define RTP_DISTANCE_ONLY_UV_BLEND
// usage of normals from blended layer at far distance
#define RTP_NORMALS_FOR_REPLACE_UV_BLEND

// comment below detail when not needed
//#define RTP_SUPER_DETAIL
//#define RTP_SUPER_DTL_MULTS
// comment below if you don't use snow features
#define RTP_SNOW
// layer number taken as snow normal for near distance (for deep snow cover)
//#define RTP_SNW_CHOOSEN_LAYER_NORM_3
// layer number taken as snow color/gloss for near distance
//#define RTP_SNW_CHOOSEN_LAYER_COLOR_3

// heightblend fake AO
//#define RTP_HEIGHTBLEND_AO

//  layer emissiveness
//#define RTP_EMISSION
// when wetness is defined and fuild on surface is emissive we can mod its emisiveness by output normal (wrinkles of flowing "water")
// below define change the way we treat output normals (works fine for "lava" like emissive fuilds)
//#define RTP_FUILD_EMISSION_WRAP
// with optional reafractive distortion to emulate hot air turbulence
//#define RTP_HOTAIR_EMISSION

// define for harder heightblend edges
#define SHARPEN_HEIGHTBLEND_EDGES_PASS1
//#define SHARPEN_HEIGHTBLEND_EDGES_PASS2

// vertical texture
//#define RTP_VERTICAL_TEXTURE

// we use wet (can't be used with superdetail as globalnormal texture BA channels are shared)
//#define RTP_WETNESS
// water droplets
//#define RTP_WET_RIPPLE_TEXTURE
// if defined water won't handle flow nor refractions
//#define SIMPLE_WATER

//#define RTP_CAUSTICS
// when we use caustics and vertical texture - with below defined we will store vertical texture and caustics together (RGB - vertical texture, A - caustics) to save texture sampler
//#define RTP_VERTALPHA_CAUSTICS

// reflection map
//#define RTP_REFLECTION
//#define RTP_ROTATE_REFLECTION

// complementary lights
#define RTP_COMPLEMENTARY_LIGHTS
// complementary lights with spec (active only with above define)
//#define RTP_SPEC_COMPLEMENTARY_LIGHTS

// physically based shading for direct lighting - use fresnel (if you ask - in IBL we use fresnel by default)
// works fine in forward, in deferred it's calculated for one light only (this set via ReliefShader_applyLightForDeferred.cs)
#define RTP_PBL_FRESNEL
// physically based shading - visibility function (enhance a bit specularity)
#define RTP_PBL_VISIBILITY_FUNCTION
// should be left defined unless you're using 3rd party PBL/PBS shading solutions that redefines hidden internal prepass shader for deferred
// (for example if you install Lux open source package - dig forum for more info - you should comment below define)
#define RTP_DEFERRED_PBL_NORMALISATION

// use IBL specular cubemap
// note : IBL diffuse is not available as this requires world normal reconstruction while we'll be lacking interpolators - colors are used for coverage
//#define RTP_IBL_SPEC

// we're working in LINEAR / GAMMA (used in IBL  fresnel , PBL fresnel and gloss calcs)
// if not defined we're rendering in GAMMA
//#define RTP_COLORSPACE_LINEAR

// helper for cross layer specularity / IBL / Refl bleeding
//#define NOSPEC_BLEED

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//

// comment if you don't need global color map
#define COLOR_MAP
// if not defined global color map will be blended (lerp)
#define COLOR_MAP_BLEND_MULTIPLY
// advanced colormap blending per layer (used when COLOR_MAP is defined)
//#define ADV_COLOR_MAP_BLENDING

//
// you can use it to control snow coverage from wet mask (special combined texture channel B)
//#define RTP_SNW_COVERAGE_FROM_WETNESS

// to compute far color basing only on global colormap
//#define SIMPLE_FAR
// global normal map (and we will treat normals from mesh as flat (0,1,0))
//#define RTP_NORMALGLOBAL
// global trees/shadow map - used with Terrain Composer / World Composer by Nathaniel Doldersum
//#define RTP_TREESGLOBAL
// global ambient emissive map
//#define RTP_AMBIENT_EMISSIVE_MAP

//
// DON'T touch defines below !
//
#define OVERWRITE_RTPBASE_DEFINES
#define RTP_SKYSHOP_SYNC
// these are must for standalone shader
#define _4LAYERS
#define VERTEX_COLOR_CONTROL
#define RTP_TRIPLANAR
#define RTP_STANDALONE
#define GEOM_BLEND

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// UV blend routing defines section
//
// DON'T touch defines below... (unless you know exactly what you're doing) - lines 482-497
#if !defined(_4LAYERS) || defined(RTP_USE_COLOR_ATLAS)
	#define UV_BLEND_SRC_0 (tex2Dlod(_SplatAtlasA, float4(uvSplat01M.xy, _MixMipActual.xx)).rgba)
	#define UV_BLEND_SRC_1 (tex2Dlod(_SplatAtlasA, float4(uvSplat01M.zw, _MixMipActual.yy)).rgba)
	#define UV_BLEND_SRC_2 (tex2Dlod(_SplatAtlasA, float4(uvSplat23M.xy, _MixMipActual.zz)).rgba)
	#define UV_BLEND_SRC_3 (tex2Dlod(_SplatAtlasA, float4(uvSplat23M.zw, _MixMipActual.ww)).rgba)
#else
	#define UV_BLEND_SRC_0 (tex2Dlod(_SplatA0, float4(uvSplat01M.xy, _MixMipActual.xx)).rgba)
	#define UV_BLEND_SRC_1 (tex2Dlod(_SplatA1, float4(uvSplat01M.zw, _MixMipActual.yy)).rgba)
	#define UV_BLEND_SRC_2 (tex2Dlod(_SplatA2, float4(uvSplat23M.xy, _MixMipActual.zz)).rgba)
	#define UV_BLEND_SRC_3 (tex2Dlod(_SplatA3, float4(uvSplat23M.zw, _MixMipActual.ww)).rgba)
#endif
#define UV_BLENDMIX_SRC_0 (_MixScale0123.x)
#define UV_BLENDMIX_SRC_1 (_MixScale0123.y)
#define UV_BLENDMIX_SRC_2 (_MixScale0123.z)
#define UV_BLENDMIX_SRC_3 (_MixScale0123.w)

// As we've got defined some shader parts, you can tweak things in following lines
////////////////////////////////////////////////////////////////////////


//
// for example, when you'd like layer 3 to be source for uv blend on layer 0 you'd set it like this:
//   #define UV_BLEND_ROUTE_LAYER_0 UV_BLEND_SRC_3
// HINT: routing one layer into all will boost performance as only 1 additional texture fetch will be performed in shader (instead of up to 8 texture fetches in default setup)
//
#define UV_BLEND_ROUTE_LAYER_0 UV_BLEND_SRC_0
#define UV_BLEND_ROUTE_LAYER_1 UV_BLEND_SRC_1
#define UV_BLEND_ROUTE_LAYER_2 UV_BLEND_SRC_3
#define UV_BLEND_ROUTE_LAYER_3 UV_BLEND_SRC_2
// below routing shiould be exactly the same as above
#define UV_BLENDMIX_ROUTE_LAYER_0 UV_BLENDMIX_SRC_0
#define UV_BLENDMIX_ROUTE_LAYER_1 UV_BLENDMIX_SRC_1
#define UV_BLENDMIX_ROUTE_LAYER_2 UV_BLENDMIX_SRC_3
#define UV_BLENDMIX_ROUTE_LAYER_3 UV_BLENDMIX_SRC_2
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// we're using the same base code for standalone shader as for RTP terrain / mesh
#include "../RTP_Base.cginc"

		ENDCG
	} 
	
	FallBack "Diffuse"
}
