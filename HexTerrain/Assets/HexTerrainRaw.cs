using UnityEngine;

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
		GetComponent<MeshFilter>().mesh = mesh;

		mesh.vertices = new Vector3[7];
		mesh.vertices[0] = Vector3.zero;
		for (int i = 1; i < mesh.vertices.Length; ++i)
		{
			mesh.vertices[i] = new Vector3(
				Mathf.Cos(Mathf.PI * 2 / 6 * i) * hexRadius,
				0,
				Mathf.Sin(Mathf.PI * 2 / 6 * i) * hexRadius
				);
		}

		mesh.uv = new Vector2[mesh.vertices.Length];

		mesh.triangles = new int[sidesPerHex * sidesPerTri];
		for (int i = 0; i < sidesPerHex; ++i)
		{
			mesh.triangles[i*3] = 0;
			mesh.triangles[i * 3 + 1] = i + 1;
			mesh.triangles[i * 3 + 2] = i + 2;
		}

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
}