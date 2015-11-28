using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelModelBone
{
	int[,,] voxelData;

	Coord dimensions;
	
	public int width
	{
		get
		{
			return (int)dimensions.x;
		}
	}
	
	public int height
	{
		get
		{
			return (int)dimensions.y;
		}
	}
	
	public int depth
	{
		get
		{
			return (int)dimensions.z;
		}
	}
	
	List<VoxelColor> colorPallet;
	
	public static VoxelModelBone Create(int width, int height, int depth, List<VoxelColor> colorPallet)
	{
		VoxelModelBone bone = new VoxelModelBone();
		
		bone.voxelData = new int[width, height, depth];
		bone.dimensions = new Coord(width, height, depth);
		bone.colorPallet = colorPallet;
		
		return bone;
	}
	
	public VoxelColor GetColorAtCoord(Coord coord)
	{
		int palletIndex = voxelData[coord.x, coord.y, coord.z];
	
		if (palletIndex == 0)
		{
			return VoxelColor.clear;
		}
		else
		{
			return colorPallet[palletIndex-1];
		}	
	}
}
