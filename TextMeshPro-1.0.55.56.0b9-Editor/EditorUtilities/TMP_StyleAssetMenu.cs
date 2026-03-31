using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public static class TMP_StyleAssetMenu
	{
		[MenuItem("Assets/Create/TextMeshPro/Style Sheet", false, 120)]
		public static void CreateTextMeshProObjectPerform()
		{
			string text;
			if (Selection.assetGUIDs.Length == 0)
			{
				text = "Assets";
			}
			else
			{
				text = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
				string extension = Path.GetExtension(text);
				if (extension != string.Empty)
				{
					text = Path.GetDirectoryName(text);
				}
			}
			string path = AssetDatabase.GenerateUniqueAssetPath(text + "/TMP StyleSheet.asset");
			TMP_StyleSheet tmp_StyleSheet = ScriptableObject.CreateInstance<TMP_StyleSheet>();
			AssetDatabase.CreateAsset(tmp_StyleSheet, path);
			EditorUtility.SetDirty(tmp_StyleSheet);
			AssetDatabase.SaveAssets();
		}
	}
}
