using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoneEditor2DView : MonoBehaviour
{
	public enum Angle
	{
		Front,
		Side,
		Top
	}
	
	public Angle angle;
	public Text viewNameText;
	public Mask gridMask;
	
	void Awake()
	{
		switch (angle)
		{
		case Angle.Front:
			viewNameText.text = "FRONT VIEW";
			GetComponent<Image>().color = new Color(0.75f, 0.75f, 1f, 0.8f);
			break;
			
		case Angle.Side:
			viewNameText.text = "SIDE VIEW";
			GetComponent<Image>().color = new Color(1f, 0.75f, 0.75f, 0.8f);			
			break;
			
		case Angle.Top:
			viewNameText.text = "TOP VIEW";
			GetComponent<Image>().color = new Color(0.75f, 1f, 0.75f, 0.8f);
			break;
		}
	}
	
	public void OnVoxelModelCreated()
	{
		switch (angle)
		{
		case Angle.Front:
			for (int x = 0; x < BoneEditor.Instance.CurrentBone.width; ++x)
			{
				for (int y = 0; y < BoneEditor.Instance.CurrentBone.height; ++y)
				{
					BoneEditor2DViewVoxel editorVoxel = CreatePointOnGrid(x, y, Mathf.Max(BoneEditor.Instance.CurrentBone.width,
					                                                                      BoneEditor.Instance.CurrentBone.height));
                    editorVoxel.Coord = new Coord(x, y, -1);
				}
			}
			
			break;
			
		case Angle.Side:
			for (int z = 0; z < BoneEditor.Instance.CurrentBone.depth; ++z)
			{
				for (int y = 0; y < BoneEditor.Instance.CurrentBone.height; ++y)
				{
					BoneEditor2DViewVoxel editorVoxel = CreatePointOnGrid(z, y, Mathf.Max(BoneEditor.Instance.CurrentBone.depth,
					                                                                      BoneEditor.Instance.CurrentBone.height));
					editorVoxel.Coord = new Coord(-1, y, z);
				}
			}
			
			break;
			
		case Angle.Top:
			for (int x = 0; x < BoneEditor.Instance.CurrentBone.width; ++x)
			{
				for (int z = 0; z < BoneEditor.Instance.CurrentBone.depth; ++z)
				{
					BoneEditor2DViewVoxel editorVoxel = CreatePointOnGrid(x, z, Mathf.Max(BoneEditor.Instance.CurrentBone.width,
					                                                                      BoneEditor.Instance.CurrentBone.depth));
					editorVoxel.Coord = new Coord(x, -1, z);
				}
			}
			
			break;
		}
	}
	
	BoneEditor2DViewVoxel CreatePointOnGrid(int x, int y, int gridSize)
	{
		BoneEditor2DViewVoxel editorVoxel = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BoneEditor2DViewVoxel")).GetComponent<BoneEditor2DViewVoxel>();
		
		float color = (x+y)%2 == 0 ? 0.45f : 0.55f;
		editorVoxel.GetComponent<Image>().color = new Color(color, color, color);
		
		editorVoxel.transform.SetParent(gridMask.transform, false);
		
		float maskSize = gridMask.GetComponent<RectTransform>().sizeDelta.x;
		
		editorVoxel.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y) * (maskSize / (gridSize+1));
		editorVoxel.GetComponent<RectTransform>().anchoredPosition -= Vector2.one * (gridSize-1) * (maskSize / (gridSize+1)) / 2;
		editorVoxel.GetComponent<RectTransform>().sizeDelta = Vector2.one * (maskSize / gridSize) * 0.8f;
		
		return editorVoxel;
	}
	
	public void OnNextSlicePressed()
	{
		if (BoneEditor.Instance.CurrentBone == null)
		{
			return;
		}
	
		switch (angle)
		{
		case Angle.Front:
			if (BoneEditor.Instance.ViewCoords.z < BoneEditor.Instance.CurrentBone.depth-1)
			{
				Coord coord = BoneEditor.Instance.ViewCoords;
				coord.z += 1;
				BoneEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Side:
			if (BoneEditor.Instance.ViewCoords.x < BoneEditor.Instance.CurrentBone.width-1)
			{
				Coord coord = BoneEditor.Instance.ViewCoords;
				coord.x += 1;
				BoneEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Top:
			if (BoneEditor.Instance.ViewCoords.y < BoneEditor.Instance.CurrentBone.height-1)
			{
				Coord coord = BoneEditor.Instance.ViewCoords;
				coord.y += 1;
				BoneEditor.Instance.ViewCoords = coord;
			}
			break;
		}
	}
	
	public void OnLastSlicePressed()
	{
		if (BoneEditor.Instance.CurrentBone == null)
		{
			return;
		}
		
		switch (angle)
		{
		case Angle.Front:
			if (BoneEditor.Instance.ViewCoords.z > 0)
			{
				Coord coord = BoneEditor.Instance.ViewCoords;
				coord.z -= 1;
				BoneEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Side:
			if (BoneEditor.Instance.ViewCoords.x > 0)
			{
				Coord coord = BoneEditor.Instance.ViewCoords;
				coord.x -= 1;
				BoneEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Top:
			if (BoneEditor.Instance.ViewCoords.y > 0)
			{
				Coord coord = BoneEditor.Instance.ViewCoords;
				coord.y -= 1;
				BoneEditor.Instance.ViewCoords = coord;
			}
			break;
		}
	}
}
