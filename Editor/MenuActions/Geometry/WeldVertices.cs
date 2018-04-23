using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;
using System.Linq;
using ProBuilder.Core;
using UnityEditor.ProBuilder;
using EditorGUILayout = UnityEditor.EditorGUILayout;
using EditorStyles = UnityEditor.EditorStyles;

namespace ProBuilder.Actions
{
	class WeldVertices : MenuAction
	{
		public override ToolbarGroup group { get { return ToolbarGroup.Geometry; } }
		public override Texture2D icon { get { return IconUtility.GetIcon("Toolbar/Vert_Weld", IconSkin.Pro); } }
		public override TooltipContent tooltip { get { return _tooltip; } }
		public override bool isProOnly { get { return true; } }

		static readonly TooltipContent _tooltip = new TooltipContent
		(
			"Weld Vertices",
			@"Searches the current selection for vertices that are within the specified distance of on another and merges them into a single vertex.",
			CMD_ALT, 'V'
		);

		public override bool IsEnabled()
		{
			return 	ProBuilderEditor.instance != null &&
					ProBuilderEditor.instance.editLevel == EditLevel.Geometry &&
					ProBuilderEditor.instance.selectionMode == SelectMode.Vertex &&
					selection != null &&
					selection.Length > 0 &&
					selection.Any(x => x.SelectedTriangleCount > 1);
		}

		public override bool IsHidden()
		{
			return 	ProBuilderEditor.instance == null ||
					ProBuilderEditor.instance.editLevel != EditLevel.Geometry ||
					ProBuilderEditor.instance.selectionMode != SelectMode.Vertex;

		}

		public override MenuActionState AltState()
		{
			return MenuActionState.VisibleAndEnabled;
		}

		static readonly GUIContent gc_weldDistance = new GUIContent("Weld Distance", "The maximum distance between two vertices in order to be welded together.");
		const float MIN_WELD_DISTANCE = .00001f;

		public override void OnSettingsGUI()
		{
			GUILayout.Label("Weld Settings", EditorStyles.boldLabel);

			EditorGUI.BeginChangeCheck();

			float weldDistance = PreferencesInternal.GetFloat(pb_Constant.pbWeldDistance);

			if(weldDistance <= MIN_WELD_DISTANCE)
				weldDistance = MIN_WELD_DISTANCE;

			weldDistance = EditorGUILayout.FloatField(gc_weldDistance, weldDistance);

			if( EditorGUI.EndChangeCheck() )
			{
				if(weldDistance < MIN_WELD_DISTANCE)
					weldDistance = MIN_WELD_DISTANCE;
				PreferencesInternal.SetFloat(pb_Constant.pbWeldDistance, weldDistance);
			}

			GUILayout.FlexibleSpace();

			if(GUILayout.Button("Weld Vertices"))
				DoAction();
		}

		public override pb_ActionResult DoAction()
		{
			return MenuCommands.MenuWeldVertices(selection);
		}
	}
}
