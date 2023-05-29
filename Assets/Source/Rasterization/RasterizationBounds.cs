using System.Collections.Generic;
using UnityEngine;

public struct RasterizationBounds
{
    public Vector2 Min;
    public Vector2 Max;
    public float MinDepth;

    public RasterizationBounds(List<MeshData> meshDatas)
    {
        float minX = float.PositiveInfinity,
              minY = float.PositiveInfinity,
              minZ = float.PositiveInfinity,
              maxX = float.NegativeInfinity,
              maxZ = float.NegativeInfinity;
        foreach (var mesh in meshDatas)
        {
            Vector3 min = mesh.Bounds.min;
            if (min.x < minX) { minX = min.x; }
            if (min.y < minY) { minY = min.y; }
            if (min.z < minZ) { minZ = min.z; }
            Vector3 max = mesh.Bounds.max;
            if (max.x > maxX) { maxX = max.x; }
            if (max.z > maxZ) { maxZ = max.z; }
        }
        Min = new Vector2(minX, minZ);
        Max = new Vector2(maxX, maxZ);
        MinDepth = minY;
    }
}
