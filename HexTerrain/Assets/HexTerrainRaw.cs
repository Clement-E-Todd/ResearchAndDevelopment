using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(MeshFilter))]
public class HexTerrainRaw : HexTerrain
{
	const float hexRadius = 1f;
	const int sidesPerHex = 6;
	const int sidesPerTri = 3;

	void Start()
	{
		GenerateMeshForHex(0, 0);
	}

	void GenerateMeshForHex(int x, int y)
	{
		Mesh mesh = new Mesh();
		mesh.name = string.Format("Hex Mesh [{0}, {1}]", x, y);
		GetComponent<MeshFilter>().mesh = mesh;

		List<Vector3> vertices = new List<Vector3>() { Vector3.zero };
		for (int i = 0; i < sidesPerHex; ++i)
		{
			vertices.Add(new Vector3(Mathf.Cos(Mathf.PI * 2 / 6 * i) * hexRadius, 0, Mathf.Sin(Mathf.PI * 2 / 6 * i) * hexRadius));
		}
		mesh.vertices = vertices.ToArray();

		mesh.uv = new Vector2[mesh.vertices.Length];

		List<int> triangles = new List<int>();
		for (int i = 0; i < sidesPerHex; ++i)
		{
			triangles.Add(0);
			triangles.Add(i + 2 < sidesPerHex ? i + 2 : 0);
			triangles.Add(i + 1);
		}
		mesh.triangles = triangles.ToArray();

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
}