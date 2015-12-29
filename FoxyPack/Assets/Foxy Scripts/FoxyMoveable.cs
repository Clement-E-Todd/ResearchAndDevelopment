﻿using UnityEngine;
using System.Collections;

/**
 * This class for multiple RigidBody.Move calls per frame to be performed relative to the object's position.
 */
[RequireComponent (typeof(Rigidbody))]
public class FoxyMoveable : MonoBehaviour
{
	Rigidbody rigid;
	Vector3 totalMovement;

	void Awake()
	{
		rigid = GetComponent<Rigidbody>();

		Reset();
	}

	void FixedUpdate()
	{
		rigid.MovePosition(transform.position + totalMovement);

		Reset();
	}

	public void MoveBy(Vector3 distance)
	{
		totalMovement += distance;
	}

	void Reset()
	{
		totalMovement = Vector3.zero;
	}
}
