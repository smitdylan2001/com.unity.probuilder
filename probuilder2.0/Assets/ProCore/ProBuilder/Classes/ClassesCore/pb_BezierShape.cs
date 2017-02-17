﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProBuilder2.Common
{
	public class pb_BezierShape : MonoBehaviour
	{
		public List<pb_BezierPoint> m_Points = new List<pb_BezierPoint>();
		public bool m_CloseLoop = false;
		public float m_Radius = .5f;
		public int m_Rows = 24;
		public int m_Columns = 32;

		private pb_Object m_Mesh;

		public pb_Object mesh
		{
			get
			{
				if(m_Mesh == null)
					m_Mesh = GetComponent<pb_Object>();

				return m_Mesh;
			}

			set
			{
				m_Mesh = value;
			}
		}

		/**
		 *	Initialize the points list with a default set.
		 */
		public void Init()
		{
			Vector3 tan = new Vector3(0f, 0f, 2f);
			Vector3 p1 = new Vector3(3f, 0f, 0f);
			m_Points.Add(new pb_BezierPoint(Vector3.zero, -tan, tan));
			m_Points.Add(new pb_BezierPoint(p1, p1 + tan, p1 + -tan));
		}

		/**
		 *	Rebuild the pb_Object with the extruded spline.
		 */
		public void Refresh()
		{
			pb_Object m = mesh;
			pb_Spline.Extrude(m_Points, m_Radius, m_Columns, m_Rows, m_CloseLoop, ref m);
		}
	}
}