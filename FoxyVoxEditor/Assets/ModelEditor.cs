using UnityEngine;
using System.Collections;

public class ModelEditor : MonoBehaviour
{
	private static ModelEditor instance;
	public static ModelEditor Instance
	{
		get
		{
			return instance;
		}
	}

	private MeshVoxelModel model;
	public MeshVoxelModel Model
	{
		get
		{
			return model;
		}
	}
	
	ModelView2D[] editorViews;
	
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
			
			Vector3 cornerZeroPosition = new Vector3(model.width, model.height, model.depth) * -0.5f;
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
		
		editorViews = FindObjectsOfType<ModelView2D>();
		
		CreateNewModel("Voxel Model (Test)", 8, 8, 8);
	}
	
	void CreateNewModel(string name, ushort width, ushort height, ushort depth)
	{
		// Init the new model with the indicated name and dimensions
		model = MeshVoxelModel.Create(width, height, depth);
		model.name = name;
		
		// Create the front-plane indicator
		if (planeFront != null)
		{
			Destroy(planeFront.gameObject);
		}
		
		planeFront = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PlaneIndicator"));
		planeFront.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 1f, 0.1f);
		planeFront.transform.localScale = new Vector3(width+1, height+1, 0.1f);
		
		// Create the side-plane indicator
		if (planeSide != null)
		{
			Destroy(planeSide.gameObject);
		}
		
		planeSide = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PlaneIndicator"));
		planeSide.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 0.1f);
		planeSide.transform.localScale = new Vector3(0.1f, height+1, depth+1);
		
		// Create the top-plane indicator
		if (planeTop != null)
		{
			Destroy(planeTop.gameObject);
		}
		
		planeTop = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PlaneIndicator"));
		planeTop.GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 0.1f);
		planeTop.transform.localScale = new Vector3(width+1, 0.1f, depth+1);
		
		// Send the new model data to the UI
		ViewCoords = new Coord();
		
		foreach (ModelView2D editorView in editorViews)
		{
			editorView.OnVoxelModelCreated();
		}
	}
}
