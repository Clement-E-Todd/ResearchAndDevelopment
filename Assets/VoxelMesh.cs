using System.Collections.Generic;
using UnityEngine;

public partial class VoxelMesh : MonoBehaviour
{
    [SerializeField] public byte[,,] data;
    public float voxelSize = 0.5f;

    void Awake()
    {
        data = new byte[,,] { { { 1 } } };

        /*data = new byte[3,3,3];

        data[0, 0, 0] = 1;
        data[1, 0, 0] = 1;
        data[2, 0, 0] = 1;

        data[0, 0, 1] = 1;
        data[1, 0, 1] = 1;
        data[2, 0, 1] = 1;

        data[0, 0, 2] = 1;
        data[1, 0, 2] = 1;
        data[2, 0, 2] = 1;

        data[0, 1, 0] = 1;
        data[0, 2, 0] = 1;*/

        GenerateMesh();
    }
}
