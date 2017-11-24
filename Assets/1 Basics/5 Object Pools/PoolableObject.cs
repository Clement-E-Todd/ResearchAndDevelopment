using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class PoolableObject : MonoBehaviour
{
	public ObjectPool Pool { get; set; }

	[System.NonSerialized] // We don't want the pool to be saved as part of the scene or prefab!
	ObjectPool poolInstanceForPrefab;

	protected virtual void Awake()
	{
		SceneManager.activeSceneChanged += OnSceneChanged;
	}

	public void ReturnToPool()
	{
		if (Pool)
		{
			Pool.AddObject(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnSceneChanged(Scene previousScene, Scene newScene)
	{
		ReturnToPool();
	}

	public T GetPooledInstance<T>() where T : PoolableObject
	{
		if (!poolInstanceForPrefab)
		{
			poolInstanceForPrefab = ObjectPool.GetPool(this);
		}
		return (T)poolInstanceForPrefab.GetObject();
	}
}