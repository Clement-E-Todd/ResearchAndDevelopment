using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FoxyPack.Gravity
{
	public abstract class GravitySource : MonoBehaviour
	{
		private static List<GravitySource> allActiveSources = new List<GravitySource>();
		public static List<GravitySource> AllActiveSources
		{
			get
			{
				return allActiveSources;
			}
		}
	
		void OnEnable()
		{
			if (!allActiveSources.Contains(this))
			{
				allActiveSources.Add(this);
			}
		}
	
		void OnDisable()
		{
			AllActiveSources.Remove(this);
		}
	
		public abstract Vector3 GetForceForReceiver(GravityReceiver receiver);
	}
}