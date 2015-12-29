using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(FoxyMoveable))]
public class FoxyGroundable : MonoBehaviour
{
	public bool Grounded
	{
		get
		{
			return currentPlatform != null;
		}
	}

	private Transform currentPlatform;
	public Transform CurrentPlatform
	{
		get
		{
			return currentPlatform;
		}
	}

	private Vector3 platformVelocity;
	public Vector3 PlatformVelocity
	{
		get
		{
			return platformVelocity;
		}
	}

	private Quaternion platformRotation;
	public Quaternion PlatformRotation
	{
		get
		{
			return platformRotation;
		}
	}

	const float MinimumGroundDotProduct = 0.5f;
	
	private List<Collision> collisions = new List<Collision>();

	Vector3 contactPositionInWorld;
	Vector3 contactPositionOnPlatform;
	Quaternion contactRotationOnPlatform;

	FoxyMoveable moveable;
	
	public bool followGroundRotation;
	
	void Awake()
	{
		moveable = GetComponent<FoxyMoveable>();
	}

	void FixedUpdate()
	{
		Transform previousPlatform = currentPlatform;

		// Find out how much the point of contact with the platform has moved since the last update
		if (previousPlatform != null)
		{
			Vector3 newContactPositionInWorld = currentPlatform.TransformPoint(contactPositionOnPlatform);
			platformVelocity = newContactPositionInWorld - contactPositionInWorld;

			Quaternion newContactRotationInWorld = currentPlatform.rotation * contactRotationOnPlatform;
			platformRotation = newContactRotationInWorld * Quaternion.Inverse(transform.rotation);
			platformRotation *= Quaternion.FromToRotation(platformRotation * transform.up, transform.up);
		}

		// Update the current platform
		currentPlatform = null;
		float highestTotalDotProduct = 0f;

		foreach (Collision collision in collisions)
		{
			// Get the dot product of the contact point which is most firmly grounded in this collision
			float highestDotProductForThisCollision = 0f;

			foreach (ContactPoint contactPoint in collision.contacts)
			{
				float groundDotProduct = Vector3.Dot(transform.up, contactPoint.normal);

				if (groundDotProduct > highestDotProductForThisCollision)
				{
					highestDotProductForThisCollision = groundDotProduct;
				}
			}

			// If the dot product we just found is high enough we can now consider this object to be "grounded"
			if (highestDotProductForThisCollision > MinimumGroundDotProduct)
			{
				// The object upon which this object is sitting most directly will be considered its "currentplatform"
				if (highestDotProductForThisCollision > highestTotalDotProduct)
				{
					highestTotalDotProduct = highestDotProductForThisCollision;
					currentPlatform = collision.transform;
				}
			}
		}

		// Handle moving platforms
		if (currentPlatform != null)
		{
			if (currentPlatform == previousPlatform)
			{
				moveable.MoveBy(platformVelocity);
				
				if (followGroundRotation)
				{
					transform.Rotate(platformRotation.eulerAngles);
				}
			}

			contactPositionInWorld = transform.position;
			contactPositionOnPlatform = currentPlatform.InverseTransformPoint(transform.position);
			contactRotationOnPlatform = Quaternion.Inverse(currentPlatform.rotation) * transform.rotation;
		}
		else
		{
			platformVelocity = Vector3.zero;
		}

		collisions.Clear();
	}

	void OnCollisionStay(Collision collision)
	{
		collisions.Add(collision);
	}
}
