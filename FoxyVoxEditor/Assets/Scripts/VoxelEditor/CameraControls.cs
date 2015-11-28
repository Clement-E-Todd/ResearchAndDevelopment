using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
	public Transform cameraTarget;

	private float camMoveSpeed = 8f;
	private float camRotateSpeed = 90f;
	
	private float camRotationHorizontal = 0f;
	private float camRotationVertical = 0f;

	public void OnCamPosCenter()
	{
		cameraTarget.position = Vector3.zero * Time.deltaTime * camMoveSpeed;
	}
	
	public void OnCamPosUp()
	{
		cameraTarget.position += Vector3.up * Time.deltaTime * camMoveSpeed;
	}
	
	public void OnCamPosDown()
	{
		cameraTarget.position -= Vector3.up * Time.deltaTime * camMoveSpeed;
	}
	
	public void OnCamPosLeft()
	{
		cameraTarget.position -= cameraTarget.transform.right * Time.deltaTime * camMoveSpeed;
	}
	
	public void OnCamPosRight()
	{
		cameraTarget.position += cameraTarget.transform.right * Time.deltaTime * camMoveSpeed;
	}
	
	public void OnCamRotFront()
	{
		cameraTarget.eulerAngles = Vector3.zero;
		
		camRotationHorizontal = 0f;
		camRotationVertical = 0f;
	}
	
	public void OnCamRotUp()
	{
		camRotationVertical += Time.deltaTime * camRotateSpeed;
		RotateCamera();
	}
	
	public void OnCamRotDown()
	{
		camRotationVertical -= Time.deltaTime * camRotateSpeed;
		RotateCamera();
	}
	
	public void OnCamRotLeft()
	{
		camRotationHorizontal += Time.deltaTime * camRotateSpeed;
		RotateCamera();
	}
	
	public void OnCamRotRight()
	{
		camRotationHorizontal -= Time.deltaTime * camRotateSpeed;
		RotateCamera();
	}
	
	void RotateCamera()
	{
		if (camRotationHorizontal > 360)
		{
			camRotationHorizontal -= 360;
		}
		else if (camRotationHorizontal < 0)
		{
			camRotationHorizontal += 360;
		}
		
		if (camRotationVertical > 360)
		{
			camRotationVertical -= 360;
		}
		else if (camRotationVertical < 0)
		{
			camRotationVertical += 360;
		}
		
		cameraTarget.eulerAngles = Vector3.zero;
		
		cameraTarget.Rotate(new Vector3(0f, camRotationHorizontal, 0f));
		cameraTarget.Rotate(new Vector3(camRotationVertical, 0f, 0f));
	}
	
	public void OnZoomInPressed()
	{
	}
	
	public void OnZoomOutPressed()
	{
	}
}
