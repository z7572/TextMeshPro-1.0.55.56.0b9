using System;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using TMPro.SpriteAssetUtilities;
using UnityEditor;
using UnityEngine;

namespace TMPro
{
	public class TMP_SpriteAssetImporter : EditorWindow
	{
		[MenuItem("Window/TextMeshPro/Sprite Importer")]
		public static void ShowFontAtlasCreatorWindow()
		{
			TMP_SpriteAssetImporter window = EditorWindow.GetWindow<TMP_SpriteAssetImporter>();
			window.titleContent = new GUIContent("Sprite Importer");
			window.Focus();
		}

		private void OnEnable()
		{
			this.SetEditorWindowSize();
			TMP_UIStyleManager.GetUIStyles();
		}

		public void OnGUI()
		{
			this.DrawEditorPanel();
		}

		private void DrawEditorPanel()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("<b>TMP Sprite Importer</b>", TMP_UIStyleManager.Section_Label, new GUILayoutOption[0]);
			GUILayout.Label("Import Settings", TMP_UIStyleManager.Section_Label, new GUILayoutOption[]
			{
				GUILayout.Width(150f)
			});
			GUILayout.BeginVertical(TMP_UIStyleManager.TextureAreaBox, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			this.m_JsonFile = (EditorGUILayout.ObjectField("Sprite Data Source", this.m_JsonFile, typeof(TextAsset), false, new GUILayoutOption[0]) as TextAsset);
			this.m_SpriteDataFormat = (SpriteAssetImportFormats)EditorGUILayout.EnumPopup("Import Format", this.m_SpriteDataFormat, new GUILayoutOption[0]);
			this.m_SpriteAtlas = (EditorGUILayout.ObjectField("Sprite Texture Atlas", this.m_SpriteAtlas, typeof(Texture2D), false, new GUILayoutOption[0]) as Texture2D);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_CreationFeedback = string.Empty;
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Create Sprite Asset", new GUILayoutOption[0]))
			{
				this.m_CreationFeedback = string.Empty;
				if (this.m_SpriteDataFormat == SpriteAssetImportFormats.TexturePacker)
				{
					TexturePacker.SpriteDataObject spriteDataObject = JsonUtility.FromJson<TexturePacker.SpriteDataObject>(this.m_JsonFile.text);
					if (spriteDataObject != null && spriteDataObject.frames != null && spriteDataObject.frames.Count > 0)
					{
						int count = spriteDataObject.frames.Count;
						this.m_CreationFeedback = "<b>Import Results</b>\n--------------------\n";
						string creationFeedback = this.m_CreationFeedback;
						this.m_CreationFeedback = string.Concat(new object[]
						{
							creationFeedback,
							"<color=#C0ffff><b>",
							count,
							"</b></color> Sprites were imported from file."
						});
						this.m_SpriteInfoList = this.CreateSpriteInfoList(spriteDataObject);
					}
				}
			}
			GUILayout.Space(5f);
			GUILayout.BeginVertical(TMP_UIStyleManager.TextAreaBoxWindow, new GUILayoutOption[]
			{
				GUILayout.Height(60f)
			});
			EditorGUILayout.LabelField(this.m_CreationFeedback, TMP_UIStyleManager.Label, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUI.enabled = (this.m_SpriteInfoList != null);
			if (GUILayout.Button("Save Sprite Asset", new GUILayoutOption[0]))
			{
				string text = string.Empty;
				text = EditorUtility.SaveFilePanel("Save Sprite Asset File", new FileInfo(AssetDatabase.GetAssetPath(this.m_JsonFile)).DirectoryName, this.m_JsonFile.name, "asset");
				if (text.Length == 0)
				{
					return;
				}
				this.SaveSpriteAsset(text);
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		private List<TMP_Sprite> CreateSpriteInfoList(TexturePacker.SpriteDataObject spriteDataObject)
		{
			List<TexturePacker.SpriteData> frames = spriteDataObject.frames;
			List<TMP_Sprite> list = new List<TMP_Sprite>();
			for (int i = 0; i < frames.Count; i++)
			{
				TMP_Sprite tmp_Sprite = new TMP_Sprite();
				tmp_Sprite.id = i;
				tmp_Sprite.name = Path.GetFileNameWithoutExtension(frames[i].filename);
				tmp_Sprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(tmp_Sprite.name);
				int num = tmp_Sprite.name.IndexOf('-');
				int unicode;
				if (num != -1)
				{
					unicode = TMP_TextUtilities.StringToInt(tmp_Sprite.name.Substring(num + 1));
				}
				else
				{
					unicode = TMP_TextUtilities.StringToInt(tmp_Sprite.name);
				}
				tmp_Sprite.unicode = unicode;
				tmp_Sprite.x = frames[i].frame.x;
				tmp_Sprite.y = (float)this.m_SpriteAtlas.height - (frames[i].frame.y + frames[i].frame.h);
				tmp_Sprite.width = frames[i].frame.w;
				tmp_Sprite.height = frames[i].frame.h;
				tmp_Sprite.pivot = frames[i].pivot;
				tmp_Sprite.xAdvance = tmp_Sprite.width;
				tmp_Sprite.scale = 1f;
				tmp_Sprite.xOffset = 0f - tmp_Sprite.width * tmp_Sprite.pivot.x;
				tmp_Sprite.yOffset = tmp_Sprite.height - tmp_Sprite.height * tmp_Sprite.pivot.y;
				list.Add(tmp_Sprite);
			}
			return list;
		}

		private void SaveSpriteAsset(string filePath)
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
			this.m_SpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
			AssetDatabase.CreateAsset(this.m_SpriteAsset, str + ".asset");
			this.m_SpriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(this.m_SpriteAsset.name);
			this.m_SpriteAsset.spriteSheet = this.m_SpriteAtlas;
			this.m_SpriteAsset.spriteInfoList = this.m_SpriteInfoList;
			TMP_SpriteAssetImporter.AddDefaultMaterial(this.m_SpriteAsset);
		}

		private static void AddDefaultMaterial(TMP_SpriteAsset spriteAsset)
		{
			Shader shader = Shader.Find("TextMeshPro/Sprite");
			Material material = new Material(shader);
			material.SetTexture(ShaderUtilities.ID_MainTex, spriteAsset.spriteSheet);
			spriteAsset.material = material;
			material.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(material, spriteAsset);
		}

		private void SetEditorWindowSize()
		{
			Vector2 minSize = this.minSize;
			this.minSize = new Vector2(Mathf.Max(230f, minSize.x), Mathf.Max(300f, minSize.y));
		}

		private Texture2D m_SpriteAtlas;

		private SpriteAssetImportFormats m_SpriteDataFormat = SpriteAssetImportFormats.TexturePacker;

		private TextAsset m_JsonFile;

		private string m_CreationFeedback;

		private TMP_SpriteAsset m_SpriteAsset;

		private List<TMP_Sprite> m_SpriteInfoList = new List<TMP_Sprite>();
	}
}
