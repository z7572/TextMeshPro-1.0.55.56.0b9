using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TextContainer))]
	[CanEditMultipleObjects]
	public class TMPro_TextContainerEditor : Editor
	{
		private void OnEnable()
		{
			this.anchorPosition_prop = base.serializedObject.FindProperty("m_anchorPosition");
			this.pivot_prop = base.serializedObject.FindProperty("m_pivot");
			this.rectangle_prop = base.serializedObject.FindProperty("m_rect");
			this.margins_prop = base.serializedObject.FindProperty("m_margins");
			this.m_textContainer = (TextContainer)base.target;
			TMP_UIStyleManager.GetUIStyles();
		}

		private void OnDisable()
		{
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			GUILayout.Label("<b>TEXT CONTAINER</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[]
			{
				GUILayout.Height(23f)
			});
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.anchorPosition_prop, new GUILayoutOption[0]);
			if (this.anchorPosition_prop.enumValueIndex == 9)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.pivot_prop, new GUIContent("Pivot Position"), new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			this.DrawDimensionProperty(this.rectangle_prop, "Dimensions");
			this.DrawMaginProperty(this.margins_prop, "Margins");
			if (EditorGUI.EndChangeCheck())
			{
				if (this.anchorPosition_prop.enumValueIndex != 9)
				{
					this.pivot_prop.vector2Value = this.GetAnchorPosition(this.anchorPosition_prop.enumValueIndex);
				}
				this.m_textContainer.hasChanged = true;
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private void DrawDimensionProperty(SerializedProperty property, string label)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = EditorGUIUtility.fieldWidth;
			Rect controlRect = EditorGUILayout.GetControlRect(false, 18f, new GUILayoutOption[0]);
			Rect position = new Rect(controlRect.x, controlRect.y + 2f, controlRect.width, 18f);
			float num = controlRect.width + 3f;
			position.width = labelWidth;
			GUI.Label(position, label);
			Rect rectValue = property.rectValue;
			float num2 = num - labelWidth;
			float num3 = num2 / 4f;
			position.width = num3 - 5f;
			position.x = labelWidth + 15f;
			GUI.Label(position, "Width");
			position.x += num3;
			rectValue.width = EditorGUI.FloatField(position, GUIContent.none, rectValue.width);
			position.x += num3;
			GUI.Label(position, "Height");
			position.x += num3;
			rectValue.height = EditorGUI.FloatField(position, GUIContent.none, rectValue.height);
			property.rectValue = rectValue;
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
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
			Vector4 zero = Vector4.zero;
			zero.x = property.FindPropertyRelative("x").floatValue;
			zero.y = property.FindPropertyRelative("y").floatValue;
			zero.z = property.FindPropertyRelative("z").floatValue;
			zero.w = property.FindPropertyRelative("w").floatValue;
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
			zero.x = EditorGUI.FloatField(position, GUIContent.none, zero.x);
			position.x += num3;
			zero.y = EditorGUI.FloatField(position, GUIContent.none, zero.y);
			position.x += num3;
			zero.z = EditorGUI.FloatField(position, GUIContent.none, zero.z);
			position.x += num3;
			zero.w = EditorGUI.FloatField(position, GUIContent.none, zero.w);
			property.FindPropertyRelative("x").floatValue = zero.x;
			property.FindPropertyRelative("y").floatValue = zero.y;
			property.FindPropertyRelative("z").floatValue = zero.z;
			property.FindPropertyRelative("w").floatValue = zero.w;
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUIUtility.fieldWidth = fieldWidth;
		}

		private Vector2 GetAnchorPosition(int index)
		{
			Vector2 zero = Vector2.zero;
			switch (index)
			{
			case 0:
				zero = new Vector2(0f, 1f);
				break;
			case 1:
				zero = new Vector2(0.5f, 1f);
				break;
			case 2:
				zero = new Vector2(1f, 1f);
				break;
			case 3:
				zero = new Vector2(0f, 0.5f);
				break;
			case 4:
				zero = new Vector2(0.5f, 0.5f);
				break;
			case 5:
				zero = new Vector2(1f, 0.5f);
				break;
			case 6:
				zero = new Vector2(0f, 0f);
				break;
			case 7:
				zero = new Vector2(0.5f, 0f);
				break;
			case 8:
				zero = new Vector2(1f, 0f);
				break;
			}
			return zero;
		}

		private SerializedProperty anchorPosition_prop;

		private SerializedProperty pivot_prop;

		private SerializedProperty rectangle_prop;

		private SerializedProperty margins_prop;

		private TextContainer m_textContainer;
	}
}
