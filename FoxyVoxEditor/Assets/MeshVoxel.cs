using UnityEngine;
using System.Collections;

public class MeshVoxel : MonoBehaviour
{
	private static GameObject prefab;

	public static MeshVoxel Create(MeshVoxelModel model, int x, int y, int z)
	{
		if (prefab == null)
		{
			prefab = Resources.Load<GameObject>("Prefabs/MeshVoxel");
		}
		
		MeshVoxel voxel = Instantiate<GameObject>(prefab).GetComponent<MeshVoxel>();
		voxel.name = string.Format("Voxel ({0}, {1}, {2})", x, y, z);
		
		voxel.transform.SetParent(model.transform);
		voxel.transform.localPosition = new Vector3(x, y, z) + new Vector3(0.5f, 0.5f, 0.5f) - new Vector3(model.width, model.height, model.depth) / 2;
		
		voxel.GetComponent<MeshRenderer>().material.color = Color.clear;
		
		return voxel;
	}
}
