using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public class TMP_ContextMenus : Editor
	{
		[MenuItem("CONTEXT/Texture/Copy", false, 2000)]
		private static void CopyTexture(MenuCommand command)
		{
			TMP_ContextMenus.m_copiedTexture = (command.context as Texture);
		}

		[MenuItem("CONTEXT/Material/Select Material", false, 500)]
		private static void SelectMaterial(MenuCommand command)
		{
			Material obj = command.context as Material;
			EditorUtility.FocusProjectWindow();
			EditorGUIUtility.PingObject(obj);
		}

		[MenuItem("CONTEXT/Material/Create Material Preset", false)]
		private static void DuplicateMaterial(MenuCommand command)
		{
			Material material = (Material)command.context;
			if (!EditorUtility.IsPersistent(material))
			{
				Debug.LogWarning("Material is an instance and cannot be converted into a permanent asset.");
				return;
			}
			string str = AssetDatabase.GetAssetPath(material).Split(new char[]
			{
				'.'
			})[0];
			Material material2 = new Material(material);
			material2.shaderKeywords = material.shaderKeywords;
			AssetDatabase.CreateAsset(material2, AssetDatabase.GenerateUniqueAssetPath(str + ".mat"));
			if (Selection.activeGameObject != null)
			{
				TMP_Text component = Selection.activeGameObject.GetComponent<TMP_Text>();
				if (component != null)
				{
					component.fontSharedMaterial = material2;
				}
				else
				{
					TMP_SubMesh component2 = Selection.activeGameObject.GetComponent<TMP_SubMesh>();
					if (component2 != null)
					{
						component2.sharedMaterial = material2;
					}
					else
					{
						TMP_SubMeshUI component3 = Selection.activeGameObject.GetComponent<TMP_SubMeshUI>();
						if (component3 != null)
						{
							component3.sharedMaterial = material2;
						}
					}
				}
			}
			EditorUtility.FocusProjectWindow();
			EditorGUIUtility.PingObject(material2);
		}

		[MenuItem("CONTEXT/Material/Copy Material Properties", false)]
		private static void CopyMaterialProperties(MenuCommand command)
		{
			Material material;
			if (command.context.GetType() == typeof(Material))
			{
				material = (Material)command.context;
			}
			else
			{
				material = Selection.activeGameObject.GetComponent<CanvasRenderer>().GetMaterial();
			}
			TMP_ContextMenus.m_copiedProperties = new Material(material);
			TMP_ContextMenus.m_copiedProperties.shaderKeywords = material.shaderKeywords;
			TMP_ContextMenus.m_copiedProperties.hideFlags = HideFlags.DontSave;
		}

		[MenuItem("CONTEXT/Material/Paste Material Properties", false)]
		private static void PasteMaterialProperties(MenuCommand command)
		{
			if (TMP_ContextMenus.m_copiedProperties == null)
			{
				Debug.LogWarning("No Material Properties to Paste. Use Copy Material Properties first.");
				return;
			}
			Material material;
			if (command.context.GetType() == typeof(Material))
			{
				material = (Material)command.context;
			}
			else
			{
				material = Selection.activeGameObject.GetComponent<CanvasRenderer>().GetMaterial();
			}
			Undo.RecordObject(material, "Paste Material");
			ShaderUtilities.GetShaderPropertyIDs();
			if (material.HasProperty(ShaderUtilities.ID_GradientScale))
			{
				TMP_ContextMenus.m_copiedProperties.SetTexture(ShaderUtilities.ID_MainTex, material.GetTexture(ShaderUtilities.ID_MainTex));
				TMP_ContextMenus.m_copiedProperties.SetFloat(ShaderUtilities.ID_GradientScale, material.GetFloat(ShaderUtilities.ID_GradientScale));
				TMP_ContextMenus.m_copiedProperties.SetFloat(ShaderUtilities.ID_TextureWidth, material.GetFloat(ShaderUtilities.ID_TextureWidth));
				TMP_ContextMenus.m_copiedProperties.SetFloat(ShaderUtilities.ID_TextureHeight, material.GetFloat(ShaderUtilities.ID_TextureHeight));
			}
			EditorShaderUtilities.CopyMaterialProperties(TMP_ContextMenus.m_copiedProperties, material);
			material.shaderKeywords = TMP_ContextMenus.m_copiedProperties.shaderKeywords;
			TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, material);
		}

		[MenuItem("CONTEXT/Material/Reset", false, 2100)]
		private static void ResetSettings(MenuCommand command)
		{
			Material material;
			if (command.context.GetType() == typeof(Material))
			{
				material = (Material)command.context;
			}
			else
			{
				material = Selection.activeGameObject.GetComponent<CanvasRenderer>().GetMaterial();
			}
			Undo.RecordObject(material, "Reset Material");
			Material material2 = new Material(material.shader);
			ShaderUtilities.GetShaderPropertyIDs();
			if (material.HasProperty(ShaderUtilities.ID_GradientScale))
			{
				material2.SetTexture(ShaderUtilities.ID_MainTex, material.GetTexture(ShaderUtilities.ID_MainTex));
				material2.SetFloat(ShaderUtilities.ID_GradientScale, material.GetFloat(ShaderUtilities.ID_GradientScale));
				material2.SetFloat(ShaderUtilities.ID_TextureWidth, material.GetFloat(ShaderUtilities.ID_TextureWidth));
				material2.SetFloat(ShaderUtilities.ID_TextureHeight, material.GetFloat(ShaderUtilities.ID_TextureHeight));
				material2.SetFloat(ShaderUtilities.ID_StencilID, material.GetFloat(ShaderUtilities.ID_StencilID));
				material2.SetFloat(ShaderUtilities.ID_StencilComp, material.GetFloat(ShaderUtilities.ID_StencilComp));
				material.CopyPropertiesFromMaterial(material2);
				material.shaderKeywords = new string[0];
			}
			else
			{
				material.CopyPropertiesFromMaterial(material2);
			}
			UnityEngine.Object.DestroyImmediate(material2);
			TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, material);
		}

		[MenuItem("CONTEXT/Material/Copy Atlas", false, 2000)]
		private static void CopyAtlas(MenuCommand command)
		{
			Material source = command.context as Material;
			TMP_ContextMenus.m_copiedAtlasProperties = new Material(source);
			TMP_ContextMenus.m_copiedAtlasProperties.hideFlags = HideFlags.DontSave;
		}

		[MenuItem("CONTEXT/Material/Paste Atlas", false, 2001)]
		private static void PasteAtlas(MenuCommand command)
		{
			Material material = command.context as Material;
			if (TMP_ContextMenus.m_copiedAtlasProperties != null)
			{
				Undo.RecordObject(material, "Paste Texture");
				ShaderUtilities.GetShaderPropertyIDs();
				material.SetTexture(ShaderUtilities.ID_MainTex, TMP_ContextMenus.m_copiedAtlasProperties.GetTexture(ShaderUtilities.ID_MainTex));
				material.SetFloat(ShaderUtilities.ID_GradientScale, TMP_ContextMenus.m_copiedAtlasProperties.GetFloat(ShaderUtilities.ID_GradientScale));
				material.SetFloat(ShaderUtilities.ID_TextureWidth, TMP_ContextMenus.m_copiedAtlasProperties.GetFloat(ShaderUtilities.ID_TextureWidth));
				material.SetFloat(ShaderUtilities.ID_TextureHeight, TMP_ContextMenus.m_copiedAtlasProperties.GetFloat(ShaderUtilities.ID_TextureHeight));
			}
			else if (TMP_ContextMenus.m_copiedTexture != null)
			{
				Undo.RecordObject(material, "Paste Texture");
				material.SetTexture(ShaderUtilities.ID_MainTex, TMP_ContextMenus.m_copiedTexture);
			}
		}

		[MenuItem("CONTEXT/TMP_FontAsset/Extract Atlas", false, 2000)]
		private static void ExtractAtlas(MenuCommand command)
		{
			TMP_FontAsset tmp_FontAsset = command.context as TMP_FontAsset;
			string assetPath = AssetDatabase.GetAssetPath(tmp_FontAsset);
			string text = Path.GetDirectoryName(assetPath) + "/" + Path.GetFileNameWithoutExtension(assetPath) + " Atlas.png";
			SerializedObject serializedObject = new SerializedObject(tmp_FontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
			serializedObject.FindProperty("m_IsReadable").boolValue = true;
			serializedObject.ApplyModifiedProperties();
			Texture2D texture2D = UnityEngine.Object.Instantiate<Texture>(tmp_FontAsset.material.GetTexture(ShaderUtilities.ID_MainTex)) as Texture2D;
			serializedObject.FindProperty("m_IsReadable").boolValue = false;
			serializedObject.ApplyModifiedProperties();
			Debug.Log(text);
			byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
			File.WriteAllBytes(text, bytes);
			AssetDatabase.Refresh();
			UnityEngine.Object.DestroyImmediate(texture2D);
		}

		private static Texture m_copiedTexture;

		private static Material m_copiedProperties;

		private static Material m_copiedAtlasProperties;
	}
}
