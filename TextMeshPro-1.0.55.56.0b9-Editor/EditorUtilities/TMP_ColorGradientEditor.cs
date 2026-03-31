using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_ColorGradient))]
	public class TMP_ColorGradientEditor : Editor
	{
		private void OnEnable()
		{
			TMP_UIStyleManager.GetUIStyles();
			this.topLeftColor = base.serializedObject.FindProperty("topLeft");
			this.topRightColor = base.serializedObject.FindProperty("topRight");
			this.bottomLeftColor = base.serializedObject.FindProperty("bottomLeft");
			this.bottomRightColor = base.serializedObject.FindProperty("bottomRight");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			GUILayout.Label("<b>TextMeshPro - Color Gradient Preset</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.topLeftColor, new GUIContent("Top Left"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.topRightColor, new GUIContent("Top Right"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.bottomLeftColor, new GUIContent("Bottom Left"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.bottomRightColor, new GUIContent("Bottom Right"), new GUILayoutOption[0]);
			EditorGUILayout.EndVertical();
			if (base.serializedObject.ApplyModifiedProperties())
			{
				TMPro_EventManager.ON_COLOR_GRAIDENT_PROPERTY_CHANGED(base.target as TMP_ColorGradient);
			}
		}

		private SerializedProperty topLeftColor;

		private SerializedProperty topRightColor;

		private SerializedProperty bottomLeftColor;

		private SerializedProperty bottomRightColor;
	}
}
