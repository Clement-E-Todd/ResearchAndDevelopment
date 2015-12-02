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
	
	public VoxelColor GetColorAtPalletIndex(int palletIndex)
	{
		if (palletIndex == 0)
		{
			return VoxelColor.clear;
		}
		else
		{
			return colorPallet[palletIndex-1];
		}
	}
	
	public void SetColorAtPalletIndex(int palletIndex, VoxelColor color)
	{
		colorPallet[palletIndex-1] = color;
	}
	
	public void CreateNewColorInPallet()
	{
		colorPallet.Add(VoxelColor.white);
	}
	
	public void RemoveColorAtPalletIndex(int palletIndex)
	{
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int z = 0; z < depth; ++z)
                {
                    if (voxelData[x, y, z] > palletIndex)
                    {
                        --voxelData[x, y, z];
                    }
                    else if (voxelData[x, y, z] == palletIndex)
                    {
                        voxelData[x, y, z] = 0;
                    }
                }
            }
        }

		colorPallet.RemoveAt(palletIndex-1);
	}
	
	public void DecreaseIndexOfColorAtPalletIndex(int currentPalletIndex)
	{
		VoxelColor indicatedColor = colorPallet[currentPalletIndex-1];
		VoxelColor colorToSwap = colorPallet[currentPalletIndex-2];
		
		colorPallet[currentPalletIndex-2] = indicatedColor;
		colorPallet[currentPalletIndex-1] = colorToSwap;
	}
	
	public void IncreaseIndexOfColorAtPalletIndex(int currentPalletIndex)
	{
		VoxelColor indicatedColor = colorPallet[currentPalletIndex-1];
		VoxelColor colorToSwap = colorPallet[currentPalletIndex];
		
		colorPallet[currentPalletIndex] = indicatedColor;
		colorPallet[currentPalletIndex-1] = colorToSwap;
	}
	
	public VoxelColor GetColorAtCoord(Coord coord)
	{
		int palletIndex = voxelData[coord.x, coord.y, coord.z];
	
		return GetColorAtPalletIndex(palletIndex);
	}
	
	public int GetColorPalletCount()
	{
		return colorPallet.Count;
	}
	
	public void SetColorIndexAtCoord(int palletIndex, Coord coord)
	{
		voxelData[coord.x, coord.y, coord.z] = palletIndex;
	}
}
