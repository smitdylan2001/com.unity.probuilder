using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Parabox.STL;
using ProBuilder.Core;
using UnityEditor.ProBuilder;
using EditorUtility = UnityEditor.EditorUtility;
using FileUtil = UnityEditor.ProBuilder.FileUtil;

namespace ProBuilder.Actions
{
	class ExportPly : MenuAction
	{
		public override ToolbarGroup group { get { return ToolbarGroup.Export; } }
		public override Texture2D icon { get { return null; } }
		public override TooltipContent tooltip { get { return _tooltip; } }
		public override bool isProOnly { get { return false; } }

		static readonly TooltipContent _tooltip = new TooltipContent
		(
			"Export Ply",
			"Export a Stanford PLY file."
		);

		public override bool IsHidden() { return true; }

		public override bool IsEnabled()
		{
			return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
		}

		public override pb_ActionResult DoAction()
		{
			string res = ExportWithFileDialog(MeshSelection.Top());

			if( string.IsNullOrEmpty(res) )
				return new pb_ActionResult(Status.Canceled, "User Canceled");
			else
				return new pb_ActionResult(Status.Success, "Export PLY");
		}

		/**
		 *	Prompt user for a save file location and export meshes as Obj.
		 */
		public static string ExportWithFileDialog(IEnumerable<pb_Object> meshes, bool asGroup = true, PlyOptions options = null)
		{
			if(meshes == null || meshes.Count() < 1)
				return null;

			string path = null, res = null;

			if(asGroup || meshes.Count() < 2)
			{
				pb_Object first = meshes.FirstOrDefault();
				string name = first != null ? first.name : "ProBuilderModel";
				path = EditorUtility.SaveFilePanel("Export as PLY", "Assets", name, "ply");

				if(string.IsNullOrEmpty(path))
					return null;

				res = DoExport(path, meshes, options);
			}
			else
			{
				path = EditorUtility.SaveFolderPanel("Export to PLY", "Assets", "");

				if(string.IsNullOrEmpty(path) || !Directory.Exists(path))
					return null;

				foreach(pb_Object model in meshes)
					res = DoExport(string.Format("{0}/{1}.ply", path, model.name), new List<pb_Object>() { model }, options);
			}

			return res;
		}

		private static string DoExport(string path, IEnumerable<pb_Object> models, PlyOptions options)
		{
			string name = Path.GetFileNameWithoutExtension(path);
			string directory = Path.GetDirectoryName(path);

			string ply;

			if( PlyExporter.Export(models, out ply, options) )
			{
				try
				{
					FileUtil.WriteAllText(string.Format("{0}/{1}.ply", directory, name), ply);
				}
				catch(System.Exception e)
				{
					Debug.LogWarning(string.Format("Failed writing PLY to path: {0}\n{1}", string.Format("{0}/{1}.ply", path, name), e.ToString()));
					return null;
				}
			}
			else
			{
				Debug.LogWarning("No meshes selected.");
				return null;
			}

			return path;
		}
	}
}
