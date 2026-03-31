using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomPropertyDrawer(typeof(KerningPair))]
	public class KerningPairDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty serializedProperty = property.FindPropertyRelative("AscII_Left");
			SerializedProperty serializedProperty2 = property.FindPropertyRelative("AscII_Right");
			SerializedProperty property2 = property.FindPropertyRelative("XadvanceOffset");
			position.yMin += 4f;
			position.yMax += 4f;
			position.height -= 2f;
			float num = position.width / 3f;
			float num2 = 5f;
			this.isEditingEnabled = (label != GUIContent.none);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.enabled = this.isEditingEnabled;
			Rect position2 = new Rect(position.x, position.y, 25f, position.height);
			this.char_left = EditorGUI.TextArea(position2, string.Empty + (char)serializedProperty.intValue);
			if (GUI.changed && this.char_left != string.Empty)
			{
				GUI.changed = false;
				serializedProperty.intValue = (int)this.char_left[0];
			}
			Rect position3 = new Rect(position.x + position2.width + num2, position.y, num - position2.width - 10f, position.height);
			EditorGUI.PropertyField(position3, serializedProperty, GUIContent.none);
			position2 = new Rect(position.x + num * 1f, position.y, 25f, position.height);
			this.char_right = EditorGUI.TextArea(position2, string.Empty + (char)serializedProperty2.intValue);
			if (GUI.changed && this.char_right != string.Empty)
			{
				GUI.changed = false;
				serializedProperty2.intValue = (int)this.char_right[0];
			}
			position3 = new Rect(position.x + num * 1f + position2.width + num2, position.y, num - position2.width - 10f, position.height);
			EditorGUI.PropertyField(position3, serializedProperty2, GUIContent.none);
			GUI.enabled = true;
			position3 = new Rect(position.x + num * 2f, position.y, num, position.height);
			EditorGUIUtility.labelWidth = 40f;
			EditorGUIUtility.fieldWidth = 45f;
			EditorGUI.PropertyField(position3, property2, new GUIContent("Offset"));
			GUILayout.EndHorizontal();
		}

		private bool isEditingEnabled;

		private string char_left;

		private string char_right;
	}
}
