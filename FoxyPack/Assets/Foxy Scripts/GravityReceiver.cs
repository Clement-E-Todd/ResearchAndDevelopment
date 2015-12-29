using UnityEngine;
using System.Collections;

namespace FoxyPack.Gravity
{
	[RequireComponent (typeof(Rigidbody))]
	public class GravityReceiver : MonoBehaviour
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
	
			foreach (GravitySource gravitySource in GravitySource.AllActiveSources)
			{
				force += gravitySource.GetForceForReceiver(this);
			}
	
			rigid.AddForce(force / Time.fixedDeltaTime);
		}
	}
}