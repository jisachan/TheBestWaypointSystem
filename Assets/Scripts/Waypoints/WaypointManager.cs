using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaypointManager : MonoBehaviour
{
	[HideInInspector]
	public List<Waypoint> waypoints = new List<Waypoint>();

	[HideInInspector, Tooltip("NOTE: For AI movement behavior.\nCheck to use a circular waypoint system.\nUncheck to use a linear waypoint system.\n(Circular connects the last and first waypoints together)")]
	public bool circularWaypointSystem;

	[HideInInspector, Tooltip("Check to enable adding new waypoints in between existing ones.\nUncheck to add new waypoints after the last one or before the first one.")]
	public bool addWaypointBetweenPoints;

	[HideInInspector, Tooltip("Check to add new waypoints after the currently last waypoint.\nUncheck to add waypoints before the currently first waypoint.\nNote: Adding waypoint to the start is default.")]
	public bool addWaypointAtEnd;

	private void OnDrawGizmos()
	{
#if UNITY_EDITOR

		for (int i = 0; i < waypoints.Count; i++)
		{
			Handles.color = waypoints[i].color;
			Handles.FreeMoveHandle(waypoints[i].position, Quaternion.identity, 0.3f, Vector3.one, Handles.SphereHandleCap);
			Handles.color = Color.white;

			int nextIndex = i + 1;
			nextIndex %= waypoints.Count;
			if (nextIndex == 0)
			{
				Handles.color = Color.cyan;
			}
			else
			{
				Handles.color = Color.white;
			}
			if (circularWaypointSystem)
			{
				//show line between last and first waypoint
				Handles.DrawAAPolyLine(waypoints[i].position, waypoints[nextIndex].position);
			}
			else
			{
				//don't show line between last and first waypoint
				if (i != waypoints.Count - 1)
				{
					Handles.DrawAAPolyLine(waypoints[i].position, waypoints[nextIndex].position);
				}
			}
		}
#endif
	}
}
