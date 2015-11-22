using UnityEngine;
using System.Collections;

public class ModelCursor3D : MonoBehaviour
{
	void Update()
	{
		if (ModelView2DVoxel.CurrentSelection != null)
		{
			transform.position = ModelView2DVoxel.CurrentSelection.VoxelObject.transform.position;
			
			transform.localScale = Vector3.one * (1 + Mathf.Abs(Mathf.Sin(Time.time*Mathf.PI*2)) / 4);
			float color = 0.5f + Mathf.Sin(Time.time*Mathf.PI*2) / 4;
			float alpha = Mathf.Lerp(0f, 0.5f, Mathf.Abs(Mathf.Sin(Time.time*Mathf.PI*2)));
			GetComponent<MeshRenderer>().material.color = new Color(color, color, color, alpha);
		}
		else
		{
			GetComponent<MeshRenderer>().material.color = Color.clear;
		}
	}
}
