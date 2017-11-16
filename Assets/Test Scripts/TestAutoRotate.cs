using UnityEngine;
using System.Collections;

public class TestAutoRotate : MonoBehaviour
{
	public Vector3 eulerAngles = new Vector3(0, 90, 0);

	void Update()
	{
		transform.Rotate(eulerAngles * Time.deltaTime);
	}
}
