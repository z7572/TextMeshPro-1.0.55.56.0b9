using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public static class TMP_EditorUtility
	{
		public static string packageRelativePath
		{
			get
			{
				if (string.IsNullOrEmpty(TMP_EditorUtility.m_PackagePath))
				{
					TMP_EditorUtility.m_PackagePath = TMP_EditorUtility.GetPackageRelativePath();
				}
				return TMP_EditorUtility.m_PackagePath;
			}
		}

		public static string packageFullPath
		{
			get
			{
				if (string.IsNullOrEmpty(TMP_EditorUtility.m_PackageFullPath))
				{
					TMP_EditorUtility.m_PackageFullPath = TMP_EditorUtility.GetPackageFullPath();
				}
				return TMP_EditorUtility.m_PackageFullPath;
			}
		}

		private static void GetGameview()
		{
			Assembly assembly = typeof(EditorWindow).Assembly;
			Type type = assembly.GetType("UnityEditor.GameView");
			TMP_EditorUtility.Gameview = EditorWindow.GetWindow(type);
		}

		public static void RepaintAll()
		{
			if (!TMP_EditorUtility.isInitialized)
			{
				TMP_EditorUtility.GetGameview();
				TMP_EditorUtility.isInitialized = true;
			}
			SceneView.RepaintAll();
			TMP_EditorUtility.Gameview.Repaint();
		}

		public static T CreateAsset<T>(string name) where T : ScriptableObject
		{
			string text = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (text.Length == 0)
			{
				text = "Assets/" + name + ".asset";
			}
			else if (Directory.Exists(text))
			{
				text = text + "/" + name + ".asset";
			}
			else
			{
				text = Path.GetDirectoryName(text) + "/" + name + ".asset";
			}
			T t = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(t, AssetDatabase.GenerateUniqueAssetPath(text));
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = t;
			return t;
		}

		public static Material[] FindMaterialReferences(TMP_FontAsset fontAsset)
		{
			List<Material> list = new List<Material>();
			Material material = fontAsset.material;
			list.Add(material);
			string filter = "t:Material " + fontAsset.name.Split(new char[]
			{
				' '
			})[0];
			string[] array = AssetDatabase.FindAssets(filter);
			for (int i = 0; i < array.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(array[i]);
				Material material2 = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
				if (material2.HasProperty(ShaderUtilities.ID_MainTex) && material2.GetTexture(ShaderUtilities.ID_MainTex) != null && material.GetTexture(ShaderUtilities.ID_MainTex) != null && material2.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() == material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
				{
					if (!list.Contains(material2))
					{
						list.Add(material2);
					}
				}
			}
			return list.ToArray();
		}

		public static TMP_FontAsset FindMatchingFontAsset(Material mat)
		{
			if (mat.GetTexture(ShaderUtilities.ID_MainTex) == null)
			{
				return null;
			}
			string[] dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(mat), false);
			for (int i = 0; i < dependencies.Length; i++)
			{
				TMP_FontAsset tmp_FontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(dependencies[i]);
				if (tmp_FontAsset != null)
				{
					return tmp_FontAsset;
				}
			}
			return null;
		}

		private static string GetPackageRelativePath()
		{
			string text = Path.GetFullPath("Packages/com.unity.textmeshpro");
			if (Directory.Exists(text))
			{
				return "Packages/com.unity.textmeshpro";
			}
			text = Path.GetFullPath("Assets/..");
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/Assets/Packages/com.unity.TextMeshPro/Editor Resources"))
				{
					return "Assets/Packages/com.unity.TextMeshPro";
				}
				if (Directory.Exists(text + "/Assets/TextMesh Pro/Editor Resources"))
				{
					return "Assets/TextMesh Pro";
				}
				string[] directories = Directory.GetDirectories(text, "TextMesh Pro", SearchOption.AllDirectories);
				text = TMP_EditorUtility.ValidateLocation(directories, text);
				if (text != null)
				{
					return text;
				}
			}
			return null;
		}

		private static string GetPackageFullPath()
		{
			string fullPath = Path.GetFullPath("Packages/com.unity.textmeshpro");
			if (Directory.Exists(fullPath))
			{
				return fullPath;
			}
			fullPath = Path.GetFullPath("Assets/..");
			if (Directory.Exists(fullPath))
			{
				if (Directory.Exists(fullPath + "/Assets/Packages/com.unity.TextMeshPro/Editor Resources"))
				{
					return fullPath + "/Assets/Packages/com.unity.TextMeshPro";
				}
				if (Directory.Exists(fullPath + "/Assets/TextMesh Pro/Editor Resources"))
				{
					return fullPath + "/Assets/TextMesh Pro";
				}
				string[] directories = Directory.GetDirectories(fullPath, "TextMesh Pro", SearchOption.AllDirectories);
				string text = TMP_EditorUtility.ValidateLocation(directories, fullPath);
				if (text != null)
				{
					return fullPath + text;
				}
			}
			return null;
		}

		private static string ValidateLocation(string[] paths, string projectPath)
		{
			for (int i = 0; i < paths.Length; i++)
			{
				if (Directory.Exists(paths[i] + "/Editor Resources"))
				{
					TMP_EditorUtility.folderPath = paths[i].Replace(projectPath, string.Empty);
					TMP_EditorUtility.folderPath = TMP_EditorUtility.folderPath.TrimStart(new char[]
					{
						'\\',
						'/'
					});
					return TMP_EditorUtility.folderPath;
				}
			}
			return null;
		}

		public static string GetDecimalCharacterSequence(int[] characterSet)
		{
			string text = string.Empty;
			int num = characterSet.Length;
			int num2 = characterSet[0];
			int num3 = num2;
			for (int i = 1; i < num; i++)
			{
				if (characterSet[i - 1] + 1 == characterSet[i])
				{
					num3 = characterSet[i];
				}
				else
				{
					if (num2 == num3)
					{
						text = text + num2 + ",";
					}
					else
					{
						string text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							num2,
							"-",
							num3,
							","
						});
					}
					num3 = (num2 = characterSet[i]);
				}
			}
			if (num2 == num3)
			{
				text += num2;
			}
			else
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					num2,
					"-",
					num3
				});
			}
			return text;
		}

		public static string GetUnicodeCharacterSequence(int[] characterSet)
		{
			string text = string.Empty;
			int num = characterSet.Length;
			int num2 = characterSet[0];
			int num3 = num2;
			for (int i = 1; i < num; i++)
			{
				if (characterSet[i - 1] + 1 == characterSet[i])
				{
					num3 = characterSet[i];
				}
				else
				{
					if (num2 == num3)
					{
						text = text + num2.ToString("X2") + ",";
					}
					else
					{
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							num2.ToString("X2"),
							"-",
							num3.ToString("X2"),
							","
						});
					}
					num3 = (num2 = characterSet[i]);
				}
			}
			if (num2 == num3)
			{
				text += num2.ToString("X2");
			}
			else
			{
				text = text + num2.ToString("X2") + "-" + num3.ToString("X2");
			}
			return text;
		}

		public static void DrawBox(Rect rect, float thickness, Color color)
		{
			EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y + thickness, rect.width + thickness * 2f, thickness), color);
			EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y + thickness, thickness, rect.height - thickness * 2f), color);
			EditorGUI.DrawRect(new Rect(rect.x - thickness, rect.y + rect.height - thickness * 2f, rect.width + thickness * 2f, thickness), color);
			EditorGUI.DrawRect(new Rect(rect.x + rect.width, rect.y + thickness, thickness, rect.height - thickness * 2f), color);
		}

		public static int GetHorizontalAlignmentGridValue(int value)
		{
			if ((value & 1) == 1)
			{
				return 0;
			}
			if ((value & 2) == 2)
			{
				return 1;
			}
			if ((value & 4) == 4)
			{
				return 2;
			}
			if ((value & 8) == 8)
			{
				return 3;
			}
			if ((value & 16) == 16)
			{
				return 4;
			}
			if ((value & 32) == 32)
			{
				return 5;
			}
			return 0;
		}

		public static int GetVerticalAlignmentGridValue(int value)
		{
			if ((value & 256) == 256)
			{
				return 0;
			}
			if ((value & 512) == 512)
			{
				return 1;
			}
			if ((value & 1024) == 1024)
			{
				return 2;
			}
			if ((value & 2048) == 2048)
			{
				return 3;
			}
			if ((value & 4096) == 4096)
			{
				return 4;
			}
			if ((value & 8192) == 8192)
			{
				return 5;
			}
			if ((value & 16384) == 16384)
			{
                return 6;
			}
			if ((value & 32768) == 32768)
			{
				return 7;
			}
			return 0;
		}

		[SerializeField]
		private static string m_PackagePath;

		[SerializeField]
		private static string m_PackageFullPath;

		private static string folderPath = "Not Found";

		private static EditorWindow Gameview;

		private static bool isInitialized;
	}
}
