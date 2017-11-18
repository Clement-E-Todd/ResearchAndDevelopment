using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : MonoBehaviour
{
	public Rigidbody Body { get; private set; }

	void Awake()
	{
		Body = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Kill Zone"))
		{
			Destroy(gameObject);
		}
	}
}