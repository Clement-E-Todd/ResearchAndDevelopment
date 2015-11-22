using UnityEngine;
using System.Collections;

public class MeshVoxelModel : MonoBehaviour
{
	MeshVoxel[,,] voxelObjects;
	public MeshVoxel[,,] VoxelObjects
	{
		get
		{
			return voxelObjects;
		}
	}
	
	Vector3 dimensions;
	
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
	
	public static MeshVoxelModel Create(ushort width, ushort height, ushort depth)
	{
		MeshVoxelModel model = (new GameObject()).AddComponent<MeshVoxelModel>();
		
		model.voxelObjects = new MeshVoxel[width,height,depth];
		model.dimensions = new Vector3(width, height, depth);
		
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				for (int z = 0; z < depth; ++z)
				{
					model.voxelObjects[x,y,z] = MeshVoxel.Create(model, x, y, z);
				}
			}
		}
		
		return model;
	}
}
