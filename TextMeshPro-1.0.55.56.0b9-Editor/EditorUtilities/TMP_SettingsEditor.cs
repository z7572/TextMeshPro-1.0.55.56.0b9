using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_Settings))]
	public class TMP_SettingsEditor : Editor
	{
		public void OnEnable()
		{
			this.prop_FontAsset = base.serializedObject.FindProperty("m_defaultFontAsset");
			this.prop_DefaultFontAssetPath = base.serializedObject.FindProperty("m_defaultFontAssetPath");
			this.prop_DefaultFontSize = base.serializedObject.FindProperty("m_defaultFontSize");
			this.prop_DefaultAutoSizeMinRatio = base.serializedObject.FindProperty("m_defaultAutoSizeMinRatio");
			this.prop_DefaultAutoSizeMaxRatio = base.serializedObject.FindProperty("m_defaultAutoSizeMaxRatio");
			this.prop_DefaultTextMeshProTextContainerSize = base.serializedObject.FindProperty("m_defaultTextMeshProTextContainerSize");
			this.prop_DefaultTextMeshProUITextContainerSize = base.serializedObject.FindProperty("m_defaultTextMeshProUITextContainerSize");
			this.prop_AutoSizeTextContainer = base.serializedObject.FindProperty("m_autoSizeTextContainer");
			this.prop_SpriteAsset = base.serializedObject.FindProperty("m_defaultSpriteAsset");
			this.prop_SpriteAssetPath = base.serializedObject.FindProperty("m_defaultSpriteAssetPath");
			this.prop_EnableEmojiSupport = base.serializedObject.FindProperty("m_enableEmojiSupport");
			this.prop_StyleSheet = base.serializedObject.FindProperty("m_defaultStyleSheet");
			this.prop_ColorGradientPresetsPath = base.serializedObject.FindProperty("m_defaultColorGradientPresetsPath");
			this.m_list = new ReorderableList(base.serializedObject, base.serializedObject.FindProperty("m_fallbackFontAssets"), true, true, true, true);
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
			this.prop_matchMaterialPreset = base.serializedObject.FindProperty("m_matchMaterialPreset");
			this.prop_WordWrapping = base.serializedObject.FindProperty("m_enableWordWrapping");
			this.prop_Kerning = base.serializedObject.FindProperty("m_enableKerning");
			this.prop_ExtraPadding = base.serializedObject.FindProperty("m_enableExtraPadding");
			this.prop_TintAllSprites = base.serializedObject.FindProperty("m_enableTintAllSprites");
			this.prop_ParseEscapeCharacters = base.serializedObject.FindProperty("m_enableParseEscapeCharacters");
			this.prop_MissingGlyphCharacter = base.serializedObject.FindProperty("m_missingGlyphCharacter");
			this.prop_WarningsDisabled = base.serializedObject.FindProperty("m_warningsDisabled");
			this.prop_LeadingCharacters = base.serializedObject.FindProperty("m_leadingCharacters");
			this.prop_FollowingCharacters = base.serializedObject.FindProperty("m_followingCharacters");
			TMP_UIStyleManager.GetUIStyles();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			GUILayout.Label("<b>TEXTMESH PRO - SETTINGS</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = 135f;
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			GUILayout.Label("<b>Default Font Asset</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Label("Select the Font Asset that will be assigned by default to newly created text objects when no Font Asset is specified.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(this.prop_FontAsset, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("The relative path to a Resources folder where the Font Assets and Material Presets are located.\nExample \"Fonts & Materials/\"", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_DefaultFontAssetPath, new GUIContent("Path:        Resources/"), new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			GUILayout.Label("<b>Fallback Font Assets</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Label("Select the Font Assets that will be searched to locate and replace missing characters from a given Font Asset.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			this.m_list.DoLayoutList();
			GUILayout.Label("<b>Fallback Material Settings</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_matchMaterialPreset, new GUIContent("Match Material Presets"), new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			GUILayout.Label("<b>New Text Object Default Settings</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Label("Default settings used by all new text objects.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			EditorGUI.BeginChangeCheck();
			GUILayout.Label("<b>Text Container Default Settings</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUIUtility.labelWidth = 150f;
			EditorGUILayout.PropertyField(this.prop_DefaultTextMeshProTextContainerSize, new GUIContent("TextMeshPro"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_DefaultTextMeshProUITextContainerSize, new GUIContent("TextMeshPro UI"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_AutoSizeTextContainer, new GUIContent("Auto Size Text Container", "Set the size of the text container to match the text."), new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("<b>Text Component Default Settings</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUIUtility.labelWidth = 150f;
			EditorGUILayout.PropertyField(this.prop_DefaultFontSize, new GUIContent("Default Font Size"), new GUILayoutOption[]
			{
				GUILayout.MinWidth(200f),
				GUILayout.MaxWidth(200f)
			});
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(new GUIContent("Text Auto Size Ratios"));
			EditorGUIUtility.labelWidth = 35f;
			EditorGUILayout.PropertyField(this.prop_DefaultAutoSizeMinRatio, new GUIContent("Min:"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_DefaultAutoSizeMaxRatio, new GUIContent("Max:"), new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = 150f;
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_WordWrapping, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_Kerning, new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_ExtraPadding, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_TintAllSprites, new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_ParseEscapeCharacters, new GUIContent("Parse Escape Sequence"), new GUILayoutOption[0]);
			EditorGUIUtility.fieldWidth = 10f;
			EditorGUILayout.PropertyField(this.prop_MissingGlyphCharacter, new GUIContent("Missing Glyph Repl."), new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = 135f;
			GUILayout.Space(10f);
			GUILayout.Label("<b>Disable warnings for missing glyphs on text objects.</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_WarningsDisabled, new GUIContent("Disable warnings"), new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			GUILayout.Label("<b>Default Sprite Asset</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Label("Select the Sprite Asset that will be assigned by default when using the <sprite> tag when no Sprite Asset is specified.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(this.prop_SpriteAsset, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			EditorGUILayout.PropertyField(this.prop_EnableEmojiSupport, new GUIContent("Enable Emoji Support", "Enables Emoji support for Touch Screen Keyboards on target devices."), new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("The relative path to a Resources folder where the Sprite Assets are located.\nExample \"Sprite Assets/\"", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_SpriteAssetPath, new GUIContent("Path:        Resources/"), new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			GUILayout.Label("<b>Default Style Sheet</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Label("Select the Style Sheet that will be used for all text objects in this project.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(this.prop_StyleSheet, new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			GUILayout.Label("<b>Color Gradient Presets</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Label("The relative path to a Resources folder where the Color Gradient Presets are located.\nExample \"Color Gradient Presets/\"", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_ColorGradientPresetsPath, new GUIContent("Path:        Resources/"), new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			GUILayout.Label("<b>Line Breaking Resources for Asian languages</b>", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Label("Select the text assets that contain the Leading and Following characters which define the rules for line breaking with Asian languages.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(this.prop_LeadingCharacters, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.prop_FollowingCharacters, new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			if (base.serializedObject.ApplyModifiedProperties())
			{
				EditorUtility.SetDirty(base.target);
				TMPro_EventManager.ON_TMP_SETTINGS_CHANGED();
			}
		}

		private SerializedProperty prop_FontAsset;

		private SerializedProperty prop_DefaultFontAssetPath;

		private SerializedProperty prop_DefaultFontSize;

		private SerializedProperty prop_DefaultAutoSizeMinRatio;

		private SerializedProperty prop_DefaultAutoSizeMaxRatio;

		private SerializedProperty prop_DefaultTextMeshProTextContainerSize;

		private SerializedProperty prop_DefaultTextMeshProUITextContainerSize;

		private SerializedProperty prop_AutoSizeTextContainer;

		private SerializedProperty prop_SpriteAsset;

		private SerializedProperty prop_SpriteAssetPath;

		private SerializedProperty prop_EnableEmojiSupport;

		private SerializedProperty prop_StyleSheet;

		private ReorderableList m_list;

		private SerializedProperty prop_ColorGradientPresetsPath;

		private SerializedProperty prop_matchMaterialPreset;

		private SerializedProperty prop_WordWrapping;

		private SerializedProperty prop_Kerning;

		private SerializedProperty prop_ExtraPadding;

		private SerializedProperty prop_TintAllSprites;

		private SerializedProperty prop_ParseEscapeCharacters;

		private SerializedProperty prop_MissingGlyphCharacter;

		private SerializedProperty prop_WarningsDisabled;

		private SerializedProperty prop_LeadingCharacters;

		private SerializedProperty prop_FollowingCharacters;
	}
}
