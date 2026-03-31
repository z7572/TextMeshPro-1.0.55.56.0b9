using System;
using UnityEditor;
using UnityEngine;

namespace TMPro.EditorUtilities
{
	public static class TMP_UIStyleManager
	{
		public static void GetUIStyles()
		{
			if (TMP_UIStyleManager.TMP_GUISkin != null)
			{
				return;
			}
			string packageRelativePath = TMP_EditorUtility.packageRelativePath;
			if (EditorGUIUtility.isProSkin)
			{
				TMP_UIStyleManager.TMP_GUISkin = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/TMPro_DarkSkin.guiskin", typeof(GUISkin)) as GUISkin);
				TMP_UIStyleManager.alignLeft = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignLeft.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignCenter = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignCenter.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignRight = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignRight.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignJustified = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignJustified.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignFlush = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignFlush.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignGeoCenter = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignCenterGeo.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignTop = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignTop.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignMiddle = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignMiddle.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignBottom = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignBottom.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignBaseline = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignBaseLine.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignMidline = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignMidLine.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignCapline = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignCapLine.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.progressTexture = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/Progress Bar.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.selectionBox = (EditorGUIUtility.Load("IN thumbnailshadow On@2x") as Texture2D);
			}
			else
			{
				TMP_UIStyleManager.TMP_GUISkin = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/TMPro_LightSkin.guiskin", typeof(GUISkin)) as GUISkin);
				TMP_UIStyleManager.alignLeft = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignLeft_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignCenter = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignCenter_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignRight = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignRight_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignJustified = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignJustified_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignFlush = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignFlush_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignGeoCenter = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignCenterGeo_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignTop = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignTop_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignMiddle = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignMiddle_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignBottom = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignBottom_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignBaseline = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignBaseLine_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignMidline = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignMidLine_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.alignCapline = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/btn_AlignCapLine_Light.psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.progressTexture = (AssetDatabase.LoadAssetAtPath(packageRelativePath + "/Editor Resources/Textures/Progress Bar (Light).psd", typeof(Texture2D)) as Texture2D);
				TMP_UIStyleManager.selectionBox = (EditorGUIUtility.Load("IN thumbnailshadow On@2x") as Texture2D);
			}
			if (TMP_UIStyleManager.TMP_GUISkin != null)
			{
				TMP_UIStyleManager.Label = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Label");
				TMP_UIStyleManager.Section_Label = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Section Label");
				TMP_UIStyleManager.Group_Label = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Group Label");
				TMP_UIStyleManager.Group_Label_Left = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Group Label - Left Half");
				TMP_UIStyleManager.TextAreaBoxEditor = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Text Area Box (Editor)");
				TMP_UIStyleManager.TextAreaBoxWindow = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Text Area Box (Window)");
				TMP_UIStyleManager.TextureAreaBox = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Texture Area Box");
				TMP_UIStyleManager.SquareAreaBox85G = TMP_UIStyleManager.TMP_GUISkin.FindStyle("Square Area Box (85 Grey)");
				TMP_UIStyleManager.alignContent_A = new GUIContent[]
				{
					new GUIContent(TMP_UIStyleManager.alignLeft, "Left"),
					new GUIContent(TMP_UIStyleManager.alignCenter, "Center"),
					new GUIContent(TMP_UIStyleManager.alignRight, "Right"),
					new GUIContent(TMP_UIStyleManager.alignJustified, "Justified"),
					new GUIContent(TMP_UIStyleManager.alignFlush, "Flush"),
					new GUIContent(TMP_UIStyleManager.alignGeoCenter, "Geometry Center")
				};
				TMP_UIStyleManager.alignContent_B = new GUIContent[]
				{
					new GUIContent(TMP_UIStyleManager.alignTop, "Top"),
					new GUIContent(TMP_UIStyleManager.alignMiddle, "Middle"),
					new GUIContent(TMP_UIStyleManager.alignBottom, "Bottom"),
					new GUIContent(TMP_UIStyleManager.alignBaseline, "Baseline"),
					new GUIContent(TMP_UIStyleManager.alignMidline, "Midline"),
					new GUIContent(TMP_UIStyleManager.alignCapline, "Capline")
				};
			}
		}

		public static GUISkin TMP_GUISkin;

		public static GUIStyle Label;

		public static GUIStyle Group_Label;

		public static GUIStyle Group_Label_Left;

		public static GUIStyle TextAreaBoxEditor;

		public static GUIStyle TextAreaBoxWindow;

		public static GUIStyle TextureAreaBox;

		public static GUIStyle Section_Label;

		public static GUIStyle SquareAreaBox85G;

		public static Texture2D alignLeft;

		public static Texture2D alignCenter;

		public static Texture2D alignRight;

		public static Texture2D alignJustified;

		public static Texture2D alignFlush;

		public static Texture2D alignGeoCenter;

		public static Texture2D alignTop;

		public static Texture2D alignMiddle;

		public static Texture2D alignBottom;

		public static Texture2D alignBaseline;

		public static Texture2D alignMidline;

		public static Texture2D alignCapline;

		public static Texture2D progressTexture;

		public static Texture2D selectionBox;

		public static GUIContent[] alignContent_A;

		public static GUIContent[] alignContent_B;
	}
}
