using UnityEngine;
using System.Collections;

/**
 * This class for multiple RigidBody.Move calls per frame to be performed relative to the object's position.
 */
[RequireComponent (typeof(Rigidbody))]
public class FoxyMoveable : MonoBehaviour
{
	Rigidbody rigid;

	Vector3 totalMovement;
	Quaternion totalRotation;

	void Awake()
	{
		rigid = GetComponent<Rigidbody>();

		Reset();
	}

	void FixedUpdate()
	{
		rigid.MovePosition(transform.position + totalMovement);
		rigid.rotation *= totalRotation;

		Reset();
	}

	public void MoveBy(Vector3 distance)
	{
		totalMovement += distance;
	}

	public void RotateBy(Quaternion rotation)
	{
		totalRotation *= rotation;
	}

	public void RotateBy(Vector3 eulerAngles)
	{
		RotateBy(Quaternion.Euler(eulerAngles));
	}

	void Reset()
	{
		totalMovement = Vector3.zero;
		totalRotation = Quaternion.identity;
	}
}
