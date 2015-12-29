using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class FoxyGravityReceiver : MonoBehaviour
{
	Rigidbody rigid;

	private Vector3 force = Vector3.zero;
	public Vector3 Force
	{
		get
		{
			return force;
		}
	}

	public Vector3 Direction
	{
		get
		{
			return force.normalized;
		}
	}

	public float Magnitude
	{
		get
		{
			return force.magnitude;
		}
	}

	void Awake()
	{
		rigid = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		if (!rigid.useGravity)
		{
			return;
		}

		if (Physics.gravity != Vector3.zero)
		{
			Debug.LogError("Gravity must be set to zero in the player settings! Use gravity sources instead.");

			return;
		}

		// Apply force from each gravity source in the scene
		force = Vector3.zero;

		foreach (FoxyGravitySource gravitySource in FoxyGravitySource.AllActiveSources)
		{
			force += gravitySource.GetForceForReceiver(this);
		}

		rigid.AddForce(force / Time.fixedDeltaTime);
	}
}
