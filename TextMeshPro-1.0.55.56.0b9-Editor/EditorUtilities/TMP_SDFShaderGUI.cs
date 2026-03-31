using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public class TMP_SDFShaderGUI : TMP_BaseShaderGUI
	{
		static TMP_SDFShaderGUI()
		{
			TMP_SDFShaderGUI.facePanel = new TMP_BaseShaderGUI.MaterialPanel("Face", true);
			TMP_SDFShaderGUI.outlinePanel = new TMP_BaseShaderGUI.MaterialPanel("Outline", true);
			TMP_SDFShaderGUI.underlayPanel = new TMP_BaseShaderGUI.MaterialPanel("Underlay", false);
			TMP_SDFShaderGUI.bevelPanel = new TMP_BaseShaderGUI.MaterialPanel("Bevel", false);
			TMP_SDFShaderGUI.lightingPanel = new TMP_BaseShaderGUI.MaterialPanel("Lighting", false);
			TMP_SDFShaderGUI.bumpMapPanel = new TMP_BaseShaderGUI.MaterialPanel("BumpMap", false);
			TMP_SDFShaderGUI.envMapPanel = new TMP_BaseShaderGUI.MaterialPanel("EnvMap", false);
			TMP_SDFShaderGUI.glowPanel = new TMP_BaseShaderGUI.MaterialPanel("Glow", false);
			TMP_SDFShaderGUI.debugPanel = new TMP_BaseShaderGUI.MaterialPanel("Debug", false);
			TMP_SDFShaderGUI.outlineFeature = new TMP_BaseShaderGUI.ShaderFeature
			{
				undoLabel = "Outline",
				keywords = new string[]
				{
					"OUTLINE_ON"
				}
			};
			TMP_SDFShaderGUI.underlayFeature = new TMP_BaseShaderGUI.ShaderFeature
			{
				undoLabel = "Underlay",
				keywords = new string[]
				{
					"UNDERLAY_ON",
					"UNDERLAY_INNER"
				},
				label = new GUIContent("Underlay Type"),
				keywordLabels = new GUIContent[]
				{
					new GUIContent("None"),
					new GUIContent("Normal"),
					new GUIContent("Inner")
				}
			};
			TMP_SDFShaderGUI.bevelFeature = new TMP_BaseShaderGUI.ShaderFeature
			{
				undoLabel = "Bevel",
				keywords = new string[]
				{
					"BEVEL_ON"
				}
			};
			TMP_SDFShaderGUI.glowFeature = new TMP_BaseShaderGUI.ShaderFeature
			{
				undoLabel = "Glow",
				keywords = new string[]
				{
					"GLOW_ON"
				}
			};
			TMP_SDFShaderGUI.maskFeature = new TMP_BaseShaderGUI.ShaderFeature
			{
				undoLabel = "Mask",
				keywords = new string[]
				{
					"MASK_HARD",
					"MASK_SOFT"
				},
				label = new GUIContent("Mask"),
				keywordLabels = new GUIContent[]
				{
					new GUIContent("Mask Off"),
					new GUIContent("Mask Hard"),
					new GUIContent("Mask Soft")
				}
			};
		}

		protected override void DoGUI()
		{
			if (base.DoPanelHeader(TMP_SDFShaderGUI.facePanel))
			{
				this.DoFacePanel();
			}
			if ((!this.material.HasProperty(ShaderUtilities.ID_OutlineTex)) ? base.DoPanelHeader(TMP_SDFShaderGUI.outlinePanel, TMP_SDFShaderGUI.outlineFeature, true) : base.DoPanelHeader(TMP_SDFShaderGUI.outlinePanel))
			{
				this.DoOutlinePanel();
			}
			if (this.material.HasProperty(ShaderUtilities.ID_UnderlayColor) && base.DoPanelHeader(TMP_SDFShaderGUI.underlayPanel, TMP_SDFShaderGUI.underlayFeature, true))
			{
				this.DoUnderlayPanel();
			}
			if (this.material.HasProperty("_SpecularColor"))
			{
				if (base.DoPanelHeader(TMP_SDFShaderGUI.bevelPanel, TMP_SDFShaderGUI.bevelFeature, true))
				{
					this.DoBevelPanel();
				}
				if (base.DoPanelHeader(TMP_SDFShaderGUI.lightingPanel, TMP_SDFShaderGUI.bevelFeature, false))
				{
					this.DoLocalLightingPanel();
				}
				if (base.DoPanelHeader(TMP_SDFShaderGUI.bumpMapPanel, TMP_SDFShaderGUI.bevelFeature, false))
				{
					this.DoBumpMapPanel();
				}
				if (base.DoPanelHeader(TMP_SDFShaderGUI.envMapPanel, TMP_SDFShaderGUI.bevelFeature, false))
				{
					this.DoEnvMapPanel();
				}
			}
			else if (this.material.HasProperty("_SpecColor"))
			{
				if (base.DoPanelHeader(TMP_SDFShaderGUI.bevelPanel))
				{
					this.DoBevelPanel();
				}
				if (base.DoPanelHeader(TMP_SDFShaderGUI.lightingPanel))
				{
					this.DoSurfaceLightingPanel();
				}
				if (base.DoPanelHeader(TMP_SDFShaderGUI.bumpMapPanel))
				{
					this.DoBumpMapPanel();
				}
				if (base.DoPanelHeader(TMP_SDFShaderGUI.envMapPanel))
				{
					this.DoEnvMapPanel();
				}
			}
			if (this.material.HasProperty(ShaderUtilities.ID_GlowColor) && base.DoPanelHeader(TMP_SDFShaderGUI.glowPanel, TMP_SDFShaderGUI.glowFeature, true))
			{
				this.DoGlowPanel();
			}
			if (base.DoPanelHeader(TMP_SDFShaderGUI.debugPanel))
			{
				this.DoDebugPanel();
			}
		}

		private void DoFacePanel()
		{
			EditorGUI.indentLevel++;
			base.DoColor("_FaceColor", "Color");
			if (this.material.HasProperty(ShaderUtilities.ID_FaceTex))
			{
				if (this.material.HasProperty("_FaceUVSpeedX"))
				{
					base.DoTexture2D("_FaceTex", "Texture", true, TMP_SDFShaderGUI.faceUVSpeedNames);
				}
				else
				{
					base.DoTexture2D("_FaceTex", "Texture", true, null);
				}
			}
			base.DoSlider("_OutlineSoftness", "Softness");
			base.DoSlider("_FaceDilate", "Dilate");
			if (this.material.HasProperty(ShaderUtilities.ID_Shininess))
			{
				base.DoSlider("_FaceShininess", "Gloss");
			}
			EditorGUI.indentLevel--;
		}

		private void DoOutlinePanel()
		{
			EditorGUI.indentLevel++;
			base.DoColor("_OutlineColor", "Color");
			if (this.material.HasProperty(ShaderUtilities.ID_OutlineTex))
			{
				if (this.material.HasProperty("_OutlineUVSpeedX"))
				{
					base.DoTexture2D("_OutlineTex", "Texture", true, TMP_SDFShaderGUI.outlineUVSpeedNames);
				}
				else
				{
					base.DoTexture2D("_OutlineTex", "Texture", true, null);
				}
			}
			base.DoSlider("_OutlineWidth", "Thickness");
			if (this.material.HasProperty("_OutlineShininess"))
			{
				base.DoSlider("_OutlineShininess", "Gloss");
			}
			EditorGUI.indentLevel--;
		}

		private void DoUnderlayPanel()
		{
			EditorGUI.indentLevel++;
			TMP_SDFShaderGUI.underlayFeature.DoPopup(this.editor, this.material);
			base.DoColor("_UnderlayColor", "Color");
			base.DoSlider("_UnderlayOffsetX", "Offset X");
			base.DoSlider("_UnderlayOffsetY", "Offset Y");
			base.DoSlider("_UnderlayDilate", "Dilate");
			base.DoSlider("_UnderlaySoftness", "Softness");
			EditorGUI.indentLevel--;
		}

		private void DoBevelPanel()
		{
			EditorGUI.indentLevel++;
			base.DoPopup("_ShaderFlags", "Type", TMP_SDFShaderGUI.bevelTypeLabels);
			base.DoSlider("_Bevel", "Amount");
			base.DoSlider("_BevelOffset", "Offset");
			base.DoSlider("_BevelWidth", "Width");
			base.DoSlider("_BevelRoundness", "Roundness");
			base.DoSlider("_BevelClamp", "Clamp");
			EditorGUI.indentLevel--;
		}

		private void DoLocalLightingPanel()
		{
			EditorGUI.indentLevel++;
			base.DoSlider("_LightAngle", "Light Angle");
			base.DoColor("_SpecularColor", "Specular Color");
			base.DoSlider("_SpecularPower", "Specular Power");
			base.DoSlider("_Reflectivity", "Reflectivity Power");
			base.DoSlider("_Diffuse", "Diffuse Shadow");
			base.DoSlider("_Ambient", "Ambient Shadow");
			EditorGUI.indentLevel--;
		}

		private void DoSurfaceLightingPanel()
		{
			EditorGUI.indentLevel++;
			base.DoColor("_SpecColor", "Specular Color");
			EditorGUI.indentLevel--;
		}

		private void DoBumpMapPanel()
		{
			EditorGUI.indentLevel++;
			base.DoTexture2D("_BumpMap", "Texture", false, null);
			base.DoSlider("_BumpFace", "Face");
			base.DoSlider("_BumpOutline", "Outline");
			EditorGUI.indentLevel--;
		}

		private void DoEnvMapPanel()
		{
			EditorGUI.indentLevel++;
			base.DoColor("_ReflectFaceColor", "Face Color");
			base.DoColor("_ReflectOutlineColor", "Outline Color");
			base.DoCubeMap("_Cube", "Texture");
			base.DoVector3("_EnvMatrixRotation", "EnvMap Rotation");
			EditorGUI.indentLevel--;
		}

		private void DoGlowPanel()
		{
			EditorGUI.indentLevel++;
			base.DoColor("_GlowColor", "Color");
			base.DoSlider("_GlowOffset", "Offset");
			base.DoSlider("_GlowInner", "Inner");
			base.DoSlider("_GlowOuter", "Outer");
			base.DoSlider("_GlowPower", "Power");
			EditorGUI.indentLevel--;
		}

		private void DoDebugPanel()
		{
			EditorGUI.indentLevel++;
			base.DoTexture2D("_MainTex", "Font Atlas", false, null);
			base.DoFloat("_GradientScale", "Gradient Scale");
			base.DoFloat("_TextureWidth", "Texture Width");
			base.DoFloat("_TextureHeight", "Texture Height");
			base.DoEmptyLine();
			base.DoFloat("_ScaleX", "Scale X");
			base.DoFloat("_ScaleY", "Scale Y");
			base.DoSlider("_PerspectiveFilter", "Perspective Filter");
			base.DoEmptyLine();
			base.DoFloat("_VertexOffsetX", "Offset X");
			base.DoFloat("_VertexOffsetY", "Offset Y");
			if (this.material.HasProperty(ShaderUtilities.ID_MaskCoord))
			{
				base.DoEmptyLine();
				TMP_SDFShaderGUI.maskFeature.ReadState(this.material);
				TMP_SDFShaderGUI.maskFeature.DoPopup(this.editor, this.material);
				if (TMP_SDFShaderGUI.maskFeature.Active)
				{
					this.DoMaskSubgroup();
				}
				base.DoEmptyLine();
				base.DoVector("_ClipRect", "Clip Rect", TMP_BaseShaderGUI.lbrtVectorLabels);
			}
			else if (this.material.HasProperty("_MaskTex"))
			{
				this.DoMaskTexSubgroup();
			}
			else if (this.material.HasProperty(ShaderUtilities.ID_MaskSoftnessX))
			{
				base.DoEmptyLine();
				base.DoFloat("_MaskSoftnessX", "Softness X");
				base.DoFloat("_MaskSoftnessY", "Softness Y");
				base.DoVector("_ClipRect", "Clip Rect", TMP_BaseShaderGUI.lbrtVectorLabels);
			}
			if (this.material.HasProperty(ShaderUtilities.ID_StencilID))
			{
				base.DoEmptyLine();
				base.DoFloat("_Stencil", "Stencil ID");
				base.DoFloat("_StencilComp", "Stencil Comp");
			}
			base.DoEmptyLine();
			EditorGUI.BeginChangeCheck();
			bool flag = EditorGUILayout.Toggle("Use Ratios?", !this.material.IsKeywordEnabled("RATIOS_OFF"), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.editor.RegisterPropertyChangeUndo("Use Ratios");
				if (flag)
				{
					this.material.DisableKeyword("RATIOS_OFF");
				}
				else
				{
					this.material.EnableKeyword("RATIOS_OFF");
				}
			}
			EditorGUI.BeginDisabledGroup(true);
			base.DoFloat("_ScaleRatioA", "Scale Ratio A");
			base.DoFloat("_ScaleRatioB", "Scale Ratio B");
			base.DoFloat("_ScaleRatioC", "Scale Ratio C");
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
		}

		private void DoMaskSubgroup()
		{
			base.DoVector("_MaskCoord", "Mask Bounds", TMP_BaseShaderGUI.xywhVectorLabels);
			if (Selection.activeGameObject != null)
			{
				Renderer component = Selection.activeGameObject.GetComponent<Renderer>();
				if (component != null)
				{
					Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
					controlRect.x += EditorGUIUtility.labelWidth;
					controlRect.width -= EditorGUIUtility.labelWidth;
					if (GUI.Button(controlRect, "Match Renderer Bounds"))
					{
						ShaderGUI.FindProperty("_MaskCoord", this.properties).vectorValue = new Vector4(0f, 0f, Mathf.Round(component.bounds.extents.x * 1000f) / 1000f, Mathf.Round(component.bounds.extents.y * 1000f) / 1000f);
					}
				}
			}
			if (TMP_SDFShaderGUI.maskFeature.State == 1)
			{
				base.DoFloat("_MaskSoftnessX", "Softness X");
				base.DoFloat("_MaskSoftnessY", "Softness Y");
			}
		}

		private void DoMaskTexSubgroup()
		{
			base.DoEmptyLine();
			base.DoTexture2D("_MaskTex", "Mask Texture", false, null);
			base.DoToggle("_MaskInverse", "Inverse Mask");
			base.DoColor("_MaskEdgeColor", "Edge Color");
			base.DoSlider("_MaskEdgeSoftness", "Edge Softness");
			base.DoSlider("_MaskWipeControl", "Wipe Position");
			base.DoFloat("_MaskSoftnessX", "Softness X");
			base.DoFloat("_MaskSoftnessY", "Softness Y");
			base.DoVector("_ClipRect", "Clip Rect", TMP_BaseShaderGUI.lbrtVectorLabels);
		}

		private static TMP_BaseShaderGUI.MaterialPanel facePanel;

		private static TMP_BaseShaderGUI.MaterialPanel outlinePanel;

		private static TMP_BaseShaderGUI.MaterialPanel underlayPanel;

		private static TMP_BaseShaderGUI.MaterialPanel bevelPanel;

		private static TMP_BaseShaderGUI.MaterialPanel lightingPanel;

		private static TMP_BaseShaderGUI.MaterialPanel bumpMapPanel;

		private static TMP_BaseShaderGUI.MaterialPanel envMapPanel;

		private static TMP_BaseShaderGUI.MaterialPanel glowPanel;

		private static TMP_BaseShaderGUI.MaterialPanel debugPanel;

		private static TMP_BaseShaderGUI.ShaderFeature outlineFeature;

		private static TMP_BaseShaderGUI.ShaderFeature underlayFeature;

		private static TMP_BaseShaderGUI.ShaderFeature bevelFeature;

		private static TMP_BaseShaderGUI.ShaderFeature glowFeature;

		private static TMP_BaseShaderGUI.ShaderFeature maskFeature;

		private static string[] faceUVSpeedNames = new string[]
		{
			"_FaceUVSpeedX",
			"_FaceUVSpeedY"
		};

		private static string[] outlineUVSpeedNames = new string[]
		{
			"_OutlineUVSpeedX",
			"_OutlineUVSpeedY"
		};

		private static GUIContent[] bevelTypeLabels = new GUIContent[]
		{
			new GUIContent("Outer Bevel"),
			new GUIContent("Inner Bevel")
		};
	}
}
