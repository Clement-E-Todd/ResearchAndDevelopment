using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelModel : MonoBehaviour
{
	List<VoxelModelBone> bones = new List<VoxelModelBone>();
	public VoxelModelBone[] Bones
	{
		get
		{
			return bones.ToArray();
		}
	}
	
	public static VoxelModel CreateNew(int rootWidth, int rootHeight, int rootDepth)
	{
		VoxelModel model = (new GameObject()).AddComponent<VoxelModel>();
		
		List<VoxelColor> colorPallet = new List<VoxelColor>();
		colorPallet.Add(VoxelColor.black);
		colorPallet.Add(VoxelColor.white);
		colorPallet.Add(VoxelColor.grey);
		
		model.AddBone(rootWidth, rootHeight, rootDepth, colorPallet);
		
		return model;
	}
	
	public void AddBone(int width, int height, int depth, List<VoxelColor> colorPallet)
	{
		bones.Add(VoxelModelBone.Create(width, height, depth, colorPallet));
	}
}
