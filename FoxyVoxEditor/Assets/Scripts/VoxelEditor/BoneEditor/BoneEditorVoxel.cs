using UnityEngine;
using System.Collections;

public class BoneEditorVoxel : MonoBehaviour
{
	private static GameObject prefab;
	
	Coord coord;

	public static BoneEditorVoxel Create(GameObject model, VoxelModelBone bone, Coord coord)
	{
		if (prefab == null)
		{
			prefab = Resources.Load<GameObject>("Prefabs/BoneEditorVoxel");
		}
		
		BoneEditorVoxel voxel = Instantiate<GameObject>(prefab).GetComponent<BoneEditorVoxel>();
		voxel.name = string.Format("Voxel ({0}, {1}, {2})", coord.x, coord.y, coord.z);
		
		voxel.coord = coord;
		
		voxel.transform.SetParent(model.transform);
		voxel.transform.localPosition = new Vector3(coord.x, coord.y, coord.z) + new Vector3(0.5f, 0.5f, 0.5f) - new Vector3(bone.width, bone.height, bone.depth) / 2;
		
		voxel.GetComponent<MeshRenderer>().material.color = Color.clear;
		
		return voxel;
	}
	
	void Update()
	{
		GetComponent<MeshRenderer>().material.color = BoneEditor.Instance.CurrentBone.GetColorAtCoord(coord).ToSystemColor();
	}
}