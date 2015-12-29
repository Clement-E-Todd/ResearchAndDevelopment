using UnityEngine;
using System.Collections;

[RequireComponent (typeof(FoxyMoveable))]
public class TestCharacterController : MonoBehaviour
{
	public Camera relativeCamera = null;

	public float walkSpeed = 2.1f;
	public float runSpeed = 4.2f;
	private float jumpForce = 2f;

	FoxyMoveable moveable;

	void Awake()
	{
		moveable = GetComponent<FoxyMoveable>();
	}

	void FixedUpdate()
	{
		// Figure out how the character should move based on the angle between them and the camera
		if (relativeCamera == null)
		{
			relativeCamera = Camera.main;
		}

		Vector3 forward = relativeCamera.transform.up;// (transform.position - camera.transform.position).normalized;

		if (Vector3.Dot(forward, transform.forward) < 0.01f)
		{
			forward = relativeCamera.transform.forward;
		}
		
		forward -= transform.up * Vector3.Dot(forward, transform.up);
		forward.Normalize();

		// Figure out whether or not to run based on player input
		float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

		// Figure out which direction to move in based on player input
		Vector3 movementDirection = Vector3.zero;

		if (Input.GetKey(KeyCode.UpArrow))
		{
			movementDirection += forward;
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			movementDirection -= forward;
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			movementDirection += Vector3.Cross(forward, transform.up);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			movementDirection -= Vector3.Cross(forward, transform.up);
		}

		// Move towards the indicated direction
		if (movementDirection != Vector3.zero)
		{
//			moveable.MoveBy(transform.forward * speed * Time.fixedDeltaTime);

			float turnSpeed = (Mathf.PI * 2) * (runSpeed / speed);
//			transform.forward = Vector3.RotateTowards(transform.forward, movementDirection, turnSpeed * Time.fixedDeltaTime, 1);
			moveable.RotateBy(new Vector3(0, 90f, 0));
		}

		FoxyGroundable groundable = GetComponent<FoxyGroundable>();

		if (Input.GetKeyDown(KeyCode.Space) && groundable != null && groundable.Grounded)
		{
			GetComponent<Rigidbody>().velocity += (transform.up * jumpForce);
		}
	}
}
