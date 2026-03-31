using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_FontAsset))]
	public class TMP_FontAssetEditor : Editor
	{
		public void OnEnable()
		{
			this.font_atlas_prop = base.serializedObject.FindProperty("atlas");
			this.font_material_prop = base.serializedObject.FindProperty("material");
			this.fontWeights_prop = base.serializedObject.FindProperty("fontWeights");
			this.m_list = new ReorderableList(base.serializedObject, base.serializedObject.FindProperty("fallbackFontAssets"), true, true, true, true);
			this.m_list.drawElementCallback = delegate(Rect rect, int index, bool isActive, bool isFocused)
			{
				SerializedProperty arrayElementAtIndex = this.m_list.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2f;
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), arrayElementAtIndex, GUIContent.none);
			};
			this.m_list.drawHeaderCallback = delegate(Rect rect)
			{
				EditorGUI.LabelField(rect, "<b>Fallback Font Asset List</b>", TMP_UIStyleManager.Label);
			};
			this.font_normalStyle_prop = base.serializedObject.FindProperty("normalStyle");
			this.font_normalSpacing_prop = base.serializedObject.FindProperty("normalSpacingOffset");
			this.font_boldStyle_prop = base.serializedObject.FindProperty("boldStyle");
			this.font_boldSpacing_prop = base.serializedObject.FindProperty("boldSpacing");
			this.font_italicStyle_prop = base.serializedObject.FindProperty("italicStyle");
			this.font_tabSize_prop = base.serializedObject.FindProperty("tabSize");
			this.m_fontInfo_prop = base.serializedObject.FindProperty("m_fontInfo");
			this.m_glyphInfoList_prop = base.serializedObject.FindProperty("m_glyphInfoList");
			this.m_kerningInfo_prop = base.serializedObject.FindProperty("m_kerningInfo");
			this.m_kerningPair_prop = base.serializedObject.FindProperty("m_kerningPair");
			this.m_fontAsset = (base.target as TMP_FontAsset);
			this.m_kerningTable = this.m_fontAsset.kerningInfo;
			this.m_materialPresets = TMP_EditorUtility.FindMaterialReferences(this.m_fontAsset);
			TMP_UIStyleManager.GetUIStyles();
			this.m_searchList = new List<int>();
		}

		public override void OnInspectorGUI()
		{
			Event current = Event.current;
			base.serializedObject.Update();
			GUILayout.Label("<b>TextMesh Pro! Font Asset</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			GUILayout.Label("Face Info", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			EditorGUI.indentLevel = 1;
			GUI.enabled = false;
			float num = 150f;
			EditorGUIUtility.labelWidth = num;
			float labelWidth = num;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("Name"), new GUIContent("Font Source"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("PointSize"), new GUILayoutOption[0]);
			GUI.enabled = true;
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("Scale"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("LineHeight"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("Ascender"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("CapHeight"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("Baseline"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("Descender"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("Underline"), new GUIContent("Underline Offset"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("strikethrough"), new GUIContent("Strikethrough Offset"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("SuperscriptOffset"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("SubscriptOffset"), new GUILayoutOption[0]);
			SerializedProperty serializedProperty = this.m_fontInfo_prop.FindPropertyRelative("SubSize");
			EditorGUILayout.PropertyField(serializedProperty, new GUIContent("Super / Subscript Size"), new GUILayoutOption[0]);
			serializedProperty.floatValue = Mathf.Clamp(serializedProperty.floatValue, 0.25f, 1f);
			GUI.enabled = false;
			EditorGUI.indentLevel = 1;
			GUILayout.Space(18f);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("Padding"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("AtlasWidth"), new GUIContent("Width"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_fontInfo_prop.FindPropertyRelative("AtlasHeight"), new GUIContent("Height"), new GUILayoutOption[0]);
			GUI.enabled = true;
			EditorGUI.indentLevel = 0;
			GUILayout.Space(20f);
			GUILayout.Label("Font Sub-Assets", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			GUI.enabled = false;
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField(this.font_atlas_prop, new GUIContent("Font Atlas:"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.font_material_prop, new GUIContent("Font Material:"), new GUILayoutOption[0]);
			GUI.enabled = true;
			string commandName = Event.current.commandName;
			EditorGUI.indentLevel = 0;
			if (GUILayout.Button("Font Weights\t" + ((!TMP_FontAssetEditor.UI_PanelState.fontWeightPanel) ? this.uiStateLabel[0] : this.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_FontAssetEditor.UI_PanelState.fontWeightPanel = !TMP_FontAssetEditor.UI_PanelState.fontWeightPanel;
			}
			if (TMP_FontAssetEditor.UI_PanelState.fontWeightPanel)
			{
				EditorGUIUtility.labelWidth = 120f;
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
				EditorGUI.indentLevel = 0;
				GUILayout.Label("Select the Font Assets that will be used for the following font weights.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("<b>Font Weight</b>", TMP_UIStyleManager.Label, new GUILayoutOption[]
				{
					GUILayout.Width(117f)
				});
				GUILayout.Label("<b>Normal Style</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Label("<b>Italic Style</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(this.fontWeights_prop.GetArrayElementAtIndex(4), new GUIContent("400 - Regular"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.fontWeights_prop.GetArrayElementAtIndex(7), new GUIContent("700 - Bold"), new GUILayoutOption[0]);
				EditorGUILayout.EndVertical();
				EditorGUIUtility.labelWidth = 120f;
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
				GUILayout.Label("Settings used to simulate a typeface when no font asset is available.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.font_normalStyle_prop, new GUIContent("Normal Weight"), new GUILayoutOption[0]);
				this.font_normalStyle_prop.floatValue = Mathf.Clamp(this.font_normalStyle_prop.floatValue, -3f, 3f);
				if (GUI.changed || commandName == "UndoRedoPerformed")
				{
					GUI.changed = false;
					for (int i = 0; i < this.m_materialPresets.Length; i++)
					{
						this.m_materialPresets[i].SetFloat("_WeightNormal", this.font_normalStyle_prop.floatValue);
					}
				}
				EditorGUILayout.PropertyField(this.font_boldStyle_prop, new GUIContent("Bold Weight"), new GUILayoutOption[]
				{
					GUILayout.MinWidth(100f)
				});
				this.font_boldStyle_prop.floatValue = Mathf.Clamp(this.font_boldStyle_prop.floatValue, -3f, 3f);
				if (GUI.changed || commandName == "UndoRedoPerformed")
				{
					GUI.changed = false;
					for (int j = 0; j < this.m_materialPresets.Length; j++)
					{
						this.m_materialPresets[j].SetFloat("_WeightBold", this.font_boldStyle_prop.floatValue);
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.font_normalSpacing_prop, new GUIContent("Spacing Offset"), new GUILayoutOption[0]);
				this.font_normalSpacing_prop.floatValue = Mathf.Clamp(this.font_normalSpacing_prop.floatValue, -100f, 100f);
				if (GUI.changed || commandName == "UndoRedoPerformed")
				{
					GUI.changed = false;
				}
				EditorGUILayout.PropertyField(this.font_boldSpacing_prop, new GUIContent("Bold Spacing"), new GUILayoutOption[0]);
				this.font_boldSpacing_prop.floatValue = Mathf.Clamp(this.font_boldSpacing_prop.floatValue, 0f, 100f);
				if (GUI.changed || commandName == "UndoRedoPerformed")
				{
					GUI.changed = false;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.font_italicStyle_prop, new GUIContent("Italic Style: "), new GUILayoutOption[0]);
				this.font_italicStyle_prop.intValue = Mathf.Clamp(this.font_italicStyle_prop.intValue, 15, 60);
				EditorGUILayout.PropertyField(this.font_tabSize_prop, new GUIContent("Tab Multiple: "), new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			GUILayout.Space(5f);
			EditorGUI.indentLevel = 0;
			if (GUILayout.Button("Fallback Font Assets\t" + ((!TMP_FontAssetEditor.UI_PanelState.fallbackFontAssetPanel) ? this.uiStateLabel[0] : this.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_FontAssetEditor.UI_PanelState.fallbackFontAssetPanel = !TMP_FontAssetEditor.UI_PanelState.fallbackFontAssetPanel;
			}
			if (TMP_FontAssetEditor.UI_PanelState.fallbackFontAssetPanel)
			{
				EditorGUIUtility.labelWidth = 120f;
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
				EditorGUI.indentLevel = 0;
				GUILayout.Label("Select the Font Assets that will be searched and used as fallback when characters are missing from this font asset.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				this.m_list.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
			GUILayout.Space(5f);
			EditorGUI.indentLevel = 0;
			if (GUILayout.Button("Glyph Info\t" + ((!TMP_FontAssetEditor.UI_PanelState.glyphInfoPanel) ? this.uiStateLabel[0] : this.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_FontAssetEditor.UI_PanelState.glyphInfoPanel = !TMP_FontAssetEditor.UI_PanelState.glyphInfoPanel;
			}
			if (TMP_FontAssetEditor.UI_PanelState.glyphInfoPanel)
			{
				int num2 = this.m_glyphInfoList_prop.arraySize;
				int num3 = 15;
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUIUtility.labelWidth = 110f;
				EditorGUI.BeginChangeCheck();
				string text = EditorGUILayout.TextField("Glyph Search", this.m_searchPattern, "SearchTextField", new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck() || this.m_isSearchDirty)
				{
					if (!string.IsNullOrEmpty(text))
					{
						this.m_searchPattern = text;
						this.SearchGlyphTable(this.m_searchPattern, ref this.m_searchList);
					}
					this.m_isSearchDirty = false;
				}
				string str = (!string.IsNullOrEmpty(this.m_searchPattern)) ? "SearchCancelButton" : "SearchCancelButtonEmpty";
				if (GUILayout.Button(GUIContent.none, str, new GUILayoutOption[0]))
				{
					GUIUtility.keyboardControl = 0;
					this.m_searchPattern = string.Empty;
				}
				EditorGUILayout.EndHorizontal();
				if (!string.IsNullOrEmpty(this.m_searchPattern))
				{
					num2 = this.m_searchList.Count;
				}
				this.DisplayGlyphPageNavigation(num2, num3);
				EditorGUILayout.EndVertical();
				if (num2 > 0)
				{
					int num4 = num3 * this.m_GlyphPage;
					while (num4 < num2 && num4 < num3 * (this.m_GlyphPage + 1))
					{
						Rect rect = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(true)
						});
						int num5 = num4;
						if (!string.IsNullOrEmpty(this.m_searchPattern))
						{
							num5 = this.m_searchList[num4];
						}
						SerializedProperty arrayElementAtIndex = this.m_glyphInfoList_prop.GetArrayElementAtIndex(num5);
						EditorGUI.BeginDisabledGroup(num4 != this.m_selectedElement);
						EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(arrayElementAtIndex, new GUILayoutOption[0]);
						EditorGUILayout.EndVertical();
						EditorGUI.EndDisabledGroup();
						Rect rect2 = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(true)
						});
						Rect rect3 = new Rect(rect.x, rect.y, rect2.width, rect2.y - rect.y);
						if (this.DoSelectionCheck(rect3))
						{
							this.m_selectedElement = num4;
							this.m_AddGlyphWarning.isEnabled = false;
							this.m_unicodeHexLabel = "<i>Unicode Hex ID</i>";
							GUIUtility.keyboardControl = 0;
						}
						if (this.m_selectedElement == num4)
						{
							TMP_EditorUtility.DrawBox(rect3, 2f, new Color32(40, 192, byte.MaxValue, byte.MaxValue));
							Rect controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 1f, new GUILayoutOption[0]);
							float num6 = controlRect.width * 0.6f;
							float num7 = num6 / 3f;
							Rect position = new Rect(controlRect.x + controlRect.width * 0.4f, controlRect.y, num7, controlRect.height);
							GUI.enabled = !string.IsNullOrEmpty(this.m_dstGlyphID);
							if (GUI.Button(position, new GUIContent("Copy to")))
							{
								GUIUtility.keyboardControl = 0;
								int dstGlyphID = TMP_TextUtilities.StringToInt(this.m_dstGlyphID);
								if (!this.AddNewGlyph(num5, dstGlyphID))
								{
									this.m_AddGlyphWarning.isEnabled = true;
									this.m_AddGlyphWarning.expirationTime = EditorApplication.timeSinceStartup + 1.0;
								}
								this.m_dstGlyphID = string.Empty;
								this.m_isSearchDirty = true;
								TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, this.m_fontAsset);
							}
							GUI.enabled = true;
							position.x += num7;
							GUI.SetNextControlName("GlyphID_Input");
							this.m_dstGlyphID = EditorGUI.TextField(position, this.m_dstGlyphID);
							EditorGUI.LabelField(position, new GUIContent(this.m_unicodeHexLabel, "The Unicode (Hex) ID of the duplicated Glyph"), TMP_UIStyleManager.Label);
							if (GUI.GetNameOfFocusedControl() == "GlyphID_Input")
							{
								this.m_unicodeHexLabel = string.Empty;
								char character = Event.current.character;
								if ((character < '0' || character > '9') && (character < 'a' || character > 'f') && (character < 'A' || character > 'F'))
								{
									Event.current.character = '\0';
								}
							}
							else
							{
								this.m_unicodeHexLabel = "<i>Unicode Hex ID</i>";
							}
							position.x += num7;
							if (GUI.Button(position, "Remove"))
							{
								GUIUtility.keyboardControl = 0;
								this.RemoveGlyphFromList(num5);
								this.m_selectedElement = -1;
								this.m_isSearchDirty = true;
								TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, this.m_fontAsset);
								return;
							}
							if (this.m_AddGlyphWarning.isEnabled && EditorApplication.timeSinceStartup < this.m_AddGlyphWarning.expirationTime)
							{
								EditorGUILayout.HelpBox("The Destination Glyph ID already exists", MessageType.Warning);
							}
						}
						num4++;
					}
				}
				this.DisplayGlyphPageNavigation(num2, num3);
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Kerning Table Info\t" + ((!TMP_FontAssetEditor.UI_PanelState.kerningInfoPanel) ? this.uiStateLabel[0] : this.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_FontAssetEditor.UI_PanelState.kerningInfoPanel = !TMP_FontAssetEditor.UI_PanelState.kerningInfoPanel;
			}
			if (TMP_FontAssetEditor.UI_PanelState.kerningInfoPanel)
			{
				SerializedProperty serializedProperty2 = this.m_kerningInfo_prop.FindPropertyRelative("kerningPairs");
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Left Char", TMP_UIStyleManager.TMP_GUISkin.label, new GUILayoutOption[0]);
				GUILayout.Label("Right Char", TMP_UIStyleManager.TMP_GUISkin.label, new GUILayoutOption[0]);
				GUILayout.Label("Offset Value", TMP_UIStyleManager.TMP_GUISkin.label, new GUILayoutOption[0]);
				GUILayout.Label(GUIContent.none, new GUILayoutOption[]
				{
					GUILayout.Width(20f)
				});
				EditorGUILayout.EndHorizontal();
				GUILayout.BeginVertical(TMP_UIStyleManager.TMP_GUISkin.label, new GUILayoutOption[0]);
				int arraySize = serializedProperty2.arraySize;
				int num8 = 25;
				Rect rect4;
				if (arraySize > 0)
				{
					int num9 = num8 * this.m_KerningPage;
					while (num9 < arraySize && num9 < num8 * (this.m_KerningPage + 1))
					{
						SerializedProperty arrayElementAtIndex2 = serializedProperty2.GetArrayElementAtIndex(num9);
						rect4 = EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
						EditorGUI.PropertyField(new Rect(rect4.x, rect4.y, rect4.width - 20f, rect4.height), arrayElementAtIndex2, GUIContent.none);
						if (GUILayout.Button("-", new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(false)
						}))
						{
							this.m_kerningTable.RemoveKerningPair(num9);
							this.m_fontAsset.ReadFontDefinition();
							base.serializedObject.Update();
							this.isAssetDirty = true;
							break;
						}
						EditorGUILayout.EndHorizontal();
						num9++;
					}
				}
				Rect controlRect2 = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
				controlRect2.width /= 3f;
				int num10 = (!current.shift) ? 1 : 10;
				if (this.m_KerningPage > 0)
				{
					GUI.enabled = true;
				}
				else
				{
					GUI.enabled = false;
				}
				if (GUI.Button(controlRect2, "Previous Page"))
				{
					this.m_KerningPage -= num10;
				}
				GUI.enabled = true;
				controlRect2.x += controlRect2.width;
				int num11 = (int)((float)arraySize / (float)num8 + 0.999f);
				GUI.Label(controlRect2, string.Concat(new object[]
				{
					"Page ",
					this.m_KerningPage + 1,
					" / ",
					num11
				}), GUI.skin.button);
				controlRect2.x += controlRect2.width;
				if (num8 * (this.m_GlyphPage + 1) < arraySize)
				{
					GUI.enabled = true;
				}
				else
				{
					GUI.enabled = false;
				}
				if (GUI.Button(controlRect2, "Next Page"))
				{
					this.m_KerningPage += num10;
				}
				this.m_KerningPage = Mathf.Clamp(this.m_KerningPage, 0, arraySize / num8);
				GUILayout.EndVertical();
				GUILayout.Space(10f);
				GUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
				rect4 = EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUI.PropertyField(new Rect(rect4.x, rect4.y, rect4.width - 20f, rect4.height), this.m_kerningPair_prop);
				GUILayout.Label(GUIContent.none, new GUILayoutOption[]
				{
					GUILayout.Height(19f)
				});
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(5f);
				if (GUILayout.Button("Add New Kerning Pair", new GUILayoutOption[0]))
				{
					int intValue = this.m_kerningPair_prop.FindPropertyRelative("AscII_Left").intValue;
					int intValue2 = this.m_kerningPair_prop.FindPropertyRelative("AscII_Right").intValue;
					float floatValue = this.m_kerningPair_prop.FindPropertyRelative("XadvanceOffset").floatValue;
					this.errorCode = this.m_kerningTable.AddKerningPair(intValue, intValue2, floatValue);
					if (this.errorCode != -1)
					{
						this.m_kerningTable.SortKerningPairs();
						this.m_fontAsset.ReadFontDefinition();
						base.serializedObject.Update();
						this.isAssetDirty = true;
					}
					else
					{
						this.timeStamp = DateTime.Now.AddSeconds(5.0);
					}
				}
				if (this.errorCode == -1)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					GUILayout.Label("Kerning Pair already <color=#ffff00>exists!</color>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					if (DateTime.Now > this.timeStamp)
					{
						this.errorCode = 0;
					}
				}
				GUILayout.EndVertical();
			}
			if (base.serializedObject.ApplyModifiedProperties() || commandName == "UndoRedoPerformed" || this.isAssetDirty)
			{
				TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, this.m_fontAsset);
				this.isAssetDirty = false;
				EditorUtility.SetDirty(base.target);
			}
			GUI.enabled = true;
			if (current.type == EventType.MouseDown && current.button == 0)
			{
				this.m_selectedElement = -1;
			}
		}

		private void DisplayGlyphPageNavigation(int arraySize, int itemsPerPage)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
			controlRect.width /= 3f;
			int num = (!Event.current.shift) ? 1 : 10;
			GUI.enabled = (this.m_GlyphPage > 0);
			if (GUI.Button(controlRect, "Previous Page"))
			{
				this.m_GlyphPage -= num;
			}
			GUIStyle style = new GUIStyle(GUI.skin.button)
			{
				normal = 
				{
					background = null
				}
			};
			GUI.enabled = true;
			controlRect.x += controlRect.width;
			int num2 = (int)((float)arraySize / (float)itemsPerPage + 0.999f);
			GUI.Button(controlRect, string.Concat(new object[]
			{
				"Page ",
				this.m_GlyphPage + 1,
				" / ",
				num2
			}), style);
			controlRect.x += controlRect.width;
			GUI.enabled = (itemsPerPage * (this.m_GlyphPage + 1) < arraySize);
			if (GUI.Button(controlRect, "Next Page"))
			{
				this.m_GlyphPage += num;
			}
			this.m_GlyphPage = Mathf.Clamp(this.m_GlyphPage, 0, arraySize / itemsPerPage);
			GUI.enabled = true;
		}

		private bool AddNewGlyph(int srcIndex, int dstGlyphID)
		{
			if (this.m_fontAsset.characterDictionary.ContainsKey(dstGlyphID))
			{
				return false;
			}
			this.m_glyphInfoList_prop.arraySize++;
			SerializedProperty arrayElementAtIndex = this.m_glyphInfoList_prop.GetArrayElementAtIndex(srcIndex);
			int index = this.m_glyphInfoList_prop.arraySize - 1;
			SerializedProperty arrayElementAtIndex2 = this.m_glyphInfoList_prop.GetArrayElementAtIndex(index);
			this.CopySerializedProperty(arrayElementAtIndex, ref arrayElementAtIndex2);
			arrayElementAtIndex2.FindPropertyRelative("id").intValue = dstGlyphID;
			base.serializedObject.ApplyModifiedProperties();
			this.m_fontAsset.SortGlyphs();
			this.m_fontAsset.ReadFontDefinition();
			return true;
		}

		private void RemoveGlyphFromList(int index)
		{
			if (index > this.m_glyphInfoList_prop.arraySize)
			{
				return;
			}
			this.m_glyphInfoList_prop.DeleteArrayElementAtIndex(index);
			base.serializedObject.ApplyModifiedProperties();
			this.m_fontAsset.ReadFontDefinition();
		}

		private bool DoSelectionCheck(Rect selectionArea)
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type == EventType.MouseDown)
			{
				if (selectionArea.Contains(current.mousePosition) && current.button == 0)
				{
					current.Use();
					return true;
				}
			}
			return false;
		}

		private void CopySerializedProperty(SerializedProperty source, ref SerializedProperty target)
		{
			target.FindPropertyRelative("id").intValue = source.FindPropertyRelative("id").intValue;
			target.FindPropertyRelative("x").floatValue = source.FindPropertyRelative("x").floatValue;
			target.FindPropertyRelative("y").floatValue = source.FindPropertyRelative("y").floatValue;
			target.FindPropertyRelative("width").floatValue = source.FindPropertyRelative("width").floatValue;
			target.FindPropertyRelative("height").floatValue = source.FindPropertyRelative("height").floatValue;
			target.FindPropertyRelative("xOffset").floatValue = source.FindPropertyRelative("xOffset").floatValue;
			target.FindPropertyRelative("yOffset").floatValue = source.FindPropertyRelative("yOffset").floatValue;
			target.FindPropertyRelative("xAdvance").floatValue = source.FindPropertyRelative("xAdvance").floatValue;
			target.FindPropertyRelative("scale").floatValue = source.FindPropertyRelative("scale").floatValue;
		}

		private void SearchGlyphTable(string searchPattern, ref List<int> searchResults)
		{
			if (searchResults == null)
			{
				searchResults = new List<int>();
			}
			searchResults.Clear();
			int arraySize = this.m_glyphInfoList_prop.arraySize;
			for (int i = 0; i < arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_glyphInfoList_prop.GetArrayElementAtIndex(i);
				int intValue = arrayElementAtIndex.FindPropertyRelative("id").intValue;
				if (searchPattern.Length == 1 && intValue == (int)searchPattern[0])
				{
					searchResults.Add(i);
				}
				if (intValue.ToString().Contains(searchPattern))
				{
					searchResults.Add(i);
				}
				if (intValue.ToString("x").Contains(searchPattern))
				{
					searchResults.Add(i);
				}
				if (intValue.ToString("X").Contains(searchPattern))
				{
					searchResults.Add(i);
				}
			}
		}

		private int m_GlyphPage;

		private int m_KerningPage;

		private int m_selectedElement = -1;

		private string m_dstGlyphID;

		private const string k_placeholderUnicodeHex = "<i>Unicode Hex ID</i>";

		private string m_unicodeHexLabel = "<i>Unicode Hex ID</i>";

		private TMP_FontAssetEditor.Warning m_AddGlyphWarning;

		private string m_searchPattern;

		private List<int> m_searchList;

		private bool m_isSearchDirty;

		private const string k_UndoRedo = "UndoRedoPerformed";

		private SerializedProperty font_atlas_prop;

		private SerializedProperty font_material_prop;

		private SerializedProperty fontWeights_prop;

		private ReorderableList m_list;

		private SerializedProperty font_normalStyle_prop;

		private SerializedProperty font_normalSpacing_prop;

		private SerializedProperty font_boldStyle_prop;

		private SerializedProperty font_boldSpacing_prop;

		private SerializedProperty font_italicStyle_prop;

		private SerializedProperty font_tabSize_prop;

		private SerializedProperty m_fontInfo_prop;

		private SerializedProperty m_glyphInfoList_prop;

		private SerializedProperty m_kerningInfo_prop;

		private KerningTable m_kerningTable;

		private SerializedProperty m_kerningPair_prop;

		private TMP_FontAsset m_fontAsset;

		private Material[] m_materialPresets;

		private bool isAssetDirty;

		private int errorCode;

		private DateTime timeStamp;

		private string[] uiStateLabel = new string[]
		{
			"<i>(Click to expand)</i>",
			"<i>(Click to collapse)</i>"
		};

		private struct UI_PanelState
		{
			public static bool fontInfoPanel = true;

			public static bool fontWeightPanel = true;

			public static bool fallbackFontAssetPanel = true;

			public static bool glyphInfoPanel;

			public static bool kerningInfoPanel;
		}

		private struct Warning
		{
			public bool isEnabled;

			public double expirationTime;
		}
	}
}
