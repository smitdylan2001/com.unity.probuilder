﻿using UnityEngine;
using System.Linq;
using ProBuilder.Core;
using UnityEditor.ProBuilder;

namespace ProBuilder.Actions
{
	class SelectFaceRing : MenuAction
	{
		public override ToolbarGroup group { get { return ToolbarGroup.Selection; } }
		public override Texture2D icon { get { return IconUtility.GetIcon("Toolbar/Selection_Ring_Face", IconSkin.Pro); } }
		public override TooltipContent tooltip { get { return m_Tooltip; } }
		public override int toolbarPriority { get { return 2; } }
		public override bool hasFileMenuEntry { get { return false; } }

		private static readonly TooltipContent m_Tooltip = new TooltipContent
		(
			"Select Face Ring",
			"Selects a ring of connected faces.\n\n<b>Shortcut</b>: Control + Double Click on Face."
		);

		public override bool IsEnabled()
		{
			return 	ProBuilderEditor.instance != null &&
					ProBuilderEditor.instance.editLevel == EditLevel.Geometry &&
					ProBuilderEditor.instance.selectionMode == SelectMode.Face &&
			       	selection != null &&
			       	selection.Length > 0 &&
			       	selection.Sum(x => x.SelectedFaceCount) > 0;
		}

		public override bool IsHidden()
		{
			return 	ProBuilderEditor.instance == null ||
					ProBuilderEditor.instance.editLevel != EditLevel.Geometry ||
					ProBuilderEditor.instance.selectionMode != SelectMode.Face;
		}

		public override pb_ActionResult DoAction()
		{
			return MenuCommands.MenuRingFaces(selection);
		}
	}
}
