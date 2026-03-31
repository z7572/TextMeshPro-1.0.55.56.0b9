using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public abstract class TMP_BaseShaderGUI : ShaderGUI
	{
		static TMP_BaseShaderGUI()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(delegate()
			{
				TMP_BaseShaderGUI.undoRedoCount++;
			}));
		}

		private void PrepareGUI()
		{
			this.isNewGUI = false;
			TMP_UIStyleManager.GetUIStyles();
			ShaderUtilities.GetShaderPropertyIDs();
			if (TMP_BaseShaderGUI.lastSeenUndoRedoCount != TMP_BaseShaderGUI.undoRedoCount)
			{
				TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, this.material);
			}
			TMP_BaseShaderGUI.lastSeenUndoRedoCount = TMP_BaseShaderGUI.undoRedoCount;
		}

		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			this.editor = materialEditor;
			this.material = (materialEditor.target as Material);
			this.properties = properties;
			if (this.isNewGUI)
			{
				this.PrepareGUI();
			}
			EditorGUIUtility.labelWidth = 130f;
			EditorGUIUtility.fieldWidth = 50f;
			this.DoDragAndDropBegin();
			EditorGUI.BeginChangeCheck();
			this.DoGUI();
			if (EditorGUI.EndChangeCheck())
			{
				TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, this.material);
			}
			this.DoDragAndDropEnd();
		}

		protected abstract void DoGUI();

		protected bool DoPanelHeader(TMP_BaseShaderGUI.MaterialPanel panel)
		{
			if (GUILayout.Button(panel.Label, TMP_UIStyleManager.Group_Label, new GUILayoutOption[0]))
			{
				panel.ToggleExpanded();
			}
			return panel.Expanded;
		}

		protected bool DoPanelHeader(TMP_BaseShaderGUI.MaterialPanel panel, TMP_BaseShaderGUI.ShaderFeature feature, bool readState = true)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(false, 22f, new GUILayoutOption[0]);
			GUI.Label(controlRect, GUIContent.none, TMP_UIStyleManager.Group_Label);
			if (GUI.Button(new Rect(controlRect.x, controlRect.y, 250f, controlRect.height), panel.Label, TMP_UIStyleManager.Group_Label_Left))
			{
				panel.ToggleExpanded();
			}
			if (readState)
			{
				feature.ReadState(this.material);
			}
			EditorGUI.BeginChangeCheck();
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 70f;
			bool active = EditorGUI.Toggle(new Rect(controlRect.width - 90f, controlRect.y + 3f, 90f, 22f), new GUIContent("Enable ->"), feature.Active);
			EditorGUIUtility.labelWidth = labelWidth;
			if (EditorGUI.EndChangeCheck())
			{
				this.editor.RegisterPropertyChangeUndo(feature.undoLabel);
				feature.SetActive(active, this.material);
			}
			return panel.Expanded;
		}

		private MaterialProperty BeginProperty(string name)
		{
			MaterialProperty materialProperty = ShaderGUI.FindProperty(name, this.properties);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = materialProperty.hasMixedValue;
			this.editor.BeginAnimatedCheck(Rect.zero, materialProperty);
			return materialProperty;
		}

		private bool EndProperty()
		{
			this.editor.EndAnimatedCheck();
			EditorGUI.showMixedValue = false;
			return EditorGUI.EndChangeCheck();
		}

		protected void DoPopup(string name, string label, GUIContent[] options)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			TMP_BaseShaderGUI.tempLabel.text = label;
			int num = EditorGUILayout.Popup(TMP_BaseShaderGUI.tempLabel, (int)materialProperty.floatValue, options, new GUILayoutOption[0]);
			if (this.EndProperty())
			{
				materialProperty.floatValue = (float)num;
			}
		}

		protected void DoCubeMap(string name, string label)
		{
			this.DoTexture(name, label, typeof(Cubemap), false, null);
		}

		protected void DoTexture2D(string name, string label, bool withTilingOffset = false, string[] speedNames = null)
		{
			this.DoTexture(name, label, typeof(Texture2D), withTilingOffset, speedNames);
		}

		private void DoTexture(string name, string label, Type type, bool withTilingOffset = false, string[] speedNames = null)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			Rect controlRect = EditorGUILayout.GetControlRect(true, 60f, new GUILayoutOption[0]);
			float width = controlRect.width;
			controlRect.width = EditorGUIUtility.labelWidth + 60f;
			TMP_BaseShaderGUI.tempLabel.text = label;
			UnityEngine.Object @object = EditorGUI.ObjectField(controlRect, TMP_BaseShaderGUI.tempLabel, materialProperty.textureValue, type, false);
			if (this.EndProperty())
			{
				materialProperty.textureValue = (@object as Texture);
			}
			controlRect.x += controlRect.width + 4f;
			controlRect.width = width - controlRect.width - 4f;
			controlRect.height = EditorGUIUtility.singleLineHeight;
			if (withTilingOffset)
			{
				this.DoTilingOffset(controlRect, materialProperty);
				controlRect.y += (controlRect.height + 2f) * 2f;
			}
			if (speedNames != null)
			{
				this.DoUVSpeed(controlRect, speedNames);
			}
		}

		private void DoTilingOffset(Rect rect, MaterialProperty property)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			int indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = 40f;
			Vector4 textureScaleAndOffset = property.textureScaleAndOffset;
			bool flag = false;
			float[] array = TMP_BaseShaderGUI.tempFloats[2];
			TMP_BaseShaderGUI.tempLabel.text = "Tiling";
			Rect position = EditorGUI.PrefixLabel(rect, TMP_BaseShaderGUI.tempLabel);
			array[0] = textureScaleAndOffset.x;
			array[1] = textureScaleAndOffset.y;
			EditorGUI.BeginChangeCheck();
			EditorGUI.MultiFloatField(position, TMP_BaseShaderGUI.xywhVectorLabels, array);
			if (this.EndProperty())
			{
				textureScaleAndOffset.x = array[0];
				textureScaleAndOffset.y = array[1];
				flag = true;
			}
			rect.y += rect.height + 2f;
			TMP_BaseShaderGUI.tempLabel.text = "Offset";
			position = EditorGUI.PrefixLabel(rect, TMP_BaseShaderGUI.tempLabel);
			array[0] = textureScaleAndOffset.z;
			array[1] = textureScaleAndOffset.w;
			EditorGUI.BeginChangeCheck();
			EditorGUI.MultiFloatField(position, TMP_BaseShaderGUI.xywhVectorLabels, array);
			if (this.EndProperty())
			{
				textureScaleAndOffset.z = array[0];
				textureScaleAndOffset.w = array[1];
				flag = true;
			}
			if (flag)
			{
				property.textureScaleAndOffset = textureScaleAndOffset;
			}
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.indentLevel = indentLevel;
		}

		protected void DoUVSpeed(Rect rect, string[] names)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			int indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = 40f;
			TMP_BaseShaderGUI.tempLabel.text = "Speed";
			rect = EditorGUI.PrefixLabel(rect, TMP_BaseShaderGUI.tempLabel);
			EditorGUIUtility.labelWidth = 13f;
			rect.width = rect.width * 0.5f - 1f;
			this.DoFloat(rect, names[0], "X");
			rect.x += rect.width + 2f;
			this.DoFloat(rect, names[1], "Y");
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.indentLevel = indentLevel;
		}

		protected void DoToggle(string name, string label)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			TMP_BaseShaderGUI.tempLabel.text = label;
			bool flag = EditorGUILayout.Toggle(TMP_BaseShaderGUI.tempLabel, materialProperty.floatValue == 1f, new GUILayoutOption[0]);
			if (this.EndProperty())
			{
				materialProperty.floatValue = ((!flag) ? 0f : 1f);
			}
		}

		protected void DoFloat(string name, string label)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			controlRect.width = 225f;
			TMP_BaseShaderGUI.tempLabel.text = label;
			float floatValue = EditorGUI.FloatField(controlRect, TMP_BaseShaderGUI.tempLabel, materialProperty.floatValue);
			if (this.EndProperty())
			{
				materialProperty.floatValue = floatValue;
			}
		}

		protected void DoColor(string name, string label)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			TMP_BaseShaderGUI.tempLabel.text = label;
			Color colorValue = EditorGUI.ColorField(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), TMP_BaseShaderGUI.tempLabel, materialProperty.colorValue);
			if (this.EndProperty())
			{
				materialProperty.colorValue = colorValue;
			}
		}

		private void DoFloat(Rect rect, string name, string label)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			TMP_BaseShaderGUI.tempLabel.text = label;
			float floatValue = EditorGUI.FloatField(rect, TMP_BaseShaderGUI.tempLabel, materialProperty.floatValue);
			if (this.EndProperty())
			{
				materialProperty.floatValue = floatValue;
			}
		}

		protected void DoSlider(string name, string label)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			Vector2 rangeLimits = materialProperty.rangeLimits;
			TMP_BaseShaderGUI.tempLabel.text = label;
			float floatValue = EditorGUI.Slider(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), TMP_BaseShaderGUI.tempLabel, materialProperty.floatValue, rangeLimits.x, rangeLimits.y);
			if (this.EndProperty())
			{
				materialProperty.floatValue = floatValue;
			}
		}

		protected void DoVector3(string name, string label)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			TMP_BaseShaderGUI.tempLabel.text = label;
			Vector4 vectorValue = EditorGUILayout.Vector3Field(TMP_BaseShaderGUI.tempLabel, materialProperty.vectorValue, new GUILayoutOption[0]);
			if (this.EndProperty())
			{
				materialProperty.vectorValue = vectorValue;
			}
		}

		protected void DoVector(string name, string label, GUIContent[] subLabels)
		{
			MaterialProperty materialProperty = this.BeginProperty(name);
			Rect rect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			TMP_BaseShaderGUI.tempLabel.text = label;
			rect = EditorGUI.PrefixLabel(rect, TMP_BaseShaderGUI.tempLabel);
			Vector4 vectorValue = materialProperty.vectorValue;
			float[] array = TMP_BaseShaderGUI.tempFloats[subLabels.Length];
			for (int i = 0; i < subLabels.Length; i++)
			{
				array[i] = vectorValue[i];
			}
			EditorGUI.MultiFloatField(rect, subLabels, array);
			if (this.EndProperty())
			{
				for (int j = 0; j < subLabels.Length; j++)
				{
					vectorValue[j] = array[j];
				}
				materialProperty.vectorValue = vectorValue;
			}
		}

		protected void DoEmptyLine()
		{
			GUILayout.Space(EditorGUIUtility.singleLineHeight);
		}

		private void DoDragAndDropBegin()
		{
			this.dragAndDropMinY = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			}).y;
		}

		private void DoDragAndDropEnd()
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			Event current = Event.current;
			if (current.type == EventType.DragUpdated)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				current.Use();
			}
			else if (current.type == EventType.DragPerform && Rect.MinMaxRect(rect.xMin, this.dragAndDropMinY, rect.xMax, rect.yMax).Contains(current.mousePosition))
			{
				DragAndDrop.AcceptDrag();
				current.Use();
				Material material = DragAndDrop.objectReferences[0] as Material;
				if (material && material != this.material)
				{
					this.PerformDrop(material);
				}
			}
		}

		private void PerformDrop(Material droppedMaterial)
		{
			Texture texture = droppedMaterial.GetTexture(ShaderUtilities.ID_MainTex);
			if (!texture)
			{
				return;
			}
			Texture texture2 = this.material.GetTexture(ShaderUtilities.ID_MainTex);
			TMP_FontAsset tmp_FontAsset = null;
			if (texture != texture2)
			{
				tmp_FontAsset = TMP_EditorUtility.FindMatchingFontAsset(droppedMaterial);
				if (!tmp_FontAsset)
				{
					return;
				}
			}
			foreach (GameObject gameObject in Selection.gameObjects)
			{
				if (tmp_FontAsset)
				{
					TMP_Text component = gameObject.GetComponent<TMP_Text>();
					if (component)
					{
						Undo.RecordObject(component, "Font Asset Change");
						component.font = tmp_FontAsset;
					}
				}
				TMPro_EventManager.ON_DRAG_AND_DROP_MATERIAL_CHANGED(gameObject, this.material, droppedMaterial);
				EditorUtility.SetDirty(gameObject);
			}
		}

		private static GUIContent tempLabel = new GUIContent();

		private static int undoRedoCount;

		private static int lastSeenUndoRedoCount;

		private static float[][] tempFloats = new float[][]
		{
			new float[0],
			new float[1],
			new float[2],
			new float[3],
			new float[4]
		};

		protected static GUIContent[] xywhVectorLabels = new GUIContent[]
		{
			new GUIContent("X"),
			new GUIContent("Y"),
			new GUIContent("W", "Width"),
			new GUIContent("H", "Height")
		};

		protected static GUIContent[] lbrtVectorLabels = new GUIContent[]
		{
			new GUIContent("L", "Left"),
			new GUIContent("B", "Bottom"),
			new GUIContent("R", "Right"),
			new GUIContent("T", "Top")
		};

		private bool isNewGUI = true;

		private float dragAndDropMinY;

		protected MaterialEditor editor;

		protected Material material;

		protected MaterialProperty[] properties;

		protected class ShaderFeature
		{
			public bool Active
			{
				get
				{
					return this.state >= 0;
				}
			}

			public int State
			{
				get
				{
					return this.state;
				}
			}

			public void ReadState(Material material)
			{
				for (int i = 0; i < this.keywords.Length; i++)
				{
					if (material.IsKeywordEnabled(this.keywords[i]))
					{
						this.state = i;
						return;
					}
				}
				this.state = -1;
			}

			public void SetActive(bool active, Material material)
			{
				this.state = ((!active) ? -1 : 0);
				this.SetStateKeywords(material);
			}

			public void DoPopup(MaterialEditor editor, Material material)
			{
				EditorGUI.BeginChangeCheck();
				int num = EditorGUILayout.Popup(this.label, this.state + 1, this.keywordLabels, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.state = num - 1;
					editor.RegisterPropertyChangeUndo(this.undoLabel);
					this.SetStateKeywords(material);
				}
			}

			private void SetStateKeywords(Material material)
			{
				for (int i = 0; i < this.keywords.Length; i++)
				{
					if (i == this.state)
					{
						material.EnableKeyword(this.keywords[i]);
					}
					else
					{
						material.DisableKeyword(this.keywords[i]);
					}
				}
			}

			public string undoLabel;

			public GUIContent label;

			public GUIContent[] keywordLabels;

			public string[] keywords;

			private int state;
		}

		protected class MaterialPanel
		{
			public MaterialPanel(string name, bool expandedByDefault)
			{
				this.label = "<b>" + name + "</b> - <i>Settings</i> -";
				this.key = "TexMeshPro.material." + name + ".expanded";
				this.expanded = EditorPrefs.GetBool(this.key, expandedByDefault);
			}

			public bool Expanded
			{
				get
				{
					return this.expanded;
				}
			}

			public string Label
			{
				get
				{
					return this.label;
				}
			}

			public void ToggleExpanded()
			{
				this.expanded = !this.expanded;
				EditorPrefs.SetBool(this.key, this.expanded);
			}

			private string key;

			private string label;

			private bool expanded;
		}
	}
}
