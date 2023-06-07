using System.Collections.Generic;
using UnityEngine;

public static class Rasterizer
{
    public static GridData Rasterize(ICollection<GameObject> gameObjects, float cellSize)
    {
        if (cellSize < 0.001f) { cellSize = 0.001f; }
        List<MeshData> meshDatas = MeshDataReceiver.GetMeshDatas(gameObjects);
        RasterizationBounds rasterizationBounds = new RasterizationBounds(meshDatas);
        Vector2 origin = rasterizationBounds.Min;
        Vector2 step = new Vector2(cellSize, cellSize);
        int width = Mathf.CeilToInt((rasterizationBounds.Max.x - rasterizationBounds.Min.x) / step.x);
        int height = Mathf.CeilToInt((rasterizationBounds.Max.y - rasterizationBounds.Min.y) / step.y);

        GridData gridData = new GridData(origin, step, width, height, rasterizationBounds.MinDepth);

        foreach (var mesh in meshDatas)
        {
            Vector3[] vertices = mesh.Vertices;
            int[] triangles = mesh.Triangles;
            int trianglesCount = triangles.Length / 3;
            for (int i = 0; i < trianglesCount; ++i)
            {
                int firstIndex = 3 * i;
                int v1 = triangles[firstIndex];
                int v2 = triangles[firstIndex + 1];
                int v3 = triangles[firstIndex + 2];
                Vector3 cross = Vector3.Cross(vertices[v2] - vertices[v1], vertices[v3] - vertices[v1]);
                float slope = Vector3.Dot(cross.normalized, Vector3.up);
                if (slope <= 0f) { continue; }

                float minX = Mathf.Min(Mathf.Min(vertices[v1].x, vertices[v2].x), vertices[v3].x),
                      minZ = Mathf.Min(Mathf.Min(vertices[v1].z, vertices[v2].z), vertices[v3].z),
                      maxX = Mathf.Max(Mathf.Max(vertices[v1].x, vertices[v2].x), vertices[v3].x),
                      maxZ = Mathf.Max(Mathf.Max(vertices[v1].z, vertices[v2].z), vertices[v3].z);
                int minWidth = Mathf.FloorToInt((minX - gridData.Origin.x) / gridData.Step.x),
                    maxWidth = Mathf.CeilToInt((maxX - gridData.Origin.x) / gridData.Step.x),
                    minHeight = Mathf.FloorToInt((minZ - gridData.Origin.y) / gridData.Step.y),
                    maxHeight = Mathf.CeilToInt((maxZ - gridData.Origin.y) / gridData.Step.y);
                for (int w = minWidth; w < maxWidth; ++w)
                {
                    for (int h = minHeight; h < maxHeight; ++h)
                    {
                        Vector2 position = gridData.GetCellData(w,h).Position;
                        if (Vector3Utils.IsPointInTriangle(position, vertices[v1], vertices[v2], vertices[v3]))
                        {
                            float depth = BarycentricCoordinates.GetDepth(position, vertices[v1], vertices[v2], vertices[v3]);
                            gridData.TrySetData(w, h, depth, slope);
                        }
                    }
                }
            }
        }

        return gridData;
    }
}
