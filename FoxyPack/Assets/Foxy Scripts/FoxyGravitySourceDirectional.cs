using UnityEngine;
using System.Collections;

public class FoxyGravitySourceDirectional : FoxyGravitySource
{
	public Vector3 force = new Vector3(0, -9.81f, 0);

	public override Vector3 GetForceForReceiver(FoxyGravityReceiver receiver)
	{
		return force;
	} 
}
