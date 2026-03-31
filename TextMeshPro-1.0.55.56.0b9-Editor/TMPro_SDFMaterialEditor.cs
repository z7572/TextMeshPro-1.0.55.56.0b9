using System;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class TMPro_SDFMaterialEditor : MaterialEditor
{
	public override void OnEnable()
	{
		base.OnEnable();
		TMP_UIStyleManager.GetUIStyles();
		ShaderUtilities.GetShaderPropertyIDs();
		Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
		if (Selection.activeGameObject != null)
		{
			this.m_textComponent = Selection.activeGameObject.GetComponent<TMP_Text>();
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
	}

	protected override void OnHeaderGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnHeaderGUI();
		if (EditorGUI.EndChangeCheck())
		{
			TMPro_SDFMaterialEditor.m_foldout.editorPanel = InternalEditorUtility.GetIsInspectorExpanded(base.target);
		}
		GUI.skin.GetStyle("HelpBox").richText = true;
		TMPro_SDFMaterialEditor.WarningTypes warning = this.m_warning;
		if (warning == TMPro_SDFMaterialEditor.WarningTypes.FontAtlasMismatch)
		{
			EditorGUILayout.HelpBox(this.m_warningMsg, MessageType.Warning);
		}
	}

	public override void OnInspectorGUI()
	{
		if (!TMPro_SDFMaterialEditor.m_foldout.editorPanel)
		{
			return;
		}
		Material material = base.target as Material;
		if (base.targets.Length > 1)
		{
			for (int i = 0; i < base.targets.Length; i++)
			{
				Material material2 = base.targets[i] as Material;
				if (material.shader != material2.shader)
				{
					return;
				}
			}
		}
		this.ReadMaterialProperties();
		if (!material.HasProperty(ShaderUtilities.ID_GradientScale))
		{
			this.m_warning = TMPro_SDFMaterialEditor.WarningTypes.ShaderMismatch;
			this.m_warningMsg = "The selected Shader is not compatible with the currently selected Font Asset type.";
			EditorGUILayout.HelpBox(this.m_warningMsg, MessageType.Warning);
			return;
		}
		this.m_Keywords = material.shaderKeywords;
		this.isOutlineEnabled = this.m_Keywords.Contains("OUTLINE_ON");
		this.isBevelEnabled = this.m_Keywords.Contains("BEVEL_ON");
		this.isGlowEnabled = this.m_Keywords.Contains("GLOW_ON");
		this.isRatiosEnabled = !this.m_Keywords.Contains("RATIOS_OFF");
		if (this.m_Keywords.Contains("UNDERLAY_ON"))
		{
			this.isUnderlayEnabled = true;
			this.m_underlaySelection = TMPro_SDFMaterialEditor.Underlay_Types.Normal;
		}
		else if (this.m_Keywords.Contains("UNDERLAY_INNER"))
		{
			this.isUnderlayEnabled = true;
			this.m_underlaySelection = TMPro_SDFMaterialEditor.Underlay_Types.Inner;
		}
		else
		{
			this.isUnderlayEnabled = false;
		}
		if (this.m_Keywords.Contains("MASK_HARD"))
		{
			this.m_mask = MaskingTypes.MaskHard;
		}
		else if (this.m_Keywords.Contains("MASK_SOFT"))
		{
			this.m_mask = MaskingTypes.MaskSoft;
		}
		else
		{
			this.m_mask = MaskingTypes.MaskOff;
		}
		if (this.m_shaderFlags.hasMixedValue)
		{
			this.m_bevelSelection = 2;
		}
		else
		{
			this.m_bevelSelection = ((int)this.m_shaderFlags.floatValue & 1);
		}
		this.m_inspectorStartRegion = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true)
		});
		EditorGUIUtility.labelWidth = 130f;
		EditorGUIUtility.fieldWidth = 50f;
		EditorGUI.indentLevel = 0;
		if (GUILayout.Button("<b>Face</b> - <i>Settings</i> -", TMP_UIStyleManager.Group_Label, new GUILayoutOption[0]))
		{
			TMPro_SDFMaterialEditor.m_foldout.face = !TMPro_SDFMaterialEditor.m_foldout.face;
		}
		if (TMPro_SDFMaterialEditor.m_foldout.face)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel = 1;
			base.ColorProperty(this.m_faceColor, "Color");
			if (material.HasProperty("_FaceTex"))
			{
				this.DrawTextureProperty(this.m_faceTex, "Texture");
				this.DrawUVProperty(new MaterialProperty[]
				{
					this.m_faceUVSpeedX,
					this.m_faceUVSpeedY
				}, "UV Speed");
			}
			this.DrawRangeProperty(this.m_outlineSoftness, "Softness");
			this.DrawRangeProperty(this.m_faceDilate, "Dilate");
			if (material.HasProperty("_FaceShininess"))
			{
				this.DrawRangeProperty(this.m_faceShininess, "Gloss");
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.havePropertiesChanged = true;
			}
		}
		if (material.HasProperty("_OutlineColor"))
		{
			if (material.HasProperty("_Bevel"))
			{
				if (GUILayout.Button("<b>Outline</b> - <i>Settings</i> -", TMP_UIStyleManager.Group_Label, new GUILayoutOption[0]))
				{
					TMPro_SDFMaterialEditor.m_foldout.outline = !TMPro_SDFMaterialEditor.m_foldout.outline;
				}
			}
			else
			{
				this.isOutlineEnabled = this.DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType.outline, "<b>Outline</b> - <i>Settings</i> -", this.isOutlineEnabled, "OUTLINE_ON");
			}
			EditorGUI.indentLevel = 0;
			if (TMPro_SDFMaterialEditor.m_foldout.outline)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.indentLevel = 1;
				base.ColorProperty(this.m_outlineColor, "Color");
				if (material.HasProperty("_OutlineTex"))
				{
					this.DrawTextureProperty(this.m_outlineTex, "Texture");
					this.DrawUVProperty(new MaterialProperty[]
					{
						this.m_outlineUVSpeedX,
						this.m_outlineUVSpeedY
					}, "UV Speed");
				}
				this.DrawRangeProperty(this.m_outlineThickness, "Thickness");
				if (material.HasProperty("_OutlineShininess"))
				{
					this.DrawRangeProperty(this.m_outlineShininess, "Gloss");
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		if (material.HasProperty("_UnderlayColor"))
		{
			string keyword = (this.m_underlaySelection != TMPro_SDFMaterialEditor.Underlay_Types.Normal) ? "UNDERLAY_INNER" : "UNDERLAY_ON";
			this.isUnderlayEnabled = this.DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType.underlay, "<b>Underlay</b> - <i>Settings</i> -", this.isUnderlayEnabled, keyword);
			if (TMPro_SDFMaterialEditor.m_foldout.underlay)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.indentLevel = 1;
				this.m_underlaySelection = (TMPro_SDFMaterialEditor.Underlay_Types)EditorGUILayout.EnumPopup("Underlay Type", this.m_underlaySelection, new GUILayoutOption[0]);
				if (GUI.changed)
				{
					this.SetUnderlayKeywords();
				}
				base.ColorProperty(this.m_underlayColor, "Color");
				this.DrawRangeProperty(this.m_underlayOffsetX, "OffsetX");
				this.DrawRangeProperty(this.m_underlayOffsetY, "OffsetY");
				this.DrawRangeProperty(this.m_underlayDilate, "Dilate");
				this.DrawRangeProperty(this.m_underlaySoftness, "Softness");
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		if (material.HasProperty("_Bevel"))
		{
			this.isBevelEnabled = this.DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType.bevel, "<b>Bevel</b> - <i>Settings</i> -", this.isBevelEnabled, "BEVEL_ON");
			if (TMPro_SDFMaterialEditor.m_foldout.bevel)
			{
				EditorGUI.indentLevel = 1;
				GUI.changed = false;
				this.m_bevelSelection = (EditorGUILayout.Popup("Type", this.m_bevelSelection, this.m_bevelOptions, new GUILayoutOption[0]) & 1);
				if (GUI.changed)
				{
					this.havePropertiesChanged = true;
					this.m_shaderFlags.floatValue = (float)this.m_bevelSelection;
				}
				EditorGUI.BeginChangeCheck();
				this.DrawRangeProperty(this.m_bevel, "Amount");
				this.DrawRangeProperty(this.m_bevelOffset, "Offset");
				this.DrawRangeProperty(this.m_bevelWidth, "Width");
				this.DrawRangeProperty(this.m_bevelRoundness, "Roundness");
				this.DrawRangeProperty(this.m_bevelClamp, "Clamp");
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		if (material.HasProperty("_SpecularColor") || material.HasProperty("_SpecColor"))
		{
			this.isBevelEnabled = this.DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType.light, "<b>Lighting</b> - <i>Settings</i> -", this.isBevelEnabled, "BEVEL_ON");
			if (TMPro_SDFMaterialEditor.m_foldout.light)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.indentLevel = 1;
				if (material.HasProperty("_LightAngle"))
				{
					this.DrawRangeProperty(this.m_lightAngle, "Light Angle");
					base.ColorProperty(this.m_specularColor, "Specular Color");
					this.DrawRangeProperty(this.m_specularPower, "Specular Power");
					this.DrawRangeProperty(this.m_reflectivity, "Reflectivity Power");
					this.DrawRangeProperty(this.m_diffuse, "Diffuse Shadow");
					this.DrawRangeProperty(this.m_ambientLight, "Ambient Shadow");
				}
				else
				{
					base.ColorProperty(this.m_specColor, "Specular Color");
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		if (material.HasProperty("_BumpMap"))
		{
			this.isBevelEnabled = this.DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType.bump, "<b>BumpMap</b> - <i>Settings</i> -", this.isBevelEnabled, "BEVEL_ON");
			if (TMPro_SDFMaterialEditor.m_foldout.bump)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.indentLevel = 1;
				this.DrawTextureProperty(this.m_bumpMap, "Texture");
				this.DrawRangeProperty(this.m_bumpFace, "Face");
				this.DrawRangeProperty(this.m_bumpOutline, "Outline");
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		if (material.HasProperty("_Cube"))
		{
			this.isBevelEnabled = this.DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType.env, "<b>EnvMap</b> - <i>Settings</i> -", this.isBevelEnabled, "BEVEL_ON");
			if (TMPro_SDFMaterialEditor.m_foldout.env)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.indentLevel = 1;
				base.ColorProperty(this.m_reflectFaceColor, "Face Color");
				base.ColorProperty(this.m_reflectOutlineColor, "Outline Color");
				this.DrawTextureProperty(this.m_reflectTex, "Texture");
				if (material.HasProperty("_Cube"))
				{
					this.DrawVectorProperty(this.m_reflectRotation, "EnvMap Rotation");
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		if (material.HasProperty("_GlowColor"))
		{
			this.isGlowEnabled = this.DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType.glow, "<b>Glow</b> - <i>Settings</i> -", this.isGlowEnabled, "GLOW_ON");
			if (TMPro_SDFMaterialEditor.m_foldout.glow)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.indentLevel = 1;
				base.ColorProperty(this.m_glowColor, "Color");
				this.DrawRangeProperty(this.m_glowOffset, "Offset");
				this.DrawRangeProperty(this.m_glowInner, "Inner");
				this.DrawRangeProperty(this.m_glowOuter, "Outer");
				this.DrawRangeProperty(this.m_glowPower, "Power");
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		if (material.HasProperty("_GradientScale"))
		{
			EditorGUI.indentLevel = 0;
			if (GUILayout.Button("<b>Debug</b> - <i>Settings</i> -", TMP_UIStyleManager.Group_Label, new GUILayoutOption[0]))
			{
				TMPro_SDFMaterialEditor.m_foldout.debug = !TMPro_SDFMaterialEditor.m_foldout.debug;
			}
			if (TMPro_SDFMaterialEditor.m_foldout.debug)
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				this.DrawTextureProperty(this.m_mainTex, "Font Atlas");
				this.DrawFloatProperty(this.m_gradientScale, "Gradient Scale");
				this.DrawFloatProperty(this.m_texSampleWidth, "Texture Width");
				this.DrawFloatProperty(this.m_texSampleHeight, "Texture Height");
				GUILayout.Space(20f);
				this.DrawFloatProperty(this.m_scaleX, "Scale X");
				this.DrawFloatProperty(this.m_scaleY, "Scale Y");
				this.DrawRangeProperty(this.m_PerspectiveFilter, "Perspective Filter");
				GUILayout.Space(20f);
				this.DrawFloatProperty(this.m_vertexOffsetX, "Offset X");
				this.DrawFloatProperty(this.m_vertexOffsetY, "Offset Y");
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				if (material.HasProperty("_ClipRect"))
				{
					GUILayout.Space(15f);
					this.m_mask = (MaskingTypes)EditorGUILayout.EnumPopup("Mask", this.m_mask, new GUILayoutOption[0]);
					if (GUI.changed)
					{
						this.havePropertiesChanged = true;
						this.SetMaskKeywords(this.m_mask);
					}
					if (this.m_mask != MaskingTypes.MaskOff)
					{
						EditorGUI.BeginChangeCheck();
						this.Draw2DBoundsProperty(this.m_maskCoord, "Mask Bounds");
						this.DrawFloatProperty(this.m_maskSoftnessX, "Softness X");
						this.DrawFloatProperty(this.m_maskSoftnessY, "Softness Y");
						if (material.HasProperty("_MaskEdgeColor"))
						{
							this.DrawTextureProperty(this.m_maskTex, "Mask Texture");
							bool flag = this.m_maskTexInverse.floatValue != 0f;
							flag = EditorGUILayout.Toggle("Inverse Mask", flag, new GUILayoutOption[0]);
							base.ColorProperty(this.m_maskTexEdgeColor, "Edge Color");
							base.RangeProperty(this.m_maskTexEdgeSoftness, "Edge Softness");
							base.RangeProperty(this.m_maskTexWipeControl, "Wipe Position");
							if (EditorGUI.EndChangeCheck())
							{
								this.m_maskTexInverse.floatValue = (float)((!flag) ? 0 : 1);
								this.havePropertiesChanged = true;
							}
						}
					}
					GUILayout.Space(15f);
				}
				EditorGUI.BeginChangeCheck();
				this.Draw2DRectBoundsProperty(this.m_clipRect, "_ClipRect");
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				GUILayout.Space(20f);
				if (material.HasProperty("_Stencil"))
				{
					base.FloatProperty(this.m_stencilID, "Stencil ID");
					base.FloatProperty(this.m_stencilComp, "Stencil Comp");
				}
				GUILayout.Space(20f);
				GUI.changed = false;
				this.isRatiosEnabled = EditorGUILayout.Toggle("Enable Ratios?", this.isRatiosEnabled, new GUILayoutOption[0]);
				if (GUI.changed)
				{
					this.SetKeyword(!this.isRatiosEnabled, "RATIOS_OFF");
				}
				EditorGUI.BeginChangeCheck();
				this.DrawFloatProperty(this.m_scaleRatio_A, "Scale Ratio A");
				this.DrawFloatProperty(this.m_scaleRatio_B, "Scale Ratio B");
				this.DrawFloatProperty(this.m_scaleRatio_C, "Scale Ratio C");
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
			}
		}
		this.m_inspectorEndRegion = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true)
		});
		this.DragAndDropGUI();
		if (this.havePropertiesChanged)
		{
			this.havePropertiesChanged = false;
			base.PropertiesChanged();
			EditorUtility.SetDirty(base.target);
			TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, base.target as Material);
		}
	}

	private void DragAndDropGUI()
	{
		Event current = Event.current;
		Rect rect = new Rect(this.m_inspectorStartRegion.x, this.m_inspectorStartRegion.y, this.m_inspectorEndRegion.width, this.m_inspectorEndRegion.y - this.m_inspectorStartRegion.y);
		EventType type = current.type;
		if (type == EventType.DragUpdated || type == EventType.DragPerform)
		{
			if (rect.Contains(current.mousePosition))
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				if (current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					Material material = base.target as Material;
					Texture texture = material.GetTexture(ShaderUtilities.ID_MainTex);
					Material material2 = DragAndDrop.objectReferences[0] as Material;
					Texture texture2 = material2.GetTexture(ShaderUtilities.ID_MainTex);
					TMP_FontAsset tmp_FontAsset = null;
					if (material2 == null || material2 == material || material2 == null || texture2 == null)
					{
						return;
					}
					if (texture2.GetInstanceID() != texture.GetInstanceID())
					{
						tmp_FontAsset = TMP_EditorUtility.FindMatchingFontAsset(material2);
						if (tmp_FontAsset == null)
						{
							return;
						}
					}
					GameObject[] gameObjects = Selection.gameObjects;
					for (int i = 0; i < gameObjects.Length; i++)
					{
						if (tmp_FontAsset != null)
						{
							TMP_Text component = gameObjects[i].GetComponent<TMP_Text>();
							if (component != null)
							{
								Undo.RecordObject(component, "Font Asset Change");
								component.font = tmp_FontAsset;
							}
						}
						TMPro_EventManager.ON_DRAG_AND_DROP_MATERIAL_CHANGED(gameObjects[i], material, material2);
						EditorUtility.SetDirty(gameObjects[i]);
					}
				}
				current.Use();
			}
		}
	}

	private void OnUndoRedo()
	{
		int currentGroup = Undo.GetCurrentGroup();
		int eventID = TMPro_SDFMaterialEditor.m_eventID;
		if (currentGroup != eventID)
		{
			TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, base.target as Material);
			TMPro_SDFMaterialEditor.m_eventID = currentGroup;
		}
	}

	private UndoPropertyModification[] OnUndoRedoEvent(UndoPropertyModification[] modifications)
	{
		return modifications;
	}

	private bool DrawTogglePanel(TMPro_SDFMaterialEditor.FoldoutType type, string label, bool toggle, string keyword)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		EditorGUI.indentLevel = 0;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 22f, new GUILayoutOption[0]);
		GUI.Label(controlRect, GUIContent.none, TMP_UIStyleManager.Group_Label);
		if (GUI.Button(new Rect(controlRect.x, controlRect.y, 250f, controlRect.height), label, TMP_UIStyleManager.Group_Label_Left))
		{
			switch (type)
			{
			case TMPro_SDFMaterialEditor.FoldoutType.outline:
				TMPro_SDFMaterialEditor.m_foldout.outline = !TMPro_SDFMaterialEditor.m_foldout.outline;
				break;
			case TMPro_SDFMaterialEditor.FoldoutType.underlay:
				TMPro_SDFMaterialEditor.m_foldout.underlay = !TMPro_SDFMaterialEditor.m_foldout.underlay;
				break;
			case TMPro_SDFMaterialEditor.FoldoutType.bevel:
				TMPro_SDFMaterialEditor.m_foldout.bevel = !TMPro_SDFMaterialEditor.m_foldout.bevel;
				break;
			case TMPro_SDFMaterialEditor.FoldoutType.light:
				TMPro_SDFMaterialEditor.m_foldout.light = !TMPro_SDFMaterialEditor.m_foldout.light;
				break;
			case TMPro_SDFMaterialEditor.FoldoutType.bump:
				TMPro_SDFMaterialEditor.m_foldout.bump = !TMPro_SDFMaterialEditor.m_foldout.bump;
				break;
			case TMPro_SDFMaterialEditor.FoldoutType.env:
				TMPro_SDFMaterialEditor.m_foldout.env = !TMPro_SDFMaterialEditor.m_foldout.env;
				break;
			case TMPro_SDFMaterialEditor.FoldoutType.glow:
				TMPro_SDFMaterialEditor.m_foldout.glow = !TMPro_SDFMaterialEditor.m_foldout.glow;
				break;
			}
		}
		EditorGUIUtility.labelWidth = 70f;
		EditorGUI.BeginChangeCheck();
		Material material = base.target as Material;
		if (!material.HasProperty("_FaceShininess") || keyword != "BEVEL_ON")
		{
			toggle = EditorGUI.Toggle(new Rect(controlRect.width - 90f, controlRect.y + 3f, 90f, 22f), new GUIContent("Enable ->"), toggle);
			if (EditorGUI.EndChangeCheck())
			{
				this.SetKeyword(toggle, keyword);
				this.havePropertiesChanged = true;
			}
		}
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
		return toggle;
	}

	private void DrawUVProperty(MaterialProperty[] properties, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x + 15f, controlRect.y, controlRect.width - 55f, 20f);
		Rect position2 = new Rect(130f, controlRect.y, 80f, 18f);
		GUI.Label(position, label);
		EditorGUIUtility.labelWidth = 35f;
		base.FloatProperty(position2, properties[0], "X");
		EditorGUIUtility.labelWidth = 35f;
		base.FloatProperty(new Rect(position2.x + 70f, position2.y, position2.width, position2.height), properties[1], "Y");
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void DrawSliderProperty(MaterialProperty property, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x, controlRect.y, controlRect.width - 55f, 20f);
		Rect source = new Rect(controlRect.width - 46f, controlRect.y, 60f, 18f);
		base.RangeProperty(position, property, label);
		EditorGUIUtility.labelWidth = 10f;
		base.FloatProperty(new Rect(source), property, string.Empty);
		if (!property.hasMixedValue)
		{
			property.floatValue = Mathf.Round(property.floatValue * 1000f) / 1000f;
		}
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void DrawTextureProperty(MaterialProperty property, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		EditorGUIUtility.fieldWidth = 70f;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 75f, new GUILayoutOption[0]);
		GUI.Label(new Rect(controlRect.x + 15f, controlRect.y + 5f, 100f, controlRect.height), label);
		base.TextureProperty(new Rect(controlRect.x, controlRect.y + 5f, 200f, controlRect.height), property, string.Empty, false);
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void DrawFloatProperty(MaterialProperty property, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x, controlRect.y, 225f, 18f);
		base.FloatProperty(position, property, label);
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void DrawRangeProperty(MaterialProperty property, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 16f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x + 15f, controlRect.y, controlRect.width, controlRect.height);
		GUI.Label(position, label);
		position.x += 100f;
		position.width -= 115f;
		base.RangeProperty(position, property, string.Empty);
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void DrawVectorProperty(MaterialProperty property, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		EditorGUIUtility.labelWidth = 160f;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x + 15f, controlRect.y + 2f, controlRect.width - 120f, 18f);
		Rect position2 = new Rect(175f, controlRect.y - 14f, controlRect.width - 160f, 18f);
		GUI.Label(position, label);
		base.VectorProperty(position2, property, string.Empty);
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void DrawVectorProperty(MaterialProperty property, string label, int floatCount)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		EditorGUIUtility.labelWidth = 160f;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x + 15f, controlRect.y + 2f, controlRect.width - 120f, 18f);
		Rect position2 = new Rect(175f, controlRect.y - 14f, controlRect.width - 160f, 18f);
		GUI.Label(position, label);
		base.VectorProperty(position2, property, string.Empty);
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void Draw2DBoundsProperty(MaterialProperty property, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 22f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x + 15f, controlRect.y + 2f, controlRect.width - 15f, 18f);
		GUI.Label(position, label);
		EditorGUIUtility.labelWidth = 30f;
		float num = (position.width - 15f) / 5f;
		position.x += labelWidth - 30f;
		Vector4 vectorValue = property.vectorValue;
		position.width = num;
		vectorValue.x = EditorGUI.FloatField(position, "X", vectorValue.x);
		position.x += num - 14f;
		vectorValue.y = EditorGUI.FloatField(position, "Y", vectorValue.y);
		position.x += num - 14f;
		vectorValue.z = EditorGUI.FloatField(position, "W", vectorValue.z);
		position.x += num - 14f;
		vectorValue.w = EditorGUI.FloatField(position, "H", vectorValue.w);
		position.x = controlRect.width - 11f;
		position.width = 25f;
		property.vectorValue = vectorValue;
		if (GUI.Button(position, "X"))
		{
			Renderer component = Selection.activeGameObject.GetComponent<Renderer>();
			if (component != null)
			{
				property.vectorValue = new Vector4(0f, 0f, Mathf.Round(component.bounds.extents.x * 1000f) / 1000f, Mathf.Round(component.bounds.extents.y * 1000f) / 1000f);
			}
		}
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void Draw2DRectBoundsProperty(MaterialProperty property, string label)
	{
		float labelWidth = EditorGUIUtility.labelWidth;
		float fieldWidth = EditorGUIUtility.fieldWidth;
		Rect controlRect = EditorGUILayout.GetControlRect(false, 22f, new GUILayoutOption[0]);
		Rect position = new Rect(controlRect.x + 15f, controlRect.y + 2f, controlRect.width - 15f, 18f);
		GUI.Label(position, label);
		float num = (position.width - position.x - 30f) / 4f;
		position.x += labelWidth - 30f;
		Vector4 vectorValue = property.vectorValue;
		EditorGUIUtility.labelWidth = 40f;
		position.width = num + 8f;
		vectorValue.x = EditorGUI.FloatField(position, "BL", vectorValue.x);
		position.x += num;
		position.width = num - 18f;
		vectorValue.y = EditorGUI.FloatField(position, string.Empty, vectorValue.y);
		position.x += num - 24f;
		position.width = num + 8f;
		vectorValue.z = EditorGUI.FloatField(position, "TR", vectorValue.z);
		position.x += num;
		position.width = num - 18f;
		vectorValue.w = EditorGUI.FloatField(position, string.Empty, vectorValue.w);
		property.vectorValue = vectorValue;
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = fieldWidth;
	}

	private void SetKeyword(bool state, string keyword)
	{
		Undo.RecordObjects(base.targets, "Keyword State Change");
		for (int i = 0; i < base.targets.Length; i++)
		{
			Material material = base.targets[i] as Material;
			if (state)
			{
				if (keyword == null)
				{
					goto IL_8C;
				}
				if (!(keyword == "UNDERLAY_ON"))
				{
					if (!(keyword == "UNDERLAY_INNER"))
					{
						goto IL_8C;
					}
					material.EnableKeyword("UNDERLAY_INNER");
					material.DisableKeyword("UNDERLAY_ON");
				}
				else
				{
					material.EnableKeyword("UNDERLAY_ON");
					material.DisableKeyword("UNDERLAY_INNER");
				}
				goto IL_10A;
				IL_8C:
				material.EnableKeyword(keyword);
			}
			else
			{
				if (keyword != null)
				{
					if (keyword == "UNDERLAY_ON")
					{
						material.DisableKeyword("UNDERLAY_ON");
						material.DisableKeyword("UNDERLAY_INNER");
						goto IL_10A;
					}
					if (keyword == "UNDERLAY_INNER")
					{
						material.DisableKeyword("UNDERLAY_INNER");
						material.DisableKeyword("UNDERLAY_ON");
						goto IL_10A;
					}
				}
				material.DisableKeyword(keyword);
			}
			IL_10A:;
		}
	}

	private void SetUnderlayKeywords()
	{
		for (int i = 0; i < base.targets.Length; i++)
		{
			Material material = base.targets[i] as Material;
			if (this.m_underlaySelection == TMPro_SDFMaterialEditor.Underlay_Types.Normal)
			{
				material.EnableKeyword("UNDERLAY_ON");
				material.DisableKeyword("UNDERLAY_INNER");
			}
			else if (this.m_underlaySelection == TMPro_SDFMaterialEditor.Underlay_Types.Inner)
			{
				material.EnableKeyword("UNDERLAY_INNER");
				material.DisableKeyword("UNDERLAY_ON");
			}
		}
	}

	private void SetMaskID(MaskingTypes id)
	{
		for (int i = 0; i < base.targets.Length; i++)
		{
			Material material = base.targets[i] as Material;
			if (id != MaskingTypes.MaskHard)
			{
				if (id != MaskingTypes.MaskSoft)
				{
					if (id == MaskingTypes.MaskOff)
					{
						material.SetFloat("_MaskID", (float)id);
					}
				}
				else
				{
					material.SetFloat("_MaskID", (float)id);
				}
			}
			else
			{
				material.SetFloat("_MaskID", (float)id);
			}
		}
	}

	private void SetMaskKeywords(MaskingTypes mask)
	{
		for (int i = 0; i < base.targets.Length; i++)
		{
			Material material = base.targets[i] as Material;
			if (mask != MaskingTypes.MaskHard)
			{
				if (mask != MaskingTypes.MaskSoft)
				{
					if (mask == MaskingTypes.MaskOff)
					{
						material.DisableKeyword("MASK_HARD");
						material.DisableKeyword("MASK_SOFT");
					}
				}
				else
				{
					material.EnableKeyword("MASK_SOFT");
					material.DisableKeyword("MASK_HARD");
				}
			}
			else
			{
				material.EnableKeyword("MASK_HARD");
				material.DisableKeyword("MASK_SOFT");
			}
		}
	}

	private void ReadMaterialProperties()
	{
		UnityEngine.Object[] targets = base.targets;
		this.m_faceColor = MaterialEditor.GetMaterialProperty(targets, "_FaceColor");
		this.m_faceTex = MaterialEditor.GetMaterialProperty(targets, "_FaceTex");
		this.m_faceUVSpeedX = MaterialEditor.GetMaterialProperty(targets, "_FaceUVSpeedX");
		this.m_faceUVSpeedY = MaterialEditor.GetMaterialProperty(targets, "_FaceUVSpeedY");
		this.m_faceDilate = MaterialEditor.GetMaterialProperty(targets, "_FaceDilate");
		this.m_faceShininess = MaterialEditor.GetMaterialProperty(targets, "_FaceShininess");
		this.m_outlineColor = MaterialEditor.GetMaterialProperty(targets, "_OutlineColor");
		this.m_outlineThickness = MaterialEditor.GetMaterialProperty(targets, "_OutlineWidth");
		this.m_outlineSoftness = MaterialEditor.GetMaterialProperty(targets, "_OutlineSoftness");
		this.m_outlineTex = MaterialEditor.GetMaterialProperty(targets, "_OutlineTex");
		this.m_outlineUVSpeedX = MaterialEditor.GetMaterialProperty(targets, "_OutlineUVSpeedX");
		this.m_outlineUVSpeedY = MaterialEditor.GetMaterialProperty(targets, "_OutlineUVSpeedY");
		this.m_outlineShininess = MaterialEditor.GetMaterialProperty(targets, "_OutlineShininess");
		this.m_underlayColor = MaterialEditor.GetMaterialProperty(targets, "_UnderlayColor");
		this.m_underlayOffsetX = MaterialEditor.GetMaterialProperty(targets, "_UnderlayOffsetX");
		this.m_underlayOffsetY = MaterialEditor.GetMaterialProperty(targets, "_UnderlayOffsetY");
		this.m_underlayDilate = MaterialEditor.GetMaterialProperty(targets, "_UnderlayDilate");
		this.m_underlaySoftness = MaterialEditor.GetMaterialProperty(targets, "_UnderlaySoftness");
		this.m_bumpMap = MaterialEditor.GetMaterialProperty(targets, "_BumpMap");
		this.m_bumpFace = MaterialEditor.GetMaterialProperty(targets, "_BumpFace");
		this.m_bumpOutline = MaterialEditor.GetMaterialProperty(targets, "_BumpOutline");
		this.m_bevel = MaterialEditor.GetMaterialProperty(targets, "_Bevel");
		this.m_bevelOffset = MaterialEditor.GetMaterialProperty(targets, "_BevelOffset");
		this.m_bevelWidth = MaterialEditor.GetMaterialProperty(targets, "_BevelWidth");
		this.m_bevelClamp = MaterialEditor.GetMaterialProperty(targets, "_BevelClamp");
		this.m_bevelRoundness = MaterialEditor.GetMaterialProperty(targets, "_BevelRoundness");
		this.m_specColor = MaterialEditor.GetMaterialProperty(targets, "_SpecColor");
		this.m_lightAngle = MaterialEditor.GetMaterialProperty(targets, "_LightAngle");
		this.m_specularColor = MaterialEditor.GetMaterialProperty(targets, "_SpecularColor");
		this.m_specularPower = MaterialEditor.GetMaterialProperty(targets, "_SpecularPower");
		this.m_reflectivity = MaterialEditor.GetMaterialProperty(targets, "_Reflectivity");
		this.m_diffuse = MaterialEditor.GetMaterialProperty(targets, "_Diffuse");
		this.m_ambientLight = MaterialEditor.GetMaterialProperty(targets, "_Ambient");
		this.m_glowColor = MaterialEditor.GetMaterialProperty(targets, "_GlowColor");
		this.m_glowOffset = MaterialEditor.GetMaterialProperty(targets, "_GlowOffset");
		this.m_glowInner = MaterialEditor.GetMaterialProperty(targets, "_GlowInner");
		this.m_glowOuter = MaterialEditor.GetMaterialProperty(targets, "_GlowOuter");
		this.m_glowPower = MaterialEditor.GetMaterialProperty(targets, "_GlowPower");
		this.m_reflectFaceColor = MaterialEditor.GetMaterialProperty(targets, "_ReflectFaceColor");
		this.m_reflectOutlineColor = MaterialEditor.GetMaterialProperty(targets, "_ReflectOutlineColor");
		this.m_reflectTex = MaterialEditor.GetMaterialProperty(targets, "_Cube");
		this.m_reflectRotation = MaterialEditor.GetMaterialProperty(targets, "_EnvMatrixRotation");
		this.m_mainTex = MaterialEditor.GetMaterialProperty(targets, "_MainTex");
		this.m_texSampleWidth = MaterialEditor.GetMaterialProperty(targets, "_TextureWidth");
		this.m_texSampleHeight = MaterialEditor.GetMaterialProperty(targets, "_TextureHeight");
		this.m_gradientScale = MaterialEditor.GetMaterialProperty(targets, "_GradientScale");
		this.m_PerspectiveFilter = MaterialEditor.GetMaterialProperty(targets, "_PerspectiveFilter");
		this.m_scaleX = MaterialEditor.GetMaterialProperty(targets, "_ScaleX");
		this.m_scaleY = MaterialEditor.GetMaterialProperty(targets, "_ScaleY");
		this.m_vertexOffsetX = MaterialEditor.GetMaterialProperty(targets, "_VertexOffsetX");
		this.m_vertexOffsetY = MaterialEditor.GetMaterialProperty(targets, "_VertexOffsetY");
		this.m_maskTex = MaterialEditor.GetMaterialProperty(targets, "_MaskTex");
		this.m_maskCoord = MaterialEditor.GetMaterialProperty(targets, "_MaskCoord");
		this.m_clipRect = MaterialEditor.GetMaterialProperty(targets, "_ClipRect");
		this.m_maskSoftnessX = MaterialEditor.GetMaterialProperty(targets, "_MaskSoftnessX");
		this.m_maskSoftnessY = MaterialEditor.GetMaterialProperty(targets, "_MaskSoftnessY");
		this.m_maskTexInverse = MaterialEditor.GetMaterialProperty(targets, "_MaskInverse");
		this.m_maskTexEdgeColor = MaterialEditor.GetMaterialProperty(targets, "_MaskEdgeColor");
		this.m_maskTexEdgeSoftness = MaterialEditor.GetMaterialProperty(targets, "_MaskEdgeSoftness");
		this.m_maskTexWipeControl = MaterialEditor.GetMaterialProperty(targets, "_MaskWipeControl");
		this.m_stencilID = MaterialEditor.GetMaterialProperty(targets, "_Stencil");
		this.m_stencilComp = MaterialEditor.GetMaterialProperty(targets, "_StencilComp");
		this.m_stencilOp = MaterialEditor.GetMaterialProperty(targets, "_StencilOp");
		this.m_stencilReadMask = MaterialEditor.GetMaterialProperty(targets, "_StencilReadMask");
		this.m_stencilWriteMask = MaterialEditor.GetMaterialProperty(targets, "_StencilWriteMask");
		this.m_shaderFlags = MaterialEditor.GetMaterialProperty(targets, "_ShaderFlags");
		this.m_scaleRatio_A = MaterialEditor.GetMaterialProperty(targets, "_ScaleRatioA");
		this.m_scaleRatio_B = MaterialEditor.GetMaterialProperty(targets, "_ScaleRatioB");
		this.m_scaleRatio_C = MaterialEditor.GetMaterialProperty(targets, "_ScaleRatioC");
	}

	private string m_warningMsg;

	private TMPro_SDFMaterialEditor.WarningTypes m_warning;

	private static int m_eventID;

	private TMP_Text m_textComponent;

	private MaterialProperty m_faceColor;

	private MaterialProperty m_faceTex;

	private MaterialProperty m_faceUVSpeedX;

	private MaterialProperty m_faceUVSpeedY;

	private MaterialProperty m_faceDilate;

	private MaterialProperty m_faceShininess;

	private MaterialProperty m_outlineColor;

	private MaterialProperty m_outlineTex;

	private MaterialProperty m_outlineUVSpeedX;

	private MaterialProperty m_outlineUVSpeedY;

	private MaterialProperty m_outlineThickness;

	private MaterialProperty m_outlineSoftness;

	private MaterialProperty m_outlineShininess;

	private MaterialProperty m_bevel;

	private MaterialProperty m_bevelOffset;

	private MaterialProperty m_bevelWidth;

	private MaterialProperty m_bevelClamp;

	private MaterialProperty m_bevelRoundness;

	private MaterialProperty m_underlayColor;

	private MaterialProperty m_underlayOffsetX;

	private MaterialProperty m_underlayOffsetY;

	private MaterialProperty m_underlayDilate;

	private MaterialProperty m_underlaySoftness;

	private MaterialProperty m_lightAngle;

	private MaterialProperty m_specularColor;

	private MaterialProperty m_specularPower;

	private MaterialProperty m_reflectivity;

	private MaterialProperty m_diffuse;

	private MaterialProperty m_ambientLight;

	private MaterialProperty m_bumpMap;

	private MaterialProperty m_bumpFace;

	private MaterialProperty m_bumpOutline;

	private MaterialProperty m_reflectFaceColor;

	private MaterialProperty m_reflectOutlineColor;

	private MaterialProperty m_reflectTex;

	private MaterialProperty m_reflectRotation;

	private MaterialProperty m_specColor;

	private MaterialProperty m_glowColor;

	private MaterialProperty m_glowInner;

	private MaterialProperty m_glowOffset;

	private MaterialProperty m_glowPower;

	private MaterialProperty m_glowOuter;

	private MaterialProperty m_mainTex;

	private MaterialProperty m_texSampleWidth;

	private MaterialProperty m_texSampleHeight;

	private MaterialProperty m_gradientScale;

	private MaterialProperty m_scaleX;

	private MaterialProperty m_scaleY;

	private MaterialProperty m_PerspectiveFilter;

	private MaterialProperty m_vertexOffsetX;

	private MaterialProperty m_vertexOffsetY;

	private MaterialProperty m_maskCoord;

	private MaterialProperty m_clipRect;

	private MaterialProperty m_maskSoftnessX;

	private MaterialProperty m_maskSoftnessY;

	private MaterialProperty m_maskTex;

	private MaterialProperty m_maskTexInverse;

	private MaterialProperty m_maskTexEdgeColor;

	private MaterialProperty m_maskTexEdgeSoftness;

	private MaterialProperty m_maskTexWipeControl;

	private MaterialProperty m_stencilID;

	private MaterialProperty m_stencilOp;

	private MaterialProperty m_stencilComp;

	private MaterialProperty m_stencilReadMask;

	private MaterialProperty m_stencilWriteMask;

	private MaterialProperty m_shaderFlags;

	private MaterialProperty m_scaleRatio_A;

	private MaterialProperty m_scaleRatio_B;

	private MaterialProperty m_scaleRatio_C;

	private string[] m_bevelOptions = new string[]
	{
		"Outer Bevel",
		"Inner Bevel",
		"--"
	};

	private int m_bevelSelection;

	private MaskingTypes m_mask;

	private TMPro_SDFMaterialEditor.Underlay_Types m_underlaySelection;

	private string[] m_Keywords;

	private bool isOutlineEnabled;

	private bool isRatiosEnabled;

	private bool isBevelEnabled;

	private bool isGlowEnabled;

	private bool isUnderlayEnabled;

	private bool havePropertiesChanged;

	private Rect m_inspectorStartRegion;

	private Rect m_inspectorEndRegion;

	private struct m_foldout
	{
		public static bool editorPanel = true;

		public static bool face = true;

		public static bool outline = true;

		public static bool underlay;

		public static bool bevel;

		public static bool light;

		public static bool bump;

		public static bool env;

		public static bool glow;

		public static bool debug;
	}

	private enum FoldoutType
	{
		face,
		outline,
		underlay,
		bevel,
		light,
		bump,
		env,
		glow,
		debug
	}

	private enum WarningTypes
	{
		None,
		ShaderMismatch,
		FontAtlasMismatch
	}

	private enum ShaderTypes
	{
		None,
		Bitmap,
		SDF
	}

	private enum Bevel_Types
	{
		OuterBevel,
		InnerBevel
	}

	private enum Underlay_Types
	{
		Normal,
		Inner
	}
}
