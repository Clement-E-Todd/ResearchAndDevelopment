using System.Collections.Generic;
using UnityEngine;

public partial class VoxelMesh : MonoBehaviour
{
    public void GenerateMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        /*
        Iterate over each voxel and generate mesh data where appropriate.
        because a surface can only be seen from the outside, we generate
        mesh data whenever we find an empty voxel wih non-empty neighbours(s).
        This means that we have to iterate slightly out-of-bounds so that we
        can generate mesh data for voxels on the edge of the grid.
        */
        for (int x = -1; x <= data.GetLength(0); x++)
        {
            for (int y = -1; y <= data.GetLength(1); y++)
            {
                for (int z = -1; z <= data.GetLength(2); z++)
                {
                    bool outOfBounds = !AreCoordsInBounds(x, y, z);

                    // If this voxel is empty, check for neighbouring
                    // surfaces to render.
                    if (outOfBounds || data[x, y, z] == 0)
                    {
                        Vector3 center = new Vector3(x, y, z) * voxelSize;
                        int[,,] indices = new int[3, 3, 3];

                        Debug.LogFormat("{0}, {1}, {2}", x, y, z);

                        // Look in all 6 directions and create vertices where there are neighbours
                        for (LeftToRight leftRight = LeftToRight.Left; leftRight < LeftToRight.MAX; leftRight++)
                        {
                            for (DownToUp downUp = DownToUp.Down; downUp < DownToUp.MAX; downUp++)
                            {
                                for (BackToFront backFront = BackToFront.Back; backFront < BackToFront.MAX; backFront++)
                                {
                                    if (leftRight == LeftToRight.Middle &&
                                        downUp == DownToUp.Middle &&
                                        backFront == BackToFront.Middle)
                                    {
                                        continue;
                                    }

                                    if ((AreCoordsInBounds(x + (int)leftRight - 1, y, z) && data[x + (int)leftRight - 1, y, z] > 0) ||
                                        (AreCoordsInBounds(x, y + (int)downUp - 1, z) && data[x, y + (int)downUp - 1, z] > 0) ||
                                        (AreCoordsInBounds(x, y, z + (int)backFront - 1) && data[x, y, z + (int)backFront - 1] > 0))
                                    {
                                        Debug.LogFormat("{0}, {1}, {2}", leftRight, downUp, backFront);

                                        indices[(int)leftRight, (int)downUp, (int)backFront] = verts.Count;

                                        verts.Add(new Vector3(
                                                center.x + (float)leftRight * voxelSize / 2,
                                                center.y + (float)downUp * voxelSize / 2,
                                                center.z + (float)backFront * voxelSize / 2
                                                ));
                                    }
                                }
                            }
                        }

                        // Create triangles...

                    }
                }
            }
        }
    }

    enum LeftToRight
    {
        Left,
        Middle,
        Right,
        MAX
    }

    enum DownToUp
    {
        Down,
        Middle,
        Up,
        MAX
    }

    enum BackToFront
    {
        Back,
        Middle,
        Front,
        MAX
    }

    bool AreCoordsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < data.GetLength(0) &&
            y >= 0 && y < data.GetLength(1) &&
            z >= 0 && z < data.GetLength(2);
    }

    int GetVoxelIndex(int x, int y, int z)
    {
        return z + (y * data.GetLength(2)) + (x * data.GetLength(2) * data.GetLength(1));
    }
}
