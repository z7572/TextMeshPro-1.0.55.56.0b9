using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public static class TMP_SpriteAssetMenu
	{
		[MenuItem("CONTEXT/TMP_SpriteAsset/Add Default Material", false, 2000)]
		private static void CopyTexture(MenuCommand command)
		{
			TMP_SpriteAsset tmp_SpriteAsset = (TMP_SpriteAsset)command.context;
			if (tmp_SpriteAsset != null && tmp_SpriteAsset.material == null)
			{
				TMP_SpriteAssetMenu.AddDefaultMaterial(tmp_SpriteAsset);
			}
		}

		[MenuItem("Assets/Create/TextMeshPro/Sprite Asset", false, 100)]
		public static void CreateTextMeshProObjectPerform()
		{
			UnityEngine.Object activeObject = Selection.activeObject;
			if (activeObject == null || activeObject.GetType() != typeof(Texture2D))
			{
				Debug.LogWarning("A texture which contains sprites must first be selected in order to create a TextMesh Pro Sprite Asset.");
				return;
			}
			Texture2D texture2D = activeObject as Texture2D;
			string assetPath = AssetDatabase.GetAssetPath(texture2D);
			string fileName = Path.GetFileName(assetPath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
			string str = assetPath.Replace(fileName, string.Empty);
			TMP_SpriteAsset tmp_SpriteAsset = AssetDatabase.LoadAssetAtPath(str + fileNameWithoutExtension + ".asset", typeof(TMP_SpriteAsset)) as TMP_SpriteAsset;
			bool flag = tmp_SpriteAsset == null;
			if (flag)
			{
				tmp_SpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
				AssetDatabase.CreateAsset(tmp_SpriteAsset, str + fileNameWithoutExtension + ".asset");
				tmp_SpriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(tmp_SpriteAsset.name);
				tmp_SpriteAsset.spriteSheet = texture2D;
				tmp_SpriteAsset.spriteInfoList = TMP_SpriteAssetMenu.GetSpriteInfo(texture2D);
				TMP_SpriteAssetMenu.AddDefaultMaterial(tmp_SpriteAsset);
			}
			else
			{
				tmp_SpriteAsset.spriteInfoList = TMP_SpriteAssetMenu.UpdateSpriteInfo(tmp_SpriteAsset);
				if (tmp_SpriteAsset.material == null)
				{
					TMP_SpriteAssetMenu.AddDefaultMaterial(tmp_SpriteAsset);
				}
			}
			EditorUtility.SetDirty(tmp_SpriteAsset);
			AssetDatabase.SaveAssets();
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tmp_SpriteAsset));
		}

		private static List<TMP_Sprite> GetSpriteInfo(Texture source)
		{
			string assetPath = AssetDatabase.GetAssetPath(source);
			Sprite[] array = (from x in AssetDatabase.LoadAllAssetsAtPath(assetPath)
			select x as Sprite into x
			where x != null
			orderby x.rect.y descending, x.rect.x
			select x).ToArray<Sprite>();
			List<TMP_Sprite> list = new List<TMP_Sprite>();
			for (int i = 0; i < array.Length; i++)
			{
				TMP_Sprite tmp_Sprite = new TMP_Sprite();
				Sprite sprite = array[i];
				tmp_Sprite.id = i;
				tmp_Sprite.name = sprite.name;
				tmp_Sprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(tmp_Sprite.name);
				Rect rect = sprite.rect;
				tmp_Sprite.x = rect.x;
				tmp_Sprite.y = rect.y;
				tmp_Sprite.width = rect.width;
				tmp_Sprite.height = rect.height;
				Vector2 vector = new Vector2(0f - sprite.bounds.min.x / (sprite.bounds.extents.x * 2f), 0f - sprite.bounds.min.y / (sprite.bounds.extents.y * 2f));
				tmp_Sprite.pivot = new Vector2(0f - vector.x * rect.width, rect.height - vector.y * rect.height);
				tmp_Sprite.sprite = sprite;
				tmp_Sprite.xAdvance = rect.width;
				tmp_Sprite.scale = 1f;
				tmp_Sprite.xOffset = tmp_Sprite.pivot.x;
				tmp_Sprite.yOffset = tmp_Sprite.pivot.y;
				list.Add(tmp_Sprite);
			}
			return list;
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

		private static List<TMP_Sprite> UpdateSpriteInfo(TMP_SpriteAsset spriteAsset)
		{
			string assetPath = AssetDatabase.GetAssetPath(spriteAsset.spriteSheet);
			Sprite[] array = (from x in AssetDatabase.LoadAllAssetsAtPath(assetPath)
			select x as Sprite into x
			where x != null
			orderby x.rect.y descending, x.rect.x
			select x).ToArray<Sprite>();
			for (int i = 0; i < array.Length; i++)
			{
				Sprite sprite = array[i];
				int num = -1;
				if (spriteAsset.spriteInfoList.Count > i && spriteAsset.spriteInfoList[i].sprite != null)
				{
					num = spriteAsset.spriteInfoList.FindIndex((TMP_Sprite item) => item.sprite.GetInstanceID() == sprite.GetInstanceID());
				}
				TMP_Sprite tmp_Sprite = (num != -1) ? spriteAsset.spriteInfoList[num] : new TMP_Sprite();
				Rect rect = sprite.rect;
				tmp_Sprite.x = rect.x;
				tmp_Sprite.y = rect.y;
				tmp_Sprite.width = rect.width;
				tmp_Sprite.height = rect.height;
				Vector2 vector = new Vector2(0f - sprite.bounds.min.x / (sprite.bounds.extents.x * 2f), 0f - sprite.bounds.min.y / (sprite.bounds.extents.y * 2f));
				tmp_Sprite.pivot = new Vector2(0f - vector.x * rect.width, rect.height - vector.y * rect.height);
				if (num == -1)
				{
					int[] array2 = (from item in spriteAsset.spriteInfoList
					select item.id).ToArray<int>();
					int id = 0;
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[0] != 0)
						{
							break;
						}
						if (j > 0 && array2[j] - array2[j - 1] > 1)
						{
							id = array2[j - 1] + 1;
							break;
						}
						id = j + 1;
					}
					tmp_Sprite.sprite = sprite;
					tmp_Sprite.name = sprite.name;
					tmp_Sprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(tmp_Sprite.name);
					tmp_Sprite.id = id;
					tmp_Sprite.xAdvance = rect.width;
					tmp_Sprite.scale = 1f;
					tmp_Sprite.xOffset = tmp_Sprite.pivot.x;
					tmp_Sprite.yOffset = tmp_Sprite.pivot.y;
					spriteAsset.spriteInfoList.Add(tmp_Sprite);
					spriteAsset.spriteInfoList = (from s in spriteAsset.spriteInfoList
					orderby s.id
					select s).ToList<TMP_Sprite>();
				}
				else
				{
					spriteAsset.spriteInfoList[num] = tmp_Sprite;
				}
			}
			return spriteAsset.spriteInfoList;
		}
	}
}
