using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FoxyGravitySource : MonoBehaviour
{
	private static List<FoxyGravitySource> allActiveSources = new List<FoxyGravitySource>();
	public static List<FoxyGravitySource> AllActiveSources
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

	public abstract Vector3 GetForceForReceiver(FoxyGravityReceiver receiver);
}
