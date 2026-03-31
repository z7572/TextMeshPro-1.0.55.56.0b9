using System;
using UnityEditor;

namespace TMPro.EditorUtilities
{
	public class TMPro_PackageImportPostProcessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			for (int i = 0; i < deletedAssets.Length; i++)
			{
				if (deletedAssets[i] == "Assets/TextMesh Pro")
				{
					string text = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
					if (text.Contains("TMP_PRESENT;"))
					{
						text = text.Replace("TMP_PRESENT;", string.Empty);
						PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, text);
					}
					else if (text.Contains("TMP_PRESENT"))
					{
						text = text.Replace("TMP_PRESENT", string.Empty);
						PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, text);
					}
				}
			}
		}
	}
}
