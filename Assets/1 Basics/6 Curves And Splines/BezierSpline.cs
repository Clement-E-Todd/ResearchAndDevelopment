using System;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
	[SerializeField] // We still want this data to be saved in the editor even though it's private
	private Vector3[] points;

	[SerializeField]
	private Bezier.ControlPointMode[] modes;

	public int ControlPointCount
	{
		get
		{
			return points.Length;
		}
	}

	[SerializeField]
	private bool loop;

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
			if (value == true)
			{
				modes[modes.Length - 1] = modes[0];
				SetControlPointPosition(0, points[0]);
			}
		}
	}

	public Vector3 GetControlPointPosition(int index)
	{
		return points[index];
	}

	public void SetControlPointPosition(int index, Vector3 point)
	{
		// If moving a central point, move its surrounding points along with it
		if (index % 3 == 0)
		{
			Vector3 delta = point - points[index];
			if (loop)
			{
				if (index == 0)
				{
					points[1] += delta;
					points[points.Length - 2] += delta;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1)
				{
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else
				{
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else
			{
				if (index > 0)
				{
					points[index - 1] += delta;
				}
				if (index + 1 < points.Length)
				{
					points[index + 1] += delta;
				}
			}
		}

		// Move the specified point
		points[index] = point;
		EnforceMode(index);
	}

	public Bezier.ControlPointMode GetControlPointMode(int index)
	{
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode(int index, Bezier.ControlPointMode mode)
	{
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;

		if (loop)
		{
			if (modeIndex == 0)
			{
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1)
			{
				modes[0] = mode;
			}
		}

		EnforceMode(index);
	}

	private void EnforceMode(int index)
	{
		int modeIndex = (index + 1) / 3;

		Bezier.ControlPointMode mode = modes[modeIndex];
		if (mode == Bezier.ControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
		{
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex)
		{
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0)
			{
				fixedIndex = points.Length - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Length)
			{
				enforcedIndex = 1;
			}
		}
		else
		{
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Length)
			{
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0)
			{
				enforcedIndex = points.Length - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == Bezier.ControlPointMode.Aligned)
		{
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount
	{
		get
		{
			return (points.Length - 1) / 3;
		}
	}

	public Vector3 GetPoint(float t)
	{
		int i;
		if (t >= 1f)
		{
			t = 1f;
			i = points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(
			Bezier.GetPoint(
				points[i],
				points[i + 1],
				points[i + 2],
				points[i + 3], t));
	}

	public Vector3 GetVelocity(float t)
	{
		int i;
		if (t >= 1f)
		{
			t = 1f;
			i = points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(
			Bezier.GetFirstDerivative(
				points[i],
				points[i + 1],
				points[i + 2],
				points[i + 3], t)) - transform.position;
	}

	public Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}

	public void Reset()
	{
		points = new Vector3[] {
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f)
		};
		modes = new Bezier.ControlPointMode[] {
			Bezier.ControlPointMode.Free,
			Bezier.ControlPointMode.Free
		};
	}

	public void AddCurve()
	{
		Vector3 point = points[points.Length - 1];
		Array.Resize(ref points, points.Length + 3);
		point.x += 1f;
		points[points.Length - 3] = point;
		point.x += 1f;
		points[points.Length - 2] = point;
		point.x += 1f;
		points[points.Length - 1] = point;

		Array.Resize(ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];

		EnforceMode(points.Length - 4);

		if (loop)
		{
			points[points.Length - 1] = points[0];
			modes[modes.Length - 1] = modes[0];
			EnforceMode(0);
		}
	}
}