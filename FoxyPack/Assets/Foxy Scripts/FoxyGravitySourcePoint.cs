using UnityEngine;
using System.Collections;

public class FoxyGravitySourcePoint : FoxyGravitySource
{
	public float strength = 9.81f;

	public override Vector3 GetForceForReceiver(FoxyGravityReceiver receiver)
	{
		return (transform.position - receiver.transform.position).normalized * strength;
	}
}
