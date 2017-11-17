using UnityEngine;

public class NucleonSpawner : MonoBehaviour
{
	public float timeBetweenSpawns;

	public float spawnDistance;

	public float spawnsPerBurst;

	public Nucleon[] nucleonPrefabs;

	float timeSinceLastSpawn;

	private void FixedUpdate()
	{
		timeSinceLastSpawn += Time.deltaTime;
		if (timeSinceLastSpawn >= timeBetweenSpawns)
		{
			timeSinceLastSpawn -= timeBetweenSpawns;
			SpawnNucleon();
		}
	}

	private void SpawnNucleon()
	{
		for (int i = 0; i < spawnsPerBurst; i++)
		{
			Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
			Nucleon spawn = Instantiate<Nucleon>(prefab);
			spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
		}
	}
}
