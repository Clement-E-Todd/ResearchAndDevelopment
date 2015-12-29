using UnityEngine;
using System.Collections;

namespace FoxyPack.Gravity
{
	public class GravitySourceDirectional : GravitySource
	{
		public Vector3 force = new Vector3(0, -9.81f, 0);
	
		public override Vector3 GetForceForReceiver(GravityReceiver receiver)
		{
			return force;
		} 
	}
}