using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModelView2D : MonoBehaviour
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
			for (int x = 0; x < ModelEditor.Instance.Model.width; ++x)
			{
				for (int y = 0; y < ModelEditor.Instance.Model.height; ++y)
				{
					ModelView2DVoxel editorVoxel = CreatePointOnGrid(x, y, Mathf.Max(ModelEditor.Instance.Model.width,
					                                                                 ModelEditor.Instance.Model.height));
                    editorVoxel.Coord = new Coord(x, y, -1);
				}
			}
			
			break;
			
		case Angle.Side:
			for (int z = 0; z < ModelEditor.Instance.Model.depth; ++z)
			{
				for (int y = 0; y < ModelEditor.Instance.Model.height; ++y)
				{
					ModelView2DVoxel editorVoxel = CreatePointOnGrid(z, y, Mathf.Max(ModelEditor.Instance.Model.depth,
					                                                                 ModelEditor.Instance.Model.height));
					editorVoxel.Coord = new Coord(-1, y, z);
				}
			}
			
			break;
			
		case Angle.Top:
			for (int x = 0; x < ModelEditor.Instance.Model.width; ++x)
			{
				for (int z = 0; z < ModelEditor.Instance.Model.depth; ++z)
				{
					ModelView2DVoxel editorVoxel = CreatePointOnGrid(x, z, Mathf.Max(ModelEditor.Instance.Model.width,
					                                                                 ModelEditor.Instance.Model.depth));
					editorVoxel.Coord = new Coord(x, -1, z);
				}
			}
			
			break;
		}
	}
	
	ModelView2DVoxel CreatePointOnGrid(int x, int y, int gridSize)
	{
		ModelView2DVoxel editorVoxel = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ModelView2DVoxel")).GetComponent<ModelView2DVoxel>();
		
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
		if (ModelEditor.Instance.Model == null)
		{
			return;
		}
	
		switch (angle)
		{
		case Angle.Front:
			if (ModelEditor.Instance.ViewCoords.z < ModelEditor.Instance.Model.depth-1)
			{
				Coord coord = ModelEditor.Instance.ViewCoords;
				coord.z += 1;
				ModelEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Side:
			if (ModelEditor.Instance.ViewCoords.x < ModelEditor.Instance.Model.width-1)
			{
				Coord coord = ModelEditor.Instance.ViewCoords;
				coord.x += 1;
				ModelEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Top:
			if (ModelEditor.Instance.ViewCoords.y < ModelEditor.Instance.Model.height-1)
			{
				Coord coord = ModelEditor.Instance.ViewCoords;
				coord.y += 1;
				ModelEditor.Instance.ViewCoords = coord;
			}
			break;
		}
	}
	
	public void OnLastSlicePressed()
	{
		if (ModelEditor.Instance.Model == null)
		{
			return;
		}
		
		switch (angle)
		{
		case Angle.Front:
			if (ModelEditor.Instance.ViewCoords.z > 0)
			{
				Coord coord = ModelEditor.Instance.ViewCoords;
				coord.z -= 1;
				ModelEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Side:
			if (ModelEditor.Instance.ViewCoords.x > 0)
			{
				Coord coord = ModelEditor.Instance.ViewCoords;
				coord.x -= 1;
				ModelEditor.Instance.ViewCoords = coord;
			}
			break;
			
		case Angle.Top:
			if (ModelEditor.Instance.ViewCoords.y > 0)
			{
				Coord coord = ModelEditor.Instance.ViewCoords;
				coord.y -= 1;
				ModelEditor.Instance.ViewCoords = coord;
			}
			break;
		}
	}
}
