using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_SubMeshUI))]
	[CanEditMultipleObjects]
	public class TMP_SubMeshUI_Editor : Editor
	{
		public void OnEnable()
		{
			TMP_UIStyleManager.GetUIStyles();
			this.fontAsset_prop = base.serializedObject.FindProperty("m_fontAsset");
			this.spriteAsset_prop = base.serializedObject.FindProperty("m_spriteAsset");
			this.m_SubMeshComponent = (base.target as TMP_SubMeshUI);
			this.m_canvasRenderer = this.m_SubMeshComponent.canvasRenderer;
			if (this.m_canvasRenderer != null && this.m_canvasRenderer.GetMaterial() != null)
			{
				this.m_materialEditor = Editor.CreateEditor(this.m_canvasRenderer.GetMaterial());
				this.m_targetMaterial = this.m_canvasRenderer.GetMaterial();
			}
		}

		public void OnDisable()
		{
			if (this.m_materialEditor != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_materialEditor);
			}
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("<b>SUB OBJECT SETTINGS</b>" + ((!TMP_SubMeshUI_Editor.m_foldout.fontSettings) ? TMP_SubMeshUI_Editor.uiStateLabel[0] : TMP_SubMeshUI_Editor.uiStateLabel[1]), TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]))
			{
				TMP_SubMeshUI_Editor.m_foldout.fontSettings = !TMP_SubMeshUI_Editor.m_foldout.fontSettings;
			}
			if (TMP_SubMeshUI_Editor.m_foldout.fontSettings)
			{
				GUI.enabled = false;
				EditorGUILayout.PropertyField(this.fontAsset_prop, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.spriteAsset_prop, new GUILayoutOption[0]);
				GUI.enabled = true;
			}
			EditorGUILayout.Space();
			if (this.m_canvasRenderer != null && this.m_canvasRenderer.GetMaterial() != null)
			{
				Material material = this.m_canvasRenderer.GetMaterial();
				if (material != this.m_targetMaterial)
				{
					this.m_targetMaterial = material;
					UnityEngine.Object.DestroyImmediate(this.m_materialEditor);
				}
				if (this.m_materialEditor == null)
				{
					this.m_materialEditor = Editor.CreateEditor(material);
				}
				this.m_materialEditor.DrawHeader();
				this.m_materialEditor.OnInspectorGUI();
			}
		}

		private static string[] uiStateLabel = new string[]
		{
			"\t- <i>Click to expand</i> -",
			"\t- <i>Click to collapse</i> -"
		};

		private SerializedProperty fontAsset_prop;

		private SerializedProperty spriteAsset_prop;

		private TMP_SubMeshUI m_SubMeshComponent;

		private CanvasRenderer m_canvasRenderer;

		private Editor m_materialEditor;

		private Material m_targetMaterial;

		private struct m_foldout
		{
			public static bool fontSettings = true;
		}
	}
}
