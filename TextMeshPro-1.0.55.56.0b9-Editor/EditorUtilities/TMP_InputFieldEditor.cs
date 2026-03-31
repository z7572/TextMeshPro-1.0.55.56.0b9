using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;

namespace TMPro.EditorUtilities
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TMP_InputField), true)]
	public class TMP_InputFieldEditor : SelectableEditor
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_TextViewport = base.serializedObject.FindProperty("m_TextViewport");
			this.m_TextComponent = base.serializedObject.FindProperty("m_TextComponent");
			this.m_Text = base.serializedObject.FindProperty("m_Text");
			this.m_ContentType = base.serializedObject.FindProperty("m_ContentType");
			this.m_LineType = base.serializedObject.FindProperty("m_LineType");
			this.m_InputType = base.serializedObject.FindProperty("m_InputType");
			this.m_CharacterValidation = base.serializedObject.FindProperty("m_CharacterValidation");
			this.m_InputValidator = base.serializedObject.FindProperty("m_InputValidator");
			this.m_RegexValue = base.serializedObject.FindProperty("m_RegexValue");
			this.m_KeyboardType = base.serializedObject.FindProperty("m_KeyboardType");
			this.m_CharacterLimit = base.serializedObject.FindProperty("m_CharacterLimit");
			this.m_CaretBlinkRate = base.serializedObject.FindProperty("m_CaretBlinkRate");
			this.m_CaretWidth = base.serializedObject.FindProperty("m_CaretWidth");
			this.m_CaretColor = base.serializedObject.FindProperty("m_CaretColor");
			this.m_CustomCaretColor = base.serializedObject.FindProperty("m_CustomCaretColor");
			this.m_SelectionColor = base.serializedObject.FindProperty("m_SelectionColor");
			this.m_HideMobileInput = base.serializedObject.FindProperty("m_HideMobileInput");
			this.m_Placeholder = base.serializedObject.FindProperty("m_Placeholder");
			this.m_VerticalScrollbar = base.serializedObject.FindProperty("m_VerticalScrollbar");
			this.m_ScrollbarScrollSensitivity = base.serializedObject.FindProperty("m_ScrollSensitivity");
			this.m_OnValueChanged = base.serializedObject.FindProperty("m_OnValueChanged");
			this.m_OnEndEdit = base.serializedObject.FindProperty("m_OnEndEdit");
			this.m_OnSelect = base.serializedObject.FindProperty("m_OnSelect");
			this.m_OnDeselect = base.serializedObject.FindProperty("m_OnDeselect");
			this.m_ReadOnly = base.serializedObject.FindProperty("m_ReadOnly");
			this.m_RichText = base.serializedObject.FindProperty("m_RichText");
			this.m_RichTextEditingAllowed = base.serializedObject.FindProperty("m_isRichTextEditingAllowed");
			this.m_ResetOnDeActivation = base.serializedObject.FindProperty("m_ResetOnDeActivation");
			this.m_RestoreOriginalTextOnEscape = base.serializedObject.FindProperty("m_RestoreOriginalTextOnEscape");
			this.m_OnFocusSelectAll = base.serializedObject.FindProperty("m_OnFocusSelectAll");
			this.m_GlobalPointSize = base.serializedObject.FindProperty("m_GlobalPointSize");
			this.m_GlobalFontAsset = base.serializedObject.FindProperty("m_GlobalFontAsset");
			this.m_CustomColor = new AnimBool(this.m_CustomCaretColor.boolValue);
			this.m_CustomColor.valueChanged.AddListener(new UnityAction(base.Repaint));
			TMP_UIStyleManager.GetUIStyles();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_CustomColor.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_TextViewport, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_TextComponent, new GUILayoutOption[0]);
			TextMeshProUGUI textMeshProUGUI = null;
			if (this.m_TextComponent != null && this.m_TextComponent.objectReferenceValue != null)
			{
				textMeshProUGUI = (this.m_TextComponent.objectReferenceValue as TextMeshProUGUI);
			}
			EditorGUI.BeginDisabledGroup(this.m_TextComponent == null || this.m_TextComponent.objectReferenceValue == null);
			Rect controlRect = EditorGUILayout.GetControlRect(false, 25f, new GUILayoutOption[0]);
			EditorGUIUtility.labelWidth = 130f;
			controlRect.y += 2f;
			GUI.Label(controlRect, "<b>TEXT INPUT BOX</b>" + ((!TMP_InputFieldEditor.m_foldout.textInput) ? TMP_InputFieldEditor.uiStateLabel[0] : TMP_InputFieldEditor.uiStateLabel[1]), TMP_UIStyleManager.Section_Label);
			if (GUI.Button(new Rect(controlRect.x, controlRect.y, controlRect.width - 150f, controlRect.height), GUIContent.none, GUI.skin.label))
			{
				TMP_InputFieldEditor.m_foldout.textInput = !TMP_InputFieldEditor.m_foldout.textInput;
			}
			if (TMP_InputFieldEditor.m_foldout.textInput)
			{
				EditorGUI.BeginChangeCheck();
				this.m_Text.stringValue = EditorGUILayout.TextArea(this.m_Text.stringValue, TMP_UIStyleManager.TextAreaBoxEditor, new GUILayoutOption[]
				{
					GUILayout.Height(125f),
					GUILayout.ExpandWidth(true)
				});
			}
			if (GUILayout.Button("<b>INPUT FIELD SETTINGS</b>" + ((!TMP_InputFieldEditor.m_foldout.fontSettings) ? TMP_InputFieldEditor.uiStateLabel[0] : TMP_InputFieldEditor.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_InputFieldEditor.m_foldout.fontSettings = !TMP_InputFieldEditor.m_foldout.fontSettings;
			}
			if (TMP_InputFieldEditor.m_foldout.fontSettings)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_GlobalFontAsset, new GUIContent("Font Asset", "Set the Font Asset for both Placeholder and Input Field text object."), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					TMP_InputField tmp_InputField = base.target as TMP_InputField;
					tmp_InputField.SetGlobalFontAsset(this.m_GlobalFontAsset.objectReferenceValue as TMP_FontAsset);
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_GlobalPointSize, new GUIContent("Point Size", "Set the point size of both Placeholder and Input Field text object."), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					TMP_InputField tmp_InputField2 = base.target as TMP_InputField;
					tmp_InputField2.SetGlobalPointSize(this.m_GlobalPointSize.floatValue);
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_CharacterLimit, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_ContentType, new GUILayoutOption[0]);
				if (!this.m_ContentType.hasMultipleDifferentValues)
				{
					EditorGUI.indentLevel++;
					if (this.m_ContentType.enumValueIndex == 0 || this.m_ContentType.enumValueIndex == 1 || this.m_ContentType.enumValueIndex == 9)
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_LineType, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck() && textMeshProUGUI != null)
						{
							if (this.m_LineType.enumValueIndex == 0)
							{
								textMeshProUGUI.enableWordWrapping = false;
							}
							else
							{
								textMeshProUGUI.enableWordWrapping = true;
							}
						}
					}
					if (this.m_ContentType.enumValueIndex == 9)
					{
						EditorGUILayout.PropertyField(this.m_InputType, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_KeyboardType, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_CharacterValidation, new GUILayoutOption[0]);
						if (this.m_CharacterValidation.enumValueIndex == 6)
						{
							EditorGUILayout.PropertyField(this.m_RegexValue, new GUILayoutOption[0]);
						}
						else if (this.m_CharacterValidation.enumValueIndex == 8)
						{
							EditorGUILayout.PropertyField(this.m_InputValidator, new GUILayoutOption[0]);
						}
					}
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_Placeholder, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_VerticalScrollbar, new GUILayoutOption[0]);
				if (this.m_VerticalScrollbar.objectReferenceValue != null)
				{
					EditorGUILayout.PropertyField(this.m_ScrollbarScrollSensitivity, new GUILayoutOption[0]);
				}
				EditorGUILayout.PropertyField(this.m_CaretBlinkRate, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CaretWidth, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CustomCaretColor, new GUILayoutOption[0]);
				this.m_CustomColor.target = this.m_CustomCaretColor.boolValue;
				if (EditorGUILayout.BeginFadeGroup(this.m_CustomColor.faded))
				{
					EditorGUILayout.PropertyField(this.m_CaretColor, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUILayout.PropertyField(this.m_SelectionColor, new GUILayoutOption[0]);
			}
			if (GUILayout.Button("<b>CONTROL SETTINGS</b>" + ((!TMP_InputFieldEditor.m_foldout.extraSettings) ? TMP_InputFieldEditor.uiStateLabel[0] : TMP_InputFieldEditor.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_InputFieldEditor.m_foldout.extraSettings = !TMP_InputFieldEditor.m_foldout.extraSettings;
			}
			if (TMP_InputFieldEditor.m_foldout.extraSettings)
			{
				EditorGUILayout.PropertyField(this.m_OnFocusSelectAll, new GUIContent("OnFocus - Select All", "Should all the text be selected when the Input Field is selected."), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ResetOnDeActivation, new GUIContent("Reset On DeActivation", "Should the Text and Caret position be reset when Input Field is DeActivated."), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_RestoreOriginalTextOnEscape, new GUIContent("Restore On ESC Key", "Should the original text be restored when pressing ESC."), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_HideMobileInput, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ReadOnly, new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_RichText, new GUILayoutOption[0]);
				EditorGUIUtility.labelWidth = 140f;
				EditorGUILayout.PropertyField(this.m_RichTextEditingAllowed, new GUIContent("Allow Rich Text Editing"), new GUILayoutOption[0]);
				EditorGUIUtility.labelWidth = 130f;
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_OnValueChanged, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_OnEndEdit, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_OnSelect, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_OnDeselect, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			base.serializedObject.ApplyModifiedProperties();
		}

		private static string[] uiStateLabel = new string[]
		{
			"\t- <i>Click to expand</i> -",
			"\t- <i>Click to collapse</i> -"
		};

		private SerializedProperty m_TextViewport;

		private SerializedProperty m_TextComponent;

		private SerializedProperty m_Text;

		private SerializedProperty m_ContentType;

		private SerializedProperty m_LineType;

		private SerializedProperty m_InputType;

		private SerializedProperty m_CharacterValidation;

		private SerializedProperty m_InputValidator;

		private SerializedProperty m_RegexValue;

		private SerializedProperty m_KeyboardType;

		private SerializedProperty m_CharacterLimit;

		private SerializedProperty m_CaretBlinkRate;

		private SerializedProperty m_CaretWidth;

		private SerializedProperty m_CaretColor;

		private SerializedProperty m_CustomCaretColor;

		private SerializedProperty m_SelectionColor;

		private SerializedProperty m_HideMobileInput;

		private SerializedProperty m_Placeholder;

		private SerializedProperty m_VerticalScrollbar;

		private SerializedProperty m_ScrollbarScrollSensitivity;

		private SerializedProperty m_OnValueChanged;

		private SerializedProperty m_OnEndEdit;

		private SerializedProperty m_OnSelect;

		private SerializedProperty m_OnDeselect;

		private SerializedProperty m_ReadOnly;

		private SerializedProperty m_RichText;

		private SerializedProperty m_RichTextEditingAllowed;

		private SerializedProperty m_ResetOnDeActivation;

		private SerializedProperty m_RestoreOriginalTextOnEscape;

		private SerializedProperty m_OnFocusSelectAll;

		private SerializedProperty m_GlobalPointSize;

		private SerializedProperty m_GlobalFontAsset;

		private AnimBool m_CustomColor;

		private struct m_foldout
		{
			public static bool textInput = true;

			public static bool fontSettings = true;

			public static bool extraSettings = true;
		}
	}
}
