using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	/// <summary>
	/// General functions for modifying pb_Object.
	/// </summary>
	public static class ObjectUtility
	{
		/// <summary>
		/// Get a copy of a mesh positions array transformed into world coordinates.
		/// </summary>
		/// <param name="mesh"></param>
		/// <returns>A Vector3[] containing all vertex points in world space.</returns>
		public static Vector3[] VerticesInWorldSpace(this ProBuilderMesh mesh)
		{
			int len = mesh.vertexCount;
			Vector3[] worldPoints = new Vector3[len];
			Vector3[] localPoints = mesh.positions;

			for(int i = 0; i < len; i++)
				worldPoints[i] = mesh.transform.TransformPoint(localPoints[i]);

			return worldPoints;
		}

		/// <summary>
		/// Returns requested vertices in world space coordinates.
		/// </summary>
		/// <param name="pb"></param>
		/// <param name="indices"></param>
		/// <returns></returns>
		public static Vector3[] VerticesInWorldSpace(this ProBuilderMesh pb, int[] indices)
		{
			Vector3[] worldPoints = pb.positions.ValuesWithIndices(indices);

			for(int i = 0; i < worldPoints.Length; i++)
				worldPoints[i] = pb.transform.TransformPoint(worldPoints[i]);

			return worldPoints;
		}

		/// <summary>
		/// Translate a set of vertices with a world space offset.
		/// </summary>
		/// <param name="pb"></param>
		/// <param name="selectedTriangles">A distinct set of indices to apply an offset to.</param>
		/// <param name="offset">The offset to apply in world coordinates.</param>
		public static void TranslateVerticesInWorldSpace(this ProBuilderMesh pb, int[] selectedTriangles, Vector3 offset)
		{
			pb.TranslateVerticesInWorldSpace(selectedTriangles, offset, 0f, false, null);
		}

		/// <summary>
		/// Translate a set of vertices with a world space offset.
		/// </summary>
		/// <param name="pb"></param>
		/// <param name="selectedTriangles">A distinct list of vertex indices.</param>
		/// <param name="offset">The direction and magnitude to translate selectedTriangles, in world space.</param>
		/// <param name="snapValue">If > 0 snap each vertex to the nearest on-grid point in world space.</param>
		/// <param name="snapAxisOnly">If true vertices will only be snapped along the active axis.</param>
		/// <param name="lookup">A shared index lookup table.  Can pass NULL to have this automatically calculated.</param>
		public static void TranslateVerticesInWorldSpace(this ProBuilderMesh pb, int[] selectedTriangles, Vector3 offset, float snapValue, bool snapAxisOnly, Dictionary<int, int> lookup)
		{
			int i = 0;
			int[] indices = lookup != null ? pb.sharedIndices.AllIndicesWithValues(lookup, selectedTriangles).ToArray() : pb.sharedIndices.AllIndicesWithValues(selectedTriangles).ToArray();

			Matrix4x4 w2l = pb.transform.worldToLocalMatrix;

			Vector3 localOffset = w2l * offset;

			Vector3[] verts = pb.positions;

			// Snaps to world grid
			if(Mathf.Abs(snapValue) > Mathf.Epsilon)
			{
				Matrix4x4 l2w = pb.transform.localToWorldMatrix;
				Vector3 v = Vector3.zero;
				Vector3 mask = snapAxisOnly ? offset.ToMask(ProBuilderMath.handleEpsilon) : Vector3.one;

				for(i = 0; i < indices.Length; i++)
				{
					v = l2w.MultiplyPoint3x4(verts[indices[i]] + localOffset);
					verts[indices[i]] = w2l.MultiplyPoint3x4( Snapping.SnapValue(v, snapValue * mask) );
				}
			}
			else
			{
				for(i = 0; i < indices.Length; i++)
					verts[indices[i]] += localOffset;
			}

			// don't bother calling a full ToMesh() here because we know for certain that the _vertices and msh.vertices arrays are equal in length
			pb.SetVertices(verts);
			pb.mesh.vertices = verts;
		}

		/// <summary>
		/// Translate a set of vertices with an offset provided in local coordinates.
		/// </summary>
		/// <param name="pb"></param>
		/// <param name="selectedTriangles"></param>
		/// <param name="offset"></param>
		public static void TranslateVertices(this ProBuilderMesh pb, int[] selectedTriangles, Vector3 offset)
		{
			int i = 0;
			int[] indices = pb.sharedIndices.AllIndicesWithValues(selectedTriangles).ToArray();

			Vector3[] verts = pb.positions;
			for(i = 0; i < indices.Length; i++)
				verts[indices[i]] += offset;

			// don't bother calling a full ToMesh() here because we know for certain that the _vertices and msh.vertices arrays are equal in length
			pb.SetVertices(verts);
			pb.mesh.vertices = verts;
		}

		/// <summary>
		/// Given a shared vertex index (index of the triangle in the sharedIndices array), move all vertices to new position.
		/// Position is in model space coordinates.
		/// </summary>
		/// <remarks>Use pb.sharedIndices.IndexOf(triangle) to get sharedIndex.</remarks>
		/// <param name="pb"></param>
		/// <param name="sharedIndex"></param>
		/// <param name="position"></param>
		public static void SetSharedVertexPosition(this ProBuilderMesh pb, int sharedIndex, Vector3 position)
		{
			Vector3[] v = pb.positions;
			int[] array = pb.sharedIndices[sharedIndex].array;

			for(int i = 0; i < array.Length; i++)
				v[array[i]] = position;

			pb.SetVertices(v);
			pb.mesh.vertices = v;
		}

		/// <summary>
		/// Given a shared vertex index (index of the triangle in the sharedIndices array), move all vertices to new position.
		/// Vertex values are in model space coordinates.
		/// </summary>
		/// <remarks>Use pb.sharedIndices.IndexOf(triangle) to get sharedIndex.</remarks>
		/// <param name="pb"></param>
		/// <param name="sharedIndex"></param>
		/// <param name="vertex"></param>
		public static void SetSharedVertexValues(this ProBuilderMesh pb, int sharedIndex, Vertex vertex)
		{
			Vertex[] vertices = Vertex.GetVertices(pb);

			int[] array = pb.sharedIndices[sharedIndex].array;

			for(int i = 0; i < array.Length; i++)
				vertices[array[i]] = vertex;

			pb.SetVertices(vertices);
		}

		/// <summary>
		/// Find the face which contains a set of triangle indices.
		/// tri must contain exactly 3 values.
		/// </summary>
		/// <param name="pb"></param>
		/// <param name="tri">int[] composed of three indices.</param>
		/// <param name="face"></param>
		/// <returns>True if a matching face was found, false if not.</returns>
		public static bool FaceWithTriangle(this ProBuilderMesh pb, int[] tri, out Face face)
		{
			for(int i = 0; i < pb.faces.Length; i++)
			{
				if(	pb.faces[i].Contains(tri) )
				{
					face = pb.faces[i];
					return true;
				}
			}

			face = null;
			return false;
		}

		/// <summary>
		/// Returns the index of the Face which contains the passed triangle.
		/// </summary>
		/// <param name="pb"></param>
		/// <param name="tri">int[] composed of three indices.</param>
		/// <param name="face"></param>
		/// <returns></returns>
		public static bool FaceWithTriangle(this ProBuilderMesh pb, int[] tri, out int face)
		{
			for(int i = 0; i < pb.faces.Length; i++)
			{
				if(	pb.faces[i].Contains(tri) )
				{
					face = i;
					return true;
				}
			}

			face = -1;
			return false;
		}
	}
}
