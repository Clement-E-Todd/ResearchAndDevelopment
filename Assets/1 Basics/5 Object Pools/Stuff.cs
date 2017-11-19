using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : PoolableObject
{
	public Rigidbody Body { get; private set; }

	MeshRenderer[] meshRenderers;

	public void SetMaterial(Material material)
	{
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			meshRenderers[i].material = material;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		Body = GetComponent<Rigidbody>();
		meshRenderers = GetComponentsInChildren<MeshRenderer>();

		// This is literally only here to make allocations heavier:
		FindObjectsOfType<Stuff>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Kill Zone"))
		{
			ReturnToPool();
		}
	}
}