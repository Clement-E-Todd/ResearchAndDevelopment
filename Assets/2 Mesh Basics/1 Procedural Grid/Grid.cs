﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
	public int xSize, ySize;

	private Vector3[] vertices;

	private Mesh mesh;

	private void Awake()
	{
		StartCoroutine(Generate());
	}

	private IEnumerator Generate()
	{
		WaitForSeconds wait = new WaitForSeconds(0.01f);

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		// Define the vertices and UVs
		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];

		for (int i = 0, y = 0; y <= ySize; y++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				vertices[i] = new Vector3(x, y);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);

				yield return wait;
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uv;

		// Define the triangles
		int[] triangles = new int[xSize * ySize * 6];

		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				triangles[ti] = vi;
				triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 2] = vi + 1;

				mesh.triangles = triangles;
				yield return wait;

				triangles[ti + 3] = vi + 1;
				triangles[ti + 4] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;

				mesh.triangles = triangles;
				yield return wait;
			}
		}

		// Recalculate Normals
		mesh.RecalculateNormals();
		yield return wait;
	}

	private void OnDrawGizmos()
	{
		if (vertices == null || vertices.Length == 0)
		{
			return;
		}

		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++)
		{
			Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
		}
	}
}