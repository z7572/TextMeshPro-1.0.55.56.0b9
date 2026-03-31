using System;
using System.Reflection;

namespace TMPro
{
	public static class SortingLayerHelper
	{
		public static string[] sortingLayerNames
		{
			get
			{
				if (SortingLayerHelper._sortingLayerNamesProperty == null)
				{
					return null;
				}
				return SortingLayerHelper._sortingLayerNamesProperty.GetValue(null, null) as string[];
			}
		}

		public static string GetSortingLayerNameFromID(int id)
		{
			string[] sortingLayerNames = SortingLayerHelper.sortingLayerNames;
			if (sortingLayerNames == null)
			{
				return null;
			}
			for (int i = 0; i < sortingLayerNames.Length; i++)
			{
				if (SortingLayerHelper.GetSortingLayerIDForIndex(i) == id)
				{
					return sortingLayerNames[i];
				}
			}
			return null;
		}

		public static int GetSortingLayerIDForName(string name)
		{
			string[] sortingLayerNames = SortingLayerHelper.sortingLayerNames;
			if (sortingLayerNames == null)
			{
				return 0;
			}
			return SortingLayerHelper.GetSortingLayerIDForIndex(Array.IndexOf<string>(sortingLayerNames, name));
		}

		public static int GetSortingLayerIDForIndex(int index)
		{
			if (SortingLayerHelper._getSortingLayerUserIdMethod == null)
			{
				return 0;
			}
			return (int)SortingLayerHelper._getSortingLayerUserIdMethod.Invoke(null, new object[]
			{
				index
			});
		}

		private static Type _utilityType = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");

		private static PropertyInfo _sortingLayerNamesProperty = SortingLayerHelper._utilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

		private static MethodInfo _getSortingLayerUserIdMethod = SortingLayerHelper._utilityType.GetMethod("GetSortingLayerUniqueID", BindingFlags.Static | BindingFlags.NonPublic);
	}
}
