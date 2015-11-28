using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class BoneEditor2DViewVoxel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	private static BoneEditor2DViewVoxel currentSelection;
	public static BoneEditor2DViewVoxel CurrentSelection
	{
		get
		{
			return currentSelection;
		}
	}
	
	public Image voxelColorImage;
	public Image highlightImage;

	private Coord coord;
	public Coord Coord
	{
		get
		{
			return new Coord(coord.x != -1 ? coord.x : BoneEditor.Instance.ViewCoords.x,
			                 coord.y != -1 ? coord.y : BoneEditor.Instance.ViewCoords.y,
			                 coord.z != -1 ? coord.z : BoneEditor.Instance.ViewCoords.z);
		}
		
		set
		{
			coord = value;
		}
	}
	
	public BoneEditorVoxel VoxelObject
	{
		get
		{
			Coord voxelObjectCoord = Coord;
			
			return BoneEditor.Instance.VoxelObjects[voxelObjectCoord.x, voxelObjectCoord.y, voxelObjectCoord.z];
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			VoxelObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			VoxelObject.GetComponent<MeshRenderer>().material.color = Color.clear;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		currentSelection = this;
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		if (currentSelection == this)
		{
			currentSelection = null;
		}
	}

	void Update()
	{
		voxelColorImage.color = VoxelObject.GetComponent<MeshRenderer>().material.color;
		
		if (BoneEditor2DViewVoxel.CurrentSelection == this)
		{
			float color = 0.5f + Mathf.Sin(Time.time*Mathf.PI*2) / 4;
			float alpha = Mathf.Lerp(0f, 0.5f, Mathf.Abs(Mathf.Sin(Time.time*Mathf.PI*2)));
			highlightImage.color = new Color(color, color, color, alpha);
		}
		else
		{
			highlightImage.color = Color.clear;
		}
	}
}
