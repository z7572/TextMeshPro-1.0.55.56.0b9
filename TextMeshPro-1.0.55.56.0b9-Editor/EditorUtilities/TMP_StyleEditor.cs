using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	[CustomEditor(typeof(TMP_StyleSheet))]
	[CanEditMultipleObjects]
	public class TMP_StyleEditor : Editor
	{
		private void OnEnable()
		{
			TMP_UIStyleManager.GetUIStyles();
			this.m_styleList_prop = base.serializedObject.FindProperty("m_StyleList");
		}

		public override void OnInspectorGUI()
		{
			Event current = Event.current;
			base.serializedObject.Update();
			GUILayout.Label("<b>TextMeshPro - Style Sheet</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			int arraySize = this.m_styleList_prop.arraySize;
			int num = (Screen.height - 178) / 111;
			if (arraySize > 0)
			{
				int num2 = num * this.m_page;
				while (num2 < arraySize && num2 < num * (this.m_page + 1))
				{
					if (this.m_selectedElement == num2)
					{
						EditorGUI.DrawRect(this.m_selectionRect, new Color32(40, 192, byte.MaxValue, byte.MaxValue));
					}
					Rect rect = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(true)
					});
					EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label, new GUILayoutOption[0]);
					SerializedProperty arrayElementAtIndex = this.m_styleList_prop.GetArrayElementAtIndex(num2);
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(arrayElementAtIndex, new GUILayoutOption[0]);
					EditorGUILayout.EndVertical();
					if (EditorGUI.EndChangeCheck())
					{
					}
					Rect rect2 = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(true)
					});
					Rect selectionArea = new Rect(rect.x, rect.y, rect2.width, rect2.y - rect.y);
					if (this.DoSelectionCheck(selectionArea))
					{
						this.m_selectedElement = num2;
						this.m_selectionRect = new Rect(selectionArea.x - 2f, selectionArea.y + 2f, selectionArea.width + 4f, selectionArea.height - 4f);
						base.Repaint();
					}
					num2++;
				}
			}
			int num3 = (!current.shift) ? 1 : 10;
			GUILayout.Space(-3f);
			Rect controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
			controlRect.width /= 6f;
			if (num == 0)
			{
				return;
			}
			controlRect.x += controlRect.width * 4f;
			if (GUI.Button(controlRect, "+"))
			{
				this.m_styleList_prop.arraySize++;
				base.serializedObject.ApplyModifiedProperties();
				TMP_StyleSheet.RefreshStyles();
			}
			controlRect.x += controlRect.width;
			if (this.m_selectedElement == -1)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(controlRect, "-"))
			{
				if (this.m_selectedElement != -1)
				{
					this.m_styleList_prop.DeleteArrayElementAtIndex(this.m_selectedElement);
				}
				this.m_selectedElement = -1;
				base.serializedObject.ApplyModifiedProperties();
				TMP_StyleSheet.RefreshStyles();
			}
			GUILayout.Space(5f);
			controlRect = EditorGUILayout.GetControlRect(false, 20f, new GUILayoutOption[0]);
			controlRect.width /= 3f;
			if (this.m_page > 0)
			{
				GUI.enabled = true;
			}
			else
			{
				GUI.enabled = false;
			}
			if (GUI.Button(controlRect, "Previous"))
			{
				this.m_page -= num3;
			}
			GUI.enabled = true;
			controlRect.x += controlRect.width;
			int num4 = (int)((float)arraySize / (float)num + 0.999f);
			GUI.Label(controlRect, string.Concat(new object[]
			{
				"Page ",
				this.m_page + 1,
				" / ",
				num4
			}), GUI.skin.button);
			controlRect.x += controlRect.width;
			if (num * (this.m_page + 1) < arraySize)
			{
				GUI.enabled = true;
			}
			else
			{
				GUI.enabled = false;
			}
			if (GUI.Button(controlRect, "Next"))
			{
				this.m_page += num3;
			}
			this.m_page = Mathf.Clamp(this.m_page, 0, arraySize / num);
			if (base.serializedObject.ApplyModifiedProperties())
			{
				TMPro_EventManager.ON_TEXT_STYLE_PROPERTY_CHANGED(true);
			}
			GUI.enabled = true;
			if (current.type == EventType.MouseDown && current.button == 0)
			{
				this.m_selectedElement = -1;
			}
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

		private SerializedProperty m_styleList_prop;

		private int m_selectedElement = -1;

		private Rect m_selectionRect;

		private int m_page;
	}
}
