using System;
using UnityEditor;

namespace TMPro.EditorUtilities
{
	public class TMP_BitmapShaderGUI : TMP_BaseShaderGUI
	{
		protected override void DoGUI()
		{
			if (base.DoPanelHeader(TMP_BitmapShaderGUI.facePanel))
			{
				this.DoFacePanel();
			}
			if (base.DoPanelHeader(TMP_BitmapShaderGUI.debugPanel))
			{
				this.DoDebugPanel();
			}
		}

		private void DoFacePanel()
		{
			EditorGUI.indentLevel++;
			if (this.material.HasProperty(ShaderUtilities.ID_FaceTex))
			{
				base.DoColor("_FaceColor", "Color");
				base.DoTexture2D("_FaceTex", "Texture", true, null);
			}
			else
			{
				base.DoColor("_Color", "Color");
				base.DoSlider("_DiffusePower", "Diffuse Power");
			}
			EditorGUI.indentLevel--;
		}

		private void DoDebugPanel()
		{
			EditorGUI.indentLevel++;
			base.DoTexture2D("_MainTex", "Font Atlas", false, null);
			if (this.material.HasProperty(ShaderUtilities.ID_VertexOffsetX))
			{
				base.DoEmptyLine();
				base.DoFloat("_VertexOffsetX", "Offset X");
				base.DoFloat("_VertexOffsetY", "Offset Y");
			}
			if (this.material.HasProperty(ShaderUtilities.ID_MaskSoftnessX))
			{
				base.DoEmptyLine();
				base.DoFloat("_MaskSoftnessX", "Softness X");
				base.DoFloat("_MaskSoftnessY", "Softness Y");
				base.DoVector("_ClipRect", "Clip Rect", TMP_BaseShaderGUI.lbrtVectorLabels);
			}
			if (this.material.HasProperty(ShaderUtilities.ID_StencilID))
			{
				base.DoEmptyLine();
				base.DoFloat("_Stencil", "Stencil ID");
				base.DoFloat("_StencilComp", "Stencil Comp");
			}
			EditorGUI.indentLevel--;
		}

		private static TMP_BaseShaderGUI.MaterialPanel facePanel = new TMP_BaseShaderGUI.MaterialPanel("Face", true);

		private static TMP_BaseShaderGUI.MaterialPanel debugPanel = new TMP_BaseShaderGUI.MaterialPanel("Debug", false);
	}
}
