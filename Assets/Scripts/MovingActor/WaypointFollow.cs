using UnityEngine;

public class WaypointFollow : MonoBehaviour
{
	[SerializeField]
	WaypointManager wpManager = default;

	[SerializeField, Tooltip("How close the character needs to be to target location before going towards new target.")]
	float accuracy = 0.1f;
	[SerializeField, Tooltip("Character movement speed.")]
	float speed = 5.0f;
	[SerializeField, Tooltip("Character turning/rotation speed. Set with caution; wrong values may cause bugs when turning.")]
	float rotationSpeed = 15.0f;

	int currentWP = 0;

	bool goingBackToStart = false;

	Vector3 lookAtGoal;
	Vector3 direction;

	void LateUpdate()
	{
		if (wpManager.waypoints.Count == 0) return;

		Move();
	}

	void Move()
	{
		SetLookAtGoal();
		SetDirection();
		SetRotation();
		CheckNewTargetDirection();
		transform.Translate(0, 0, speed * Time.deltaTime);
	}

	void SetLookAtGoal()
	{
		lookAtGoal = wpManager.waypoints[currentWP].position;
	}

	void SetDirection()
	{
		direction = lookAtGoal - transform.position;
	}

	void SetRotation()
	{
		transform.rotation = Quaternion.Slerp(transform.rotation,
												Quaternion.LookRotation(direction),
												Time.deltaTime * rotationSpeed);
	}

	void CheckNewTargetDirection()
	{
		if (direction.magnitude < accuracy)
		{
			if (wpManager.circularWaypointSystem == true)
			{
				currentWP++;
				if (currentWP >= wpManager.waypoints.Count)
				{
					currentWP = 0;
				}
			}
			else
			{
				if (currentWP >= wpManager.waypoints.Count -1)
				{
					currentWP = wpManager.waypoints.Count - 1;
					goingBackToStart = true;

				}
				else if (currentWP <= 0)
				{
					goingBackToStart = false;
					currentWP = 0;
				}
				if (goingBackToStart)
				{
					currentWP--;
				}
				else
				{
					currentWP++;
				}
			}
		}
	}
}