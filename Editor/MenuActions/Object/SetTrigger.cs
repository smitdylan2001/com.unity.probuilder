using System;
using ProBuilder.Core;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;

namespace ProBuilder.Actions
{
	class SetTrigger : MenuAction
	{
		public override ToolbarGroup group { get { return ToolbarGroup.Entity; } }
		public override Texture2D icon { get { return null; } }
		public override TooltipContent tooltip { get { return _tooltip; } }
		public override bool isProOnly { get { return true; } }

		static readonly TooltipContent _tooltip = new TooltipContent
		(
			"Set Trigger",
			"Apply the Trigger material and adds a collider marked as a trigger. The MeshRenderer will be automatically turned off on entering play mode."
		);

		public override bool IsEnabled()
		{
			return ProBuilderEditor.instance != null && selection != null && selection.Length > 0;
		}

		public override pb_ActionResult DoAction()
		{
			foreach (pb_Object pb in MeshSelection.All())
			{
				var existing = pb.GetComponents<pb_EntityBehaviour>();

				// For now just nuke any existing entity types (since there are only two). In the future we should be
				// smarter about conflicting entity types.
				for (int i = 0, c = existing.Length; i < c; i++)
					Undo.DestroyObjectImmediate(existing[i]);

				var entity = pb.GetComponent<pb_Entity>();

				if (entity != null)
					Undo.DestroyObjectImmediate(entity);

				if (!pb.GetComponent<Collider>())
					Undo.AddComponent<MeshCollider>(pb.gameObject);

				if (!pb.GetComponent<Renderer>())
					Undo.AddComponent<MeshRenderer>(pb.gameObject);

				pb_Undo.RegisterCompleteObjectUndo(pb, "Set Trigger");

				Undo.AddComponent<pb_TriggerBehaviour>(pb.gameObject).Initialize();
			}

			int selectionCount = MeshSelection.All().Length;

			if(selectionCount < 1)
				return new pb_ActionResult(Status.NoChange, "Set Trigger\nNo objects selected");

			return new pb_ActionResult(Status.Success, "Set Trigger\nSet " + selectionCount + " Objects");
		}
	}
}
