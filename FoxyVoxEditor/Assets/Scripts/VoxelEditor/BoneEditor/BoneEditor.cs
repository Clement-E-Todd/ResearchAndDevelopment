using UnityEngine;
using System.Collections;

public class BoneEditor : MonoBehaviour
{
	private static BoneEditor instance;
	public static BoneEditor Instance
	{
		get
		{
			return instance;
		}
	}
	
	VoxelModelBone currentBone;
	public VoxelModelBone CurrentBone
	{
		get
		{
			return currentBone;
		}
	}
	
	BoneEditorVoxel[,,] voxelObjects;
	public BoneEditorVoxel[,,] VoxelObjects
	{
		get
		{
			return voxelObjects;
		}
	}
	
	GameObject currentBoneObject;
	
	BoneEditor2DView[] editorViews;
	
	private Coord viewCoords = new Coord();
	public Coord ViewCoords
	{
		get
		{
			return viewCoords;
		}
		
		set
		{
			viewCoords = value;
			
			Vector3 cornerZeroPosition = new Vector3(currentBone.width, currentBone.height, currentBone.depth) * -0.5f;
			cornerZeroPosition += new Vector3(0.5f, 0.5f, 0.5f);
			
			planeFront.transform.position = new Vector3(0f, 0f, cornerZeroPosition.z + value.z) ;
			planeSide.transform.position = new Vector3(cornerZeroPosition.x + value.x, 0f, 0f);
			planeTop.transform.position = new Vector3(0f, cornerZeroPosition.y + value.y, 0f);
		}
	}
	
	GameObject planeFront;
	GameObject planeSide;
	GameObject planeTop;
	
	void Awake()
	{
		instance = this;
		
		editorViews = FindObjectsOfType<BoneEditor2DView>();
		
		VoxelModel tempModel = VoxelModel.CreateNew(8, 8, 8);
		LoadBone(tempModel.Bones[0]);
	}
	
	public void LoadBone(VoxelModelBone bone)
	{
		currentBone = bone;
	
		// Clear the working area for the last bone that was loaded
		voxelObjects = new BoneEditorVoxel[bone.width, bone.height, bone.depth];
		
		if (currentBoneObject != null)
		{
			Destroy(currentBoneObject);
		}
	
		// Create the working area for the bone we are now loading
		currentBoneObject = new GameObject();
		currentBoneObject.name = "Bone Editor Voxel Model";
		
		for (int x = 0; x < bone.width; ++x)
		{
			for (int y = 0; y < bone.height; ++y)
			{
				for (int z = 0; z < bone.depth; ++z)
				{
					voxelObjects[x,y,z] = BoneEditorVoxel.Create(currentBoneObject, currentBone, x, y, z);
				}
			}
		}
		
		// Create the front-plane indicator
		if (planeFront != null)
		{
			Destroy(planeFront.gameObject);
		}
		
		planeFront = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PlaneIndicator"));
		planeFront.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 1f, 0.1f);
		planeFront.transform.localScale = new Vector3(bone.width+1, bone.height+1, 0.05f);
		
		// Create the side-plane indicator
		if (planeSide != null)
		{
			Destroy(planeSide.gameObject);
		}
		
		planeSide = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PlaneIndicator"));
		planeSide.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 0.1f);
		planeSide.transform.localScale = new Vector3(0.05f, bone.height+1, bone.depth+1);
		
		// Create the top-plane indicator
		if (planeTop != null)
		{
			Destroy(planeTop.gameObject);
		}
		
		planeTop = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PlaneIndicator"));
		planeTop.GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 0.1f);
		planeTop.transform.localScale = new Vector3(bone.width+1, 0.05f, bone.depth+1);
		
		// Send the new model data to the UI
		ViewCoords = new Coord();
		
		foreach (BoneEditor2DView editorView in editorViews)
		{
			editorView.OnVoxelModelCreated();
		}
	}
}
