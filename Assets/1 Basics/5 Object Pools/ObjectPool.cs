using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
	PoolableObject prefab;

	List<PoolableObject> availableObjects = new List<PoolableObject>();

	public static ObjectPool GetPool(PoolableObject prefab)
	{
		GameObject obj;
		ObjectPool pool;

		if (Application.isEditor)
		{
			obj = GameObject.Find(prefab.name + " Pool");
			if (obj)
			{
				pool = obj.GetComponent<ObjectPool>();
				if (pool)
				{
					return pool;
				}
			}
		}

		obj = new GameObject(prefab.name + " Pool");
		DontDestroyOnLoad(obj);
		pool = obj.AddComponent<ObjectPool>();
		pool.prefab = prefab;

		return pool;
	}

	public PoolableObject GetObject()
	{
		PoolableObject obj;
		int lastAvailableIndex = availableObjects.Count - 1;
		if (lastAvailableIndex >= 0)
		{
			obj = availableObjects[lastAvailableIndex];
			availableObjects.RemoveAt(lastAvailableIndex);
			obj.gameObject.SetActive(true);
		}
		else
		{
			obj = Instantiate<PoolableObject>(prefab);
			obj.transform.SetParent(transform, false);
			obj.Pool = this;
		}
		return obj;
	}

	public void AddObject(PoolableObject obj)
	{
		obj.gameObject.SetActive(false);
		availableObjects.Add(obj);
	}
}