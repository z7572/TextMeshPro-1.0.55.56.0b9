using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public static class TMP_ColorGradientAssetMenu
	{
		[MenuItem("Assets/Create/TextMeshPro/Color Gradient", false, 110)]
		public static void CreateColorGradient(MenuCommand context)
		{
			string text;
			if (Selection.assetGUIDs.Length == 0)
			{
				text = "Assets/New TMP Color Gradient.asset";
			}
			else
			{
				text = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
			}
			if (Directory.Exists(text))
			{
				text += "/New TMP Color Gradient.asset";
			}
			else
			{
				text = Path.GetDirectoryName(text) + "/New TMP Color Gradient.asset";
			}
			text = AssetDatabase.GenerateUniqueAssetPath(text);
			TMP_ColorGradient tmp_ColorGradient = ScriptableObject.CreateInstance<TMP_ColorGradient>();
			AssetDatabase.CreateAsset(tmp_ColorGradient, text);
			AssetDatabase.SaveAssets();
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tmp_ColorGradient));
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = tmp_ColorGradient;
		}
	}
}
