using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_SpriteAsset))]
	public class TMP_SpriteAssetEditor : Editor
	{
		public void OnEnable()
		{
			this.m_spriteAtlas_prop = base.serializedObject.FindProperty("spriteSheet");
			this.m_material_prop = base.serializedObject.FindProperty("material");
			this.m_spriteInfoList_prop = base.serializedObject.FindProperty("spriteInfoList");
			this.m_fallbackSpriteAssetList = new ReorderableList(base.serializedObject, base.serializedObject.FindProperty("fallbackSpriteAssets"), true, true, true, true);
			this.m_fallbackSpriteAssetList.drawElementCallback = delegate(Rect rect, int index, bool isActive, bool isFocused)
			{
				SerializedProperty arrayElementAtIndex = this.m_fallbackSpriteAssetList.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2f;
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), arrayElementAtIndex, GUIContent.none);
			};
			this.m_fallbackSpriteAssetList.drawHeaderCallback = delegate(Rect rect)
			{
				EditorGUI.LabelField(rect, "<b>Fallback Sprite Asset List</b>", TMP_UIStyleManager.Label);
			};
			TMP_UIStyleManager.GetUIStyles();
		}

		public override void OnInspectorGUI()
		{
			Event current = Event.current;
			string commandName = current.commandName;
			base.serializedObject.Update();
			EditorGUIUtility.labelWidth = 135f;
			GUILayout.Label("<b>TextMeshPro - Sprite Asset</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			GUILayout.Label("Sprite Info", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			EditorGUI.indentLevel = 1;
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_spriteAtlas_prop, new GUIContent("Sprite Atlas"), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				Texture2D texture2D = this.m_spriteAtlas_prop.objectReferenceValue as Texture2D;
				if (texture2D != null)
				{
					Material material = this.m_material_prop.objectReferenceValue as Material;
					if (material != null)
					{
						material.mainTexture = texture2D;
					}
				}
			}
			GUI.enabled = true;
			EditorGUILayout.PropertyField(this.m_material_prop, new GUIContent("Default Material"), new GUILayoutOption[0]);
			EditorGUI.indentLevel = 0;
			if (GUILayout.Button("Fallback Sprite Assets\t" + ((!TMP_SpriteAssetEditor.UI_PanelState.fallbackSpriteAssetPanel) ? this.uiStateLabel[0] : this.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_SpriteAssetEditor.UI_PanelState.fallbackSpriteAssetPanel = !TMP_SpriteAssetEditor.UI_PanelState.fallbackSpriteAssetPanel;
			}
			if (TMP_SpriteAssetEditor.UI_PanelState.fallbackSpriteAssetPanel)
			{
				EditorGUIUtility.labelWidth = 120f;
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
				EditorGUI.indentLevel = 0;
				GUILayout.Label("Select the Sprite Assets that will be searched and used as fallback when a given sprite is missing from this sprite asset.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				this.m_fallbackSpriteAssetList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
			GUI.enabled = true;
			GUILayout.Space(10f);
			EditorGUI.indentLevel = 0;
			if (GUILayout.Button("Sprite List\t\t" + ((!TMP_SpriteAssetEditor.UI_PanelState.spriteInfoPanel) ? this.uiStateLabel[0] : this.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_SpriteAssetEditor.UI_PanelState.spriteInfoPanel = !TMP_SpriteAssetEditor.UI_PanelState.spriteInfoPanel;
			}
			if (TMP_SpriteAssetEditor.UI_PanelState.spriteInfoPanel)
			{
				int num = this.m_spriteInfoList_prop.arraySize;
				int num2 = 10;
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUIUtility.labelWidth = 110f;
				EditorGUI.BeginChangeCheck();
				string text = EditorGUILayout.TextField("Sprite Search", this.m_searchPattern, "SearchTextField", new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck() || this.m_isSearchDirty)
				{
					if (!string.IsNullOrEmpty(text))
					{
						this.m_searchPattern = text.ToLower(CultureInfo.InvariantCulture).Trim();
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
					num = this.m_searchList.Count;
				}
				this.DisplayGlyphPageNavigation(num, num2);
				EditorGUILayout.EndVertical();
				if (num > 0)
				{
					int num3 = num2 * this.m_page;
					while (num3 < num && num3 < num2 * (this.m_page + 1))
					{
						Rect rect = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(true)
						});
						int index = num3;
						if (!string.IsNullOrEmpty(this.m_searchPattern))
						{
							index = this.m_searchList[num3];
						}
						SerializedProperty arrayElementAtIndex = this.m_spriteInfoList_prop.GetArrayElementAtIndex(index);
						EditorGUI.BeginDisabledGroup(num3 != this.m_selectedElement);
						EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label, new GUILayoutOption[]
						{
							GUILayout.Height(75f)
						});
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
							this.m_selectedElement = num3;
							GUIUtility.keyboardControl = 0;
						}
						if (this.m_selectedElement == num3)
						{
							TMP_EditorUtility.DrawBox(rect3, 2f, new Color32(40, 192, byte.MaxValue, byte.MaxValue));
							Rect controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 1f, new GUILayoutOption[0]);
							controlRect.width /= 8f;
							bool enabled = GUI.enabled;
							if (num3 == 0)
							{
								GUI.enabled = false;
							}
							if (GUI.Button(controlRect, "Up"))
							{
								this.SwapSpriteElement(num3, num3 - 1);
							}
							GUI.enabled = enabled;
							controlRect.x += controlRect.width;
							if (num3 == num - 1)
							{
								GUI.enabled = false;
							}
							if (GUI.Button(controlRect, "Down"))
							{
								this.SwapSpriteElement(num3, num3 + 1);
							}
							GUI.enabled = enabled;
							controlRect.x += controlRect.width * 2f;
							this.m_moveToIndex = EditorGUI.IntField(controlRect, this.m_moveToIndex);
							controlRect.x -= controlRect.width;
							if (GUI.Button(controlRect, "Goto"))
							{
								this.MoveSpriteElement(num3, this.m_moveToIndex);
							}
							GUI.enabled = enabled;
							controlRect.x += controlRect.width * 4f;
							if (GUI.Button(controlRect, "+"))
							{
								this.m_spriteInfoList_prop.arraySize++;
								int num4 = this.m_spriteInfoList_prop.arraySize - 1;
								SerializedProperty arrayElementAtIndex2 = this.m_spriteInfoList_prop.GetArrayElementAtIndex(num4);
								this.CopySerializedProperty(this.m_spriteInfoList_prop.GetArrayElementAtIndex(index), ref arrayElementAtIndex2);
								arrayElementAtIndex2.FindPropertyRelative("id").intValue = num4;
								base.serializedObject.ApplyModifiedProperties();
								this.m_isSearchDirty = true;
							}
							controlRect.x += controlRect.width;
							if (this.m_selectedElement == -1)
							{
								GUI.enabled = false;
							}
							if (GUI.Button(controlRect, "-"))
							{
								this.m_spriteInfoList_prop.DeleteArrayElementAtIndex(index);
								this.m_selectedElement = -1;
								base.serializedObject.ApplyModifiedProperties();
								this.m_isSearchDirty = true;
								return;
							}
						}
						num3++;
					}
				}
				this.DisplayGlyphPageNavigation(num, num2);
				EditorGUIUtility.labelWidth = 40f;
				EditorGUIUtility.fieldWidth = 20f;
				GUILayout.Space(5f);
				GUI.enabled = true;
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label, new GUILayoutOption[0]);
				Rect controlRect2 = EditorGUILayout.GetControlRect(false, 40f, new GUILayoutOption[0]);
				float num5 = (controlRect2.width - 75f) / 4f;
				EditorGUI.LabelField(controlRect2, "Global Offsets & Scale", EditorStyles.boldLabel);
				controlRect2.x += 70f;
				bool changed = GUI.changed;
				GUI.changed = false;
				this.m_xOffset = EditorGUI.FloatField(new Rect(controlRect2.x + 5f + num5 * 0f, controlRect2.y + 20f, num5 - 5f, 18f), new GUIContent("OX:"), this.m_xOffset);
				if (GUI.changed)
				{
					this.UpdateGlobalProperty("xOffset", this.m_xOffset);
				}
				this.m_yOffset = EditorGUI.FloatField(new Rect(controlRect2.x + 5f + num5 * 1f, controlRect2.y + 20f, num5 - 5f, 18f), new GUIContent("OY:"), this.m_yOffset);
				if (GUI.changed)
				{
					this.UpdateGlobalProperty("yOffset", this.m_yOffset);
				}
				this.m_xAdvance = EditorGUI.FloatField(new Rect(controlRect2.x + 5f + num5 * 2f, controlRect2.y + 20f, num5 - 5f, 18f), new GUIContent("ADV."), this.m_xAdvance);
				if (GUI.changed)
				{
					this.UpdateGlobalProperty("xAdvance", this.m_xAdvance);
				}
				this.m_scale = EditorGUI.FloatField(new Rect(controlRect2.x + 5f + num5 * 3f, controlRect2.y + 20f, num5 - 5f, 18f), new GUIContent("SF."), this.m_scale);
				if (GUI.changed)
				{
					this.UpdateGlobalProperty("scale", this.m_scale);
				}
				EditorGUILayout.EndVertical();
				GUI.changed = changed;
			}
			if (base.serializedObject.ApplyModifiedProperties() || commandName == "UndoRedoPerformed" || this.isAssetDirty)
			{
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
			GUI.enabled = (this.m_page > 0);
			if (GUI.Button(controlRect, "Previous Page"))
			{
				this.m_page -= num;
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
				this.m_page + 1,
				" / ",
				num2
			}), style);
			controlRect.x += controlRect.width;
			GUI.enabled = (itemsPerPage * (this.m_page + 1) < arraySize);
			if (GUI.Button(controlRect, "Next Page"))
			{
				this.m_page += num;
			}
			this.m_page = Mathf.Clamp(this.m_page, 0, arraySize / itemsPerPage);
			GUI.enabled = true;
		}

		private void UpdateGlobalProperty(string property, float value)
		{
			int arraySize = this.m_spriteInfoList_prop.arraySize;
			for (int i = 0; i < arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_spriteInfoList_prop.GetArrayElementAtIndex(i);
				arrayElementAtIndex.FindPropertyRelative(property).floatValue = value;
			}
			GUI.changed = false;
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

		private void SwapSpriteElement(int selectedIndex, int newIndex)
		{
			this.m_spriteInfoList_prop.MoveArrayElement(selectedIndex, newIndex);
			this.m_spriteInfoList_prop.GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("id").intValue = selectedIndex;
			this.m_spriteInfoList_prop.GetArrayElementAtIndex(newIndex).FindPropertyRelative("id").intValue = newIndex;
			this.m_selectedElement = newIndex;
			this.m_isSearchDirty = true;
		}

		private void MoveSpriteElement(int selectedIndex, int newIndex)
		{
			this.m_spriteInfoList_prop.MoveArrayElement(selectedIndex, newIndex);
			for (int i = 0; i < this.m_spriteInfoList_prop.arraySize; i++)
			{
				this.m_spriteInfoList_prop.GetArrayElementAtIndex(i).FindPropertyRelative("id").intValue = i;
			}
			this.m_selectedElement = newIndex;
			this.m_isSearchDirty = true;
		}

		private void CopySerializedProperty(SerializedProperty source, ref SerializedProperty target)
		{
			target.FindPropertyRelative("name").stringValue = source.FindPropertyRelative("name").stringValue;
			target.FindPropertyRelative("hashCode").intValue = source.FindPropertyRelative("hashCode").intValue;
			target.FindPropertyRelative("x").floatValue = source.FindPropertyRelative("x").floatValue;
			target.FindPropertyRelative("y").floatValue = source.FindPropertyRelative("y").floatValue;
			target.FindPropertyRelative("width").floatValue = source.FindPropertyRelative("width").floatValue;
			target.FindPropertyRelative("height").floatValue = source.FindPropertyRelative("height").floatValue;
			target.FindPropertyRelative("xOffset").floatValue = source.FindPropertyRelative("xOffset").floatValue;
			target.FindPropertyRelative("yOffset").floatValue = source.FindPropertyRelative("yOffset").floatValue;
			target.FindPropertyRelative("xAdvance").floatValue = source.FindPropertyRelative("xAdvance").floatValue;
			target.FindPropertyRelative("scale").floatValue = source.FindPropertyRelative("scale").floatValue;
			target.FindPropertyRelative("sprite").objectReferenceValue = source.FindPropertyRelative("sprite").objectReferenceValue;
		}

		private void SearchGlyphTable(string searchPattern, ref List<int> searchResults)
		{
			if (searchResults == null)
			{
				searchResults = new List<int>();
			}
			searchResults.Clear();
			int arraySize = this.m_spriteInfoList_prop.arraySize;
			for (int i = 0; i < arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_spriteInfoList_prop.GetArrayElementAtIndex(i);
				if (arrayElementAtIndex.FindPropertyRelative("id").intValue.ToString().Contains(searchPattern))
				{
					searchResults.Add(i);
				}
				string text = arrayElementAtIndex.FindPropertyRelative("name").stringValue.ToLower(CultureInfo.InvariantCulture).Trim();
				if (text.Contains(searchPattern))
				{
					searchResults.Add(i);
				}
			}
		}

		private int m_moveToIndex;

		private int m_selectedElement = -1;

		private int m_page;

		private const string k_UndoRedo = "UndoRedoPerformed";

		private string m_searchPattern;

		private List<int> m_searchList;

		private bool m_isSearchDirty;

		private SerializedProperty m_spriteAtlas_prop;

		private SerializedProperty m_material_prop;

		private SerializedProperty m_spriteInfoList_prop;

		private ReorderableList m_fallbackSpriteAssetList;

		private bool isAssetDirty;

		private string[] uiStateLabel = new string[]
		{
			"<i>(Click to expand)</i>",
			"<i>(Click to collapse)</i>"
		};

		private float m_xOffset;

		private float m_yOffset;

		private float m_xAdvance;

		private float m_scale;

		private struct UI_PanelState
		{
			public static bool spriteAssetInfoPanel = true;

			public static bool fallbackSpriteAssetPanel = true;

			public static bool spriteInfoPanel;
		}
	}
}
