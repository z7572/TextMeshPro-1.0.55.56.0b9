using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public class TMPro_FontAssetCreatorWindow : EditorWindow
	{
		[MenuItem("Window/TextMeshPro/Font Asset Creator")]
		public static void ShowFontAtlasCreatorWindow()
		{
			TMPro_FontAssetCreatorWindow window = EditorWindow.GetWindow<TMPro_FontAssetCreatorWindow>();
			window.titleContent = new GUIContent("Asset Creator");
			window.Focus();
		}

		public void OnEnable()
		{
			this.m_editorWindow = this;
			this.UpdateEditorWindowSize(768f, 768f);
			TMP_UIStyleManager.GetUIStyles();
			ShaderUtilities.GetShaderPropertyIDs();
			TMPro_EventManager.COMPUTE_DT_EVENT.Add(new Action<object, Compute_DT_EventArgs>(this.ON_COMPUTE_DT_EVENT));
		}

		public void OnDisable()
		{
			TMPro_EventManager.COMPUTE_DT_EVENT.Remove(new Action<object, Compute_DT_EventArgs>(this.ON_COMPUTE_DT_EVENT));
			if (TMPro_FontPlugin.Initialize_FontEngine() == 99)
			{
				TMPro_FontPlugin.Destroy_FontEngine();
			}
			if (this.m_destination_Atlas != null && !EditorUtility.IsPersistent(this.m_destination_Atlas))
			{
				UnityEngine.Object.DestroyImmediate(this.m_destination_Atlas);
			}
			if (this.m_font_Atlas != null && !EditorUtility.IsPersistent(this.m_font_Atlas))
			{
				UnityEngine.Object.DestroyImmediate(this.m_font_Atlas);
			}
			string fullPath = Path.GetFullPath("Assets/..");
			if (File.Exists(fullPath + "/Assets/Glyph Report.txt"))
			{
				File.Delete(fullPath + "/Assets/Glyph Report.txt");
				File.Delete(fullPath + "/Assets/Glyph Report.txt.meta");
				AssetDatabase.Refresh();
			}
			Resources.UnloadUnusedAssets();
		}

		public void OnGUI()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Width(310f)
			});
			this.DrawControls();
			this.DrawPreview();
			GUILayout.EndHorizontal();
		}

		public void ON_COMPUTE_DT_EVENT(object Sender, Compute_DT_EventArgs e)
		{
			if (e.EventType == Compute_DistanceTransform_EventTypes.Completed)
			{
				this.Output = e.Colors;
				this.isProcessing = false;
				this.isDistanceMapReady = true;
			}
			else if (e.EventType == Compute_DistanceTransform_EventTypes.Processing)
			{
				TMPro_FontAssetCreatorWindow.ProgressPercentage = e.ProgressPercentage;
				this.isRepaintNeeded = true;
			}
		}

		public void Update()
		{
			if (this.isDistanceMapReady)
			{
				if (this.m_font_Atlas != null)
				{
					this.m_destination_Atlas = new Texture2D(this.m_font_Atlas.width / this.font_scaledownFactor, this.m_font_Atlas.height / this.font_scaledownFactor, TextureFormat.Alpha8, false, true);
					this.m_destination_Atlas.SetPixels(this.Output);
					this.m_destination_Atlas.Apply(false, true);
				}
				this.isDistanceMapReady = false;
				base.Repaint();
			}
			if (this.isRepaintNeeded)
			{
				this.isRepaintNeeded = false;
				base.Repaint();
			}
			if (this.isProcessing)
			{
				this.m_renderingProgress = TMPro_FontPlugin.Check_RenderProgress();
				this.isRepaintNeeded = true;
			}
			if (this.isRenderingDone)
			{
				this.isProcessing = false;
				this.isRenderingDone = false;
				this.UpdateRenderFeedbackWindow();
				this.CreateFontTexture();
			}
		}

		private int[] ParseNumberSequence(string sequence)
		{
			List<int> list = new List<int>();
			string[] array = sequence.Split(new char[]
			{
				','
			});
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					'-'
				});
				if (array3.Length == 1)
				{
					try
					{
						list.Add(int.Parse(array3[0]));
					}
					catch
					{
						Debug.Log("No characters selected or invalid format.");
					}
				}
				else
				{
					for (int j = int.Parse(array3[0]); j < int.Parse(array3[1]) + 1; j++)
					{
						list.Add(j);
					}
				}
			}
			return list.ToArray();
		}

		private int[] ParseHexNumberSequence(string sequence)
		{
			List<int> list = new List<int>();
			string[] array = sequence.Split(new char[]
			{
				','
			});
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					'-'
				});
				if (array3.Length == 1)
				{
					try
					{
						list.Add(int.Parse(array3[0], NumberStyles.AllowHexSpecifier));
					}
					catch
					{
						Debug.Log("No characters selected or invalid format.");
					}
				}
				else
				{
					for (int j = int.Parse(array3[0], NumberStyles.AllowHexSpecifier); j < int.Parse(array3[1], NumberStyles.AllowHexSpecifier) + 1; j++)
					{
						list.Add(j);
					}
				}
			}
			return list.ToArray();
		}

		private void DrawControls()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("<b>TextMeshPro - Font Asset Creator</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[]
			{
				GUILayout.Width(300f)
			});
			GUILayout.Label("Font Settings", TMP_UIStyleManager.Section_Label, new GUILayoutOption[]
			{
				GUILayout.Width(300f)
			});
			GUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox, new GUILayoutOption[]
			{
				GUILayout.Width(300f)
			});
			EditorGUIUtility.labelWidth = 120f;
			EditorGUIUtility.fieldWidth = 160f;
			EditorGUI.BeginChangeCheck();
			this.font_TTF = (EditorGUILayout.ObjectField("Font Source", this.font_TTF, typeof(Font), false, new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			}) as Font);
			if (EditorGUI.EndChangeCheck())
			{
			}
			if (this.FontSizingOption_Selection == 0)
			{
				this.FontSizingOption_Selection = EditorGUILayout.Popup("Font Size", this.FontSizingOption_Selection, this.FontSizingOptions, new GUILayoutOption[]
				{
					GUILayout.Width(290f)
				});
			}
			else
			{
				EditorGUIUtility.labelWidth = 120f;
				EditorGUIUtility.fieldWidth = 40f;
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Width(290f)
				});
				this.FontSizingOption_Selection = EditorGUILayout.Popup("Font Size", this.FontSizingOption_Selection, this.FontSizingOptions, new GUILayoutOption[]
				{
					GUILayout.Width(225f)
				});
				this.font_size = EditorGUILayout.IntField(this.font_size, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			EditorGUIUtility.labelWidth = 120f;
			EditorGUIUtility.fieldWidth = 160f;
			this.font_padding = EditorGUILayout.IntField("Font Padding", this.font_padding, new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			});
			this.font_padding = (int)Mathf.Clamp((float)this.font_padding, 0f, 64f);
			this.m_fontPackingSelection = (TMPro_FontAssetCreatorWindow.FontPackingModes)EditorGUILayout.EnumPopup("Packing Method", this.m_fontPackingSelection, new GUILayoutOption[]
			{
				GUILayout.Width(225f)
			});
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			});
			GUI.changed = false;
			EditorGUIUtility.labelWidth = 120f;
			EditorGUIUtility.fieldWidth = 40f;
			GUILayout.Label("Atlas Resolution:", new GUILayoutOption[]
			{
				GUILayout.Width(116f)
			});
			this.font_atlas_width = EditorGUILayout.IntPopup(this.font_atlas_width, this.FontResolutionLabels, this.FontAtlasResolutions, new GUILayoutOption[0]);
			this.font_atlas_height = EditorGUILayout.IntPopup(this.font_atlas_height, this.FontResolutionLabels, this.FontAtlasResolutions, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			EditorGUI.BeginChangeCheck();
			bool flag = false;
			this.font_CharacterSet_Selection = EditorGUILayout.Popup("Character Set", this.font_CharacterSet_Selection, this.FontCharacterSets, new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				this.characterSequence = string.Empty;
				flag = true;
			}
			switch (this.font_CharacterSet_Selection)
			{
			case 0:
				this.characterSequence = "32 - 126, 160, 8203, 8230, 9633";
				break;
			case 1:
				this.characterSequence = "32 - 126, 160 - 255, 8192 - 8303, 8364, 8482, 9633";
				break;
			case 2:
				this.characterSequence = "32 - 64, 91 - 126, 160";
				break;
			case 3:
				this.characterSequence = "32 - 96, 123 - 126, 160";
				break;
			case 4:
				this.characterSequence = "32 - 64, 91 - 96, 123 - 126, 160";
				break;
			case 5:
			{
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox, new GUILayoutOption[0]);
				GUILayout.Label("Enter a sequence of decimal values to define the characters to be included in the font asset or retrieve one from another font asset.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				EditorGUI.BeginChangeCheck();
				this.m_fontAssetSelection = (EditorGUILayout.ObjectField("Select Font Asset", this.m_fontAssetSelection, typeof(TMP_FontAsset), false, new GUILayoutOption[]
				{
					GUILayout.Width(290f)
				}) as TMP_FontAsset);
				if ((EditorGUI.EndChangeCheck() || flag) && this.m_fontAssetSelection != null)
				{
					this.characterSequence = TMP_EditorUtility.GetDecimalCharacterSequence(TMP_FontAsset.GetCharactersArray(this.m_fontAssetSelection));
				}
				EditorGUIUtility.labelWidth = 120f;
				char character = Event.current.character;
				if ((character < '0' || character > '9') && (character < ',' || character > '-'))
				{
					Event.current.character = '\0';
				}
				GUILayout.Label("Character Sequence (Decimal)", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
				this.characterSequence = EditorGUILayout.TextArea(this.characterSequence, TMP_UIStyleManager.TextAreaBoxWindow, new GUILayoutOption[]
				{
					GUILayout.Height(120f),
					GUILayout.MaxWidth(290f)
				});
				EditorGUILayout.EndVertical();
				break;
			}
			case 6:
			{
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox, new GUILayoutOption[0]);
				GUILayout.Label("Enter a sequence of Unicode (hex) values to define the characters to be included in the font asset or retrieve one from another font asset.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				EditorGUI.BeginChangeCheck();
				this.m_fontAssetSelection = (EditorGUILayout.ObjectField("Select Font Asset", this.m_fontAssetSelection, typeof(TMP_FontAsset), false, new GUILayoutOption[]
				{
					GUILayout.Width(290f)
				}) as TMP_FontAsset);
				if ((EditorGUI.EndChangeCheck() || flag) && this.m_fontAssetSelection != null)
				{
					this.characterSequence = TMP_EditorUtility.GetUnicodeCharacterSequence(TMP_FontAsset.GetCharactersArray(this.m_fontAssetSelection));
				}
				EditorGUIUtility.labelWidth = 120f;
				char character = Event.current.character;
				if ((character < '0' || character > '9') && (character < 'a' || character > 'f') && (character < 'A' || character > 'F') && (character < ',' || character > '-'))
				{
					Event.current.character = '\0';
				}
				GUILayout.Label("Character Sequence (Hex)", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
				this.characterSequence = EditorGUILayout.TextArea(this.characterSequence, TMP_UIStyleManager.TextAreaBoxWindow, new GUILayoutOption[]
				{
					GUILayout.Height(120f),
					GUILayout.MaxWidth(290f)
				});
				EditorGUILayout.EndVertical();
				break;
			}
			case 7:
				EditorGUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox, new GUILayoutOption[0]);
				GUILayout.Label("Type the characters to be included in the font asset or retrieve them from another font asset.", TMP_UIStyleManager.Label, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				EditorGUI.BeginChangeCheck();
				this.m_fontAssetSelection = (EditorGUILayout.ObjectField("Select Font Asset", this.m_fontAssetSelection, typeof(TMP_FontAsset), false, new GUILayoutOption[]
				{
					GUILayout.Width(290f)
				}) as TMP_FontAsset);
				if ((EditorGUI.EndChangeCheck() || flag) && this.m_fontAssetSelection != null)
				{
					this.characterSequence = TMP_FontAsset.GetCharacters(this.m_fontAssetSelection);
				}
				EditorGUIUtility.labelWidth = 120f;
				EditorGUI.indentLevel = 0;
				GUILayout.Label("Custom Character List", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
				this.characterSequence = EditorGUILayout.TextArea(this.characterSequence, TMP_UIStyleManager.TextAreaBoxWindow, new GUILayoutOption[]
				{
					GUILayout.Height(120f),
					GUILayout.MaxWidth(290f)
				});
				EditorGUILayout.EndVertical();
				break;
			case 8:
				this.characterList = (EditorGUILayout.ObjectField("Character File", this.characterList, typeof(TextAsset), false, new GUILayoutOption[]
				{
					GUILayout.Width(290f)
				}) as TextAsset);
				if (this.characterList != null)
				{
					this.characterSequence = this.characterList.text;
				}
				break;
			}
			EditorGUIUtility.labelWidth = 120f;
			EditorGUIUtility.fieldWidth = 40f;
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			});
			this.font_style = (FaceStyles)EditorGUILayout.EnumPopup("Font Style:", this.font_style, new GUILayoutOption[]
			{
				GUILayout.Width(225f)
			});
			this.font_style_mod = (float)EditorGUILayout.IntField((int)this.font_style_mod, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			EditorGUI.BeginChangeCheck();
			this.font_renderMode = (RenderModes)EditorGUILayout.EnumPopup("Font Render Mode:", this.font_renderMode, new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			});
			if (EditorGUI.EndChangeCheck())
			{
			}
			this.includeKerningPairs = EditorGUILayout.Toggle("Get Kerning Pairs?", this.includeKerningPairs, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(290f)
			});
			EditorGUIUtility.labelWidth = 120f;
			EditorGUIUtility.fieldWidth = 160f;
			GUILayout.Space(20f);
			GUI.enabled = (!(this.font_TTF == null) && !this.isProcessing);
			if (GUILayout.Button("Generate Font Atlas", new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			}) && this.characterSequence.Length != 0 && GUI.enabled && this.font_TTF != null)
			{
				int error_Code = TMPro_FontPlugin.Initialize_FontEngine();
				if (error_Code != 0)
				{
					if (error_Code == 99)
					{
						error_Code = 0;
					}
					else
					{
						Debug.Log("Error Code: " + error_Code + "  occurred while Initializing the FreeType Library.");
					}
				}
				string assetPath = AssetDatabase.GetAssetPath(this.font_TTF);
				if (error_Code == 0)
				{
					error_Code = TMPro_FontPlugin.Load_TrueType_Font(assetPath);
					if (error_Code != 0)
					{
						if (error_Code == 99)
						{
							error_Code = 0;
						}
						else
						{
							Debug.Log("Error Code: " + error_Code + "  occurred while Loading the font.");
						}
					}
				}
				if (error_Code == 0)
				{
					if (this.FontSizingOption_Selection == 0)
					{
						this.font_size = 72;
					}
					error_Code = TMPro_FontPlugin.FT_Size_Font(this.font_size);
					if (error_Code != 0)
					{
						Debug.Log("Error Code: " + error_Code + "  occurred while Sizing the font.");
					}
				}
				if (error_Code == 0)
				{
					int[] character_Set = null;
					if (this.font_CharacterSet_Selection == 7 || this.font_CharacterSet_Selection == 8)
					{
						List<int> list = new List<int>();
						int i;
						for (i = 0; i < this.characterSequence.Length; i++)
						{
							if (list.FindIndex((int item) => item == (int)this.characterSequence[i]) == -1)
							{
								list.Add((int)this.characterSequence[i]);
							}
						}
						character_Set = list.ToArray();
					}
					else if (this.font_CharacterSet_Selection == 6)
					{
						character_Set = this.ParseHexNumberSequence(this.characterSequence);
					}
					else
					{
						character_Set = this.ParseNumberSequence(this.characterSequence);
					}
					this.m_character_Count = character_Set.Length;
					this.m_texture_buffer = new byte[this.font_atlas_width * this.font_atlas_height];
					this.m_font_faceInfo = default(FT_FaceInfo);
					this.m_font_glyphInfo = new FT_GlyphInfo[this.m_character_Count];
					int padding = this.font_padding;
					bool autoSizing = this.FontSizingOption_Selection == 0;
					float strokeSize = this.font_style_mod;
					if (this.font_renderMode == RenderModes.DistanceField16)
					{
						strokeSize = this.font_style_mod * 16f;
					}
					if (this.font_renderMode == RenderModes.DistanceField32)
					{
						strokeSize = this.font_style_mod * 32f;
					}
					this.isProcessing = true;
					ThreadPool.QueueUserWorkItem(delegate(object SomeTask)
					{
						this.isRenderingDone = false;
						error_Code = TMPro_FontPlugin.Render_Characters(this.m_texture_buffer, this.font_atlas_width, this.font_atlas_height, padding, character_Set, this.m_character_Count, this.font_style, strokeSize, autoSizing, this.font_renderMode, (int)this.m_fontPackingSelection, ref this.m_font_faceInfo, this.m_font_glyphInfo);
						this.isRenderingDone = true;
					});
					this.previewSelection = TMPro_FontAssetCreatorWindow.PreviewSelectionTypes.PreviewFont;
				}
			}
			GUILayout.Space(1f);
			this.progressRect = GUILayoutUtility.GetRect(288f, 20f, TMP_UIStyleManager.TextAreaBoxWindow, new GUILayoutOption[]
			{
				GUILayout.Width(288f),
				GUILayout.Height(20f)
			});
			GUI.BeginGroup(this.progressRect);
			GUI.DrawTextureWithTexCoords(new Rect(2f, 0f, 288f, 20f), TMP_UIStyleManager.progressTexture, new Rect(1f - this.m_renderingProgress, 0f, 1f, 1f));
			GUI.EndGroup();
			GUISkin skin = GUI.skin;
			GUI.skin = TMP_UIStyleManager.TMP_GUISkin;
			GUILayout.Space(5f);
			GUILayout.BeginVertical(TMP_UIStyleManager.TextAreaBoxWindow, new GUILayoutOption[0]);
			this.output_ScrollPosition = EditorGUILayout.BeginScrollView(this.output_ScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(145f)
			});
			EditorGUILayout.LabelField(this.output_feedback, TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			EditorGUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUI.skin = skin;
			GUILayout.Space(10f);
			GUI.enabled = (this.m_font_Atlas != null);
			if (GUILayout.Button("Save TextMeshPro Font Asset", new GUILayoutOption[]
			{
				GUILayout.Width(290f)
			}) && GUI.enabled)
			{
				string text = string.Empty;
				if (this.font_renderMode < RenderModes.DistanceField16)
				{
					text = EditorUtility.SaveFilePanel("Save TextMesh Pro! Font Asset File", new FileInfo(AssetDatabase.GetAssetPath(this.font_TTF)).DirectoryName, this.font_TTF.name, "asset");
					if (text.Length == 0)
					{
						return;
					}
					this.Save_Normal_FontAsset(text);
				}
				else if (this.font_renderMode >= RenderModes.DistanceField16)
				{
					text = EditorUtility.SaveFilePanel("Save TextMesh Pro! Font Asset File", new FileInfo(AssetDatabase.GetAssetPath(this.font_TTF)).DirectoryName, this.font_TTF.name + " SDF", "asset");
					if (text.Length == 0)
					{
						return;
					}
					this.Save_SDF_FontAsset(text);
				}
			}
			GUI.enabled = true;
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			GUILayout.Space(25f);
			Rect controlRect = EditorGUILayout.GetControlRect(false, 5f, new GUILayoutOption[0]);
			if (Event.current.type == EventType.Repaint)
			{
				this.m_UI_Panel_Size = controlRect;
			}
			GUILayout.EndVertical();
		}

		private void UpdateRenderFeedbackWindow()
		{
			this.font_size = this.m_font_faceInfo.pointSize;
			string text = string.Empty;
			string text2 = (this.m_font_faceInfo.characterCount != this.m_character_Count) ? "<color=#ffff00>" : "<color=#C0ffff>";
			string text3 = "<color=#C0ffff>";
			text = string.Concat(new string[]
			{
				this.output_name_label,
				"<b>",
				text3,
				this.m_font_faceInfo.name,
				"</color></b>"
			});
			string text4;
			if (text.Length > 60)
			{
				text4 = text;
				text = string.Concat(new object[]
				{
					text4,
					"\n",
					this.output_size_label,
					"<b>",
					text3,
					this.m_font_faceInfo.pointSize,
					"</color></b>"
				});
			}
			else
			{
				text4 = text;
				text = string.Concat(new object[]
				{
					text4,
					"  ",
					this.output_size_label,
					"<b>",
					text3,
					this.m_font_faceInfo.pointSize,
					"</color></b>"
				});
			}
			text4 = text;
			text = string.Concat(new object[]
			{
				text4,
				"\n",
				this.output_count_label,
				"<b>",
				text2,
				this.m_font_faceInfo.characterCount,
				"/",
				this.m_character_Count,
				"</color></b>"
			});
			text += "\n\n<color=#ffff00><b>Missing Characters</b></color>";
			text += "\n----------------------------------------";
			this.output_feedback = text;
			for (int i = 0; i < this.m_character_Count; i++)
			{
				if (this.m_font_glyphInfo[i].x == -1f)
				{
					text4 = text;
					text = string.Concat(new object[]
					{
						text4,
						"\nID: <color=#C0ffff>",
						this.m_font_glyphInfo[i].id,
						"\t</color>Hex: <color=#C0ffff>",
						this.m_font_glyphInfo[i].id.ToString("X"),
						"\t</color>Char [<color=#C0ffff>",
						(char)this.m_font_glyphInfo[i].id,
						"</color>]"
					});
					if (text.Length < 16300)
					{
						this.output_feedback = text;
					}
				}
			}
			if (text.Length > 16300)
			{
				this.output_feedback += "\n\n<color=#ffff00>Report truncated.</color>\n<color=#c0ffff>See</color> \"TextMesh Pro\\Glyph Report.txt\"";
			}
			string fullPath = Path.GetFullPath("Assets/..");
			text = Regex.Replace(text, "<[^>]*>", string.Empty);
			File.WriteAllText(fullPath + "/Assets/Glyph Report.txt", text);
			AssetDatabase.Refresh();
		}

		private void CreateFontTexture()
		{
			this.m_font_Atlas = new Texture2D(this.font_atlas_width, this.font_atlas_height, TextureFormat.Alpha8, false, true);
			Color32[] array = new Color32[this.font_atlas_width * this.font_atlas_height];
			for (int i = 0; i < this.font_atlas_width * this.font_atlas_height; i++)
			{
				byte b = this.m_texture_buffer[i];
				array[i] = new Color32(b, b, b, b);
			}
			if (this.font_renderMode == RenderModes.RasterHinted)
			{
				this.m_font_Atlas.filterMode = FilterMode.Point;
			}
			this.m_font_Atlas.SetPixels32(array, 0);
			this.m_font_Atlas.Apply(false, true);
			this.UpdateEditorWindowSize((float)this.m_font_Atlas.width, (float)this.m_font_Atlas.height);
		}

		private void Save_Normal_FontAsset(string filePath)
		{
			filePath = filePath.Substring(0, filePath.Length - 6);
			string dataPath = Application.dataPath;
			if (filePath.IndexOf(dataPath, StringComparison.InvariantCultureIgnoreCase) == -1)
			{
				Debug.LogError("You're saving the font asset in a directory outside of this project folder. This is not supported. Please select a directory under \"" + dataPath + "\"");
				return;
			}
			string path = filePath.Substring(dataPath.Length - 6);
			string directoryName = Path.GetDirectoryName(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			string str = directoryName + "/" + fileNameWithoutExtension;
			TMP_FontAsset tmp_FontAsset = AssetDatabase.LoadAssetAtPath(str + ".asset", typeof(TMP_FontAsset)) as TMP_FontAsset;
			if (tmp_FontAsset == null)
			{
				tmp_FontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
				AssetDatabase.CreateAsset(tmp_FontAsset, str + ".asset");
				tmp_FontAsset.fontAssetType = TMP_FontAsset.FontAssetTypes.Bitmap;
				FaceInfo faceInfo = this.GetFaceInfo(this.m_font_faceInfo, 1);
				tmp_FontAsset.AddFaceInfo(faceInfo);
				TMP_Glyph[] glyphInfo = this.GetGlyphInfo(this.m_font_glyphInfo, 1);
				tmp_FontAsset.AddGlyphInfo(glyphInfo);
				if (this.includeKerningPairs)
				{
					string assetPath = AssetDatabase.GetAssetPath(this.font_TTF);
					KerningTable kerningTable = this.GetKerningTable(assetPath, (int)faceInfo.PointSize);
					tmp_FontAsset.AddKerningInfo(kerningTable);
				}
				tmp_FontAsset.atlas = this.m_font_Atlas;
				this.m_font_Atlas.name = fileNameWithoutExtension + " Atlas";
				AssetDatabase.AddObjectToAsset(this.m_font_Atlas, tmp_FontAsset);
				Shader shader = Shader.Find("TextMeshPro/Bitmap");
				Material material = new Material(shader);
				material.name = fileNameWithoutExtension + " Material";
				material.SetTexture(ShaderUtilities.ID_MainTex, this.m_font_Atlas);
				tmp_FontAsset.material = material;
				AssetDatabase.AddObjectToAsset(material, tmp_FontAsset);
			}
			else
			{
				Material[] array = TMP_EditorUtility.FindMaterialReferences(tmp_FontAsset);
				UnityEngine.Object.DestroyImmediate(tmp_FontAsset.atlas, true);
				tmp_FontAsset.fontAssetType = TMP_FontAsset.FontAssetTypes.Bitmap;
				FaceInfo faceInfo2 = this.GetFaceInfo(this.m_font_faceInfo, 1);
				tmp_FontAsset.AddFaceInfo(faceInfo2);
				TMP_Glyph[] glyphInfo2 = this.GetGlyphInfo(this.m_font_glyphInfo, 1);
				tmp_FontAsset.AddGlyphInfo(glyphInfo2);
				if (this.includeKerningPairs)
				{
					string assetPath2 = AssetDatabase.GetAssetPath(this.font_TTF);
					KerningTable kerningTable2 = this.GetKerningTable(assetPath2, (int)faceInfo2.PointSize);
					tmp_FontAsset.AddKerningInfo(kerningTable2);
				}
				tmp_FontAsset.atlas = this.m_font_Atlas;
				this.m_font_Atlas.name = fileNameWithoutExtension + " Atlas";
				this.m_font_Atlas.hideFlags = HideFlags.None;
				tmp_FontAsset.material.hideFlags = HideFlags.None;
				AssetDatabase.AddObjectToAsset(this.m_font_Atlas, tmp_FontAsset);
				tmp_FontAsset.material.SetTexture(ShaderUtilities.ID_MainTex, tmp_FontAsset.atlas);
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetTexture(ShaderUtilities.ID_MainTex, this.m_font_Atlas);
				}
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tmp_FontAsset));
			tmp_FontAsset.ReadFontDefinition();
			AssetDatabase.Refresh();
			this.m_font_Atlas = null;
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, tmp_FontAsset);
		}

		private void Save_SDF_FontAsset(string filePath)
		{
			filePath = filePath.Substring(0, filePath.Length - 6);
			string dataPath = Application.dataPath;
			if (filePath.IndexOf(dataPath, StringComparison.InvariantCultureIgnoreCase) == -1)
			{
				Debug.LogError("You're saving the font asset in a directory outside of this project folder. This is not supported. Please select a directory under \"" + dataPath + "\"");
				return;
			}
			string path = filePath.Substring(dataPath.Length - 6);
			string directoryName = Path.GetDirectoryName(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			string str = directoryName + "/" + fileNameWithoutExtension;
			TMP_FontAsset tmp_FontAsset = AssetDatabase.LoadAssetAtPath(str + ".asset", typeof(TMP_FontAsset)) as TMP_FontAsset;
			if (tmp_FontAsset == null)
			{
				tmp_FontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
				AssetDatabase.CreateAsset(tmp_FontAsset, str + ".asset");
				tmp_FontAsset.fontAssetType = TMP_FontAsset.FontAssetTypes.SDF;
				int scaleFactor = (this.font_renderMode < RenderModes.DistanceField16) ? this.font_scaledownFactor : 1;
				FaceInfo faceInfo = this.GetFaceInfo(this.m_font_faceInfo, scaleFactor);
				tmp_FontAsset.AddFaceInfo(faceInfo);
				TMP_Glyph[] glyphInfo = this.GetGlyphInfo(this.m_font_glyphInfo, scaleFactor);
				tmp_FontAsset.AddGlyphInfo(glyphInfo);
				if (this.includeKerningPairs)
				{
					string assetPath = AssetDatabase.GetAssetPath(this.font_TTF);
					KerningTable kerningTable = this.GetKerningTable(assetPath, (int)faceInfo.PointSize);
					tmp_FontAsset.AddKerningInfo(kerningTable);
				}
				tmp_FontAsset.atlas = this.m_font_Atlas;
				this.m_font_Atlas.name = fileNameWithoutExtension + " Atlas";
				AssetDatabase.AddObjectToAsset(this.m_font_Atlas, tmp_FontAsset);
				Shader shader = Shader.Find("TextMeshPro/Distance Field");
				Material material = new Material(shader);
				material.name = fileNameWithoutExtension + " Material";
				material.SetTexture(ShaderUtilities.ID_MainTex, this.m_font_Atlas);
				material.SetFloat(ShaderUtilities.ID_TextureWidth, (float)this.m_font_Atlas.width);
				material.SetFloat(ShaderUtilities.ID_TextureHeight, (float)this.m_font_Atlas.height);
				int num = this.font_padding + 1;
				material.SetFloat(ShaderUtilities.ID_GradientScale, (float)num);
				material.SetFloat(ShaderUtilities.ID_WeightNormal, tmp_FontAsset.normalStyle);
				material.SetFloat(ShaderUtilities.ID_WeightBold, tmp_FontAsset.boldStyle);
				tmp_FontAsset.material = material;
				AssetDatabase.AddObjectToAsset(material, tmp_FontAsset);
			}
			else
			{
				Material[] array = TMP_EditorUtility.FindMaterialReferences(tmp_FontAsset);
				UnityEngine.Object.DestroyImmediate(tmp_FontAsset.atlas, true);
				tmp_FontAsset.fontAssetType = TMP_FontAsset.FontAssetTypes.SDF;
				int scaleFactor2 = (this.font_renderMode < RenderModes.DistanceField16) ? this.font_scaledownFactor : 1;
				FaceInfo faceInfo2 = this.GetFaceInfo(this.m_font_faceInfo, scaleFactor2);
				tmp_FontAsset.AddFaceInfo(faceInfo2);
				TMP_Glyph[] glyphInfo2 = this.GetGlyphInfo(this.m_font_glyphInfo, scaleFactor2);
				tmp_FontAsset.AddGlyphInfo(glyphInfo2);
				if (this.includeKerningPairs)
				{
					string assetPath2 = AssetDatabase.GetAssetPath(this.font_TTF);
					KerningTable kerningTable2 = this.GetKerningTable(assetPath2, (int)faceInfo2.PointSize);
					tmp_FontAsset.AddKerningInfo(kerningTable2);
				}
				tmp_FontAsset.atlas = this.m_font_Atlas;
				this.m_font_Atlas.name = fileNameWithoutExtension + " Atlas";
				this.m_font_Atlas.hideFlags = HideFlags.None;
				tmp_FontAsset.material.hideFlags = HideFlags.None;
				AssetDatabase.AddObjectToAsset(this.m_font_Atlas, tmp_FontAsset);
				tmp_FontAsset.material.SetTexture(ShaderUtilities.ID_MainTex, tmp_FontAsset.atlas);
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetTexture(ShaderUtilities.ID_MainTex, this.m_font_Atlas);
					array[i].SetFloat(ShaderUtilities.ID_TextureWidth, (float)this.m_font_Atlas.width);
					array[i].SetFloat(ShaderUtilities.ID_TextureHeight, (float)this.m_font_Atlas.height);
					int num2 = this.font_padding + 1;
					array[i].SetFloat(ShaderUtilities.ID_GradientScale, (float)num2);
					array[i].SetFloat(ShaderUtilities.ID_WeightNormal, tmp_FontAsset.normalStyle);
					array[i].SetFloat(ShaderUtilities.ID_WeightBold, tmp_FontAsset.boldStyle);
				}
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tmp_FontAsset));
			tmp_FontAsset.ReadFontDefinition();
			AssetDatabase.Refresh();
			this.m_font_Atlas = null;
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, tmp_FontAsset);
		}

		private FontCreationSetting SaveFontCreationSettings()
		{
			return new FontCreationSetting
			{
				fontSourcePath = AssetDatabase.GetAssetPath(this.font_TTF),
				fontSizingMode = this.FontSizingOption_Selection,
				fontSize = this.font_size,
				fontPadding = this.font_padding,
				fontPackingMode = (int)this.m_fontPackingSelection,
				fontAtlasWidth = this.font_atlas_width,
				fontAtlasHeight = this.font_atlas_height,
				fontCharacterSet = this.font_CharacterSet_Selection,
				fontStyle = (int)this.font_style,
				fontStlyeModifier = this.font_style_mod,
				fontRenderMode = (int)this.font_renderMode,
				fontKerning = this.includeKerningPairs
			};
		}

		private void UpdateEditorWindowSize(float width, float height)
		{
			this.m_previewWindow_Size = new Vector2(768f, 768f);
			if (width > height)
			{
				this.m_previewWindow_Size = new Vector2(768f, height / (width / 768f));
			}
			else if (height > width)
			{
				this.m_previewWindow_Size = new Vector2(width / (height / 768f), 768f);
			}
			this.m_editorWindow.minSize = new Vector2(this.m_previewWindow_Size.x + 330f, Mathf.Max(this.m_UI_Panel_Size.y + 20f, this.m_previewWindow_Size.y + 20f));
			this.m_editorWindow.maxSize = this.m_editorWindow.minSize + new Vector2(0.25f, 0f);
		}

		private void DrawPreview()
		{
			GUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(this.m_previewWindow_Size.x, this.m_previewWindow_Size.y, TMP_UIStyleManager.Section_Label);
			if (this.m_destination_Atlas != null && this.previewSelection == TMPro_FontAssetCreatorWindow.PreviewSelectionTypes.PreviewDistanceField)
			{
				EditorGUI.DrawTextureAlpha(new Rect(rect.x, rect.y, this.m_previewWindow_Size.x, this.m_previewWindow_Size.y), this.m_destination_Atlas, ScaleMode.ScaleToFit);
			}
			else if (this.m_font_Atlas != null && this.previewSelection == TMPro_FontAssetCreatorWindow.PreviewSelectionTypes.PreviewFont)
			{
				EditorGUI.DrawTextureAlpha(new Rect(rect.x, rect.y, this.m_previewWindow_Size.x, this.m_previewWindow_Size.y), this.m_font_Atlas, ScaleMode.ScaleToFit);
			}
			GUILayout.EndVertical();
		}

		private FaceInfo GetFaceInfo(FT_FaceInfo ft_face, int scaleFactor)
		{
			FaceInfo faceInfo = new FaceInfo();
			faceInfo.Name = ft_face.name;
			faceInfo.PointSize = (float)ft_face.pointSize / (float)scaleFactor;
			faceInfo.Padding = (float)(ft_face.padding / scaleFactor);
			faceInfo.LineHeight = ft_face.lineHeight / (float)scaleFactor;
			faceInfo.CapHeight = 0f;
			faceInfo.Baseline = 0f;
			faceInfo.Ascender = ft_face.ascender / (float)scaleFactor;
			faceInfo.Descender = ft_face.descender / (float)scaleFactor;
			faceInfo.CenterLine = ft_face.centerLine / (float)scaleFactor;
			faceInfo.Underline = ft_face.underline / (float)scaleFactor;
			faceInfo.UnderlineThickness = ((ft_face.underlineThickness != 0f) ? (ft_face.underlineThickness / (float)scaleFactor) : 5f);
			faceInfo.strikethrough = (faceInfo.Ascender + faceInfo.Descender) / 2.75f;
			faceInfo.strikethroughThickness = faceInfo.UnderlineThickness;
			faceInfo.SuperscriptOffset = faceInfo.Ascender;
			faceInfo.SubscriptOffset = faceInfo.Underline;
			faceInfo.SubSize = 0.5f;
			faceInfo.AtlasWidth = (float)(ft_face.atlasWidth / scaleFactor);
			faceInfo.AtlasHeight = (float)(ft_face.atlasHeight / scaleFactor);
			return faceInfo;
		}

		private TMP_Glyph[] GetGlyphInfo(FT_GlyphInfo[] ft_glyphs, int scaleFactor)
		{
			List<TMP_Glyph> list = new List<TMP_Glyph>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < ft_glyphs.Length; i++)
			{
				TMP_Glyph tmp_Glyph = new TMP_Glyph();
				tmp_Glyph.id = ft_glyphs[i].id;
				tmp_Glyph.x = ft_glyphs[i].x / (float)scaleFactor;
				tmp_Glyph.y = ft_glyphs[i].y / (float)scaleFactor;
				tmp_Glyph.width = ft_glyphs[i].width / (float)scaleFactor;
				tmp_Glyph.height = ft_glyphs[i].height / (float)scaleFactor;
				tmp_Glyph.xOffset = ft_glyphs[i].xOffset / (float)scaleFactor;
				tmp_Glyph.yOffset = ft_glyphs[i].yOffset / (float)scaleFactor;
				tmp_Glyph.xAdvance = ft_glyphs[i].xAdvance / (float)scaleFactor;
				if (tmp_Glyph.x != -1f)
				{
					list.Add(tmp_Glyph);
					list2.Add(tmp_Glyph.id);
				}
			}
			this.m_kerningSet = list2.ToArray();
			return list.ToArray();
		}

		public KerningTable GetKerningTable(string fontFilePath, int pointSize)
		{
			KerningTable kerningTable = new KerningTable();
			kerningTable.kerningPairs = new List<KerningPair>();
			FT_KerningPair[] array = new FT_KerningPair[7500];
			int num = TMPro_FontPlugin.FT_GetKerningPairs(fontFilePath, this.m_kerningSet, this.m_kerningSet.Length, array);
			for (int i = 0; i < num; i++)
			{
				KerningPair kp = new KerningPair(array[i].ascII_Left, array[i].ascII_Right, array[i].xAdvanceOffset * (float)pointSize);
				int num2 = kerningTable.kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == kp.AscII_Left && item.AscII_Right == kp.AscII_Right);
				if (num2 == -1)
				{
					kerningTable.kerningPairs.Add(kp);
				}
				else if (!TMP_Settings.warningsDisabled)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"Kerning Key for [",
						kp.AscII_Left,
						"] and [",
						kp.AscII_Right,
						"] is a duplicate."
					}));
				}
			}
			return kerningTable;
		}

		private string[] UpdateShaderList(RenderModes mode, out Shader[] shaders)
		{
			string text = "t:Shader TMP_";
			if (mode == RenderModes.DistanceField16 || mode == RenderModes.DistanceField32)
			{
				text += " SDF";
			}
			else
			{
				text += " Bitmap";
			}
			string[] array = AssetDatabase.FindAssets(text);
			string[] array2 = new string[array.Length];
			shaders = new Shader[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(array[i]);
				Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);
				shaders[i] = shader;
				string text2 = shader.name.Replace("TextMeshPro/", string.Empty);
				text2 = text2.Replace("Mobile/", "Mobile - ");
				array2[i] = text2;
			}
			return array2;
		}

		private string[] FontSizingOptions = new string[]
		{
			"Auto Sizing",
			"Custom Size"
		};

		private int FontSizingOption_Selection;

		private string[] FontResolutionLabels = new string[]
		{
			"16",
			"32",
			"64",
			"128",
			"256",
			"512",
			"1024",
			"2048",
			"4096",
			"8192",
			"16384",
			"32768"
		};

		private int[] FontAtlasResolutions = new int[]
		{
			16,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048,
			4096,
			8192,
            16384,
            32768
		};

		private string[] FontCharacterSets = new string[]
		{
			"ASCII",
			"Extended ASCII",
			"ASCII Lowercase",
			"ASCII Uppercase",
			"Numbers + Symbols",
			"Custom Range",
			"Unicode Range (Hex)",
			"Custom Characters",
			"Characters from File"
		};

		private TMPro_FontAssetCreatorWindow.FontPackingModes m_fontPackingSelection;

		private int font_CharacterSet_Selection;

		private TMPro_FontAssetCreatorWindow.PreviewSelectionTypes previewSelection;

		private string characterSequence = string.Empty;

		private string output_feedback = string.Empty;

		private string output_name_label = "Font: ";

		private string output_size_label = "Pt. Size: ";

		private string output_count_label = "Characters packed: ";

		private int m_character_Count;

		private Vector2 output_ScrollPosition;

		private Color[] Output;

		private bool isDistanceMapReady;

		private bool isRepaintNeeded;

		private Rect progressRect;

		public static float ProgressPercentage;

		private float m_renderingProgress;

		private bool isRenderingDone;

		private bool isProcessing;

		private UnityEngine.Object font_TTF;

		private TMP_FontAsset m_fontAssetSelection;

		private TextAsset characterList;

		private int font_size;

		private int font_padding = 5;

		private FaceStyles font_style;

		private float font_style_mod = 2f;

		private RenderModes font_renderMode = RenderModes.DistanceField16;

		private int font_atlas_width = 512;

		private int font_atlas_height = 512;

		private int font_scaledownFactor = 1;

		private FT_FaceInfo m_font_faceInfo;

		private FT_GlyphInfo[] m_font_glyphInfo;

		private byte[] m_texture_buffer;

		private Texture2D m_font_Atlas;

		private Texture2D m_destination_Atlas;

		private bool includeKerningPairs;

		private int[] m_kerningSet;

		private EditorWindow m_editorWindow;

		private Vector2 m_previewWindow_Size = new Vector2(768f, 768f);

		private Rect m_UI_Panel_Size;

		private enum FontPackingModes
		{
			Fast,
			Optimum = 4
		}

		private enum PreviewSelectionTypes
		{
			PreviewFont,
			PreviewTexture,
			PreviewDistanceField
		}
	}
}
