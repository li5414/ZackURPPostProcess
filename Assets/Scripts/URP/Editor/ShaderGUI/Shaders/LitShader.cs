using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.Rendering;

namespace Zack.UniversalRP.ShaderGUI
{
    public struct LitProperties
    {
        // Surface Option Props
        public MaterialProperty workflowMode;

        // Surface Input Props
        public MaterialProperty metallic;
        public MaterialProperty specColor;
        public MaterialProperty metallicGlossMap;
        public MaterialProperty specGlossMap;
        public MaterialProperty smoothness;
        public MaterialProperty smoothnessMapChannel;
        public MaterialProperty bumpMapProp;
        public MaterialProperty bumpScaleProp;
        public MaterialProperty parallaxMapProp;
        public MaterialProperty parallaxScaleProp;
        public MaterialProperty occlusionStrength;
        public MaterialProperty occlusionMap;
        // 受伤特效
        public MaterialProperty useHurt;
        public MaterialProperty hurtMap;
        public MaterialProperty hurtColor;
        public MaterialProperty hurtParameter;

        // Advanced Props
        public MaterialProperty highlights;
        public MaterialProperty reflections;

        public MaterialProperty clearCoat;  // Enable/Disable dummy property
        public MaterialProperty clearCoatMap;
        public MaterialProperty clearCoatMask;
        public MaterialProperty clearCoatSmoothness;

        public LitProperties(MaterialProperty[] properties)
        {
            // Surface Option Props
            workflowMode = BaseShaderGUI.FindProperty("_WorkflowMode", properties, false);
            // Surface Input Props
            metallic = BaseShaderGUI.FindProperty("_Metallic", properties);
            specColor = BaseShaderGUI.FindProperty("_SpecColor", properties, false);
            metallicGlossMap = BaseShaderGUI.FindProperty("_MetallicGlossMap", properties);
            specGlossMap = BaseShaderGUI.FindProperty("_SpecGlossMap", properties, false);
            smoothness = BaseShaderGUI.FindProperty("_Smoothness", properties, false);
            smoothnessMapChannel = BaseShaderGUI.FindProperty("_SmoothnessTextureChannel", properties, false);
            bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
            bumpScaleProp = BaseShaderGUI.FindProperty("_BumpScale", properties, false);
            parallaxMapProp = BaseShaderGUI.FindProperty("_ParallaxMap", properties, false);
            parallaxScaleProp = BaseShaderGUI.FindProperty("_Parallax", properties, false);
            occlusionStrength = BaseShaderGUI.FindProperty("_OcclusionStrength", properties, false);
            occlusionMap = BaseShaderGUI.FindProperty("_OcclusionMap", properties, false);
            // 受伤特效
            useHurt = BaseShaderGUI.FindProperty("_UseHurt", properties, false);
            hurtMap = BaseShaderGUI.FindProperty("_HurtMap", properties, false);
            hurtColor = BaseShaderGUI.FindProperty("_HurtColor", properties, false);
            hurtParameter = BaseShaderGUI.FindProperty("_HurtParameter", properties, false);
            // Advanced Props
            highlights = BaseShaderGUI.FindProperty("_SpecularHighlights", properties, false);
            reflections = BaseShaderGUI.FindProperty("_EnvironmentReflections", properties, false);

            clearCoat           = BaseShaderGUI.FindProperty("_ClearCoat", properties, false);
            clearCoatMap        = BaseShaderGUI.FindProperty("_ClearCoatMap", properties, false);
            clearCoatMask       = BaseShaderGUI.FindProperty("_ClearCoatMask", properties, false);
            clearCoatSmoothness = BaseShaderGUI.FindProperty("_ClearCoatSmoothness", properties, false);
        }
    }

    internal class LitShader : BaseShaderGUI
    {
        static GUIContent Style_OcclusionText = new GUIContent("Occlusion Map",
            "Sets an occlusion map to simulate shadowing from ambient lighting.");
        static GUIContent Style_SpecularMapText =
            new GUIContent("Specular Map", "Sets and configures the map and color for the Specular workflow.");
        static GUIContent Style_MetallicMapText =
            new GUIContent("Metallic Map", "Sets and configures the map for the Metallic workflow.");
        static GUIContent Style_SmoothnessText = new GUIContent("Smoothness",
            "Controls the spread of highlights and reflections on the surface.");
        static GUIContent Style_HurtMapText =
            new GUIContent("Hurt Map", "Sets and configures the map for the Hurt Effect.");
        static GUIContent Style_UseHurtText =
            new GUIContent("Use Hurt", "Makes your Material has a hurt effect.");

        
        private LitProperties litProperties;

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitProperties(properties);
        }
        
        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            SetMaterialKeywords(material, SetMaterialKeywords);
        }
        
        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            if (litProperties.workflowMode != null)
            {
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, Enum.GetNames(typeof(LitGUI.WorkflowMode)));
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
            base.DrawSurfaceOptions(material);
            
            // 受伤特效
            bool useHurtEnabled = EditorGUILayout.Toggle(Style_UseHurtText, litProperties.useHurt.floatValue == 1);
            litProperties.useHurt.floatValue = useHurtEnabled ? 1 : 0;
        }
        
        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            LitProperties properties = litProperties;
            
            EditorGUI.BeginChangeCheck();

            // _BaseMap
            DrawBaseProperties(material);
            // _BumpMap
            DrawNormalArea(materialEditor, litProperties.bumpMapProp, litProperties.bumpScaleProp);
            // _OcclusionMap
            if (properties.occlusionMap != null)
            {
                materialEditor.TexturePropertySingleLine(Style_OcclusionText, properties.occlusionMap,
                    properties.occlusionMap.textureValue != null ? properties.occlusionStrength : null);
            }
            
            bool hasGlossMap = false;
            if (properties.workflowMode == null ||
                (LitGUI.WorkflowMode) properties.workflowMode.floatValue == LitGUI.WorkflowMode.Metallic)
            {
                // _MetallicGlossMap 或 _Metallic
                hasGlossMap = properties.metallicGlossMap.textureValue != null;
                materialEditor.TexturePropertySingleLine(Style_MetallicMapText, properties.metallicGlossMap,
                    hasGlossMap ? null : properties.metallic);
            }
            else
            {
                hasGlossMap = properties.specGlossMap.textureValue != null;
                BaseShaderGUI.TextureColorProps(materialEditor, Style_SpecularMapText, properties.specGlossMap,
                    hasGlossMap ? null : properties.specColor);
            }
            // _Smoothness
            properties.smoothness.floatValue = EditorGUILayout.Slider(Style_SmoothnessText, properties.smoothness.floatValue, 0f, 1f);
            // 受伤特效
            if (properties.useHurt.floatValue == 1)
            {
                // _HurtMap
                TextureColorProps(materialEditor, Style_HurtMapText, properties.hurtMap, properties.hurtColor);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged((Material)material);
            }
        }
        
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }

            MaterialChanged(material);
        }
        
        static void SetMaterialKeywords(Material material)
        {
            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)
            var hasGlossMap = false;
            var isSpecularWorkFlow = false;
            var opaque = ((BaseShaderGUI.SurfaceType) material.GetFloat("_Surface") ==
                          BaseShaderGUI.SurfaceType.Opaque);
            if (material.HasProperty("_WorkflowMode"))
            {
                isSpecularWorkFlow = (LitGUI.WorkflowMode) material.GetFloat("_WorkflowMode") == LitGUI.WorkflowMode.Specular;
                if (isSpecularWorkFlow)
                    hasGlossMap = material.GetTexture("_SpecGlossMap") != null;
                else
                    hasGlossMap = material.GetTexture("_MetallicGlossMap") != null;
            }
            else
            {
                hasGlossMap = material.GetTexture("_MetallicGlossMap") != null;
            }

            CoreUtils.SetKeyword(material, "_SPECULAR_SETUP", isSpecularWorkFlow);

            CoreUtils.SetKeyword(material, "_METALLICSPECGLOSSMAP", hasGlossMap);

            if (material.HasProperty("_SpecularHighlights"))
                CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF",
                    material.GetFloat("_SpecularHighlights") == 0.0f);
            if (material.HasProperty("_EnvironmentReflections"))
                CoreUtils.SetKeyword(material, "_ENVIRONMENTREFLECTIONS_OFF",
                    material.GetFloat("_EnvironmentReflections") == 0.0f);
            if (material.HasProperty("_OcclusionMap"))
                CoreUtils.SetKeyword(material, "_OCCLUSIONMAP", material.GetTexture("_OcclusionMap"));

            if (material.HasProperty("_ParallaxMap"))
                CoreUtils.SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));

            if (material.HasProperty("_SmoothnessTextureChannel"))
            {
                CoreUtils.SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
                    LitGUI.GetSmoothnessMapChannel(material) == LitGUI.SmoothnessMapChannel.AlbedoAlpha && opaque);
            }

            // 受伤特效
            if (material.HasProperty("_UseHurt"))
            {
                if (material.GetFloat("_UseHurt") == 1)
                {
                    CoreUtils.SetKeyword(material, "_USE_HURT", material.GetTexture("_HurtMap"));
                }
                else
                {
                    CoreUtils.SetKeyword(material, "_USE_HURT", false);
                }
            }

        }
        
    }
}

