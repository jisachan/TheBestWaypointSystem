Jonico's Awesome Waypoint Editor

To start adding your waypoints into your scene, simply hold down 'alt' and click in the sceneview wherever you want the first waypoint to be.
(Note: If your scene is empty or you click on a completely empty area, the position of the first waypoint will be default at 0,0,0.)
(Note: Adding several waypoints into an empty scene will cause their positions to not be completely accurate. Reorganize them manually if it's an issue.)

To add new waypoints, select your desired method. The available methods are:
*Add Waypoint Between - This adds a new waypoint between the two nearest waypoints where you click.
*Add Waypoint At End - This adds a new waypoint after the currently last waypoint. The new waypoint becomes the last waypoint.
*Add Waypoint At Beginning - This adds a new waypoint after the currently first waypoint. The new waypoint becomes the first waypoint.
(Note: To enable adding at end or beginning, uncheck the "AddWaypointBetween" box.)
(Note: If no box is checked, Add Waypoint At Beginning is the default.)

Additional ways of adding waypoints:
*Adding waypoints through the reorderable list in the editor - 
	You can add or remove waypoints by clicking the + or -. 
	If using the +, the new waypoint will be located ontop of the previously last waypoint. 
	Change its position by entering the values, or by dragging it to new desired position in the scene view.

Waypoint Selection:
To select a waypoint, simply click on it in the editor. The sceneview will focus on the selected waypoint and change its color.
To select a waypoint in the sceneview (for example, to see where it is in the editor list), hold down 'ctrl' and right click on the desired waypoint.

Additional features:
There is a selection box related to the AI unit's movement behavior. 
The unit may either circulate the waypoints (think racetrack) or go back and forth between the first and last waypoints (think pacing back and forth in a hallway).
This feature requires you to have a bool named "goingBackToStart" in your unit's movement script, along with the related method.
Please see this project's "WaypointFollow" script for the code. That is this project's unit's movement script.

To use with an AI unit, not in this project:
1. Serialize your waypoint manager variable in the unit's movement script's backingfield.
2. Create empty game object "Waypoint Manager" in your scene heirarchy. 
3. Attach your "WaypointManager" script to your "Waypoint Manager" game object.
4. If not already attached, attach your unit's movement script to your unit game object.
5. Drag your "Waypoint Manager" game object into the serialized field in the editor, created in step 1.
Ready to use!

Final Note: 
While this is strongly related to AI unit movement, this is still just a waypoint editor. 
The AI unit's movement script ("WaypointFollow") in this project is the very most basic. It isn't usable in an actual game in its current state.
Feel free to edit and change the code as you see fit, for your own projects. Though please give due credit to me for the editor itself. 
It's my first ever tool, so it's my lil pet. Thank you for using it and let me know if you find any bugs! ^^/

===WHEN IN DOUBT, READ THE TOOLTIPS===