using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TextMeshPro))]
	[CanEditMultipleObjects]
	public class TMP_EditorPanel : Editor
	{
		public void OnEnable()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
			this.text_prop = base.serializedObject.FindProperty("m_text");
			this.isRightToLeft_prop = base.serializedObject.FindProperty("m_isRightToLeft");
			this.fontAsset_prop = base.serializedObject.FindProperty("m_fontAsset");
			this.fontSharedMaterial_prop = base.serializedObject.FindProperty("m_sharedMaterial");
			this.fontStyle_prop = base.serializedObject.FindProperty("m_fontStyle");
			this.fontSize_prop = base.serializedObject.FindProperty("m_fontSize");
			this.fontSizeBase_prop = base.serializedObject.FindProperty("m_fontSizeBase");
			this.autoSizing_prop = base.serializedObject.FindProperty("m_enableAutoSizing");
			this.fontSizeMin_prop = base.serializedObject.FindProperty("m_fontSizeMin");
			this.fontSizeMax_prop = base.serializedObject.FindProperty("m_fontSizeMax");
			this.lineSpacingMax_prop = base.serializedObject.FindProperty("m_lineSpacingMax");
			this.charWidthMaxAdj_prop = base.serializedObject.FindProperty("m_charWidthMaxAdj");
			this.fontColor_prop = base.serializedObject.FindProperty("m_fontColor");
			this.enableVertexGradient_prop = base.serializedObject.FindProperty("m_enableVertexGradient");
			this.fontColorGradient_prop = base.serializedObject.FindProperty("m_fontColorGradient");
			this.fontColorGradientPreset_prop = base.serializedObject.FindProperty("m_fontColorGradientPreset");
			this.overrideHtmlColor_prop = base.serializedObject.FindProperty("m_overrideHtmlColors");
			this.characterSpacing_prop = base.serializedObject.FindProperty("m_characterSpacing");
			this.wordSpacing_prop = base.serializedObject.FindProperty("m_wordSpacing");
			this.lineSpacing_prop = base.serializedObject.FindProperty("m_lineSpacing");
			this.paragraphSpacing_prop = base.serializedObject.FindProperty("m_paragraphSpacing");
			this.textAlignment_prop = base.serializedObject.FindProperty("m_textAlignment");
			this.horizontalMapping_prop = base.serializedObject.FindProperty("m_horizontalMapping");
			this.verticalMapping_prop = base.serializedObject.FindProperty("m_verticalMapping");
			this.uvLineOffset_prop = base.serializedObject.FindProperty("m_uvLineOffset");
			this.enableWordWrapping_prop = base.serializedObject.FindProperty("m_enableWordWrapping");
			this.wordWrappingRatios_prop = base.serializedObject.FindProperty("m_wordWrappingRatios");
			this.textOverflowMode_prop = base.serializedObject.FindProperty("m_overflowMode");
			this.pageToDisplay_prop = base.serializedObject.FindProperty("m_pageToDisplay");
			this.linkedTextComponent_prop = base.serializedObject.FindProperty("m_linkedTextComponent");
			this.isLinkedTextComponent_prop = base.serializedObject.FindProperty("m_isLinkedTextComponent");
			this.enableKerning_prop = base.serializedObject.FindProperty("m_enableKerning");
			this.isOrthographic_prop = base.serializedObject.FindProperty("m_isOrthographic");
			this.havePropertiesChanged_prop = base.serializedObject.FindProperty("m_havePropertiesChanged");
			this.inputSource_prop = base.serializedObject.FindProperty("m_inputSource");
			this.isInputPasingRequired_prop = base.serializedObject.FindProperty("m_isInputParsingRequired");
			this.enableExtraPadding_prop = base.serializedObject.FindProperty("m_enableExtraPadding");
			this.isRichText_prop = base.serializedObject.FindProperty("m_isRichText");
			this.checkPaddingRequired_prop = base.serializedObject.FindProperty("checkPaddingRequired");
			this.enableEscapeCharacterParsing_prop = base.serializedObject.FindProperty("m_parseCtrlCharacters");
			this.useMaxVisibleDescender_prop = base.serializedObject.FindProperty("m_useMaxVisibleDescender");
			this.isVolumetricText_prop = base.serializedObject.FindProperty("m_isVolumetricText");
			this.geometrySortingOrder_prop = base.serializedObject.FindProperty("m_geometrySortingOrder");
			this.spriteAsset_prop = base.serializedObject.FindProperty("m_spriteAsset");
			this.margin_prop = base.serializedObject.FindProperty("m_margin");
			this.hasFontAssetChanged_prop = base.serializedObject.FindProperty("m_hasFontAssetChanged");
			TMP_UIStyleManager.GetUIStyles();
			this.m_textComponent = (base.target as TextMeshPro);
			this.m_rectTransform = this.m_textComponent.rectTransform;
			this.m_targetMaterial = this.m_textComponent.fontSharedMaterial;
			this.m_materialPresetNames = this.GetMaterialPresets();
		}

		public void OnDisable()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
		}

		public override void OnInspectorGUI()
		{
			if (this.toggleStyle == null)
			{
				this.toggleStyle = new GUIStyle(GUI.skin.label);
				this.toggleStyle.fontSize = 12;
				this.toggleStyle.normal.textColor = TMP_UIStyleManager.Section_Label.normal.textColor;
				this.toggleStyle.richText = true;
			}
			base.serializedObject.Update();
			Rect controlRect = EditorGUILayout.GetControlRect(false, 25f, new GUILayoutOption[0]);
			float num = 130f;
			EditorGUIUtility.labelWidth = num;
			float labelWidth = num;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			controlRect.y += 2f;
			GUI.Label(controlRect, "<b>TEXT INPUT BOX</b>" + ((!TMP_EditorPanel.m_foldout.textInput) ? TMP_EditorPanel.uiStateLabel[0] : TMP_EditorPanel.uiStateLabel[1]), TMP_UIStyleManager.Section_Label);
			if (GUI.Button(new Rect(controlRect.x, controlRect.y, controlRect.width - 150f, controlRect.height), GUIContent.none, GUI.skin.label))
			{
				TMP_EditorPanel.m_foldout.textInput = !TMP_EditorPanel.m_foldout.textInput;
			}
			GUI.Label(new Rect(controlRect.width - 125f, controlRect.y + 4f, 125f, 24f), "<i>Enable RTL Editor</i>", this.toggleStyle);
			this.isRightToLeft_prop.boolValue = EditorGUI.Toggle(new Rect(controlRect.width - 10f, controlRect.y + 3f, 20f, 24f), GUIContent.none, this.isRightToLeft_prop.boolValue);
			if (TMP_EditorPanel.m_foldout.textInput)
			{
				if (this.isLinkedTextComponent_prop.boolValue)
				{
					EditorGUILayout.HelpBox("The Text Input Box is disabled due to this text component being linked to another.", MessageType.Info);
				}
				else
				{
					EditorGUI.BeginChangeCheck();
					this.text_prop.stringValue = EditorGUILayout.TextArea(this.text_prop.stringValue, TMP_UIStyleManager.TextAreaBoxEditor, new GUILayoutOption[]
					{
						GUILayout.Height(125f),
						GUILayout.ExpandWidth(true)
					});
					if (EditorGUI.EndChangeCheck() || (this.isRightToLeft_prop.boolValue && (this.m_RTLText == null || this.m_RTLText == string.Empty)))
					{
						this.inputSource_prop.enumValueIndex = 0;
						this.isInputPasingRequired_prop.boolValue = true;
						this.havePropertiesChanged = true;
						if (this.isRightToLeft_prop.boolValue)
						{
							this.m_RTLText = string.Empty;
							string stringValue = this.text_prop.stringValue;
							for (int i = 0; i < stringValue.Length; i++)
							{
								this.m_RTLText += stringValue[stringValue.Length - i - 1];
							}
						}
					}
					if (this.isRightToLeft_prop.boolValue)
					{
						EditorGUI.BeginChangeCheck();
						this.m_RTLText = EditorGUILayout.TextArea(this.m_RTLText, TMP_UIStyleManager.TextAreaBoxEditor, new GUILayoutOption[]
						{
							GUILayout.Height(125f),
							GUILayout.ExpandWidth(true)
						});
						if (EditorGUI.EndChangeCheck())
						{
							string text = string.Empty;
							for (int j = 0; j < this.m_RTLText.Length; j++)
							{
								text += this.m_RTLText[this.m_RTLText.Length - j - 1];
							}
							this.text_prop.stringValue = text;
						}
					}
				}
			}
			if (GUILayout.Button("<b>FONT SETTINGS</b>" + ((!TMP_EditorPanel.m_foldout.fontSettings) ? TMP_EditorPanel.uiStateLabel[0] : TMP_EditorPanel.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_EditorPanel.m_foldout.fontSettings = !TMP_EditorPanel.m_foldout.fontSettings;
			}
			if (this.m_isPresetListDirty)
			{
				this.m_materialPresetNames = this.GetMaterialPresets();
			}
			if (TMP_EditorPanel.m_foldout.fontSettings)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.fontAsset_prop, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
					this.hasFontAssetChanged_prop.boolValue = true;
					this.m_isPresetListDirty = true;
					this.m_materialPresetSelectionIndex = 0;
				}
				if (this.m_materialPresetNames != null)
				{
					EditorGUI.BeginChangeCheck();
					controlRect = EditorGUILayout.GetControlRect(false, 18f, new GUILayoutOption[0]);
					float fixedHeight = EditorStyles.popup.fixedHeight;
					EditorStyles.popup.fixedHeight = controlRect.height;
					int fontSize = EditorStyles.popup.fontSize;
					EditorStyles.popup.fontSize = 11;
					this.m_materialPresetSelectionIndex = EditorGUI.Popup(controlRect, "Material Preset", this.m_materialPresetSelectionIndex, this.m_materialPresetNames);
					if (EditorGUI.EndChangeCheck())
					{
						this.fontSharedMaterial_prop.objectReferenceValue = this.m_materialPresets[this.m_materialPresetSelectionIndex];
						this.havePropertiesChanged = true;
					}
					if (this.m_materialPresetSelectionIndex < this.m_materialPresetNames.Length && this.m_targetMaterial != this.m_materialPresets[this.m_materialPresetSelectionIndex] && !this.havePropertiesChanged)
					{
						this.m_isPresetListDirty = true;
					}
					EditorStyles.popup.fixedHeight = fixedHeight;
					EditorStyles.popup.fontSize = fontSize;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PrefixLabel("Font Style");
				int intValue = this.fontStyle_prop.intValue;
				int num2 = (!GUILayout.Toggle((intValue & 1) == 1, "B", GUI.skin.button, new GUILayoutOption[0])) ? 0 : 1;
				int num3 = (!GUILayout.Toggle((intValue & 2) == 2, "I", GUI.skin.button, new GUILayoutOption[0])) ? 0 : 2;
				int num4 = (!GUILayout.Toggle((intValue & 4) == 4, "U", GUI.skin.button, new GUILayoutOption[0])) ? 0 : 4;
				int num5 = (!GUILayout.Toggle((intValue & 64) == 64, "S", GUI.skin.button, new GUILayoutOption[0])) ? 0 : 64;
				int num6 = (!GUILayout.Toggle((intValue & 8) == 8, "ab", GUI.skin.button, new GUILayoutOption[0])) ? 0 : 8;
				int num7 = (!GUILayout.Toggle((intValue & 16) == 16, "AB", GUI.skin.button, new GUILayoutOption[0])) ? 0 : 16;
				int num8 = (!GUILayout.Toggle((intValue & 32) == 32, "SC", GUI.skin.button, new GUILayoutOption[0])) ? 0 : 32;
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					this.fontStyle_prop.intValue = num2 + num3 + num4 + num6 + num7 + num8 + num5;
					this.havePropertiesChanged = true;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.fontColor_prop, new GUIContent("Color (Vertex)"), new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.enableVertexGradient_prop, new GUIContent("Color Gradient"), new GUILayoutOption[]
				{
					GUILayout.MinWidth(140f),
					GUILayout.MaxWidth(200f)
				});
				EditorGUIUtility.labelWidth = 95f;
				EditorGUILayout.PropertyField(this.overrideHtmlColor_prop, new GUIContent("Override Tags"), new GUILayoutOption[0]);
				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				if (this.enableVertexGradient_prop.boolValue)
				{
					EditorGUILayout.PropertyField(this.fontColorGradientPreset_prop, new GUIContent("Gradient (Preset)"), new GUILayoutOption[0]);
					if (this.fontColorGradientPreset_prop.objectReferenceValue == null)
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.fontColorGradient_prop.FindPropertyRelative("topLeft"), new GUIContent("Top Left"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.fontColorGradient_prop.FindPropertyRelative("topRight"), new GUIContent("Top Right"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.fontColorGradient_prop.FindPropertyRelative("bottomLeft"), new GUIContent("Bottom Left"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.fontColorGradient_prop.FindPropertyRelative("bottomRight"), new GUIContent("Bottom Right"), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							this.havePropertiesChanged = true;
						}
					}
					else
					{
						SerializedObject serializedObject = new SerializedObject(this.fontColorGradientPreset_prop.objectReferenceValue);
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(serializedObject.FindProperty("topLeft"), new GUIContent("Top Left"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("topRight"), new GUIContent("Top Right"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("bottomLeft"), new GUIContent("Bottom Left"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("bottomRight"), new GUIContent("Bottom Right"), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							serializedObject.ApplyModifiedProperties();
							this.havePropertiesChanged = true;
							TMPro_EventManager.ON_COLOR_GRAIDENT_PROPERTY_CHANGED(this.fontColorGradientPreset_prop.objectReferenceValue as TMP_ColorGradient);
						}
					}
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.fontSize_prop, new GUIContent("Font Size"), new GUILayoutOption[]
				{
					GUILayout.MinWidth(168f),
					GUILayout.MaxWidth(200f)
				});
				EditorGUIUtility.fieldWidth = fieldWidth;
				if (EditorGUI.EndChangeCheck())
				{
					this.fontSizeBase_prop.floatValue = this.fontSize_prop.floatValue;
					this.havePropertiesChanged = true;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUIUtility.labelWidth = 70f;
				EditorGUILayout.PropertyField(this.autoSizing_prop, new GUIContent("Auto Size"), new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = labelWidth;
				if (EditorGUI.EndChangeCheck())
				{
					if (!this.autoSizing_prop.boolValue)
					{
						this.fontSize_prop.floatValue = this.fontSizeBase_prop.floatValue;
					}
					this.havePropertiesChanged = true;
				}
				if (this.autoSizing_prop.boolValue)
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					EditorGUILayout.PrefixLabel("Auto Size Options");
					EditorGUIUtility.labelWidth = 24f;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.fontSizeMin_prop, new GUIContent("Min"), new GUILayoutOption[]
					{
						GUILayout.MinWidth(46f)
					});
					if (EditorGUI.EndChangeCheck())
					{
						this.fontSizeMin_prop.floatValue = Mathf.Min(this.fontSizeMin_prop.floatValue, this.fontSizeMax_prop.floatValue);
						this.havePropertiesChanged = true;
					}
					EditorGUIUtility.labelWidth = 27f;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.fontSizeMax_prop, new GUIContent("Max"), new GUILayoutOption[]
					{
						GUILayout.MinWidth(49f)
					});
					if (EditorGUI.EndChangeCheck())
					{
						this.fontSizeMax_prop.floatValue = Mathf.Max(this.fontSizeMin_prop.floatValue, this.fontSizeMax_prop.floatValue);
						this.havePropertiesChanged = true;
					}
					EditorGUI.BeginChangeCheck();
					EditorGUIUtility.labelWidth = 36f;
					EditorGUILayout.PropertyField(this.charWidthMaxAdj_prop, new GUIContent("WD%"), new GUILayoutOption[]
					{
						GUILayout.MinWidth(58f)
					});
					EditorGUIUtility.labelWidth = 28f;
					EditorGUILayout.PropertyField(this.lineSpacingMax_prop, new GUIContent("Line"), new GUILayoutOption[]
					{
						GUILayout.MinWidth(50f)
					});
					EditorGUIUtility.labelWidth = labelWidth;
					EditorGUILayout.EndHorizontal();
					if (EditorGUI.EndChangeCheck())
					{
						this.charWidthMaxAdj_prop.floatValue = Mathf.Clamp(this.charWidthMaxAdj_prop.floatValue, 0f, 50f);
						this.lineSpacingMax_prop.floatValue = Mathf.Min(0f, this.lineSpacingMax_prop.floatValue);
						this.havePropertiesChanged = true;
					}
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PrefixLabel("Spacing Options");
				EditorGUIUtility.labelWidth = 35f;
				EditorGUILayout.PropertyField(this.characterSpacing_prop, new GUIContent("Char"), new GUILayoutOption[]
				{
					GUILayout.MinWidth(50f)
				});
				EditorGUILayout.PropertyField(this.wordSpacing_prop, new GUIContent("Word"), new GUILayoutOption[]
				{
					GUILayout.MinWidth(50f)
				});
				EditorGUILayout.PropertyField(this.lineSpacing_prop, new GUIContent("Line"), new GUILayoutOption[]
				{
					GUILayout.MinWidth(50f)
				});
				EditorGUILayout.PropertyField(this.paragraphSpacing_prop, new GUIContent(" Par."), new GUILayoutOption[]
				{
					GUILayout.MinWidth(50f)
				});
				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				EditorGUI.BeginChangeCheck();
				controlRect = EditorGUILayout.GetControlRect(false, 19f, new GUILayoutOption[0]);
				GUIStyle guistyle = new GUIStyle(GUI.skin.button);
				guistyle.margin = new RectOffset(1, 1, 1, 1);
				guistyle.padding = new RectOffset(1, 1, 1, 0);
				this.selAlignGrid_A = TMP_EditorUtility.GetHorizontalAlignmentGridValue(this.textAlignment_prop.intValue);
				this.selAlignGrid_B = TMP_EditorUtility.GetVerticalAlignmentGridValue(this.textAlignment_prop.intValue);
				GUI.Label(new Rect(controlRect.x, controlRect.y + 2f, 100f, controlRect.height), "Alignment");
				float num9 = EditorGUIUtility.labelWidth + 15f;
				this.selAlignGrid_A = GUI.SelectionGrid(new Rect(num9, controlRect.y, 138f, controlRect.height), this.selAlignGrid_A, TMP_UIStyleManager.alignContent_A, 6, guistyle);
				this.selAlignGrid_B = GUI.SelectionGrid(new Rect(num9 + 138f + 20f, controlRect.y, 138f, controlRect.height), this.selAlignGrid_B, TMP_UIStyleManager.alignContent_B, 6, guistyle);
				if (EditorGUI.EndChangeCheck())
				{
					int intValue2 = 1 << this.selAlignGrid_A | 256 << this.selAlignGrid_B;
					this.textAlignment_prop.intValue = intValue2;
					this.havePropertiesChanged = true;
				}
				EditorGUI.BeginChangeCheck();
				if ((this.textAlignment_prop.intValue & 8) == 8 || (this.textAlignment_prop.intValue & 16) == 16)
				{
					this.DrawPropertySlider("Wrap Mix (W <-> C)", this.wordWrappingRatios_prop);
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				EditorGUI.BeginChangeCheck();
				controlRect = EditorGUILayout.GetControlRect(false, new GUILayoutOption[0]);
				EditorGUI.PrefixLabel(new Rect(controlRect.x, controlRect.y, 130f, controlRect.height), new GUIContent("Wrapping & Overflow"));
				controlRect.width = (controlRect.width - 130f) / 2f;
				controlRect.x += 130f;
				int num10 = EditorGUI.Popup(controlRect, (!this.enableWordWrapping_prop.boolValue) ? 0 : 1, new string[]
				{
					"Disabled",
					"Enabled"
				});
				if (EditorGUI.EndChangeCheck())
				{
					this.enableWordWrapping_prop.boolValue = (num10 == 1);
					this.havePropertiesChanged = true;
					this.isInputPasingRequired_prop.boolValue = true;
				}
				EditorGUI.BeginChangeCheck();
				TMP_Text exists = this.linkedTextComponent_prop.objectReferenceValue as TMP_Text;
				if (this.textOverflowMode_prop.enumValueIndex == 6)
				{
					controlRect.x += controlRect.width + 5f;
					controlRect.width /= 3f;
					EditorGUI.PropertyField(controlRect, this.textOverflowMode_prop, GUIContent.none);
					controlRect.x += controlRect.width;
					controlRect.width = controlRect.width * 2f - 5f;
					EditorGUI.PropertyField(controlRect, this.linkedTextComponent_prop, GUIContent.none);
					if (GUI.changed)
					{
						TMP_Text tmp_Text = this.linkedTextComponent_prop.objectReferenceValue as TMP_Text;
						if (tmp_Text)
						{
							this.m_textComponent.linkedTextComponent = tmp_Text;
						}
					}
				}
				else if (this.textOverflowMode_prop.enumValueIndex == 5)
				{
					controlRect.x += controlRect.width + 5f;
					controlRect.width /= 2f;
					EditorGUI.PropertyField(controlRect, this.textOverflowMode_prop, GUIContent.none);
					controlRect.x += controlRect.width;
					controlRect.width -= 5f;
					EditorGUI.PropertyField(controlRect, this.pageToDisplay_prop, GUIContent.none);
					if (exists)
					{
						this.m_textComponent.linkedTextComponent = null;
					}
				}
				else
				{
					controlRect.x += controlRect.width + 5f;
					controlRect.width -= 5f;
					EditorGUI.PropertyField(controlRect, this.textOverflowMode_prop, GUIContent.none);
					if (exists)
					{
						this.m_textComponent.linkedTextComponent = null;
					}
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
					this.isInputPasingRequired_prop.boolValue = true;
				}
				EditorGUI.BeginChangeCheck();
				controlRect = EditorGUILayout.GetControlRect(false, new GUILayoutOption[0]);
				EditorGUI.PrefixLabel(new Rect(controlRect.x, controlRect.y, 130f, controlRect.height), new GUIContent("UV Mapping Options"));
				controlRect.width = (controlRect.width - 130f) / 2f;
				controlRect.x += 130f;
				EditorGUI.PropertyField(controlRect, this.horizontalMapping_prop, GUIContent.none);
				controlRect.x += controlRect.width + 5f;
				controlRect.width -= 5f;
				EditorGUI.PropertyField(controlRect, this.verticalMapping_prop, GUIContent.none);
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				if (this.horizontalMapping_prop.enumValueIndex > 0)
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.uvLineOffset_prop, new GUIContent("UV Line Offset"), new GUILayoutOption[]
					{
						GUILayout.MinWidth(70f)
					});
					if (EditorGUI.EndChangeCheck())
					{
						this.havePropertiesChanged = true;
					}
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.enableKerning_prop, new GUIContent("Enable Kerning?"), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.enableExtraPadding_prop, new GUIContent("Extra Padding?"), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
					this.checkPaddingRequired_prop.boolValue = true;
				}
				EditorGUILayout.EndHorizontal();
			}
			if (GUILayout.Button("<b>EXTRA SETTINGS</b>" + ((!TMP_EditorPanel.m_foldout.extraSettings) ? TMP_EditorPanel.uiStateLabel[0] : TMP_EditorPanel.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_EditorPanel.m_foldout.extraSettings = !TMP_EditorPanel.m_foldout.extraSettings;
			}
			if (TMP_EditorPanel.m_foldout.extraSettings)
			{
				EditorGUI.indentLevel = 0;
				EditorGUI.BeginChangeCheck();
				this.DrawMaginProperty(this.margin_prop, "Margins");
				if (EditorGUI.EndChangeCheck())
				{
					this.m_textComponent.margin = this.margin_prop.vector4Value;
					this.havePropertiesChanged = true;
				}
				GUILayout.Space(10f);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PrefixLabel("Sorting Layer");
				EditorGUI.BeginChangeCheck();
				float labelWidth2 = EditorGUIUtility.labelWidth;
				float fieldWidth2 = EditorGUIUtility.fieldWidth;
				string[] sortingLayerNames = SortingLayerHelper.sortingLayerNames;
				string sortingLayerNameFromID = SortingLayerHelper.GetSortingLayerNameFromID(this.m_textComponent.sortingLayerID);
				int num11 = Array.IndexOf<string>(sortingLayerNames, sortingLayerNameFromID);
				EditorGUIUtility.fieldWidth = 0f;
				int num12 = EditorGUILayout.Popup(string.Empty, num11, sortingLayerNames, new GUILayoutOption[]
				{
					GUILayout.MinWidth(80f)
				});
				if (num12 != num11)
				{
					this.m_textComponent.sortingLayerID = SortingLayerHelper.GetSortingLayerIDForIndex(num12);
				}
				EditorGUIUtility.labelWidth = 40f;
				EditorGUIUtility.fieldWidth = 80f;
				int num13 = EditorGUILayout.IntField("Order", this.m_textComponent.sortingOrder, new GUILayoutOption[0]);
				if (num13 != this.m_textComponent.sortingOrder)
				{
					this.m_textComponent.sortingOrder = num13;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = labelWidth2;
				EditorGUIUtility.fieldWidth = fieldWidth2;
				EditorGUILayout.PropertyField(this.geometrySortingOrder_prop, new GUIContent("Geometry Sorting"), new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				EditorGUIUtility.labelWidth = 150f;
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.isOrthographic_prop, new GUIContent("Orthographic Mode?"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.isRichText_prop, new GUIContent("Enable Rich Text?"), new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.enableEscapeCharacterParsing_prop, new GUIContent("Parse Escape Characters"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.useMaxVisibleDescender_prop, new GUIContent("Use Visible Descender"), new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(this.spriteAsset_prop, new GUIContent("Sprite Asset", "The Sprite Asset used when NOT specifically referencing one using <sprite=\"Sprite Asset Name\"."), true, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.isVolumetricText_prop, new GUIContent("Enabled Volumetric Setup"), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.havePropertiesChanged = true;
					this.m_textComponent.textInfo.ResetVertexLayout(this.isVolumetricText_prop.boolValue);
				}
				EditorGUIUtility.labelWidth = 135f;
				GUILayout.Space(10f);
			}
			if (this.havePropertiesChanged)
			{
				this.havePropertiesChanged_prop.boolValue = true;
				this.havePropertiesChanged = false;
			}
			EditorGUILayout.Space();
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
		{
			if (this.IsMixSelectionTypes())
			{
				return;
			}
			this.m_rectTransform.GetWorldCorners(this.m_rectCorners);
			Vector4 margin = this.m_textComponent.margin;
			Vector3 lossyScale = this.m_rectTransform.lossyScale;
			this.handlePoints[0] = this.m_rectCorners[0] + this.m_rectTransform.TransformDirection(new Vector3(margin.x * lossyScale.x, margin.w * lossyScale.y, 0f));
			this.handlePoints[1] = this.m_rectCorners[1] + this.m_rectTransform.TransformDirection(new Vector3(margin.x * lossyScale.x, -margin.y * lossyScale.y, 0f));
			this.handlePoints[2] = this.m_rectCorners[2] + this.m_rectTransform.TransformDirection(new Vector3(-margin.z * lossyScale.x, -margin.y * lossyScale.y, 0f));
			this.handlePoints[3] = this.m_rectCorners[3] + this.m_rectTransform.TransformDirection(new Vector3(-margin.z * lossyScale.x, margin.w * lossyScale.y, 0f));
			Handles.color = Color.yellow;
			Handles.DrawSolidRectangleWithOutline(this.handlePoints, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0), new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue));
			Vector3 vector = (this.handlePoints[0] + this.handlePoints[1]) * 0.5f;
			Vector3 position = vector;
			Quaternion identity = Quaternion.identity;
			float size = HandleUtility.GetHandleSize(this.m_rectTransform.position) * 0.05f;
			Vector3 zero = Vector3.zero;
			if (TMP_EditorPanel.cache0 == null)
			{
				TMP_EditorPanel.cache0 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			Vector3 rhs = Handles.FreeMoveHandle(position, identity, size, zero, TMP_EditorPanel.cache0);
			bool flag = false;
			if (vector != rhs)
			{
				float num = vector.x - rhs.x;
				margin.x += -num / lossyScale.x;
				flag = true;
			}
			Vector3 vector2 = (this.handlePoints[1] + this.handlePoints[2]) * 0.5f;
			Vector3 position2 = vector2;
			Quaternion identity2 = Quaternion.identity;
			float size2 = HandleUtility.GetHandleSize(this.m_rectTransform.position) * 0.05f;
			Vector3 zero2 = Vector3.zero;
			if (TMP_EditorPanel.cache1 == null)
			{
				TMP_EditorPanel.cache1 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			Vector3 rhs2 = Handles.FreeMoveHandle(position2, identity2, size2, zero2, TMP_EditorPanel.cache1);
			if (vector2 != rhs2)
			{
				float num2 = vector2.y - rhs2.y;
				margin.y += num2 / lossyScale.y;
				flag = true;
			}
			Vector3 vector3 = (this.handlePoints[2] + this.handlePoints[3]) * 0.5f;
			Vector3 position3 = vector3;
			Quaternion identity3 = Quaternion.identity;
			float size3 = HandleUtility.GetHandleSize(this.m_rectTransform.position) * 0.05f;
			Vector3 zero3 = Vector3.zero;
			if (TMP_EditorPanel.cache2 == null)
			{
				TMP_EditorPanel.cache2 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			Vector3 rhs3 = Handles.FreeMoveHandle(position3, identity3, size3, zero3, TMP_EditorPanel.cache2);
			if (vector3 != rhs3)
			{
				float num3 = vector3.x - rhs3.x;
				margin.z += num3 / lossyScale.x;
				flag = true;
			}
			Vector3 vector4 = (this.handlePoints[3] + this.handlePoints[0]) * 0.5f;
			Vector3 position4 = vector4;
			Quaternion identity4 = Quaternion.identity;
			float size4 = HandleUtility.GetHandleSize(this.m_rectTransform.position) * 0.05f;
			Vector3 zero4 = Vector3.zero;
			if (TMP_EditorPanel.cache3 == null)
			{
				TMP_EditorPanel.cache3 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			Vector3 rhs4 = Handles.FreeMoveHandle(position4, identity4, size4, zero4, TMP_EditorPanel.cache3);
			if (vector4 != rhs4)
			{
				float num4 = vector4.y - rhs4.y;
				margin.w += -num4 / lossyScale.y;
				flag = true;
			}
			if (flag)
			{
				Undo.RecordObjects(new UnityEngine.Object[]
				{
					this.m_rectTransform,
					this.m_textComponent
				}, "Margin Changes");
				this.m_textComponent.margin = margin;
				EditorUtility.SetDirty(base.target);
			}
		}

		private bool IsMixSelectionTypes()
		{
			UnityEngine.Object[] gameObjects = Selection.gameObjects;
			if (gameObjects.Length > 1)
			{
				for (int i = 0; i < gameObjects.Length; i++)
				{
					if (((GameObject)gameObjects[i]).GetComponent<TextMeshPro>() == null)
					{
						return true;
					}
				}
			}
			return false;
		}

		private string[] GetMaterialPresets()
		{
			TMP_FontAsset tmp_FontAsset = this.fontAsset_prop.objectReferenceValue as TMP_FontAsset;
			if (tmp_FontAsset == null)
			{
				return null;
			}
			this.m_materialPresets = TMP_EditorUtility.FindMaterialReferences(tmp_FontAsset);
			this.m_materialPresetNames = new string[this.m_materialPresets.Length];
			for (int i = 0; i < this.m_materialPresetNames.Length; i++)
			{
				this.m_materialPresetNames[i] = this.m_materialPresets[i].name;
				if (this.m_targetMaterial.GetInstanceID() == this.m_materialPresets[i].GetInstanceID())
				{
					this.m_materialPresetSelectionIndex = i;
				}
			}
			this.m_isPresetListDirty = false;
			return this.m_materialPresetNames;
		}

		private void DrawMaginProperty(SerializedProperty property, string label)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			Rect controlRect = EditorGUILayout.GetControlRect(false, 36f, new GUILayoutOption[0]);
			Rect position = new Rect(controlRect.x, controlRect.y + 2f, controlRect.width, 18f);
			float num = controlRect.width + 3f;
			position.width = labelWidth;
			GUI.Label(position, label);
			Vector4 vector4Value = property.vector4Value;
			float num2 = num - labelWidth;
			float num3 = num2 / 4f;
			position.width = num3 - 5f;
			position.x = labelWidth + 15f;
			GUI.Label(position, "Left");
			position.x += num3;
			GUI.Label(position, "Top");
			position.x += num3;
			GUI.Label(position, "Right");
			position.x += num3;
			GUI.Label(position, "Bottom");
			position.y += 18f;
			position.x = labelWidth + 15f;
			vector4Value.x = EditorGUI.FloatField(position, GUIContent.none, vector4Value.x);
			position.x += num3;
			vector4Value.y = EditorGUI.FloatField(position, GUIContent.none, vector4Value.y);
			position.x += num3;
			vector4Value.z = EditorGUI.FloatField(position, GUIContent.none, vector4Value.z);
			position.x += num3;
			vector4Value.w = EditorGUI.FloatField(position, GUIContent.none, vector4Value.w);
			property.vector4Value = vector4Value;
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
		}

		private void DrawPropertySlider(string label, SerializedProperty property)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			Rect controlRect = EditorGUILayout.GetControlRect(false, 17f, new GUILayoutOption[0]);
			GUIContent label2 = (!(label == string.Empty)) ? new GUIContent(label) : GUIContent.none;
			EditorGUI.Slider(new Rect(controlRect.x, controlRect.y, controlRect.width, controlRect.height), property, 0f, 1f, label2);
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
		}

		private void DrawDoubleEnumPopup(SerializedProperty property1, SerializedProperty property2, string label)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			Rect controlRect = EditorGUILayout.GetControlRect(false, 17f, new GUILayoutOption[0]);
			Rect rect = new Rect(controlRect.x, controlRect.y, EditorGUIUtility.labelWidth, controlRect.height);
			EditorGUI.PrefixLabel(rect, new GUIContent(label));
			rect.x += rect.width;
			rect.width = (controlRect.width - rect.x) / 2f + 5f;
			EditorGUI.PropertyField(rect, property1, GUIContent.none);
			rect.x += rect.width + 5f;
			EditorGUI.PropertyField(rect, property2, GUIContent.none);
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
		}

		private void DrawPropertyBlock(string[] labels, SerializedProperty[] properties)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			Rect controlRect = EditorGUILayout.GetControlRect(false, 17f, new GUILayoutOption[0]);
			GUI.Label(new Rect(controlRect.x, controlRect.y, labelWidth, controlRect.height), labels[0]);
			controlRect.x = labelWidth + 15f;
			controlRect.width = (controlRect.width + 20f - controlRect.x) / (float)labels.Length;
			for (int i = 0; i < labels.Length; i++)
			{
				if (i == 0)
				{
					EditorGUIUtility.labelWidth = 20f;
					bool enabled;
					if (properties[i] == this.fontSize_prop && this.autoSizing_prop.boolValue)
					{
						bool flag = false;
						GUI.enabled = flag;
						enabled = flag;
					}
					else
					{
						bool flag = true;
						GUI.enabled = flag;
						enabled = flag;
					}
					GUI.enabled = enabled;
					EditorGUI.PropertyField(new Rect(controlRect.x - 20f, controlRect.y, 80f, controlRect.height), properties[i], new GUIContent("  "));
					controlRect.x += controlRect.width;
					GUI.enabled = true;
				}
				else
				{
					EditorGUIUtility.labelWidth = GUI.skin.textArea.CalcSize(new GUIContent(labels[i])).x;
					EditorGUI.PropertyField(new Rect(controlRect.x, controlRect.y, controlRect.width - 5f, controlRect.height), properties[i], new GUIContent(labels[i]));
					controlRect.x += controlRect.width;
				}
			}
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
		}

		private void OnUndoRedo()
		{
			int currentGroup = Undo.GetCurrentGroup();
			int eventID = TMP_EditorPanel.m_eventID;
			if (currentGroup != eventID)
			{
				for (int i = 0; i < base.targets.Length; i++)
				{
					TMPro_EventManager.ON_TEXTMESHPRO_PROPERTY_CHANGED(true, base.targets[i] as TextMeshPro);
					TMP_EditorPanel.m_eventID = currentGroup;
				}
			}
		}

		private static int m_eventID;

		private static string[] uiStateLabel = new string[]
		{
			"\t- <i>Click to expand</i> -",
			"\t- <i>Click to collapse</i> -"
		};

		private const string k_UndoRedo = "UndoRedoPerformed";

		private GUIStyle toggleStyle;

		public int selAlignGrid_A;

		public int selAlignGrid_B;

		private SerializedProperty text_prop;

		private SerializedProperty isRightToLeft_prop;

		private string m_RTLText;

		private SerializedProperty fontAsset_prop;

		private SerializedProperty fontSharedMaterial_prop;

		private Material[] m_materialPresets;

		private string[] m_materialPresetNames;

		private int m_materialPresetSelectionIndex;

		private bool m_isPresetListDirty;

		private SerializedProperty fontStyle_prop;

		private SerializedProperty fontColor_prop;

		private SerializedProperty enableVertexGradient_prop;

		private SerializedProperty fontColorGradient_prop;

		private SerializedProperty fontColorGradientPreset_prop;

		private SerializedProperty overrideHtmlColor_prop;

		private SerializedProperty fontSize_prop;

		private SerializedProperty fontSizeBase_prop;

		private SerializedProperty autoSizing_prop;

		private SerializedProperty fontSizeMin_prop;

		private SerializedProperty fontSizeMax_prop;

		private SerializedProperty lineSpacingMax_prop;

		private SerializedProperty charWidthMaxAdj_prop;

		private SerializedProperty characterSpacing_prop;

		private SerializedProperty wordSpacing_prop;

		private SerializedProperty lineSpacing_prop;

		private SerializedProperty paragraphSpacing_prop;

		private SerializedProperty textAlignment_prop;

		private SerializedProperty horizontalMapping_prop;

		private SerializedProperty verticalMapping_prop;

		private SerializedProperty uvLineOffset_prop;

		private SerializedProperty enableWordWrapping_prop;

		private SerializedProperty wordWrappingRatios_prop;

		private SerializedProperty textOverflowMode_prop;

		private SerializedProperty pageToDisplay_prop;

		private SerializedProperty linkedTextComponent_prop;

		private SerializedProperty isLinkedTextComponent_prop;

		private SerializedProperty enableKerning_prop;

		private SerializedProperty inputSource_prop;

		private SerializedProperty havePropertiesChanged_prop;

		private SerializedProperty isInputPasingRequired_prop;

		private SerializedProperty isRichText_prop;

		private SerializedProperty hasFontAssetChanged_prop;

		private SerializedProperty enableExtraPadding_prop;

		private SerializedProperty checkPaddingRequired_prop;

		private SerializedProperty enableEscapeCharacterParsing_prop;

		private SerializedProperty useMaxVisibleDescender_prop;

		private SerializedProperty isVolumetricText_prop;

		private SerializedProperty geometrySortingOrder_prop;

		private SerializedProperty spriteAsset_prop;

		private SerializedProperty isOrthographic_prop;

		private bool havePropertiesChanged;

		private TextMeshPro m_textComponent;

		private RectTransform m_rectTransform;

		private Material m_targetMaterial;

		private SerializedProperty margin_prop;

		private Vector3[] m_rectCorners = new Vector3[4];

		private Vector3[] handlePoints = new Vector3[4];

		[CompilerGenerated]
		private static Handles.CapFunction cache0;

		[CompilerGenerated]
		private static Handles.CapFunction cache1;

		[CompilerGenerated]
		private static Handles.CapFunction cache2;

		[CompilerGenerated]
		private static Handles.CapFunction cache3;

		private struct m_foldout
		{
			public static bool textInput = true;

			public static bool fontSettings = true;

			public static bool extraSettings;

			public static bool shadowSetting;
		}
	}
}
