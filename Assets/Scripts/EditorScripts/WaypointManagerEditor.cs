using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : Editor
{
	WaypointManager waypointManager;

	SerializedObject waypoint;

	SerializedProperty property;

	SerializedProperty propertyPosition;

	SerializedProperty newProperty;

	SerializedProperty newPropertyPosition;
	
	SerializedProperty newPropertyColor;

	SerializedProperty propertyCircularSystem;

	SerializedProperty propertyAddWaypointBetween;

	SerializedProperty propertyAddWaypointAtEnd;

	ReorderableList waypointList;

	void OnEnable()
	{
		Debug.Log("Hold down 'alt' and click in the sceneview to add a new waypoint.");
		Debug.Log("Hold down 'ctrl' and right click on a waypoint in the scene view to select and mark it in both the sceneView and the editor.");
		Debug.Log("Click on a waypoint in the editor list to select and mark it in both the sceneView and the editor.");

		//target is the object you're inspecting in the inspector. 
		//when making a custom editor, target is always the object you made the editor for.
		//in this case, the waypointmanager.
		waypointManager = (WaypointManager)target;

		waypoint = serializedObject; //similar to target, but serialized (can edit/undo/redo)

		waypointList = new ReorderableList(waypoint, waypoint.FindProperty("waypoints"), true, true, true, true);

		waypointList.drawElementCallback = DrawListItems;
		waypointList.drawHeaderCallback = DrawHeader;
		waypointList.onSelectCallback = WhenSelectedInEditor;

		propertyCircularSystem = waypoint.FindProperty("circularWaypointSystem");
		propertyAddWaypointBetween = waypoint.FindProperty("addWaypointBetweenPoints");
		propertyAddWaypointAtEnd = waypoint.FindProperty("addWaypointAtEnd");

		SceneView.duringSceneGui += DuringSceneGUI;
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= DuringSceneGUI;
	}

	void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
	{
		SerializedProperty element = waypointList.serializedProperty.GetArrayElementAtIndex(index);

		EditorGUI.LabelField(new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight), "Waypoint " + (index + 1).ToString());

		EditorGUI.PropertyField(new Rect(new Rect(rect.x + 81, rect.y, rect.width - 81, EditorGUIUtility.singleLineHeight)),
								element.FindPropertyRelative("position"), GUIContent.none);
	}

	void DrawHeader(Rect rect)
	{
		string name = "Waypoints";
		EditorGUI.LabelField(rect, name);
	}

	private void WhenSelectedInEditor(ReorderableList list)
	{
		for (int i = 0; i < list.count; i++)
		{
			SceneView.lastActiveSceneView.LookAt(waypointManager.waypoints[list.index].position);

			GetPropertyValues(i);

			//to mark the selected waypoint in the scene view.
			if (i == list.index && waypointManager.waypoints[i].color != Color.red)
			{
				waypointManager.waypoints[i].color = Color.red;
			}
			//to unmark when not selected in scene view.
			else
			{
				waypointManager.waypoints[i].color = Color.white;
			}
			SceneView.RepaintAll();
		}
	}

	private void WhenSelectedInSceneView()
	{
		//get the index of waypoint
		//focus on element with same index in reordblablalist
		int mouseClickIndex;

		for (int i = 0; i < waypointList.count; i++)
		{
			GetPropertyValues(i);

			// get click point
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Plane plane = new Plane(ray.direction, propertyPosition.vector3Value);

			if (plane.Raycast(ray, out float hitDistance))
			{
				Vector3 v1 = ray.direction;
				Vector3 v2 = (propertyPosition.vector3Value/*end*/ - ray.origin/*start*/).normalized;

				float dot = Vector3.Dot(v1, v2);

				if (dot > 0.999)
				{
					mouseClickIndex = i;
					waypointManager.waypoints[mouseClickIndex].color = Color.red;
					waypointList.index = mouseClickIndex;
				}
				else
				{
					waypointManager.waypoints[i].color = Color.white;
				}
			}
		}
	}

	private void CreateNewWaypoint()
	{
		if(waypointList.count == 0)
		{
			AddFirstWaypoint();
		}
		else if (propertyAddWaypointBetween.boolValue == true)
		{
			propertyAddWaypointAtEnd.boolValue = false;
			Debug.Log(propertyAddWaypointAtEnd.boolValue.ToString());
			AddWaypointBetweenPoints();
		}
		else if (propertyAddWaypointAtEnd.boolValue == true)
		{
			propertyAddWaypointBetween.boolValue = false;
			Debug.Log(propertyAddWaypointBetween.boolValue.ToString());
			AddWaypointAtTheEnd();
		}
		else
		{
			AddWaypointAtTheBeginning();
		}
	}

	private void AddFirstWaypoint()
	{
		int insertIndex = 0;

		//Change by using plane/floor/etc when applying to an actual game.
		Vector3 newWaypointPosition = Vector3.zero;

		waypointList.serializedProperty.InsertArrayElementAtIndex(insertIndex);
		GetNewPropertyValues(insertIndex);
		newPropertyPosition.vector3Value = newWaypointPosition;

		newPropertyColor.colorValue = Color.white;
	}

	private void AddWaypointAtTheBeginning()
	{
		int insertIndex = 0;

		SerializedProperty planeInPoint = waypointList.serializedProperty.GetArrayElementAtIndex(insertIndex);
		SerializedProperty planeInPointPosition = planeInPoint.FindPropertyRelative("position");

		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Plane plane = new Plane(ray.direction, planeInPointPosition.vector3Value);

		Vector3 newWaypointPosition = Vector3.zero;

		if (plane.Raycast(ray, out float hitDistance))
		{
			newWaypointPosition = ray.origin + (ray.direction / 2) + (ray.direction * hitDistance);
		}

		waypointList.serializedProperty.InsertArrayElementAtIndex(insertIndex);
		GetNewPropertyValues(insertIndex);
		newPropertyPosition.vector3Value = newWaypointPosition;

		if (newPropertyColor.colorValue == Color.red)
		{
			newPropertyColor.colorValue = Color.white;
		}
	}

	private void AddWaypointAtTheEnd()
	{
		int insertIndex = waypointList.count - 1;
		SerializedProperty planeInPoint = waypointList.serializedProperty.GetArrayElementAtIndex(insertIndex);
		SerializedProperty planeInPointPosition = planeInPoint.FindPropertyRelative("position");

		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Plane plane = new Plane(ray.direction, planeInPointPosition.vector3Value);

		Vector3 newWaypointPosition = Vector3.zero;

		if (plane.Raycast(ray, out float hitDistance))
		{
			newWaypointPosition = ray.origin + (ray.direction / 2) + (ray.direction * hitDistance);
		}

		waypointList.serializedProperty.InsertArrayElementAtIndex(insertIndex);
		GetNewPropertyValues(insertIndex + 1);
		newPropertyPosition.vector3Value = newWaypointPosition;


		if (newPropertyColor.colorValue == Color.red)
		{
			newPropertyColor.colorValue = Color.white;
		}
	}

	private void AddWaypointBetweenPoints()
	{
		int firstWaypointIndex = 0;
		float distanceComparisonValue = 515;

		for (int i = 0; i < waypointList.count; i++)
		{
			GetPropertyValues(i);

			int nextIndex = i + 1;

			//same as Mathf.Repeat(), but looks cooler with %= ^^
			nextIndex %= waypointList.count;

			SerializedProperty nextProperty = waypointList.serializedProperty.GetArrayElementAtIndex(nextIndex);

			SerializedProperty nextPropertyPosition = nextProperty.FindPropertyRelative("position");

			float distanceFromMouseToLine = HandleUtility.DistanceToLine(propertyPosition.vector3Value, nextPropertyPosition.vector3Value);

			if (distanceComparisonValue > distanceFromMouseToLine)
			{
				distanceComparisonValue = distanceFromMouseToLine;
				firstWaypointIndex = i;
			}
		}

		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Plane plane = new Plane(ray.direction, propertyPosition.vector3Value);

		Vector3 newWaypointPosition = Vector3.zero;

		if (plane.Raycast(ray, out float hitDistance))
		{
			newWaypointPosition = ray.origin + (ray.direction / 2) + (ray.direction * hitDistance);
		}

		waypointList.serializedProperty.InsertArrayElementAtIndex(firstWaypointIndex);
		GetNewPropertyValues(firstWaypointIndex + 1);
		newPropertyPosition.vector3Value = newWaypointPosition;

		if(newPropertyColor.colorValue == Color.red)
		{
			newPropertyColor.colorValue = Color.white;
		}
	}


	private void DuringSceneGUI(SceneView sceneView)
	{
		//need to use Update and ApplyModifiedProperties to be able to undo/redo changes in the scene view.
		waypoint.Update();

		for (int i = 0; i < waypointList.count; i++)
		{
			GetPropertyValues(i);
			propertyPosition.vector3Value = Handles.PositionHandle(propertyPosition.vector3Value, Quaternion.identity);
			Handles.color = waypointManager.waypoints[i].color;
			propertyPosition.vector3Value = Handles.FreeMoveHandle(propertyPosition.vector3Value, Quaternion.identity, 0.3f, Vector3.one, Handles.SphereHandleCap);
			Handles.color = Color.white;
		}

		//note: use right mouse button to click
		bool holdingCtrl = (Event.current.modifiers & EventModifiers.Control) != 0;
		if (Event.current.type == EventType.MouseDown && holdingCtrl)
		{
			WhenSelectedInSceneView();
			Repaint();
			Event.current.Use();
		}

		//note: use right mouse button to click
		bool holdingAlt = (Event.current.modifiers & EventModifiers.Alt) != 0;
		if (Event.current.type == EventType.MouseDown && holdingAlt)
		{
			CreateNewWaypoint();
			Repaint();
			Event.current.Use();
		}

		waypoint.ApplyModifiedProperties();
	}

	public override void OnInspectorGUI()
	{
		//need to use Update and ApplyModifiedProperties to be able to undo/redo changes in the editor.
		waypoint.Update();

		waypointList.DoLayoutList();
		EditorGUILayout.PropertyField(propertyCircularSystem);
		EditorGUILayout.PropertyField(propertyAddWaypointBetween);
		if (propertyAddWaypointBetween.boolValue == false)
		{
			EditorGUILayout.PropertyField(propertyAddWaypointAtEnd);
		}
		waypoint.ApplyModifiedProperties();

		GUILayout.Space(75);

		base.OnInspectorGUI();
	}

	private void GetPropertyValues(int i)
	{
		property = waypointList.serializedProperty.GetArrayElementAtIndex(i);
		propertyPosition = property.FindPropertyRelative("position");
	}

	private void GetNewPropertyValues(int index)
	{
		newProperty = waypointList.serializedProperty.GetArrayElementAtIndex(index);
		newPropertyPosition = newProperty.FindPropertyRelative("position");
		newPropertyColor = newProperty.FindPropertyRelative("color");
	}
}