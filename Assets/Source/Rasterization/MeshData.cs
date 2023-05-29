using UnityEngine;

public class MeshData
{
    public Vector3[] Vertices { get; private set; }
    public int[] Triangles { get; private set; }
    public Bounds Bounds { get; private set; }

    public MeshData(MeshFilter meshFilter, MeshRenderer meshRenderer)
    {
        GetTransformedMesh(meshFilter);
        Bounds = meshRenderer.bounds;
    }

    private void GetTransformedMesh(MeshFilter meshFilter)
    {
        Transform transform = meshFilter.transform;
        Mesh mesh = meshFilter.mesh;
        Vertices = mesh.vertices;
        Triangles = mesh.triangles;
        for (int i = 0; i < Vertices.Length; ++i)
        {
            Vertices[i] = transform.TransformPoint(Vertices[i]);
        }
    }
}
