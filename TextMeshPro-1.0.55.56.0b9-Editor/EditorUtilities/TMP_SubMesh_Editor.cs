using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_SubMesh))]
	[CanEditMultipleObjects]
	public class TMP_SubMesh_Editor : Editor
	{
		public void OnEnable()
		{
			TMP_UIStyleManager.GetUIStyles();
			this.fontAsset_prop = base.serializedObject.FindProperty("m_fontAsset");
			this.spriteAsset_prop = base.serializedObject.FindProperty("m_spriteAsset");
			this.m_SubMeshComponent = (base.target as TMP_SubMesh);
			this.m_Renderer = this.m_SubMeshComponent.renderer;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.indentLevel = 0;
			if (GUILayout.Button("<b>SUB OBJECT SETTINGS</b>" + ((!TMP_SubMesh_Editor.m_foldout.fontSettings) ? TMP_SubMesh_Editor.uiStateLabel[0] : TMP_SubMesh_Editor.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_SubMesh_Editor.m_foldout.fontSettings = !TMP_SubMesh_Editor.m_foldout.fontSettings;
			}
			if (TMP_SubMesh_Editor.m_foldout.fontSettings)
			{
				GUI.enabled = false;
				EditorGUILayout.PropertyField(this.fontAsset_prop, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.spriteAsset_prop, new GUILayoutOption[0]);
				GUI.enabled = true;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Sorting Layer");
			EditorGUI.BeginChangeCheck();
			string[] sortingLayerNames = SortingLayerHelper.sortingLayerNames;
			string sortingLayerNameFromID = SortingLayerHelper.GetSortingLayerNameFromID(this.m_Renderer.sortingLayerID);
			int num = Array.IndexOf<string>(sortingLayerNames, sortingLayerNameFromID);
			EditorGUIUtility.fieldWidth = 0f;
			int num2 = EditorGUILayout.Popup(string.Empty, num, sortingLayerNames, new GUILayoutOption[]
			{
				GUILayout.MinWidth(80f)
			});
			if (num2 != num)
			{
				this.m_Renderer.sortingLayerID = SortingLayerHelper.GetSortingLayerIDForIndex(num2);
			}
			EditorGUIUtility.labelWidth = 40f;
			EditorGUIUtility.fieldWidth = 80f;
			int num3 = EditorGUILayout.IntField("Order", this.m_Renderer.sortingOrder, new GUILayoutOption[0]);
			if (num3 != this.m_Renderer.sortingOrder)
			{
				this.m_Renderer.sortingOrder = num3;
			}
			EditorGUILayout.EndHorizontal();
		}

		private static string[] uiStateLabel = new string[]
		{
			"\t- <i>Click to expand</i> -",
			"\t- <i>Click to collapse</i> -"
		};

		private SerializedProperty fontAsset_prop;

		private SerializedProperty spriteAsset_prop;

		private TMP_SubMesh m_SubMeshComponent;

		private Renderer m_Renderer;

		private struct m_foldout
		{
			public static bool fontSettings = true;
		}
	}
}
