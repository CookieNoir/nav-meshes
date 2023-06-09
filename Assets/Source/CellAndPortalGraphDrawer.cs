using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAndPortalGraphDrawer : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;

    public void Draw(CellAndPortalGraph graph)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vertexIndexOffset = 0;
        foreach (var cell in graph.Cells)
        {
            int t0 = vertexIndexOffset;
            for (int i = 2; i < cell.Vertices.Length; ++i)
            {
                int t1 = vertexIndexOffset + i - 1;
                int t2 = vertexIndexOffset + i;
                triangles.Add(t0);
                triangles.Add(t2);
                triangles.Add(t1);

            }
            vertices.AddRange(cell.Vertices);
            vertexIndexOffset = vertices.Count;
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        _meshFilter.mesh = mesh;
    }
}
