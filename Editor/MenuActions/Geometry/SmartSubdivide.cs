using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;
using System.Linq;
using ProBuilder.Core;
using UnityEditor.ProBuilder;

namespace ProBuilder.Actions
{
	class SmartSubdivide : MenuAction
	{
		public override ToolbarGroup group { get { return ToolbarGroup.Geometry; } }
		public override Texture2D icon { get { return null; } }
		public override TooltipContent tooltip { get { return _tooltip; } }
		public override bool isProOnly { get { return true; } }

		static readonly TooltipContent _tooltip = new TooltipContent
		(
			"Smart Subdivide",
			"",
			CMD_ALT, 'S'
		);

		public override bool IsEnabled()
		{
			return 	ProBuilderEditor.instance != null &&
					ProBuilderEditor.instance.editLevel == EditLevel.Geometry &&
					ProBuilderEditor.instance.selectionMode != SelectMode.Vertex &&
					selection != null &&
					selection.Length > 0 &&
					selection.Any(x => x.SelectedEdgeCount > 0);
		}

		public override bool IsHidden()
		{
			return true;
		}

		public override pb_ActionResult DoAction()
		{
			switch(ProBuilderEditor.instance.selectionMode)
			{
				case SelectMode.Edge:
					return MenuCommands.MenuSubdivideEdge(selection);

				default:
					return MenuCommands.MenuSubdivideFace(selection);
			}
		}
	}
}
