using UnityEngine;

public class BezierCurve : MonoBehaviour
{
	public Vector3[] points;

	public void Reset()
	{
		points = new Vector3[] {
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f)
		};
	}

	public Vector3 GetPoint(float t)
	{
		return transform.TransformPoint(BezierHelper.GetPoint(points[0], points[1], points[2], points[3], t));
	}

	// The "velocity" is a straight line indicating the current direction that the curve is going at a given point
	public Vector3 GetVelocity(float t)
	{
		return transform.TransformPoint(BezierHelper.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
	}

	public Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}
}