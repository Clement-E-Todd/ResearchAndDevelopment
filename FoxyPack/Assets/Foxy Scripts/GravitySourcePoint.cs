using UnityEngine;
using System.Collections;

namespace FoxyPack.Gravity
{
	public class GravitySourcePoint : GravitySource
	{
		public float strength = 9.81f;
	
		public override Vector3 GetForceForReceiver(GravityReceiver receiver)
		{
			return (transform.position - receiver.transform.position).normalized * strength;
		}
	}
}