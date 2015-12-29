using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class TestFollowCamera : MonoBehaviour
{
	public Transform target;
	public float minDistance = 5;
	public float maxDistance = 10;

	[Range (0f, 1f)]
	public float zoom = 0.5f;
	public float height = 0.25f;

	void Update()
	{
		// Set the zoom
		if (Input.GetKey(KeyCode.W))
		{
			zoom = Mathf.MoveTowards(zoom, 1f, Time.deltaTime);
		}
		
		if (Input.GetKey(KeyCode.S))
		{
			zoom = Mathf.MoveTowards(zoom, 0f, Time.deltaTime);
		}

		float distance = Mathf.Lerp(maxDistance, minDistance, zoom);

		// Set the rotation angle
		Vector3 direction = (transform.position - target.position).normalized;

		direction = new Vector3(direction.x, height, direction.z).normalized;

		if (Input.GetKey(KeyCode.A))
		{
			direction = Quaternion.Euler(0, 180 * Time.deltaTime, 0) * direction;
		}

		if (Input.GetKey(KeyCode.D))
		{
			direction = Quaternion.Euler(0, -180 * Time.deltaTime, 0) * direction;
		}

		// Use the zoom and rotation values to position and angle the camera
		transform.position = target.position + direction * distance;
		transform.forward = (target.position - transform.position).normalized;
	}
}
