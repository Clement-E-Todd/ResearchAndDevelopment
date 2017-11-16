using UnityEngine;
using System.Collections;

public class VoxelColor
{
	[Range (0, 15)]
	public int r;
	
	[Range (0, 15)]
	public int g;
	
	[Range (0, 15)]
	public int b;
	
	[Range (0, 15)]
	public int a;
	
	public VoxelColor(float r, float g, float b, float a)
		: this((int)(r*15), (int)(g*15), (int)(b*15), (int)(a*15))
	{ }
	
	public VoxelColor(int r, int g, int b, int a)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}
	
	public Color ToSystemColor()
	{
		return new Color(r/15f, g/15f, b/15f, a/15f);
	}
	
	public static VoxelColor white
	{
		get
		{
			return new VoxelColor(15, 15, 15, 15);
		}
	}
	
	public static VoxelColor black
	{
		get
		{
			return new VoxelColor(0, 0, 0, 15);
		}
	}
	
	public static VoxelColor grey
	{
		get
		{
			return new VoxelColor(7, 7, 7, 15);
		}
	}
	
	public static VoxelColor clear
	{
		get
		{
			return new VoxelColor(15, 15, 15, 0);
		}
	}
	
	public static VoxelColor red
	{
		get
		{
			return new VoxelColor(15, 0, 0, 15);
		}
	}
	
	public static VoxelColor green
	{
		get
		{
			return new VoxelColor(0, 15, 0, 15);
		}
	}
	
	public static VoxelColor blue
	{
		get
		{
			return new VoxelColor(0, 0, 15, 15);
		}
	}
}